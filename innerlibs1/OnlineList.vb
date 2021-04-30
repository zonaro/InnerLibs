Imports System.Dynamic
Imports System.IO
Imports System.Text

Imports InnerLibs.LINQ

Namespace Online

    ''' <summary>
    ''' <see cref="Dictionary(Of UserType, IdType)"/> utilizado para controle de usuários que estão online/offline em uma aplicação
    ''' </summary>
    ''' <typeparam name="UserType">Tipo da Classe do usuário</typeparam>
    ''' <typeparam name="IdType">Tipo do ID do usuário</typeparam>
    Public Class OnlineList(Of UserType As Class, IdType As Structure)
        Inherits Dictionary(Of IdType, OnlineUser(Of UserType, IdType))

        Private idgetter As Func(Of UserType, IdType)

        Private _tolerancetime As TimeSpan = New TimeSpan(0, 1, 0)


        ''' <summary>
        ''' Retorna o ID do usuário
        ''' </summary>
        ''' <param name="User"></param>
        ''' <returns></returns>
        Public Function GetID(User As UserType) As IdType
            Return idgetter(User)
        End Function

        ''' <summary>
        ''' Função que será executada quando ocorrer uma entrada no log
        ''' </summary>
        ''' <returns></returns>
        Public Property OnCreateLog As Action(Of UserLogEntry(Of UserType, IdType)) = Sub(x) Debug.WriteLine("Log entry created for " & x.GetUser().ID.ToString())

        Public Property OnUserOnlineChanged As Action(Of OnlineUser(Of UserType, IdType)) = Sub(x) Debug.WriteLine("User Updated -> " & x.ID.ToString())


        ''' <summary>
        ''' Cria uma nova instancia de OnlineList apontando a propriedade do ID do usuario  e opcionalmente
        ''' </summary>
        ''' <param name="IdProperty">Expressão lambda que indica qual propriedade da classe <see cref="UserType"/> é o ID de tipo <see cref="IdType"/></param>
        Sub New(IdProperty As Func(Of UserType, IdType))
            idgetter = IdProperty
        End Sub


        ''' <summary>
        ''' Lista de conversas dos usuários
        ''' </summary>
        ''' <returns></returns>

        Public ReadOnly Property Chat As New UserChat(Of UserType, IdType)(Me)

        ''' <summary>
        ''' Entradas de ações dos usuários
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Log As New UserLog(Of UserType, IdType)(Me)


        ''' <summary>
        ''' Diretorio onde serão guardados os XMLs deta lista
        ''' </summary>
        ''' <returns></returns>
        Property BackupDirectory As DirectoryInfo = Nothing

        ''' <summary>
        ''' Caminho do arquivo XML do Log
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property LogFile As FileInfo
            Get
                If BackupDirectory IsNot Nothing Then
                    BackupDirectory = BackupDirectory.FullName.ToDirectoryInfo()
                End If
                Return New FileInfo(Path.Combine(BackupDirectory.FullName, "LOG.XML"))
            End Get
        End Property

        ''' <summary>
        ''' Caminho do arquivo XML do Chat
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property ChatFile As FileInfo
            Get
                If BackupDirectory IsNot Nothing Then
                    BackupDirectory = BackupDirectory.FullName.ToDirectoryInfo()
                End If
                Return New FileInfo(Path.Combine(BackupDirectory.FullName, "CHAT.XML"))
            End Get
        End Property

        Public Sub SaveLogXML()

            Log.Select(Function(x)
                           Dim bkp = New LogEntryBackup()
                           bkp.DateTime = x.DateTime
                           bkp.Message = x.Message
                           bkp.UserID = x.UserID.ToString()
                           bkp.ID = x.ID
                           bkp.LogData = x.LogData
                           Return bkp
                       End Function).ToList().CreateXmlFile(LogFile.FullName)
        End Sub

        Public Sub SaveChatXML()
            Chat.Select(Function(x)
                            Dim bkp = New UserConversationBackup()
                            bkp.SentDate = x.SentDate
                            bkp.Message = x.Message
                            bkp.FromUserID = x.FromUserID.ToString()
                            bkp.ToUserID = x.ToUserID.ToString()
                            bkp.ViewedDate = x.ViewedDate
                            Return bkp
                        End Function).ToList().CreateXmlFile(ChatFile.FullName)
        End Sub

        Public Sub OpenLogXML()
            If LogFile.Exists Then
                Dim x = ClassTools.CreateObjectFromXMLFile(Of LogEntryBackup())(LogFile)
                For Each ii In x
                    If ii.ID.IsNotBlank() AndAlso ii.ID.IsNotIn(Log.IDs) Then
                        Dim usu_id = Convert.ChangeType(ii.ID, GetType(IdType))
                        Dim log = New UserLogEntry(Of UserType, IdType)(usu_id, Me)
                        log.Message = ii.Message
                        log.DateTime = ii.DateTime
                        log.URL = ii.Url
                        log.LogData = ii.LogData
                        Me.Log.Add(log)
                    End If
                Next
            End If
        End Sub

        Public Sub OpenChatXML()
            If ChatFile.Exists Then
                Dim x = ClassTools.CreateObjectFromXMLFile(Of UserConversationBackup())(ChatFile)
                For Each ii In x
                    If ii.ID.IsNotIn(Chat.IDs) Then
                        Dim cvn = New UserConversation(Of UserType, IdType)
                        cvn.chatlist = Chat
                        cvn.FromUserID = ii.FromUserID.ChangeType(Of IdType)
                        cvn.ToUserID = ii.ToUserID.ChangeType(Of IdType)
                        cvn.SentDate = ii.SentDate
                        cvn.ViewedDate = ii.ViewedDate
                        cvn.Message = ii.Message
                        Chat.Add(cvn)
                    End If
                Next
            End If
        End Sub

        ''' <summary>
        ''' Tolerancia que o servidor consifera um usuário online
        ''' </summary>
        ''' <returns></returns>
        Public Property ToleranceTime As TimeSpan
            Get
                If _tolerancetime.Ticks > 0 Then
                    Return _tolerancetime.Negate
                Else
                    Return _tolerancetime
                End If
            End Get
            Set(value As TimeSpan)
                _tolerancetime = value
            End Set
        End Property

        ''' <summary>
        ''' Retorna um OnlineUser a partir de uma instancia de <typeparamref name="UserType"/>
        ''' </summary>
        ''' <param name="User"></param>
        ''' <returns></returns>
        Function GetUser(User As UserType) As OnlineUser(Of UserType, IdType)
            If Me.ContainsUser(User) Then
                Return MyBase.Item(GetID(User))
            Else
                Return Me.Add(User)
            End If
        End Function

        ''' <summary>
        ''' Retorna um usuario de acordo com seu ID
        ''' </summary>
        ''' <param name="User"></param>
        ''' <returns></returns>
        Default Overloads Property Item(User As UserType) As OnlineUser(Of UserType, IdType)
            Get
                Return GetUser(User)
            End Get
            Set(value As OnlineUser(Of UserType, IdType))
                value = Me.Add(value.User)
            End Set
        End Property

        ''' <summary>
        ''' Adciona um usuario a esta lista
        ''' </summary>
        ''' <param name="User"></param>
        ''' <returns></returns>
        Public Shadows Function Add(User As UserType) As OnlineUser(Of UserType, IdType)
            Return Me.Add(User, Nothing, Nothing, Nothing)
        End Function

        ''' <summary>
        ''' Adciona varios usuarios a esta lista
        ''' </summary>
        ''' <param name="Users"></param>
        ''' <returns></returns>
        Public Shadows Function Add(Users As IEnumerable(Of UserType)) As IEnumerable(Of OnlineUser(Of UserType, IdType))
            Return Me.AddMany(Users.ToArray())
        End Function

        ''' <summary>
        ''' Adciona varios usuarios a esta lista
        ''' </summary>
        ''' <returns></returns>
        Public Function AddMany(ParamArray Users As UserType()) As IEnumerable(Of OnlineUser(Of UserType, IdType))
            Dim l = New List(Of OnlineUser(Of UserType, IdType))
            For Each u In Users
                l.Add(Me.Add(u, Nothing, Nothing, Nothing))
            Next
            Return l
        End Function


        ''' <summary>
        ''' Adciona um usuario a esta lista
        ''' </summary>
        ''' <param name="Obj"></param>
        ''' <returns></returns>
        Public Shadows Function Add(Obj As UserType, Optional Online As Boolean? = Nothing, Optional Activity As String = Nothing, Optional Url As String = Nothing, Optional LogData As Object = Nothing, Optional DateTime As DateTime? = Nothing) As OnlineUser(Of UserType, IdType)
            If Obj IsNot Nothing Then
                Dim ID = Me.GetID(Obj)
                If Not Me.ContainsKey(ID) Then
                    MyBase.Item(ID) = New OnlineUser(Of UserType, IdType)(Obj, Me)
                End If
                If Online.HasValue Then
                    If Activity.IsNotBlank Then
                        Dim dt = If(DateTime, System.DateTime.Now)
                        CreateLog(Obj, Activity, Url, LogData, dt)
                    End If
                    If MyBase.Item(ID).IsOnline <> Online Then
                        If Me.OnUserOnlineChanged IsNot Nothing Then
                            Me.OnUserOnlineChanged.Invoke(MyBase.Item(ID))
                        End If
                    End If
                End If
                Return MyBase.Item(ID)
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Verifica se um usuario está nesta lista
        ''' </summary>
        ''' <returns></returns>
        Public Function ContainsUser(User As UserType) As Boolean
            Return Me.ContainsKey(GetID(User))
        End Function

        ''' <summary>
        ''' Retorna todos os <see cref="OnlineUser(Of UserType, IdType)"/> que estão Online no momento
        ''' </summary>
        ''' <returns></returns>
        Public Function OnlineUsers() As IEnumerable(Of OnlineUser(Of UserType, IdType))
            Return Users.Where(Function(x) x.IsOnline = True)
        End Function

        ''' <summary>
        ''' Retorna todos os <see cref="OnlineUser(Of UserType, IdType)"/> que estão Offline
        ''' </summary>
        ''' <returns></returns>
        Public Function OfflineUsers() As IEnumerable(Of OnlineUser(Of UserType, IdType))
            Return Users.Where(Function(x) x.IsOnline = False)
        End Function

        ''' <summary>
        ''' Retorna todos os <see cref="OnlineUser(Of UserType, IdType)"/>
        ''' </summary>
        ''' <returns></returns>
        Public Function Users() As IEnumerable(Of OnlineUser(Of UserType, IdType))
            Return Me.Select(Function(x) x.Value)
        End Function

        ''' <summary>
        ''' Retorna todo os usuarios
        ''' </summary>
        ''' <param name="IsOnline">True para online, false para offline, null para todos</param>
        ''' <returns></returns>
        Public Function GetUsersData(Optional IsOnline As Boolean? = Nothing) As IEnumerable(Of UserType)
            Return Me.Users().Where(Function(x) x.IsOnline = If(IsOnline, x.IsOnline)).Select(Function(x) x.User)
        End Function

        ''' <summary>
        ''' Remove um usuário desta lista
        ''' </summary>
        ''' <param name="Obj"></param>
        Public Shadows Sub Remove(ParamArray Obj As UserType())
            Obj = If(Obj, {})
            Me.RemoveIfExist(Obj.Select(Function(x) GetID(x)).ToArray)
        End Sub

        ''' <summary>
        ''' Remove um usuário desta lista a partir do ID
        ''' </summary>
        ''' <param name="ID"></param>
        Public Shadows Sub Remove(ID As IdType)
            Me.RemoveIfExist(ID)
        End Sub

        ''' <summary>
        ''' Seta um usuario como offline e cria uma entrada no log
        ''' </summary>
        ''' <param name="Obj"></param>
        ''' <returns></returns>
        Public Function SetOffline(Obj As UserType) As OnlineUser(Of UserType, IdType)
            Return Me.Add(Obj, False, "Offline", Nothing, Nothing, Now)
        End Function

        ''' <summary>
        ''' Seta um usuario como online e atribui uma atividade a ele. Cria entrada no log automaticamente
        ''' </summary>
        ''' <param name="Obj"></param>
        ''' <returns></returns>
        Public Function SetOnlineActivity(Obj As UserType, Activity As String, Optional Url As String = Nothing, Optional LogData As Object = Nothing, Optional DateTime As Date? = Nothing) As OnlineUser(Of UserType, IdType)
            Return Me.Add(Obj, True, Activity, Url, LogData, DateTime)
        End Function

        ''' <summary>
        ''' Seta um usuario como online e cria uma entrada no Log
        ''' </summary>
        ''' <param name="Obj"></param>
        ''' <returns></returns>
        Public Function SetOnline(Obj As UserType) As OnlineUser(Of UserType, IdType)
            Return Me.Add(Obj, True, "Online", Nothing, Nothing, Now)
        End Function

        ''' <summary>
        ''' Mantém um usuario online mas não atribui nenhuma nova atividade nem cria entradas no LOG
        ''' </summary>
        ''' <param name="Obj"></param>
        ''' <returns></returns>
        Public Function KeepOnline(Obj As UserType) As OnlineUser(Of UserType, IdType)
            Return Me.Add(Obj, True)
        End Function

        ''' <summary>
        ''' Retorna um usuário desta lista a partir do ID
        ''' </summary>
        ''' <param name="Key"></param>
        ''' <returns></returns>
        Function UserById(Key As IdType) As OnlineUser(Of UserType, IdType)
            If Me.ContainsKey(Key) Then
                Return MyBase.Item(Key)
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Retorna um usuário desta lista a partir do ID
        ''' </summary>
        ''' <param name="Key"></param>
        ''' <returns></returns>
        Function UserDataById(Key As IdType) As UserType
            Return UserById(Key)?.User
        End Function



        ''' <summary>
        ''' Cria uma entrada no log deste usuário
        ''' </summary>
        ''' <param name="Logdata"></param>
        ''' <returns></returns>
        Public Function CreateLog(User As UserType, Message As String, Optional URL As String = Nothing, Optional LogData As Object = Nothing, Optional DateTime As Date? = Nothing) As UserLogEntry(Of UserType, IdType)
            Return Log.CreateLog(User, Message, URL, LogData, Now)
        End Function

    End Class

    Public Class UserLog(Of UserType As Class, IdType As Structure)
        Inherits List(Of UserLogEntry(Of UserType, IdType))

        Friend OnlineList As OnlineList(Of UserType, IdType)

        Sub New()
            MyBase.New()
        End Sub

        Friend Sub New(List As OnlineList(Of UserType, IdType))
            MyBase.New
            Me.OnlineList = List
        End Sub

        Public ReadOnly Property IDs As IEnumerable(Of String)
            Get
                Return Me.Select(Function(x) x.ID)
            End Get
        End Property



        ''' <summary>
        ''' Cria uma entrada no log deste usuário com uma data especifica
        ''' </summary>
        ''' <param name="Logdata"></param>
        ''' <returns></returns>
        Public Function CreateLog(User As UserType, Message As String, Optional URL As String = Nothing, Optional LogData As Object = Nothing, Optional DateAndTime As Date? = Nothing) As UserLogEntry(Of UserType, IdType)
            If User IsNot Nothing AndAlso Message.IsNotBlank Then
                DateAndTime = If(DateAndTime, Now)
                Dim d = New UserLogEntry(Of UserType, IdType)(OnlineList.GetID(User), Me.OnlineList)
                d.DateTime = DateAndTime
                d.Message = Message
                d.LogData = LogData
                d.URL = URL.NullIf(URL.IsBlank())
                Me.OnlineList.Log.Add(d)
                If Me.OnlineList.OnCreateLog IsNot Nothing Then
                    Me.OnlineList.OnCreateLog.Invoke(d)
                End If
                Return d
            End If
            Return Nothing
        End Function

    End Class

    ''' <summary>
    ''' Entrada de ação do usuário no sistema
    ''' </summary>
    ''' <typeparam name="UserType"></typeparam>
    ''' <typeparam name="IdType"></typeparam>
    Public Class UserLogEntry(Of UserType As Class, IdType As Structure)

        Friend list As OnlineList(Of UserType, IdType)



        Friend Sub New(ID As IdType, list As OnlineList(Of UserType, IdType))
            MyBase.New
            Me.UserID = ID
            Me.list = list
        End Sub

        ''' <summary>
        ''' Texto sobre a ocorrencia
        ''' </summary>
        ''' <returns></returns>
        Public Property Message As String

        ''' <summary>
        ''' Data e hora da ocorrência
        ''' </summary>
        ''' <returns></returns>
        Public Property DateTime As DateTime

        ''' <summary>
        ''' Ultima URL da ocorrencia
        ''' </summary>
        ''' <returns></returns>
        Public Property URL As String

        ''' <summary>
        ''' Usuário
        ''' </summary>
        ''' <returns></returns>
        Public Function GetUser() As OnlineUser(Of UserType, IdType)
            Return Me.list.UserById(UserID)
        End Function

        ''' <summary>
        ''' ID do Usuário
        ''' </summary>
        ''' <returns></returns>
        Public Property UserID As IdType

        ''' <summary>
        ''' Informações adicionais
        ''' </summary>
        ''' <returns></returns>
        Public Property LogData As New ExpandoObject

        ''' <summary>
        ''' ID desta entrada
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property ID As String
            Get
                Return New String() {GetUser.OnlineList.Log.IndexOf(Me).ToString(), "-", GetUser.ID.ToString()}.Join("")
            End Get
        End Property

    End Class

    ''' <summary>
    ''' Usuario Online/Offline
    ''' </summary>
    ''' <typeparam name="UserType"></typeparam>
    ''' <typeparam name="IdType"></typeparam>
    Public Class OnlineUser(Of UserType As Class, IdType As Structure)

        Public Overrides Function ToString() As String
            Return User.ToString()
        End Function

        Friend OnlineList As OnlineList(Of UserType, IdType)

        Private online As Boolean = True

        Friend Sub New(Data As UserType, list As OnlineList(Of UserType, IdType))
            Me.User = Data
            Me.OnlineList = list
        End Sub

        ''' <summary>
        ''' Envia uma mensagem no chat para outro usuario
        ''' </summary>
        ''' <param name="ToUser"></param>
        ''' <param name="Message"></param>
        ''' <returns></returns>
        Function SendMessage(ToUser As UserType, Message As String) As UserConversation(Of UserType, IdType)
            Return Me.OnlineList.Chat.Send(Me.User, ToUser, Message)
        End Function

        ''' <summary>
        ''' ID deste usuário
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property ID As IdType
            Get
                Return OnlineList.GetID(Me.User)
            End Get
        End Property

        ''' <summary>
        ''' Indica se o usuario está online ou não
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property IsOnline As Boolean
            Get
                If OnlineList.ContainsUser(Me.User) Then
                    If online Then
                        Dim d = DateTime.Now
                        online = LastOnline >= d.Add(OnlineList.ToleranceTime)
                    End If
                    Return online
                End If
                Return False
            End Get
        End Property

        ''' <summary>
        ''' Informações relacionadas do usuário do tipo <typeparamref name="UserType"/>
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property User As UserType


        ''' <summary>
        ''' Entradas no Log para este usuário
        ''' </summary>
        ''' <returns></returns>
        Public Function LogEntries() As IEnumerable(Of UserLogEntry(Of UserType, IdType))
            Return If(Me.OnlineList?.Log?.Where(Function(x) x.UserID.Equals(Me.ID)).OrderByDescending(Function(x) x.DateTime), New List(Of UserLogEntry(Of UserType, IdType)))
        End Function

        ''' <summary>
        ''' Ultima atividade do usuário
        ''' </summary>
        ''' <returns></returns>
        Property LastActivity As String
            Get
                Return Me.LogEntries().FirstOrDefault()?.Message.IfBlank("Offline")
            End Get
            Set(value As String)
                Me.OnlineList.CreateLog(Me.User, value)
            End Set
        End Property



        ''' <summary>
        ''' Ultima vez que o usuário esteve Online
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property LastOnline As Date?
            Get
                Return LogEntries().FirstOrDefault()?.DateTime
            End Get
        End Property

        ''' <summary>
        ''' Ultima URL/caminho que o usuário acessou
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property LastUrl As String
            Get
                Return LogEntries().Where(Function(x) x.URL.IsNotBlank).FirstOrDefault()?.URL
            End Get
        End Property

        ''' <summary>
        ''' Conversas deste usuário
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property Conversations As UserConversation(Of UserType, IdType)()
            Get
                Return GetConversations()
            End Get
        End Property

        ''' <summary>
        ''' Retorna uma lista de conversas com um usuario específico ou todas
        ''' </summary>
        ''' <param name="WithUser"></param>
        ''' <returns></returns>
        Public Function GetConversations(Optional WithUser As UserType = Nothing) As UserConversation(Of UserType, IdType)()
            Return OnlineList.Chat.GetConversation(Me.User, WithUser)
        End Function

    End Class

    ''' <summary>
    ''' Lista de conversas entre usuários
    ''' </summary>
    ''' <typeparam name="UserType"></typeparam>
    ''' <typeparam name="IdType"></typeparam>
    Public Class UserChat(Of UserType As Class, IdType As Structure)
        Inherits List(Of UserConversation(Of UserType, IdType))

        Friend ChatList As OnlineList(Of UserType, IdType)

        Sub New()
            MyBase.New()
        End Sub

        Friend Sub New(List As OnlineList(Of UserType, IdType))
            MyBase.New
            Me.ChatList = List
        End Sub

        Property Encoding As Encoding = Encoding.UTF8

        Public ReadOnly Property IDs As IEnumerable(Of String)
            Get
                Return Me.Select(Function(x) x.ID)
            End Get
        End Property

        ''' <summary>
        ''' Retorna uma conversa entre 2 usuários
        ''' </summary>
        ''' <param name="User"></param>
        ''' <param name="WithUser"></param>
        ''' <returns></returns>
        Function GetConversation(User As UserType, Optional WithUser As UserType = Nothing) As IEnumerable(Of UserConversation(Of UserType, IdType))
            Dim lista As UserConversation(Of UserType, IdType)()
            If WithUser IsNot Nothing Then
                lista = Me.Where(Function(x) (ChatList.GetID(User).Equals(x.FromUserID) AndAlso ChatList.GetID(WithUser).Equals(ChatList.GetID(x.ToUser.User))) Or (ChatList.GetID(User).Equals(x.ToUserID) AndAlso ChatList.GetID(WithUser).Equals(x.FromUserID))).ToArray
            Else
                lista = Me.Where(Function(x) (ChatList.GetID(User).Equals(x.FromUserID)) OrElse (ChatList.GetID(User).Equals(x.ToUserID))).ToArray
            End If
            lista = lista.DistinctBy(Function(x) x.ID).OrderByDescending(Function(x) x.SentDate).ToArray
            Return lista
        End Function

        ''' <summary>
        ''' Apaga uma conversa com um usuário
        ''' </summary>
        ''' <param name="User"></param>
        ''' <param name="WithUser"></param>
        Sub DeleteConversation(User As UserType, Optional WithUser As UserType = Nothing)
            Dim lista As UserConversation(Of UserType, IdType)()
            lista = GetConversation(User, WithUser)
            For Each el In lista
                Me.Remove(el)
            Next
        End Sub

        ''' <summary>
        ''' Envia uma nova mensagem
        ''' </summary>
        ''' <param name="FromUser"></param>
        ''' <param name="ToUser"></param>
        ''' <param name="Message"></param>
        ''' <returns></returns>
        Function Send(FromUser As UserType, ToUser As UserType, Message As String) As UserConversation(Of UserType, IdType)
            Dim i = New UserConversation(Of UserType, IdType)(Me) With {.Message = Message, .FromUserID = ChatList.GetID(FromUser), .ToUserID = ChatList.GetID(ToUser), .ViewedDate = Nothing}
            Me.Add(i)
            Return i
        End Function

    End Class

    ''' <summary>
    ''' Mensagen do chat do usuário
    ''' </summary>
    ''' <typeparam name="UserType"></typeparam>
    ''' <typeparam name="IdType"></typeparam>
    Public Class UserConversation(Of UserType As Class, IdType As Structure)

        Friend chatlist As UserChat(Of UserType, IdType)

        Sub New()
            MyBase.New()
        End Sub

        Friend Sub New(chatlist As UserChat(Of UserType, IdType))
            Me.chatlist = chatlist
        End Sub

        ''' <summary>
        ''' Id desta mensagem
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property ID As String
            Get
                Return {"F[", FromUserID.ToString, "]T[", ToUserID.ToString, "]@", SentDate.Ticks}.Join("")
            End Get
        End Property

        ''' <summary>
        ''' Inndica se esta conversa foi visualizada
        ''' </summary>
        ''' <returns></returns>
        Property Viewed As Boolean
            Get
                Return ViewedDate.HasValue AndAlso ViewedDate <= DateTime.Now
            End Get
            Set(value As Boolean)
                If value Then
                    ViewedDate = DateTime.Now
                Else
                    ViewedDate = Nothing
                End If
            End Set
        End Property

        ''' <summary>
        ''' Usuario Emissor
        ''' </summary>
        ''' <returns></returns>
        Public Function FromUser() As OnlineUser(Of UserType, IdType)
            Return Me.chatlist.ChatList.UserById(FromUserID)
        End Function

        ''' <summary>
        ''' Id do usuário emissor
        ''' </summary>
        ''' <returns></returns>
        Public Property FromUserID As IdType

        ''' <summary>
        ''' ID do usuário destinatario
        ''' </summary>
        ''' <returns></returns>
        Public Property ToUserID As IdType

        ''' <summary>
        ''' Mensagem
        ''' </summary>
        ''' <returns></returns>
        Property Message As String

        ''' <summary>
        ''' Data de envio da mensagem
        ''' </summary>
        ''' <returns></returns>
        Property SentDate As DateTime = DateTime.Now

        ''' <summary>
        ''' Usuario destinatário
        ''' </summary>
        ''' <returns></returns>
        Public Function ToUser() As OnlineUser(Of UserType, IdType)
            Return Me.chatlist.ChatList.UserById(ToUserID)
        End Function

        ''' <summary>
        ''' Data de visualização da mensagem
        ''' </summary>
        ''' <returns></returns>
        Property ViewedDate As Date? = Nothing

        ''' <summary>
        ''' Retorna a instancia de <see cref="OnlineUser(Of UserType, IdType)"/> (Emissor ou destinatário) de um <see cref="UserType"/> especifico
        ''' </summary>
        ''' <param name="Myself">Seu usuário</param>
        ''' <returns></returns>
        Public Function GetMyUser(MySelf As UserType) As OnlineUser(Of UserType, IdType)
            If IsFrom(MySelf) Then
                Return FromUser()
            End If
            If IsTo(MySelf) Then
                Return ToUser()
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Retorna a instancia de <see cref="OnlineUser(Of UserType, IdType)"/> (Emissor ou destinatário) de um <see cref="UserType"/> especifico
        ''' </summary>
        ''' <param name="Myself">Seu usuário</param>
        ''' <returns></returns>
        Public Function GetOtherUser(MySelf As UserType) As OnlineUser(Of UserType, IdType)
            If IsFrom(MySelf) Then
                Return ToUser()
            End If
            If IsTo(MySelf) Then
                Return FromUser()
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Retorna true se a mensagem for de <paramref name="User"/>
        ''' </summary>
        ''' <param name="User"></param>
        ''' <returns></returns>
        Function IsFrom(User As UserType) As Boolean
            Return chatlist.ChatList.GetID(User).Equals(FromUserID)
        End Function

        ''' <summary>
        ''' Retorna true se a mensagem for para <paramref name="User"/>
        ''' </summary>
        ''' <param name="User"></param>
        ''' <returns></returns>
        Function IsTo(User As UserType) As Boolean
            Return chatlist.ChatList.GetID(User).Equals(ToUserID)
        End Function

    End Class

    Public Class UserConversationBackup

        Property ID As String
        Property FromUserID As String

        Property ToUserID As String

        Property Message As String

        Property SentDate As Date
        Property ViewedDate As Date?

    End Class

    Public Class LogEntryBackup

        Property ID As String

        Property UserID As String

        Property Message As String
        Property Url As String
        Property DateTime As DateTime

        Property LogData As Object

    End Class

End Namespace