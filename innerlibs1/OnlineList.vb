
Imports System.Web
Imports System.Web.Script.Serialization
Imports System.Web.UI

Public Class OnlineList(Of UserType, IdType)
    Inherits Dictionary(Of IdType, OnlineUser(Of UserType, IdType))

    Private idgetter As Func(Of UserType, IdType)
    Private _ToleranceTime As TimeSpan = New TimeSpan(0, 0, -15)

    Private Sub Online()
        Dim offline = Me.Where(Function(x) x.Value.LastOnline < Now.Add(ToleranceTime)).Select(Function(x) x.Value.Data)
        Me.Remove(offline.ToArray)
    End Sub

    Private WithEvents Timer As New System.Timers.Timer

    Public Property ToleranceTime As TimeSpan
        Get
            Return _ToleranceTime
        End Get
        Set(value As TimeSpan)
            If value.Ticks > 0 Then
                value = value.Negate
            End If
            _ToleranceTime = value
        End Set
    End Property

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
                MyBase.Item(ID) = New OnlineUser(Of UserType, IdType)(Obj)
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


End Class

Public Class OnlineUser(Of UserType, IdType)

    Friend Sub New(Data As UserType)
        Me.Data = Data
    End Sub

    Property LastOnline As DateTime = Now
    Property LastUrl As String
    <ScriptIgnore>
    Property LastPage As Page
    Property LastActivity As String
    <ScriptIgnore>
    ReadOnly Property Data As UserType
End Class




Public Class UserChat(Of UserType, IdType)
    Inherits List(Of UserConversation(Of UserType))


    Private idgetter As Func(Of UserType, IdType)


    Function Send(FromUser As UserType, ToUser As UserType, Message As String) As UserConversation(Of UserType)
        Dim i = New UserConversation(Of UserType) With {.Message = Message, .User = FromUser, .WithUser = ToUser}
        Me.Add(i)
        Return i
    End Function


    Function GetConversation(User As UserType, WithUser As UserType) As IEnumerable(Of UserConversation(Of UserType))
        Return Me.Where(Function(x)
                            Return (idgetter(User).Equals(idgetter(x.User)) AndAlso idgetter(WithUser).Equals(idgetter(x.WithUser))) Or (idgetter(User).Equals(idgetter(x.WithUser)) AndAlso idgetter(WithUser).Equals(idgetter(x.User)))
                        End Function).OrderBy(Function(x) x.SentDate)
    End Function
End Class

Public Class UserConversation(Of Usertype)

    Friend Sub New()

    End Sub

    Property User As Usertype
    Property WithUser As Usertype

    Property Message As String
    Property SentDate As DateTime

    Property Attachments As IEnumerable(Of ConversationAttachment)

End Class

Public Class ConversationAttachment
    Property Name As String
    Property Data As Object
End Class
