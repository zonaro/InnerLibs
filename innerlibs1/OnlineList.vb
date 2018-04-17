
Imports System.Net.Mail
Imports System.Web
Imports System.Web.Script.Serialization
Imports System.Web.UI
Imports InnerLibs.LINQ

Public Class OnlineList(Of UserType, IdType)
    Inherits Dictionary(Of IdType, OnlineUser(Of UserType, IdType))

    Friend idgetter As Func(Of UserType, IdType)

    Public Function OnlineUsers() As IEnumerable(Of OnlineUser(Of UserType, IdType))
        Return Me.Where(Function(x) x.Value.IsOnline).Select(Function(x) x.Value)
    End Function

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
    Private _tolerancetime As TimeSpan = New TimeSpan(0, 1, 0)

    Sub New(IdProperty As Func(Of UserType, IdType))
        idgetter = IdProperty
    End Sub

    Public Function SetOnline(Obj As UserType, Optional Activity As String = Nothing) As OnlineUser(Of UserType, IdType)
        Return Me.Add(Obj, True, Activity)
    End Function

    Public Function SetOffline(Obj As UserType) As OnlineUser(Of UserType, IdType)
        Return Me.Add(Obj, False)
    End Function

    Public Shadows Function Add(Obj As UserType, Online As Boolean, Optional Activity As String = Nothing) As OnlineUser(Of UserType, IdType)
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

    Default Shadows ReadOnly Property Item(User As UserType) As OnlineUser(Of UserType, IdType)
        Get
            If Me.ContainsUser(User) Then
                Return MyBase.Item(idgetter(User))
            Else
                Return Me.Add(User, False)
            End If
        End Get
    End Property

    Public Function ContainsUser(User As UserType) As Boolean
        Return Me.ContainsKey(idgetter(User))
    End Function

    Public Shadows Sub Remove(ParamArray Obj As UserType())
        Obj = If(Obj, {})
        Me.RemoveIfExist(Obj.Select(Function(x) idgetter(x)).ToArray)
    End Sub

    ReadOnly Property Chat As New UserChat(Of UserType, IdType)(Function(x) idgetter(x))

End Class

Public Class OnlineUser(Of UserType, IdType)

    Friend Sub New(Data As UserType, list As OnlineList(Of UserType, IdType))
        Me.Data = Data
        Me.list = list
    End Sub


    Private list As OnlineList(Of UserType, IdType)

    Property LastOnline As DateTime = Now

    Property LastUrl As String

    Property IsOnline As Boolean
        Get
            If list.ContainsUser(Me.Data) Then
                If online Then
                    online = LastOnline >= Now.Add(list.ToleranceTime)
                    If online Then
                        LastOnline = Now
                    End If
                End If
                Return online
            End If
            Return False
        End Get
        Set(value As Boolean)
            online = value
        End Set
    End Property



    Private online As Boolean = True

    Property LastActivity As String = "Offline"

    <ScriptIgnore> ReadOnly Property Data As UserType

    <ScriptIgnore> Property LastPage As Page

    ReadOnly Property Conversations(Optional WithUser As UserType = Nothing) As UserConversation(Of UserType)()
        Get
            If WithUser IsNot Nothing Then
                Return list.Chat.GetConversation(Me.Data, WithUser)
            Else
                Return list.Chat.Where(Function(x) list.idgetter(x.FromUser).Equals(list.idgetter(Me.Data)) Or list.idgetter(x.ToUser).Equals(list.idgetter(Me.Data))).ToArray
            End If
        End Get
    End Property

End Class




Public Class UserChat(Of UserType, IdType)
    Inherits List(Of UserConversation(Of UserType))

    Sub New(IdProperty As Func(Of UserType, IdType))
        MyBase.New
        Me.idgetter = IdProperty
    End Sub

    Private idgetter As Func(Of UserType, IdType)


    Function Send(FromUser As UserType, ToUser As UserType, Message As String) As UserConversation(Of UserType)
        Dim i = New UserConversation(Of UserType) With {.Message = Message, .FromUser = FromUser, .ToUser = ToUser}
        Me.Add(i)
        Return i
    End Function


    Function GetConversation(User As UserType, Optional WithUser As UserType = Nothing) As IEnumerable(Of UserConversation(Of UserType))

        Dim lista As IEnumerable(Of UserConversation(Of UserType))
        If WithUser IsNot Nothing Then
            lista = Me.Where(Function(x) (idgetter(User).Equals(idgetter(x.FromUser)) AndAlso idgetter(WithUser).Equals(idgetter(x.ToUser))) Or (idgetter(User).Equals(idgetter(x.ToUser)) AndAlso idgetter(WithUser).Equals(idgetter(x.FromUser))))
        Else
            lista = Me.Where(Function(x) (idgetter(User).Equals(idgetter(x.FromUser))) Or (idgetter(User).Equals(idgetter(x.ToUser))))
        End If

        Return lista.Distinct.OrderByDescending(Function(x) x.SentDate).AsEnumerable
    End Function


    Sub ChangeViewed(User As UserType, Optional WithUser As UserType = Nothing)
        For Each i In GetConversation(User, WithUser).Where(Function(x) x.ToUser.Equals(User))
            i.ViewedDate = Now
        Next
    End Sub

    Function GetChangedConversation(User As UserType)

    End Function
End Class

Public Enum ChangeStatus
    CloseWindow = -1
    Same = 0
    OpenWindow = 1
End Enum

Public Class UserConversation(Of UserType)

    Friend Sub New()

    End Sub

    Property WindowStatus As ChangeStatus = ChangeStatus.Same

    Property FromUser As UserType
    Property ToUser As UserType

    Property Message As String
    Property SentDate As DateTime = Now

    Property ViewedDate As DateTime

    Property Attachments As New List(Of Attachment)

End Class


