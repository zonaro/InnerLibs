Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports System.Text
Imports InnerLibs.LINQ


Public Class TemplateMailAddress
    Inherits MailAddress

    Public Overridable Property TemplateData As Object

    Public Sub New(address As String, Optional TemplateData As Object = Nothing)
        MyBase.New(address)
        Me.TemplateData = TemplateData
    End Sub

    Public Sub New(address As String, displayName As String, Optional TemplateData As Object = Nothing)
        MyBase.New(address, displayName)
        Me.TemplateData = TemplateData
    End Sub

    Public Sub New(address As String, displayName As String, displayNameEncoding As Encoding, Optional TemplateData As Object = Nothing)
        MyBase.New(address, displayName, displayNameEncoding)
        Me.TemplateData = TemplateData
    End Sub

End Class

Public Class FluentMailMessage
    Inherits MailMessage


    Public Property Smtp As New SmtpClient()

    Public Property SuccessAction As Action(Of MailAddress)
    Public Property ErrorAction As Action(Of MailAddress, Exception)

    Public Shared Function GmailSmtp() As SmtpClient
        Return New SmtpClient("smtp.gmail.com", 587) With {.EnableSsl = True}
    End Function

    Public Shared Function OutlookSmtp() As SmtpClient
        Return New SmtpClient("smtp-mail.outlook.com", 587) With {.EnableSsl = True}
    End Function

    Public Shared Function Office365Smtp() As SmtpClient
        Return New SmtpClient("smtp.office365.com", 587) With {.EnableSsl = True}
    End Function

    Public Function WithSmtp(Client As SmtpClient) As FluentMailMessage
        Me.Smtp = Client
        Return Me
    End Function

    Public Function WithSmtp(Host As String) As FluentMailMessage
        Return WithSmtp(New SmtpClient(Host))
    End Function

    Public Function WithSmtp(Host As String, Port As Integer) As FluentMailMessage
        Return WithSmtp(New SmtpClient(Host, Port))
    End Function

    Public Function WithSmtp(Host As String, Port As Integer, UseSSL As Boolean) As FluentMailMessage
        Return WithSmtp(New SmtpClient(Host, Port) With {.EnableSsl = UseSSL})
    End Function

    Public Function UseGmailSmtp() As FluentMailMessage
        Me.Smtp = GmailSmtp()
        Return Me
    End Function

    Public Function UseOutlookSmtp() As FluentMailMessage
        Me.Smtp = OutlookSmtp()
        Return Me
    End Function

    Public Function UseOffice365Smtp() As FluentMailMessage
        Me.Smtp = Office365Smtp()
        Return Me
    End Function

    Public Function WithMessage(Text As String, Optional IsHtml As Boolean = True) As FluentMailMessage
        Me.Body = Text
        Me.IsBodyHtml = IsHtml
        Return Me
    End Function

    Public Function WithSubject(Text As String) As FluentMailMessage
        Me.Subject = Text
        Return Me
    End Function

    Public Function UseTemplate(TemplateOrFilePath As String) As FluentMailMessage
        If TemplateOrFilePath.IsFilePath Then
            If File.Exists(TemplateOrFilePath) Then TemplateOrFilePath = File.ReadAllText(TemplateOrFilePath)
        End If
        Return WithMessage(TemplateOrFilePath, True)
    End Function

    Public Function FromEmail(Email As String) As FluentMailMessage
        FromEmail(New MailAddress(Email))
        Return Me
    End Function

    Public Function FromEmail(Email As String, DisplayName As String) As FluentMailMessage
        FromEmail(New MailAddress(Email, DisplayName))
        Return Me
    End Function

    Public Function FromEmail(Email As MailAddress) As FluentMailMessage
        Me.From = Email
        Me.Sender = If(Me.Sender, Email)
        Return Me
    End Function


    Public Function WithSender(Email As String) As FluentMailMessage
        WithSender(New MailAddress(Email))
        Return Me
    End Function

    Public Function WithSender(Email As String, DisplayName As String) As FluentMailMessage
        WithSender(New MailAddress(Email, DisplayName))
        Return Me
    End Function

    Public Function WithSender(Email As MailAddress) As FluentMailMessage
        Me.Sender = Email
        Me.From = If(Me.From, Email)
        Return Me
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Email"></param>
    ''' <param name="TemplateData"></param>
    ''' <returns></returns>

    Public Function AddRecipient(Of T)(Email As String, TemplateData As T) As FluentMailMessage
        Me.To.Add(New TemplateMailAddress(Email, TemplateData))
        Return Me
    End Function


    Public Function AddRecipient(ParamArray Emails As String()) As FluentMailMessage
        For Each email In If(Emails, {})
            Me.To.Add(New TemplateMailAddress(email))
        Next
        Return Me
    End Function

    Public Function AddRecipient(ParamArray Emails As TemplateMailAddress()) As FluentMailMessage
        For Each email In If(Emails, {})
            Me.To.Add(email)
        Next
        Return Me
    End Function

    Public Function AddCarbonCopy(ParamArray Emails As String()) As FluentMailMessage
        For Each email In If(Emails, {})
            Me.CC.Add(New TemplateMailAddress(email))
        Next
        Return Me
    End Function

    Public Function AddCarbonCopy(ParamArray Emails As MailAddress()) As FluentMailMessage
        For Each email In If(Emails, {})
            Me.CC.Add(email)
        Next
        Return Me
    End Function

    Public Function AddBlindCarbonCopy(ParamArray Emails As String()) As FluentMailMessage
        For Each email In If(Emails, {})
            Me.Bcc.Add(New TemplateMailAddress(email))
        Next
        Return Me
    End Function

    Public Function AddBlindCarbonCopy(ParamArray Emails As MailAddress()) As FluentMailMessage
        For Each email In If(Emails, {})
            Me.Bcc.Add(email)
        Next
        Return Me
    End Function

    Public Function WithCredentials(Credentials As NetworkCredential) As FluentMailMessage
        If Credentials Is Nothing Then Throw New ArgumentNullException("Credentials")
        If Smtp Is Nothing Then Throw New Exception("SMTP is null")
        Smtp.Credentials = Credentials
        If Me.From Is Nothing OrElse Me.From.Address.IsBlank() Then
            Me.From = New MailAddress(Credentials.UserName)
        End If
        Me.Sender = If(Me.Sender, Me.From)
        Return Me
    End Function

    Public Function WithCredentials(Login As String, Password As String) As FluentMailMessage
        Return WithCredentials(New NetworkCredential(Login, Password))
    End Function

    Public Function OnSuccess(Action As Action(Of MailAddress)) As FluentMailMessage
        Me.SuccessAction = Action
        Return Me
    End Function

    Public Function OnError(Action As Action(Of MailAddress, Exception)) As FluentMailMessage
        Me.ErrorAction = Action
        Return Me
    End Function

    Public Function Send() As FluentMailMessage

        For Each item In Me.To
            If item IsNot Nothing Then
                Try
                    Using mm As New MailMessage()

                        Dim msg = Me.Body.IfBlank("")
                        Dim subj = Me.Subject.IfBlank("")
                        If item.GetType() Is GetType(TemplateMailAddress) Then
                            Dim data = CType(item, TemplateMailAddress)?.TemplateData
                            If data IsNot Nothing Then
                                msg = msg.Inject(data)
                                subj = subj.Inject(data)
                            End If
                        End If


                        mm.To.Add(item)
                        mm.From = Me.From
                        mm.Sender = Me.Sender
                        mm.Body = msg
                        mm.Subject = subj
                        mm.IsBodyHtml = Me.IsBodyHtml
                        mm.BodyEncoding = Me.BodyEncoding
                        mm.BodyTransferEncoding = Me.BodyTransferEncoding
                        mm.Priority = Me.Priority
                        mm.DeliveryNotificationOptions = Me.DeliveryNotificationOptions
                        mm.HeadersEncoding = Me.HeadersEncoding
                        mm.ReplyTo = Me.ReplyTo
                        mm.SubjectEncoding = Me.SubjectEncoding

                        For Each r In Me.ReplyToList
                            mm.ReplyToList.Add(r)
                        Next

                        For Each h In Me.Headers
                            mm.Headers.Add(h)
                        Next

                        For Each email In Me.CC
                            mm.CC.Add(email)
                        Next

                        For Each email In Me.Bcc
                            mm.Bcc.Add(email)
                        Next

                        For Each ann In Me.Attachments
                            mm.Attachments.Add(ann)
                        Next


                        For Each vv In Me.AlternateViews
                            mm.AlternateViews.Add(vv)
                        Next

                        If Smtp Is Nothing Then
                            Throw New NullReferenceException("Smtp not defined")
                        End If

                        Smtp.Send(mm)

                        If SuccessAction IsNot Nothing Then
                            SuccessAction.Invoke(item)
                        End If
                    End Using

                Catch ex As Exception
                    If ErrorAction IsNot Nothing Then
                        ErrorAction.Invoke(item, ex)
                    Else
                        Throw ex
                    End If
                End Try
            End If
        Next
        Return Me
    End Function

    Public Overrides Function ToString() As String
        Return Body
    End Function


End Class