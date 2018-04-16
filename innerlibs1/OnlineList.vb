
Imports System.Net.Mail
Imports System.Web
Imports System.Web.Script.Serialization
Imports System.Web.UI
Imports InnerLibs.LINQ

Public Class OnlineList(Of UserType, IdType)
    Inherits Dictionary(Of IdType, OnlineUser(Of UserType, IdType))

    Friend idgetter As Func(Of UserType, IdType)

    Private Sub Online()
        If ToleranceTime.Ticks > 0 Then
            ToleranceTime = ToleranceTime.Negate
        End If
        Dim offline = Me.Where(Function(x) x.Value.LastOnline <= Now.Add(ToleranceTime)).Select(Function(x) x.Value.Data)
        Me.Remove(offline.ToArray)
    End Sub

    Private WithEvents Timer As New System.Timers.Timer

    Public Property ToleranceTime As TimeSpan = New TimeSpan(0, 1, 0)


    Sub New(IdProperty As Func(Of UserType, IdType), Optional Interval As Double = 10000)
        idgetter = IdProperty
        AddHandler Timer.Elapsed, AddressOf Online
        Me.Timer.Interval = Interval
        Me.Timer.AutoReset = True
        Me.Timer.Enabled = True
        Me.Timer.Start()
    End Sub

    Public Shadows Function Add(Obj As UserType, Optional Activity As String = Nothing) As OnlineUser(Of UserType, IdType)
        If Obj IsNot Nothing Then
            Dim ID = Me.idgetter(Obj)

            If Not Me.ContainsKey(ID) Then
                MyBase.Item(ID) = New OnlineUser(Of UserType, IdType)(Obj, Me)
            End If

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
                        Me(ID).LastPage = Page
                    End If

                End If
            End If
            Return MyBase.Item(ID)
        End If
        Return Nothing
    End Function

    Default Overloads Property Item(User As UserType) As OnlineUser(Of UserType, IdType)
        Get
            Return Me(idgetter(User))
        End Get
        Set(value As OnlineUser(Of UserType, IdType))
            Me(idgetter(User)) = value
        End Set
    End Property

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
    <ScriptIgnore>
    Property LastPage As Page
    Property LastActivity As String
    <ScriptIgnore>
    ReadOnly Property Data As UserType

    ReadOnly Property Conversations(Optional WithUser As UserType = Nothing) As UserConversation(Of UserType)()
        Get
            If WithUser IsNot Nothing Then
                Return list.Chat.GetConversation(Me.Data, WithUser)
            Else
                Return list.Chat.Where(Function(x) list.idgetter(x.User).Equals(list.idgetter(Me.Data)) Or list.idgetter(x.WithUser).Equals(list.idgetter(Me.Data))).ToArray
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
        Dim i = New UserConversation(Of UserType) With {.Message = Message, .User = FromUser, .WithUser = ToUser}
        Me.Add(i)
        Return i
    End Function


    Function GetConversation(User As UserType, Optional WithUser As UserType = Nothing) As UserConversation(Of UserType)()

        Dim lista As IEnumerable(Of UserConversation(Of UserType))
        If WithUser IsNot Nothing Then
            lista = Me.Where(Function(x) (idgetter(User).Equals(idgetter(x.User)) AndAlso idgetter(WithUser).Equals(idgetter(x.WithUser))) Or (idgetter(User).Equals(idgetter(x.WithUser)) AndAlso idgetter(WithUser).Equals(idgetter(x.User))))
        Else
            lista = Me.Where(Function(x) (idgetter(User).Equals(idgetter(x.User))) Or (idgetter(User).Equals(idgetter(x.WithUser))))
        End If

        Return lista.Distinct.OrderByDescending(Function(x) x.SentDate).ToArray
    End Function
End Class

Public Class UserConversation(Of UserType)

    Friend Sub New()

    End Sub

    Property User As UserType
    Property WithUser As UserType

    Property Message As String
    Property SentDate As DateTime = Now

    Property Attachments As New List(Of Attachment)

End Class


