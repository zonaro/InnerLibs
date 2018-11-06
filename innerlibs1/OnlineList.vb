Imports System.IO
Imports System.Text
Imports System.Web
Imports System.Web.Script.Serialization
Imports System.Web.UI
Imports InnerLibs.LINQ

Public Class OnlineList(Of UserType As Class, IdType As Structure)
    Inherits Dictionary(Of IdType, OnlineUser(Of UserType, IdType))

    Friend idgetter As Func(Of UserType, IdType)

    Private _tolerancetime As TimeSpan = New TimeSpan(0, 1, 0)

    Sub New(IdProperty As Func(Of UserType, IdType))
        idgetter = IdProperty
    End Sub

    ReadOnly Property Chat As New UserChat(Of UserType, IdType)(Function(x) idgetter(x), Me)

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

    Default Shadows ReadOnly Property Item(User As UserType) As OnlineUser(Of UserType, IdType)
        Get
            If Me.ContainsUser(User) Then
                Return MyBase.Item(idgetter(User))
            Else
                Return Me.Add(User)
            End If
        End Get
    End Property

    Public Shadows Function Add(Obj As UserType) As OnlineUser(Of UserType, IdType)
        If Obj IsNot Nothing Then
            Dim ID = Me.idgetter(Obj)
            If Not Me.ContainsKey(ID) Then
                MyBase.Item(ID) = New OnlineUser(Of UserType, IdType)(Obj, Me)
                MyBase.Item(ID).IsOnline = False
            End If
            Return MyBase.Item(ID)
        End If
        Return Nothing
    End Function

    Public Shadows Function Add(Obj As UserType, Optional Online As Boolean = False, Optional Activity As String = Nothing) As OnlineUser(Of UserType, IdType)
        If Obj IsNot Nothing Then
            Dim ID = Me.idgetter(Obj)
            If Not Me.ContainsKey(ID) Then
                MyBase.Item(ID) = New OnlineUser(Of UserType, IdType)(Obj, Me)
            End If
            If Online Then
                MyBase.Item(ID).LastOnline = Now
                If Activity.IsNotBlank Then
                    MyBase.Item(ID).LastActivity = Activity
                End If
                If HttpContext.Current IsNot Nothing Then
                    MyBase.Item(ID).LastUrl = HttpContext.Current.Request.Url.AbsoluteUri
                    Dim Page = HttpContext.Current.Handler
                    If Page IsNot Nothing AndAlso Page.GetType Is GetType(Page) Then
                        Dim title = CType(Page, Page).Title
                        If title.IsNotBlank Then
                            MyBase.Item(ID).LastPage = Page
                        End If

                    End If
                End If
            End If
            MyBase.Item(ID).IsOnline = Online
            Return MyBase.Item(ID)
        End If
        Return Nothing
    End Function

    Public Function ContainsUser(User As UserType) As Boolean
        Return Me.ContainsKey(idgetter(User))
    End Function

    Public Function OnlineUsers() As IEnumerable(Of OnlineUser(Of UserType, IdType))
        Return Me.Where(Function(x) x.Value.IsOnline).Select(Function(x) x.Value)
    End Function

    Public Shadows Sub Remove(ParamArray Obj As UserType())
        Obj = If(Obj, {})
        Me.RemoveIfExist(Obj.Select(Function(x) idgetter(x)).ToArray)
    End Sub

    Public Function SetOffline(Obj As UserType) As OnlineUser(Of UserType, IdType)
        Return Me.Add(Obj, False)
    End Function

    Public Function SetOnline(Obj As UserType, Optional Activity As String = Nothing) As OnlineUser(Of UserType, IdType)
        Return Me.Add(Obj, True, Activity)
    End Function

    Function UserById(Key As IdType) As UserType
        If Me.ContainsKey(Key) Then
            Return MyBase.Item(Key).Data
        End If
        Return Nothing
    End Function

End Class

