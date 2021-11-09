using InnerLibs.LINQ;
using Microsoft.VisualBasic;
 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace InnerLibs.Online
{
    /// <summary>
    /// <see cref="Dictionary(Of UserType, IdType)"/> utilizado para controle de usuários que estão online/offline em uma aplicação
    /// </summary>
    /// <typeparam name="UserType">Tipo da Classe do usuário</typeparam>
    /// <typeparam name="IdType">Tipo do ID do usuário</typeparam>
    public class OnlineList<UserType, IdType> : Dictionary<IdType, OnlineUser<UserType, IdType>>
        where UserType : class
        where IdType : struct
    {
        private Func<UserType, IdType> idgetter;
        private TimeSpan _tolerancetime = new TimeSpan(0, 1, 0);

        /// <summary>
        /// Retorna o ID do usuário
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public IdType GetID(UserType User)
        {
            return idgetter(User);
        }

        /// <summary>
        /// Função que será executada quando ocorrer uma entrada no log
        /// </summary>
        /// <returns></returns>
        public Action<UserLogEntry<UserType, IdType>> OnCreateLog { get; set; } = x => Debug.WriteLine("Log entry created for " + x.GetUser().ID.ToString());

        public Action<OnlineUser<UserType, IdType>> OnUserOnlineChanged { get; set; } = x => Debug.WriteLine("User Updated -> " + x.ID.ToString());

        /// <summary>
        /// Cria uma nova instancia de OnlineList apontando a propriedade do ID do usuario  e opcionalmente
        /// </summary>
        /// <param name="IdProperty">Expressão lambda que indica qual propriedade da classe <see cref="UserType"/> é o ID de tipo <see cref="IdType"/></param>
        public OnlineList(Func<UserType, IdType> IdProperty)
        {
            idgetter = IdProperty;
            Chat = new UserChat<UserType, IdType>(this);
            Log = new UserLog<UserType, IdType>(this);
        }

        /// <summary>
        /// Lista de conversas dos usuários
        /// </summary>
        /// <returns></returns>

        public UserChat<UserType, IdType> Chat { get; private set; }

        /// <summary>
        /// Entradas de ações dos usuários
        /// </summary>
        /// <returns></returns>
        public UserLog<UserType, IdType> Log { get; private set; }

        /// <summary>
        /// Diretorio onde serão guardados os XMLs deta lista
        /// </summary>
        /// <returns></returns>
        public DirectoryInfo BackupDirectory { get; set; } = null;

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

        public void SaveLogXML() => Log.Select(x =>
            {
                var bkp = new LogEntryBackup();
                bkp.DateTime = x.DateTime;
                bkp.Message = x.Message;
                bkp.UserID = x.UserID.ToString();
                bkp.ID = x.ID;
                bkp.LogData = x.LogData;
                return bkp;
            }).ToList().CreateXmlFile(LogFile.FullName);

        public void SaveChatXML() => Chat.Select(x =>
        {
            var bkp = new UserConversationBackup();
            bkp.SentDate = x.SentDate;
            bkp.Message = x.Message;
            bkp.FromUserID = x.FromUserID.ToString();
            bkp.ToUserID = x.ToUserID.ToString();
            bkp.ViewedDate = x.ViewedDate;
            return bkp;
        }).ToList().CreateXmlFile(ChatFile.FullName);

        public void OpenLogXML()
        {
            if (LogFile.Exists)
            {
                var x = LogFile.CreateObjectFromXMLFile<LogEntryBackup[]>();
                foreach (var ii in x)
                {
                    if (ii.ID.IsNotBlank() && ii.ID.IsNotIn(Log.IDs))
                    {
                        IdType usu_id = (IdType)TypeDescriptor.GetConverter(typeof(IdType)).ConvertFromInvariantString(ii.UserID);
                        var log = new UserLogEntry<UserType, IdType>(usu_id, this);
                        log.Message = ii.Message;
                        log.DateTime = ii.DateTime;
                        log.URL = ii.Url;
                        log.LogData = ii.LogData;
                        Log.Add(log);
                    }
                }
            }
        }

        public void OpenChatXML()
        {
            if (ChatFile.Exists)
            {
                var x = ChatFile.CreateObjectFromXMLFile<UserConversationBackup[]>();
                foreach (var ii in x)
                {
                    if (ii.ID.IsNotIn(Chat.IDs))
                    {
                        var cvn = new UserConversation<UserType, IdType>();
                        cvn.chatlist = Chat;
                        cvn.FromUserID = ii.FromUserID.ChangeType<IdType, string>();
                        cvn.ToUserID = ii.ToUserID.ChangeType<IdType, string>();
                        cvn.SentDate = ii.SentDate;
                        cvn.ViewedDate = ii.ViewedDate;
                        cvn.Message = ii.Message;
                        Chat.Add(cvn);
                    }
                }
            }
        }

        /// <summary>
        /// Tolerancia que o servidor considera um usuário online ou na mesma atividade
        /// </summary>
        /// <returns></returns>
        public TimeSpan ToleranceTime
        {
            get
            {
                if (_tolerancetime.Ticks > 0L)
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

        /// <summary>
        /// Retorna um OnlineUser a partir de uma instancia de <typeparamref name="UserType"/>
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public OnlineUser<UserType, IdType> GetUser(UserType User)
        {
            if (ContainsUser(User))
            {
                return base[GetID(User)];
            }
            else
            {
                return Add(User);
            }
        }

        /// <summary>
        /// Retorna um usuario de acordo com seu ID
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public OnlineUser<UserType, IdType> this[UserType User]
        {
            get
            {
                return GetUser(User);
            }

            set
            {
                value = Add(value.User);
            }
        }

        /// <summary>
        /// Adciona um usuario a esta lista
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public OnlineUser<UserType, IdType> Add(UserType User) => Add(User, default, null, null);

        /// <summary>
        /// Adciona varios usuarios a esta lista
        /// </summary>
        /// <param name="Users"></param>
        /// <returns></returns>
        public   IEnumerable<OnlineUser<UserType, IdType>> Add(IEnumerable<UserType> Users) => AddMany(Users.ToArray());

        /// <summary>
        /// Adciona varios usuarios a esta lista
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OnlineUser<UserType, IdType>> AddMany(params UserType[] Users)
        {
            var l = new List<OnlineUser<UserType, IdType>>();
            foreach (var u in Users)
                l.Add(Add(u, default, null, null));
            return l;
        }

        /// <summary>
        /// Adciona um usuario a esta lista
        /// </summary>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public   OnlineUser<UserType, IdType> Add(UserType Obj, bool? Online = default, string Activity = null, string Url = null, Dictionary<string, string> LogData = null, DateTime? DateTime = default)
        {
            if (Obj != null)
            {
                var ID = GetID(Obj);
                if (!ContainsKey(ID))
                {
                    base[ID] = new OnlineUser<UserType, IdType>(Obj, this);
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
                        if (OnUserOnlineChanged != null)
                        {
                            OnUserOnlineChanged.Invoke(base[ID]);
                        }
                    }
                }

                return base[ID];
            }

            return null;
        }

        /// <summary>
        /// Verifica se um usuario está nesta lista
        /// </summary>
        /// <returns></returns>
        public bool ContainsUser(UserType User)
        {
            return ContainsKey(GetID(User));
        }

        /// <summary>
        /// Retorna todos os <see cref="OnlineUser(Of UserType, IdType)"/> que estão Online no momento
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OnlineUser<UserType, IdType>> OnlineUsers()
        {
            return Users().Where(x => x.IsOnline == true);
        }

        /// <summary>
        /// Retorna todos os <see cref="OnlineUser(Of UserType, IdType)"/> que estão Offline
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OnlineUser<UserType, IdType>> OfflineUsers()
        {
            return Users().Where(x => x.IsOnline == false);
        }

        /// <summary>
        /// Retorna todos os <see cref="OnlineUser(Of UserType, IdType)"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OnlineUser<UserType, IdType>> Users()
        {
            return this.Select(x => x.Value);
        }

        /// <summary>
        /// Retorna todo os usuarios
        /// </summary>
        /// <param name="IsOnline">True para online, false para offline, null para todos</param>
        /// <returns></returns>
        public IEnumerable<UserType> GetUsersData(bool? IsOnline = default)
        {
            return Users().Where(x => x.IsOnline == (IsOnline ?? x.IsOnline)).Select(x => x.User);
        }

        /// <summary>
        /// Remove um usuário desta lista
        /// </summary>
        /// <param name="Obj"></param>
        public void Remove(params UserType[] Obj)
        {
            Obj  = Obj ?? Array.Empty<UserType>();
            this.RemoveIfExist(Obj.Select(x => GetID(x)).ToArray());
        }

        /// <summary>
        /// Remove um usuário desta lista a partir do ID
        /// </summary>
        /// <param name="ID"></param>
        public new void Remove(IdType ID)
        {
            this.RemoveIfExist(ID);
        }

        /// <summary>
        /// Seta um usuario como offline e cria uma entrada no log
        /// </summary>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public OnlineUser<UserType, IdType> SetOffline(UserType Obj)
        {
            return Add(Obj, false, "Offline", null, null, DateTime.Now);
        }

        /// <summary>
        /// Seta um usuario como online e atribui uma atividade a ele. Cria entrada no log automaticamente
        /// </summary>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public OnlineUser<UserType, IdType> SetOnlineActivity(UserType Obj, string Activity, string Url = null, Dictionary<string, string> LogData = null, DateTime? DateTime = default)
        {
            return Add(Obj, true, Activity, Url, LogData, DateTime);
        }

        /// <summary>
        /// Seta um usuario como online e cria uma entrada no Log
        /// </summary>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public OnlineUser<UserType, IdType> SetOnline(UserType Obj)
        {
            return Add(Obj, true, "Online", null, null, DateTime.Now);
        }

        /// <summary>
        /// Mantém um usuario online mas não atribui nenhuma nova atividade nem cria entradas no LOG
        /// </summary>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public OnlineUser<UserType, IdType> KeepOnline(UserType Obj)
        {
            return Add(Obj, true);
        }

        /// <summary>
        /// Retorna um usuário desta lista a partir do ID
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public OnlineUser<UserType, IdType> UserById(IdType Key)
        {
            if (ContainsKey(Key))
            {
                return base[Key];
            }

            return null;
        }

        /// <summary>
        /// Retorna um usuário desta lista a partir do ID
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public UserType UserDataById(IdType Key)
        {
            return UserById(Key)?.User;
        }

        /// <summary>
        /// Cria uma entrada no log deste usuário
        /// </summary>
        /// <param name="Logdata"></param>
        /// <returns></returns>
        public UserLogEntry<UserType, IdType> CreateLog(UserType User, string Message, string URL = null, Dictionary<string, string> LogData = null, DateTime? DateAndTime = default)
        {
            return Log.CreateLog(User, Message, URL, LogData, DateTime.Now);
        }
    }

    public class UserLog<UserType, IdType> : List<UserLogEntry<UserType, IdType>>
        where UserType : class
        where IdType : struct
    {
        internal OnlineList<UserType, IdType> OnlineList;

        public UserLog() : base()
        {
        }

        internal UserLog(OnlineList<UserType, IdType> List) : base()
        {
            OnlineList = List;
        }

        public IEnumerable<string> IDs
        {
            get
            {
                return this.Select(x => x.ID);
            }
        }

        /// <summary>
        /// Cria uma entrada no log deste usuário com uma data especifica
        /// </summary>
        /// <param name="Logdata"></param>
        /// <returns></returns>
        public UserLogEntry<UserType, IdType> CreateLog(UserType User, string Message, string URL = null, Dictionary<string, string> LogData = null, DateTime? DateAndTime = default)
        {
            if (User != null && Message.IsNotBlank())
            {
                DateAndTime = DateAndTime ?? DateTime.Now;
                if ((OnlineList[User].LastActivity ?? "") == (Message ?? ""))
                {
                    var lo = OnlineList[User].LastOnline; // nao cria log para locais repedidos dentro do tempo de N minutos
                    if (lo.HasValue && lo.Value.Add(OnlineList.ToleranceTime.Negate()) >= DateAndTime == true)
                    {
                        return null;
                    }
                }

                var d = new UserLogEntry<UserType, IdType>(OnlineList.GetID(User), OnlineList);
                d.DateTime = (DateTime)DateAndTime;
                d.Message = Message;
                d.LogData = LogData;
                d.URL = URL.NullIf(Convert.ToString(URL.IsBlank()));
                OnlineList.Log.Add(d);
                if (OnlineList.OnCreateLog != null)
                {
                    OnlineList.OnCreateLog.Invoke(d);
                }

                return d;
            }

            return null;
        }
    }

    /// <summary>
    /// Entrada de ação do usuário no sistema
    /// </summary>
    /// <typeparam name="UserType"></typeparam>
    /// <typeparam name="IdType"></typeparam>
    public class UserLogEntry<UserType, IdType>
        where UserType : class
        where IdType : struct
    {
        internal OnlineList<UserType, IdType> list;

        internal UserLogEntry(IdType ID, OnlineList<UserType, IdType> list) : base()
        {
            UserID = ID;
            this.list = list;
        }

        /// <summary>
        /// Texto sobre a ocorrencia
        /// </summary>
        /// <returns></returns>
        public string Message { get; set; }

        /// <summary>
        /// Data e hora da ocorrência
        /// </summary>
        /// <returns></returns>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Ultima URL da ocorrencia
        /// </summary>
        /// <returns></returns>
        public string URL { get; set; }

        /// <summary>
        /// Usuário
        /// </summary>
        /// <returns></returns>
        public OnlineUser<UserType, IdType> GetUser()
        {
            return list.UserById(UserID);
        }

        /// <summary>
        /// ID do Usuário
        /// </summary>
        /// <returns></returns>
        public IdType UserID { get; set; }

        /// <summary>
        /// Informações adicionais
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> LogData { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// ID desta entrada
        /// </summary>
        /// <returns></returns>
        public string ID
        {
            get
            {
                return new string[] { GetUser().OnlineList.Log.IndexOf(this).ToString(), "-", GetUser().ID.ToString() }.JoinString("");
            }
        }
    }

    /// <summary>
    /// Usuario Online/Offline
    /// </summary>
    /// <typeparam name="UserType"></typeparam>
    /// <typeparam name="IdType"></typeparam>
    public class OnlineUser<UserType, IdType>
        where UserType : class
        where IdType : struct
    {
        public override string ToString()
        {
            return User.ToString();
        }

        internal OnlineList<UserType, IdType> OnlineList;

        internal OnlineUser(UserType Data, OnlineList<UserType, IdType> list)
        {
            User = Data;
            OnlineList = list;
        }

        /// <summary>
        /// Envia uma mensagem no chat para outro usuario
        /// </summary>
        /// <param name="ToUser"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public UserConversation<UserType, IdType> SendMessage(UserType ToUser, string Message)
        {
            return OnlineList.Chat.Send(User, ToUser, Message);
        }

        /// <summary>
        /// ID deste usuário
        /// </summary>
        /// <returns></returns>
        public IdType ID
        {
            get
            {
                return OnlineList.GetID(User);
            }
        }

        /// <summary>
        /// Indica se o usuario está online ou não
        /// </summary>
        /// <returns></returns>
        public bool IsOnline
        {
            get
            {
                return OnlineList.ContainsUser(User) && LastOnline.HasValue && LastOnline >= DateTime.Now.Add(OnlineList.ToleranceTime) == true;
            }
        }

        /// <summary>
        /// Informações relacionadas do usuário do tipo <typeparamref name="UserType"/>
        /// </summary>
        /// <returns></returns>
        public UserType User { get; private set; }

        /// <summary>
        /// Entradas no Log para este usuário
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserLogEntry<UserType, IdType>> LogEntries()
        {
            return OnlineList?.Log?
                .Where(x => x.UserID.Equals(ID))
                .OrderByDescending(x => x.DateTime).AsEnumerable() ?? new List<UserLogEntry<UserType, IdType>>().AsEnumerable();
        }

        /// <summary>
        /// Ultima atividade do usuário
        /// </summary>
        /// <returns></returns>
        public string LastActivity
        {
            get
            {
                return LogEntries().FirstOrDefault()?.Message.IfBlank("Offline");
            }

            set
            {
                OnlineList.CreateLog(User, value);
            }
        }

        /// <summary>
        /// Ultima vez que o usuário esteve Online
        /// </summary>
        /// <returns></returns>
        public DateTime? LastOnline
        {
            get
            {
                return LogEntries().FirstOrDefault()?.DateTime;
            }
        }

        /// <summary>
        /// Ultima URL/caminho que o usuário acessou
        /// </summary>
        /// <returns></returns>
        public string LastUrl
        {
            get
            {
                return LogEntries().Where(x => x.URL.IsNotBlank()).FirstOrDefault()?.URL;
            }
        }

        /// <summary>
        /// Conversas deste usuário
        /// </summary>
        /// <returns></returns>
        public UserConversation<UserType, IdType>[] Conversations
        {
            get
            {
                return GetConversations();
            }
        }

        /// <summary>
        /// Retorna uma lista de conversas com um usuario específico ou todas
        /// </summary>
        /// <param name="WithUser"></param>
        /// <returns></returns>
        public UserConversation<UserType, IdType>[] GetConversations(UserType WithUser = null)
        {
            return (UserConversation<UserType, IdType>[])OnlineList.Chat.GetConversation(User, WithUser);
        }
    }

    /// <summary>
    /// Lista de conversas entre usuários
    /// </summary>
    /// <typeparam name="UserType"></typeparam>
    /// <typeparam name="IdType"></typeparam>
    public class UserChat<UserType, IdType> : List<UserConversation<UserType, IdType>>
        where UserType : class
        where IdType : struct
    {
        internal OnlineList<UserType, IdType> ChatList;

        public UserChat() : base()
        {
        }

        internal UserChat(OnlineList<UserType, IdType> List) : base()
        {
            ChatList = List;
        }

        public Encoding Encoding { get; set; } = new UTF8Encoding(false);

        public IEnumerable<string> IDs
        {
            get
            {
                return this.Select(x => x.ID);
            }
        }

        /// <summary>
        /// Retorna uma conversa entre 2 usuários
        /// </summary>
        /// <param name="User"></param>
        /// <param name="WithUser"></param>
        /// <returns></returns>
        public IEnumerable<UserConversation<UserType, IdType>> GetConversation(UserType User, UserType WithUser = null)
        {
            UserConversation<UserType, IdType>[] lista;
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
        /// Apaga uma conversa com um usuário
        /// </summary>
        /// <param name="User"></param>
        /// <param name="WithUser"></param>
        public void DeleteConversation(UserType User, UserType WithUser = null)
        {
            UserConversation<UserType, IdType>[] lista;
            lista = (UserConversation<UserType, IdType>[])GetConversation(User, WithUser);
            foreach (var el in lista)
                Remove(el);
        }

        /// <summary>
        /// Envia uma nova mensagem
        /// </summary>
        /// <param name="FromUser"></param>
        /// <param name="ToUser"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public UserConversation<UserType, IdType> Send(UserType FromUser, UserType ToUser, string Message)
        {
            var i = new UserConversation<UserType, IdType>(this) { Message = Message, FromUserID = ChatList.GetID(FromUser), ToUserID = ChatList.GetID(ToUser), ViewedDate = default };
            Add(i);
            return i;
        }
    }

    /// <summary>
    /// Mensagen do chat do usuário
    /// </summary>
    /// <typeparam name="UserType"></typeparam>
    /// <typeparam name="IdType"></typeparam>
    public class UserConversation<UserType, IdType>
        where UserType : class
        where IdType : struct
    {
        internal UserChat<UserType, IdType> chatlist;

        public UserConversation() : base()
        {
        }

        internal UserConversation(UserChat<UserType, IdType> chatlist)
        {
            this.chatlist = chatlist;
        }

        /// <summary>
        /// Id desta mensagem
        /// </summary>
        /// <returns></returns>
        public string ID
        {
            get
            {
                return new[] { "F[", FromUserID.ToString(), "]T[", ToUserID.ToString(), "]@", (object)SentDate.Ticks }.JoinString("");
            }
        }

        /// <summary>
        /// Inndica se esta conversa foi visualizada
        /// </summary>
        /// <returns></returns>
        public bool Viewed
        {
            get
            {
                return ViewedDate.HasValue && ViewedDate <= DateTime.Now == true;
            }

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
        /// Usuario Emissor
        /// </summary>
        /// <returns></returns>
        public OnlineUser<UserType, IdType> FromUser()
        {
            return chatlist.ChatList.UserById(FromUserID);
        }

        /// <summary>
        /// Id do usuário emissor
        /// </summary>
        /// <returns></returns>
        public IdType FromUserID { get; set; }

        /// <summary>
        /// ID do usuário destinatario
        /// </summary>
        /// <returns></returns>
        public IdType ToUserID { get; set; }

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
        /// Usuario destinatário
        /// </summary>
        /// <returns></returns>
        public OnlineUser<UserType, IdType> ToUser()
        {
            return chatlist.ChatList.UserById(ToUserID);
        }

        /// <summary>
        /// Data de visualização da mensagem
        /// </summary>
        /// <returns></returns>
        public DateTime? ViewedDate { get; set; } = default;

        /// <summary>
        /// Retorna a instancia de <see cref="OnlineUser(Of UserType, IdType)"/> (Emissor ou destinatário) de um <see cref="UserType"/> especifico
        /// </summary>
        /// <param name="Myself">Seu usuário</param>
        /// <returns></returns>
        public OnlineUser<UserType, IdType> GetMyUser(UserType MySelf)
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
        /// Retorna a instancia de <see cref="OnlineUser(Of UserType, IdType)"/> (Emissor ou destinatário) de um <see cref="UserType"/> especifico
        /// </summary>
        /// <param name="Myself">Seu usuário</param>
        /// <returns></returns>
        public OnlineUser<UserType, IdType> GetOtherUser(UserType MySelf)
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
        public bool IsFrom(UserType User)
        {
            return chatlist.ChatList.GetID(User).Equals(FromUserID);
        }

        /// <summary>
        /// Retorna true se a mensagem for para <paramref name="User"/>
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public bool IsTo(UserType User)
        {
            return chatlist.ChatList.GetID(User).Equals(ToUserID);
        }
    }

    public class UserConversationBackup
    {
        public string ID { get; set; }
        public string FromUserID { get; set; }
        public string ToUserID { get; set; }
        public string Message { get; set; }
        public DateTime SentDate { get; set; }
        public DateTime? ViewedDate { get; set; }
    }

    public class LogEntryBackup
    {
        public string ID { get; set; }
        public string UserID { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
        public DateTime DateTime { get; set; }
        public Dictionary<string, string> LogData { get; set; }
    }
}