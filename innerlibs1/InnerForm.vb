Imports System.Net
Imports System.Net.Mail
Imports System.Web
Imports System.Web.UI

Namespace InnerForm

    Public Class InnerForm
        Inherits Page

        Private _mailfield As String = "email"

        ''' <summary>
        ''' Servidor SMTP que será utilizado para o disparo de email
        ''' </summary>
        ''' <returns></returns>
        Property SmtpServer As New SmtpClient

        ''' <summary>
        ''' Nome da tabela no Banco de Dados. É utilizado pela propriedade <see cref="DataBase"/> na
        ''' hora de salvar as informaçoes no banco.
        ''' </summary>
        ''' <returns></returns>
        Property TableName As String = "InnerForm"

        ''' <summary>
        ''' Lista de destinatários que irão receber o conteudo do formulário
        ''' </summary>
        ''' <returns></returns>
        Property Recipients As String()

        ''' <summary>
        ''' Dominios autorizados a utilizar o formulário.
        ''' </summary>
        ''' <returns></returns>
        Property AuthorizedDomains As String()

        ''' <summary>
        ''' Mensagem de sucesso
        ''' </summary>
        ''' <returns></returns>
        Property SuccessMessage As String = "Mensagem enviada com sucesso"

        ''' <summary>
        ''' Mensagem de erro
        ''' </summary>
        ''' <returns></returns>
        Property ErrorMessage As String = "Erro ao enviar mensagem"

        ''' <summary>
        ''' Objeto de conexão com o banco de dados. Se estiver nulo, a informação não será salva no
        ''' banco de dados porém o email será enviado normalmente
        ''' </summary>
        ''' <returns></returns>
        Property DataBase As DataBase = Nothing

        ''' <summary>
        ''' Nome do campo de email do formulario
        ''' </summary>
        ''' <returns></returns>
        Property MailField As String
            Get
                Return _mailfield
            End Get
            Set(value As String)
                _mailfield = value.IfBlank("email")
            End Set
        End Property

        ''' <summary>
        ''' Conteudo do email de AutoResposta que será enviado ao usuário. O email somente é enviado
        ''' se esta propriedade nao estiver nula ou em branco
        ''' </summary>
        ''' <returns></returns>
        Property UserMailTemplate As String = ""

        ''' <summary>
        ''' Assunto do email de AutoResposta que será enviado ao usuário. O email somente é enviado
        ''' se a propriedade <see cref="UserMailTemplate"/> nao estiver nula ou em branco
        ''' </summary>
        ''' <returns></returns>
        Property UserSubject As String = ""

        Sub SendForm(sender As Object, e As EventArgs) Handles Me.Load
            Response.Clear()
            Dim agora = Now
            Dim rep = New AJAX.Response("success", SuccessMessage)
            Dim usermail As String = Request(MailField)
            usermail = usermail.ToLower
            Dim mail As New MailMessage
            mail.IsBodyHtml = True
            mail.From = New MailAddress(DirectCast(SmtpServer.Credentials, NetworkCredential).UserName)
            mail.Bcc.Add(mail.From)
            Try
                If Recipients.Count < 1 Then Throw New NullReferenceException("No recipients configured")
                If Not usermail.IsEmail Then Throw New InvalidCastException("Mail field is not a valid email.  (" & MailField & " = " & usermail.Quote & ")")
                Dim url As Uri = If(Request.UrlReferrer, Request.Url)
                If url.GetDomain.IsIn(AuthorizedDomains) Then
                    Dim itens As New Dictionary(Of String, Object)

                    For Each i In Request.QueryString.ToDictionary
                        If Not itens.ContainsKey(i.Key) Then
                            itens.Add(i.Key, i.Value)
                        End If
                    Next
                    For Each i In Request.Form.ToDictionary
                        If Not itens.ContainsKey(i.Key) Then
                            itens.Add(i.Key, i.Value)
                        End If
                    Next

                    rep.response = itens

                    'salva no banco de dados
                    If Not IsNothing(Me.DataBase) Then

                        Dim param_ip = Me.DataBase.CreateParameter("@IP", Request.UserHostAddress)
                        Dim param_url = Me.DataBase.CreateParameter("@URL", url.GetDomain)
                        Dim param_data = Me.DataBase.CreateParameter("@DATA", agora)
                        Dim param_form = Me.DataBase.CreateParameter("@FORM", itens.SerializeJSON)
                        Dim param_email = Me.DataBase.CreateParameter("@EMAIL", usermail)

                        DataBase.RunSQL("INSERT INTO " & TableName & " (IP,URL,EMAIL,DATA,FORM) values (@IP,@URL,@EMAIL,@DATA,@FORM)", param_ip, param_url, param_email, param_data, param_form)
                    End If

                    'anexa o form em json
                    Using s = itens.SerializeJSON.ToStream
                        mail.Attachments.Add(New Attachment(s, usermail.ToSlug & ".json", New FileType(".json").ToString))
                    End Using

                    'anexa os arquivos
                    For index = 0 To Request.Files.Count - 1
                        Dim arq = Request.Files(index)
                        mail.Attachments.Add(New Attachment(arq.InputStream, arq.FileName, arq.ContentType))
                    Next

                    'monta a mensagem
                    For Each item In itens
                        mail.Body.Append(TableRow(item.Key, "<strong>" & item.Key & "</strong>", "" & item.Value))
                    Next
                    mail.Body = Table("", mail.Body)

                    'envia o email
                    For Each u In Recipients
                        If u.IsEmail Then
                            mail.To.Add(u)
                        End If
                    Next
                    mail.ReplyToList.Add(usermail)
                    mail.Subject = "Formulário recebido através do site " & url.GetDomain.Quote
                    SmtpServer.Send(mail)

                    'envia autoresposta do usuário
                    If Not IsNothing(UserMailTemplate) AndAlso UserMailTemplate.IsNotBlank Then
                        mail.To.Clear()
                        For Each u In Recipients
                            If u.IsEmail Then
                                mail.ReplyToList.Add(u)
                            End If
                        Next
                        mail.To.Add(usermail)
                        mail.Subject = UserSubject.IfBlank("Formulário recebido através do site " & url.GetDomain)
                        mail.Body = UserMailTemplate
                        For Each item In itens
                            mail.Subject = mail.Subject.Replace("##" & item.Key & "##", item.Value)
                            mail.Body = mail.Body.Replace("##" & item.Key & "##", item.Value)
                        Next
                        SmtpServer.Send(mail)
                    End If
                Else
                    Throw New UnauthorizedAccessException("The domain " & url.GetDomain.Quote & " is not authorized. Check the AuthorizedDomains List")
                End If
            Catch ex As Exception
                rep.status = "error"
                rep.message = ErrorMessage
                rep.response = If(ex.InnerException, ex).Message
            End Try
            Response.WriteJSON(rep)
        End Sub

        ''' <summary>
        ''' </summary>
        ''' <param name="WhereConditions"></param>
        ''' <returns></returns>
        Public Shared Function GetForm(DataBase As DataBase, Optional TableName As String = "InnerForm", Optional WhereConditions As String = "") As List(Of Dictionary(Of String, Object))
            Dim table As New List(Of Dictionary(Of String, Object))
            If Not IsNothing(DataBase) Then
                Using r = DataBase(TableName, WhereConditions)
                    While r.Read
                        Dim row = r.GetCurrentRow
                        Dim form = ParseJSON(Of Dictionary(Of String, Object))(row("FORM"))
                        form("IP") = row("IP")
                        form("URL") = row("URL")
                        form("DATA") = row("DATA")
                        table.Add(form)
                    End While
                End Using
            End If
            table.Uniform
            Return table
        End Function

    End Class

End Namespace