Public Class OnlineUser(Of UserType As Class, IdType As Structure)

    Friend list As OnlineList(Of UserType, IdType)

    Private online As Boolean = True

    Friend Sub New(Data As UserType, list As OnlineList(Of UserType, IdType))
        Me.Data = Data
        Me.list = list
        Me.IsOnline = False
    End Sub

    Function SendMessage(ToUser As UserType, Message As String) As UserConversation(Of UserType, IdType)
        Return Me.list.Chat.Send(Me.Data, ToUser, Message)
    End Function

    ReadOnly Property ID As IdType
        Get
            Return list.idgetter(Me.Data)
        End Get
    End Property

    Property IsOnline As Boolean
        Get
            If list.ContainsUser(Me.Data) Then
                If online Then
                    online = LastOnline.HasValue AndAlso LastOnline >= Now.Add(list.ToleranceTime)
                    If online Then
                        LastOnline = Now
                    End If
                End If
                Return online
            End If
            Return False
        End Get
        Set(value As Boolean)
            If online Then
                LastOnline = Now
            End If
            online = value
        End Set
    End Property

    <ScriptIgnore> ReadOnly Property Data As UserType
    Property LastActivity As String = "Offline"
    Property LastOnline As Date? = Nothing

    <ScriptIgnore> Property LastPage As Page
    Property LastUrl As String

    ReadOnly Property Conversations(Optional WithUser As UserType = Nothing) As UserConversation(Of UserType, IdType)()
        Get
            If WithUser IsNot Nothing Then
                Return list.Chat.GetConversation(Me.Data, WithUser)
            Else
                Return list.Chat.Where(Function(x) list.idgetter(x.FromUser.Data).Equals(list.idgetter(Me.Data)) Or list.idgetter(x.ToUser.Data).Equals(list.idgetter(Me.Data))).ToArray
            End If
        End Get
    End Property

End Class

Public Class UserChat(Of UserType As Class, IdType As Structure)
    Inherits List(Of UserConversation(Of UserType, IdType))

    Friend WithEvents BackupTimer As New System.Timers.Timer
    Friend BackupPath As FileInfo
    Friend list As OnlineList(Of UserType, IdType)

    Private idgetter As Func(Of UserType, IdType)

    Friend Sub New(IdProperty As Func(Of UserType, IdType), List As OnlineList(Of UserType, IdType))
        MyBase.New
        Me.idgetter = IdProperty
        Me.list = List
        Me.BackupTimer = New Timers.Timer
    End Sub

    Property Encoding As Encoding = Encoding.UTF8

    Function Backup() As Byte()
        Dim str = Me.Select(Function(x) New UserConversationBackup(Of IdType) With {.FromId = x.FromUser.ID, .ToId = x.ToUser.ID, .Message = x.Message.InnCrypt, .SentDate = x.SentDate.Ticks, .ViewedDate = If(x.ViewedDate.HasValue, x.ViewedDate.Value.Ticks, -1)}).SerializeJSON
        Return Encoding.GetBytes(str)
    End Function

    Function Backup(Path As String) As FileInfo
        Return Backup().WriteToFile(Path)
    End Function

    Function GetConversation(User As UserType, Optional WithUser As UserType = Nothing) As IEnumerable(Of UserConversation(Of UserType, IdType))
        Dim lista As UserConversation(Of UserType, IdType)()
        If WithUser IsNot Nothing Then
            lista = Me.Where(Function(x) (idgetter(User).Equals(idgetter(x.FromUser.Data)) AndAlso idgetter(WithUser).Equals(idgetter(x.ToUser.Data))) Or (idgetter(User).Equals(idgetter(x.ToUser.Data)) AndAlso idgetter(WithUser).Equals(idgetter(x.FromUser.Data)))).ToArray
        Else
            lista = Me.Where(Function(x) (idgetter(User).Equals(idgetter(x.FromUser.Data))) Or (idgetter(User).Equals(idgetter(x.ToUser.Data)))).ToArray
        End If
        lista = lista.DistinctBy(Function(x) x.ID).OrderByDescending(Function(x) x.SentDate).ToArray
        Return lista
    End Function


    Sub DeleteConversation(User As UserType, Optional WithUser As UserType = Nothing)
        Dim lista As UserConversation(Of UserType, IdType)()
        If WithUser IsNot Nothing Then
            lista = Me.Where(Function(x) (idgetter(User).Equals(idgetter(x.FromUser.Data)) AndAlso idgetter(WithUser).Equals(idgetter(x.ToUser.Data))) Or (idgetter(User).Equals(idgetter(x.ToUser.Data)) AndAlso idgetter(WithUser).Equals(idgetter(x.FromUser.Data)))).ToArray
        Else
            lista = Me.Where(Function(x) (idgetter(User).Equals(idgetter(x.FromUser.Data))) Or (idgetter(User).Equals(idgetter(x.ToUser.Data)))).ToArray
        End If
        For Each el In lista
            Me.Remove(el)
        Next
    End Sub

    Sub Restore(Backup As Byte())
        Dim backupstring = Encoding.GetString(Backup)
        Dim obj = backupstring.ParseJSON(Of IEnumerable(Of UserConversationBackup(Of IdType)))
        Dim conversas = obj.Where(Function(x) Me.list.ContainsKey(x.FromId) AndAlso Me.list.ContainsKey(x.ToId)).Select(Function(x) New UserConversation(Of UserType, IdType)(Me) With {.FromUser = Me.list(Me.list.UserById(x.FromId)), .ToUser = Me.list(Me.list.UserById(x.ToId)), .Message = x.Message.UnnCrypt, .SentDate = New Date(x.SentDate), .ViewedDate = If(x.ViewedDate = -1, Nothing, New Date(x.ViewedDate))}).ToArray
        Dim datafinal = conversas.Max(Function(x) x.SentDate)
        Me.AddRange(conversas.Where(Function(x) x.ID.IsNotIn(Me.Select(Function(y) y.ID))).ToArray)
    End Sub

    Sub Restore(File As FileInfo)
        Restore(IO.File.ReadAllBytes(File.FullName))
    End Sub

    Function Send(FromUser As UserType, ToUser As UserType, Message As String) As UserConversation(Of UserType, IdType)
        Dim i = New UserConversation(Of UserType, IdType)(Me) With {.Message = Message, .FromUser = list(FromUser), .ToUser = list(ToUser), .ViewedDate = Nothing}
        Me.Add(i)
        Return i
    End Function

    Sub SetPeriodicBackup(Path As String, Interval As Double)
        BackupTimer.Interval = Interval
        BackupPath = New FileInfo(Path)
        AddHandler BackupTimer.Elapsed, AddressOf periodic
        BackupTimer.Enabled = True
    End Sub

    Sub StopPeriodicBackup()
        RemoveHandler BackupTimer.Elapsed, AddressOf periodic
        BackupTimer.Enabled = False
    End Sub

    Private Sub periodic(sender As Object, e As EventArgs)
        Backup(BackupPath.FullName)
    End Sub

