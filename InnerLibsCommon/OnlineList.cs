using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Extensions;


namespace Extensions.Web.Online
{

    public class LogEntryBackup
    {
        #region Public Properties

        public DateTime DateTime { get; set; }
        public string ID { get; set; }
        public Dictionary<string, string> LogData { get; internal set; }
        public string Message { get; set; }
        public Uri Url { get; set; }
        public string UserID { get; set; }

        #endregion Public Properties
    }

    /// <summary>
    /// <see cref="Dictionary{TUser, TID}"/> utilizado para controle de usuários que estão
    /// online/offline em uma aplicação
    /// </summary>
    /// <typeparam name="TUser">Tipo da Classe do usuário</typeparam>
    /// <typeparam name="TID">Tipo do Id do usuário</typeparam>

    [Serializable]
    public class OnlineList<TUser, TID> : Dictionary<TID, OnlineUser<TUser, TID>>
        where TUser : class
        where TID : struct
    {
        #region Private Fields

        private TimeSpan _tolerancetime = new TimeSpan(0, 1, 0);
        private readonly Func<TUser, TID> idgetter;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Cria uma nova instancia de OnlineList apontando a propriedade do Id do usuario e opcionalmente
        /// </summary>
        /// <param name="IdProperty">
        /// Expressão lambda que indica qual propriedade da classe <see cref="TUser"/> é o Id de
        /// tipo <see cref="TID"/>
        /// </param>
        public OnlineList(Func<TUser, TID> IdProperty)
        {
            idgetter = IdProperty;
            Chat = new UserChat<TUser, TID>(this);
            Log = new UserLog<TUser, TID>(this);
        }

        #endregion Public Constructors

        #region Public Indexers

        /// <summary>
        /// Retorna um usuario de acordo com seu Id
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public OnlineUser<TUser, TID> this[TUser User]
        {
            get => GetUser(User);

            set => Add(value?.User);
        }

        #endregion Public Indexers

        #region Public Properties

        /// <summary>
        /// Diretorio onde serão guardados os XMLs deta lista
        /// </summary>
        /// <returns></returns>
        public DirectoryInfo BackupDirectory { get; set; } = null;

        public UserChat<TUser, TID> Chat { get; private set; }

        /// <summary>
        /// Caminho do arquivo XML do Chat
        /// </summary>
        /// <returns></returns>
        public FileInfo ChatFile
        {
            get
            {
                if (BackupDirectory != null)
                {
                    BackupDirectory = BackupDirectory.FullName.CreateDirectoryIfNotExists();
                }

                return new FileInfo(Path.Combine(BackupDirectory.FullName, "CHAT.XML"));
            }
        }

        /// <summary>
        /// Lista de conversas dos usuários
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// Entradas de ações dos usuários
        /// </summary>
        /// <returns></returns>
        public UserLog<TUser, TID> Log { get; private set; }

        /// <summary>
        /// Caminho do arquivo XML do Log
        /// </summary>
        /// <returns></returns>
        public FileInfo LogFile
        {
            get
            {
                if (BackupDirectory != null)
                {
                    BackupDirectory = BackupDirectory.FullName.CreateDirectoryIfNotExists();
                }

                return new FileInfo(Path.Combine(BackupDirectory.FullName, "LOG.XML"));
            }
        }

        /// <summary>
        /// Função que será executada quando ocorrer uma entrada no log
        /// </summary>
        /// <returns></returns>
        public Action<UserLogEntry<TUser, TID>> OnCreateLog { get; set; } = x => Util.WriteDebug("Log entry created for " + x.GetUser().ID.ToString());

        public Action<OnlineUser<TUser, TID>> OnUserOnlineChanged { get; set; } = x => Util.WriteDebug("User Updated -> " + x.ID.ToString());

        /// <summary>
        /// Tolerancia que o servidor considera um usuário online ou na mesma atividade
        /// </summary>
        /// <returns></returns>
        public TimeSpan ToleranceTime
        {
            get
            {
                if (_tolerancetime.Ticks < 0L)
                {
                    return _tolerancetime.Negate();
                }
                else
                {
                    return _tolerancetime;
                }
            }

            set
            {
                _tolerancetime = value;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Adciona um usuario a esta lista
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public OnlineUser<TUser, TID> Add(TUser User) => Add(User, default, null, null);

        /// <summary>
        /// Adciona varios usuarios a esta lista
        /// </summary>
        /// <param name="Users"></param>
        /// <returns></returns>
        public IEnumerable<OnlineUser<TUser, TID>> Add(IEnumerable<TUser> Users) => AddMany(Users.ToArray());

        /// <summary>
        /// Adciona um usuario a esta lista
        /// </summary>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public OnlineUser<TUser, TID> Add(TUser Obj, bool? Online = default, string Activity = null, Uri Url = null, Dictionary<string, string> LogData = null, DateTime? DateTime = default)
        {
            if (Obj != null)
            {
                var ID = GetID(Obj);
                if (!ContainsKey(ID))
                {
                    base[ID] = new OnlineUser<TUser, TID>(Obj, this);
                }

                if (Online.HasValue)
                {
                    if (Activity.IsNotBlank())
                    {
                        var dt = DateTime ?? System.DateTime.Now;
                        CreateLog(Obj, Activity, Url, LogData, dt);
                    }

                    if (base[ID].IsOnline != Online == true)
                    {
                        OnUserOnlineChanged?.Invoke(base[ID]);
                    }
                }

                return base[ID];
            }

            return null;
        }

        /// <summary>
        /// Adciona varios usuarios a esta lista
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OnlineUser<TUser, TID>> AddMany(params TUser[] Users)
        {
            var l = new List<OnlineUser<TUser, TID>>();
            foreach (var u in Users)
                l.Add(Add(u, default, null, null));
            return l;
        }

        /// <summary>
        /// Verifica se um usuario está nesta lista
        /// </summary>
        /// <returns></returns>
        public bool ContainsUser(TUser User) => ContainsKey(GetID(User));

        /// <summary>
        /// Cria uma entrada no log deste usuário
        /// </summary>
        /// <param name="Logdata"></param>
        /// <returns></returns>
        public UserLogEntry<TUser, TID> CreateLog(TUser User, string Message, Uri URL = null, Dictionary<string, string> LogData = null, DateTime? DateAndTime = default)
        => Log.CreateLog(User, Message, URL, LogData, DateAndTime ?? DateTime.Now);

        /// <summary>
        /// Retorna o Id do usuário
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public TID GetID(TUser User) => idgetter(User);

        /// <summary>
        /// Retorna um OnlineUser a partir de uma instancia de <typeparamref name="TUser"/>
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public OnlineUser<TUser, TID> GetUser(TUser User) => ContainsUser(User) ? base[GetID(User)] : Add(User);

        /// <summary>
        /// Retorna todo os usuarios
        /// </summary>
        /// <param name="IsOnline">True para online, false para offline, null para todos</param>
        /// <returns></returns>
        public IEnumerable<TUser> GetUsersData(bool? IsOnline = default) => Users().Where(x => x.IsOnline == (IsOnline ?? x.IsOnline)).Select(x => x.User);

        /// <summary>
        /// Mantém um usuario online mas não atribui nenhuma nova atividade nem cria entradas no LOG
        /// </summary>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public OnlineUser<TUser, TID> KeepOnline(TUser Obj) => Add(Obj, true);

        /// <summary>
        /// Retorna todos os <see cref="OnlineUser(Of TUser, TID)"/> que estão Offline
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OnlineUser<TUser, TID>> OfflineUsers() => Users().Where(x => x.IsOnline == false);

        /// <summary>
        /// Retorna todos os <see cref="OnlineUser(Of TUser, TID)"/> que estão Online no momento
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OnlineUser<TUser, TID>> OnlineUsers() => Users().Where(x => x.IsOnline == true);

        public void OpenChatXML()
        {
            if (ChatFile.Exists)
            {
                var x = ChatFile.CreateObjectFromXMLFile<UserConversationBackup[]>();
                foreach (var ii in x)
                {
                    if (ii.ID.IsNotIn(Chat.IDs))
                    {
                        var cvn = new UserConversation<TUser, TID>
                        {
                            chatlist = Chat,
                            FromUserID = ii.FromUserID.ChangeType<TID>(),
                            ToUserID = ii.ToUserID.ChangeType<TID>(),
                            SentDate = ii.SentDate,
                            ViewedDate = ii.ViewedDate,
                            Message = ii.Message
                        };
                        Chat.Add(cvn);
                    }
                }
            }
        }

        public void OpenLogXML()
        {
            if (LogFile.Exists)
            {
                var x = LogFile.CreateObjectFromXMLFile<LogEntryBackup[]>();
                foreach (var ii in x)
                {
                    if (ii.ID.IsNotBlank() && ii.ID.IsNotIn(Log.IDs))
                    {
                        TID usu_id = (TID)TypeDescriptor.GetConverter(typeof(TID)).ConvertFromInvariantString(ii.UserID);
                        var log = new UserLogEntry<TUser, TID>(usu_id, this)
                        {
                            Message = ii.Message,
                            DateTime = ii.DateTime,
                            URL = ii.Url,
                            LogData = ii.LogData
                        };
                        Log.Add(log);
                    }
                }
            }
        }

        /// <summary>
        /// Remove um usuário desta lista
        /// </summary>
        /// <param name="Obj"></param>
        public void Remove(params TUser[] Obj)
        {
            Obj = Obj ?? Array.Empty<TUser>();
            this.RemoveIfExist(Obj.Select(x => GetID(x)).ToArray());
        }

        /// <summary>
        /// Remove um usuário desta lista a partir do Id
        /// </summary>
        /// <param name="ID"></param>
        public new void Remove(TID ID) => this.RemoveIfExist(ID);

        public void SaveChatXML() => Chat.Select(x =>
            {
                var bkp = new UserConversationBackup
                {
                    SentDate = x.SentDate,
                    Message = x.Message,
                    FromUserID = x.FromUserID.ToString(),
                    ToUserID = x.ToUserID.ToString(),
                    ViewedDate = x.ViewedDate
                };
                return bkp;
            }).ToList().CreateXmlFile(ChatFile.FullName);

        public void SaveLogXML() => Log.Select(x =>
                                               {
                                                   var bkp = new LogEntryBackup
                                                   {
                                                       DateTime = x.DateTime,
                                                       Message = x.Message,
                                                       UserID = x.UserID.ToString(),
                                                       ID = x.ID,
                                                       LogData = x.LogData
                                                   };
                                                   return bkp;
                                               }).ToList().CreateXmlFile(LogFile.FullName);

        /// <summary>
        /// Seta um usuario como offline e cria uma entrada no log
        /// </summary>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public OnlineUser<TUser, TID> SetOffline(TUser Obj) => Add(Obj, false, "Offline", null, null, DateTime.Now);

        /// <summary>
        /// Seta um usuario como online e cria uma entrada no Log
        /// </summary>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public OnlineUser<TUser, TID> SetOnline(TUser Obj)
        => Add(Obj, true, "Online", null, null, DateTime.Now);

        /// <summary>
        /// Seta um usuario como online e atribui uma atividade a ele. Cria entrada no log automaticamente
        /// </summary>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public OnlineUser<TUser, TID> SetOnlineActivity(TUser Obj, string Activity, Uri Url = null, Dictionary<string, string> LogData = null, DateTime? DateTime = default) => Add(Obj, true, Activity, Url, LogData, DateTime);

        /// <summary>
        /// Retorna um usuário desta lista a partir do Id
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public OnlineUser<TUser, TID> UserById(TID Key)
        {
            if (ContainsKey(Key))
            {
                return base[Key];
            }

            return null;
        }

        /// <summary>
        /// Retorna um usuário desta lista a partir do Id
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public TUser UserDataById(TID Key) => UserById(Key)?.User;

        /// <summary>
        /// Retorna todos os <see cref="OnlineUser(Of TUser, TID)"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OnlineUser<TUser, TID>> Users() => this.Select(x => x.Value);

        #endregion Public Methods
    }

    /// <summary>
    /// Usuario Online/Offline
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TID"></typeparam>
    public class OnlineUser<TUser, TID>
        where TUser : class
        where TID : struct
    {
        #region Internal Fields

        internal OnlineList<TUser, TID> OnlineList;

        #endregion Internal Fields

        #region Internal Constructors

        internal OnlineUser(TUser Data, OnlineList<TUser, TID> list)
        {
            User = Data;
            OnlineList = list;
        }

        #endregion Internal Constructors

        #region Public Properties

        /// <summary>
        /// Conversas deste usuário
        /// </summary>
        /// <returns></returns>
        public UserConversation<TUser, TID>[] Conversations => GetConversations();

        /// <summary>
        /// Id deste usuário
        /// </summary>
        /// <returns></returns>
        public TID ID => OnlineList.GetID(User);

        /// <summary>
        /// Indica se o usuario está online ou não
        /// </summary>
        /// <returns></returns>
        public bool IsOnline => OnlineList.ContainsUser(User) && LastOnline.HasValue && LastOnline >= DateTime.Now.Add(OnlineList.ToleranceTime.Negate()) == true;

        /// <summary>
        /// Ultima atividade do usuário
        /// </summary>
        /// <returns></returns>
        public string LastActivity
        {
            get => LogEntries().FirstOrDefault()?.Message.IfBlank("Offline");

            set => OnlineList.CreateLog(User, value);
        }

        /// <summary>
        /// Ultima vez que o usuário esteve Online
        /// </summary>
        /// <returns></returns>
        public DateTime? LastOnline => LogEntries().FirstOrDefault()?.DateTime;

        /// <summary>
        /// Ultima URL/caminho que o usuário acessou
        /// </summary>
        /// <returns></returns>
        public Uri LastUrl => LogEntries().FirstOrDefault(x => x.URL != null)?.URL;

        /// <summary>
        /// Informações relacionadas do usuário do tipo <typeparamref name="TUser"/>
        /// </summary>
        /// <returns></returns>
        public TUser User { get; private set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Retorna uma lista de conversas com um usuario específico ou todas
        /// </summary>
        /// <param name="WithUser"></param>
        /// <returns></returns>
        public UserConversation<TUser, TID>[] GetConversations(TUser WithUser = null) => (UserConversation<TUser, TID>[])OnlineList.Chat.GetConversation(User, WithUser);

        /// <summary>
        /// Entradas no Log para este usuário
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserLogEntry<TUser, TID>> LogEntries()
        => OnlineList?.Log?.Where(x => x.UserID.Equals(ID)).OrderByDescending(x => x.DateTime).AsEnumerable() ?? new List<UserLogEntry<TUser, TID>>().AsEnumerable();

        /// <summary>
        /// Envia uma mensagem no chat para outro usuario
        /// </summary>
        /// <param name="ToUser"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public UserConversation<TUser, TID> SendMessage(TUser ToUser, string Message) => OnlineList.Chat.Send(User, ToUser, Message);

        public override string ToString() => User.ToString();

        #endregion Public Methods
    }

    /// <summary>
    /// Lista de conversas entre usuários
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TID"></typeparam>
    public class UserChat<TUser, TID> : List<UserConversation<TUser, TID>>
        where TUser : class
        where TID : struct
    {
        #region Internal Fields

        internal OnlineList<TUser, TID> ChatList;

        #endregion Internal Fields

        #region Internal Constructors

        internal UserChat(OnlineList<TUser, TID> List) : base()
        {
            ChatList = List;
        }

        #endregion Internal Constructors

        #region Public Constructors

        public UserChat() : base()
        {
        }

        #endregion Public Constructors

        #region Public Properties

        public Encoding Encoding { get; set; } = new UTF8Encoding(false);

        public IEnumerable<string> IDs => this.Select(x => x.ID);

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Apaga uma conversa com um usuário
        /// </summary>
        /// <param name="User"></param>
        /// <param name="WithUser"></param>
        public void DeleteConversation(TUser User, TUser WithUser = null)
        {
            UserConversation<TUser, TID>[] lista;
            lista = (UserConversation<TUser, TID>[])GetConversation(User, WithUser);
            foreach (var el in lista)
                Remove(el);
        }

        /// <summary>
        /// Retorna uma conversa entre 2 usuários
        /// </summary>
        /// <param name="User"></param>
        /// <param name="WithUser"></param>
        /// <returns></returns>
        public IEnumerable<UserConversation<TUser, TID>> GetConversation(TUser User, TUser WithUser = null)
        {
            UserConversation<TUser, TID>[] lista;
            if (WithUser != null)
            {
                lista = this.Where(x => (ChatList.GetID(User).Equals(x.FromUserID) && ChatList.GetID(WithUser).Equals(ChatList.GetID(x.ToUser().User))) | (ChatList.GetID(User).Equals(x.ToUserID) && ChatList.GetID(WithUser).Equals(x.FromUserID))).ToArray();
            }
            else
            {
                lista = this.Where(x => ChatList.GetID(User).Equals(x.FromUserID) || ChatList.GetID(User).Equals(x.ToUserID)).ToArray();
            }

            lista = lista.DistinctBy(x => x.ID).OrderByDescending(x => x.SentDate).ToArray();
            return lista;
        }

        /// <summary>
        /// Envia uma nova mensagem
        /// </summary>
        /// <param name="FromUser"></param>
        /// <param name="ToUser"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public UserConversation<TUser, TID> Send(TUser FromUser, TUser ToUser, string Message)
        {
            var i = new UserConversation<TUser, TID>(this) { Message = Message, FromUserID = ChatList.GetID(FromUser), ToUserID = ChatList.GetID(ToUser), ViewedDate = default };
            Add(i);
            return i;
        }

        #endregion Public Methods
    }

    /// <summary>
    /// Mensagem do chat do usuário
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TID"></typeparam>
    public class UserConversation<TUser, TID>
        where TUser : class
        where TID : struct
    {
        #region Internal Fields

        internal UserChat<TUser, TID> chatlist;

        #endregion Internal Fields

        #region Internal Constructors

        internal UserConversation(UserChat<TUser, TID> chatlist)
        {
            this.chatlist = chatlist;
        }

        #endregion Internal Constructors

        #region Public Constructors

        public UserConversation() : base()
        {
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Id do usuário emissor
        /// </summary>
        /// <returns></returns>
        public TID FromUserID { get; set; }

        /// <summary>
        /// Id desta mensagem
        /// </summary>
        /// <returns></returns>
        public string ID => new[] { "F[", FromUserID.ToString(), "]T[", ToUserID.ToString(), "]@", (object)SentDate.Ticks }.SelectJoinString(Util.EmptyString);

        /// <summary>
        /// Mensagem
        /// </summary>
        /// <returns></returns>
        public string Message { get; set; }

        /// <summary>
        /// Data de envio da mensagem
        /// </summary>
        /// <returns></returns>
        public DateTime SentDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Id do usuário destinatario
        /// </summary>
        /// <returns></returns>
        public TID ToUserID { get; set; }

        /// <summary>
        /// Inndica se esta conversa foi visualizada
        /// </summary>
        /// <returns></returns>
        public bool Viewed
        {
            get => ViewedDate.HasValue && ViewedDate <= DateTime.Now == true;

            set
            {
                if (value)
                {
                    ViewedDate = DateTime.Now;
                }
                else
                {
                    ViewedDate = default;
                }
            }
        }

        /// <summary>
        /// Data de visualização da mensagem
        /// </summary>
        /// <returns></returns>
        public DateTime? ViewedDate { get; set; } = default;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Usuario Emissor
        /// </summary>
        /// <returns></returns>
        public OnlineUser<TUser, TID> FromUser() => chatlist.ChatList.UserById(FromUserID);

        /// <summary>
        /// Retorna a instancia de <see cref="OnlineUser(Of TUser, TID)"/> (Emissor ou
        /// destinatário) de um <see cref="TUser"/> especifico
        /// </summary>
        /// <param name="Myself">Seu usuário</param>
        /// <returns></returns>
        public OnlineUser<TUser, TID> GetMyUser(TUser MySelf)
        {
            if (IsFrom(MySelf))
            {
                return FromUser();
            }

            if (IsTo(MySelf))
            {
                return ToUser();
            }

            return null;
        }

        /// <summary>
        /// Retorna a instancia de <see cref="OnlineUser(Of TUser, TID)"/> (Emissor ou
        /// destinatário) de um <see cref="TUser"/> especifico
        /// </summary>
        /// <param name="Myself">Seu usuário</param>
        /// <returns></returns>
        public OnlineUser<TUser, TID> GetOtherUser(TUser MySelf)
        {
            if (IsFrom(MySelf))
            {
                return ToUser();
            }

            if (IsTo(MySelf))
            {
                return FromUser();
            }

            return null;
        }

        /// <summary>
        /// Retorna true se a mensagem for de <paramref name="User"/>
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public bool IsFrom(TUser User) => chatlist.ChatList.GetID(User).Equals(FromUserID);

        /// <summary>
        /// Retorna true se a mensagem for para <paramref name="User"/>
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public bool IsTo(TUser User) => chatlist.ChatList.GetID(User).Equals(ToUserID);

        /// <summary>
        /// Usuario destinatário
        /// </summary>
        /// <returns></returns>
        public OnlineUser<TUser, TID> ToUser() => chatlist.ChatList.UserById(ToUserID);

        #endregion Public Methods
    }

    public class UserConversationBackup
    {
        #region Public Properties

        public string FromUserID { get; set; }
        public string ID { get; set; }
        public string Message { get; set; }
        public DateTime SentDate { get; set; }
        public string ToUserID { get; set; }
        public DateTime? ViewedDate { get; set; }

        #endregion Public Properties
    }

    public class UserLog<TUser, TID> : List<UserLogEntry<TUser, TID>>
        where TUser : class
        where TID : struct
    {
        #region Internal Fields

        internal OnlineList<TUser, TID> OnlineList;

        #endregion Internal Fields

        #region Internal Constructors

        internal UserLog(OnlineList<TUser, TID> List) : base()
        {
            OnlineList = List;
        }

        #endregion Internal Constructors

        #region Public Constructors

        public UserLog() : base()
        {
        }

        #endregion Public Constructors

        #region Public Properties

        public IEnumerable<string> IDs => this.Select(x => x.ID);

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Cria uma entrada no log deste usuário com uma data especifica
        /// </summary>
        /// <param name="Logdata"></param>
        /// <returns></returns>
        public UserLogEntry<TUser, TID> CreateLog(TUser User, string Message, Uri URL = null, Dictionary<string, string> LogData = null, DateTime? DateAndTime = default)
        {
            if (User != null && Message.IsNotBlank())
            {
                DateAndTime = DateAndTime ?? DateTime.Now;
                if ((OnlineList[User].LastActivity ?? Util.EmptyString) == (Message ?? Util.EmptyString))
                {
                    var lo = OnlineList[User].LastOnline; // nao cria log para locais repedidos dentro do tempo de N minutos
                    if (lo.HasValue && lo.Value.Add(OnlineList.ToleranceTime) >= DateAndTime == true)
                    {
                        return null;
                    }
                }

                var d = new UserLogEntry<TUser, TID>(OnlineList.GetID(User), OnlineList)
                {
                    DateTime = (DateTime)DateAndTime,
                    Message = Message,
                    LogData = LogData,
                    URL = URL
                };
                OnlineList.Log.Add(d);
                OnlineList.OnCreateLog?.Invoke(d);

                return d;
            }

            return null;
        }

        #endregion Public Methods
    }

    /// <summary>
    /// Entrada de ação do usuário no sistema
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TID"></typeparam>
    public class UserLogEntry<TUser, TID>
        where TUser : class
        where TID : struct
    {
        #region Internal Fields

        internal OnlineList<TUser, TID> list;

        #endregion Internal Fields

        #region Internal Constructors

        internal UserLogEntry(TID ID, OnlineList<TUser, TID> list) : base()
        {
            UserID = ID;
            this.list = list;
        }

        #endregion Internal Constructors

        #region Public Properties

        /// <summary>
        /// Data e hora da ocorrência
        /// </summary>
        /// <returns></returns>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Id desta entrada
        /// </summary>
        /// <returns></returns>
        public string ID => new string[] { GetUser().OnlineList.Log.IndexOf(this).ToString(), "-", GetUser().ID.ToString() }.SelectJoinString(Util.EmptyString);

        /// <summary>
        /// Informações adicionais
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> LogData { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Texto sobre a ocorrencia
        /// </summary>
        /// <returns></returns>
        public string Message { get; set; }

        /// <summary>
        /// Ultima URL da ocorrencia
        /// </summary>
        /// <returns></returns>
        public Uri URL { get; set; }

        /// <summary>
        /// Id do Usuário
        /// </summary>
        /// <returns></returns>
        public TID UserID { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Usuário
        /// </summary>
        /// <returns></returns>
        public OnlineUser<TUser, TID> GetUser() => list.UserById(UserID);

        #endregion Public Methods
    }

}