End Class

Public Class UserConversation(Of UserType As Class, IdType As Structure)

    Friend chatlist As UserChat(Of UserType, IdType)

    Friend Sub New(chatlist As UserChat(Of UserType, IdType))
        Me.chatlist = chatlist
    End Sub

    ReadOnly Property ID As String
        Get
            Return {"F", FromUser.ID.ToString, "T", ToUser.ID.ToString, "@", SentDate.Ticks}.Join("")
        End Get
    End Property

    Property Viewed As Boolean
        Get
            Return ViewedDate.HasValue AndAlso ViewedDate <= Now
        End Get
        Set(value As Boolean)
            If value Then
                ViewedDate = Now
            Else
                ViewedDate = Nothing
            End If
        End Set
    End Property

    Property FromUser As OnlineUser(Of UserType, IdType)
    Property Message As String
    Property SentDate As DateTime = Now
    Property ToUser As OnlineUser(Of UserType, IdType)
    Property ViewedDate As Date? = Nothing

    Function GetMyUser(Myself As UserType) As OnlineUser(Of UserType, IdType)
        If IsFrom(Myself) Then
            Return FromUser
        Else
            Return ToUser
        End If
    End Function

    Function GetOtherUser(Myself As UserType) As OnlineUser(Of UserType, IdType)
        If IsFrom(Myself) Then
            Return ToUser
        Else
            Return FromUser
        End If
    End Function

    Function IsFrom(User As UserType) As Boolean
        Return chatlist.list.idgetter(User).Equals(chatlist.list.idgetter(FromUser.Data))
    End Function

End Class

Friend Class UserConversationBackup(Of IdType As Structure)
    Property FromId As IdType
    Property Message As String
    Property SentDate As Long
    Property ToId As IdType
    Property ViewedDate As Long

    ReadOnly Property ID As String
        Get
            Return {"F", FromId.ToString, "T", ToId.ToString, "@", SentDate}.Join("")
        End Get
    End Property

End Class