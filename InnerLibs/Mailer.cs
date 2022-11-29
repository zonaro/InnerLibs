using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace InnerLibs.Mail
{
    [Flags]
    public enum SentStatus
    {
        NotSent = 0,
        Success = 1,
        Error = 2,
        PartialSuccess = Error + Success,
    }

    /// <summary>
    /// Wrapper de <see cref="System.Net.Mail"/> em FluentAPI com configurações de Template e
    /// métodos auxiliares.
    /// </summary>
    public class FluentMailMessage : MailMessage
    {
        private List<(MailAddress, SentStatus, Exception)> _status = new List<(MailAddress, SentStatus, Exception)>();

        public FluentMailMessage()
        {
            this.IsBodyHtml = true;
        }

        /// <summary>
        /// Ação executada quando ocorrer erro no disparo
        /// </summary>
        public Action<MailAddress, FluentMailMessage, Exception> ErrorAction { get; set; }

        /// <summary>
        /// Lista contendo os endereços de email que encontraram algum erro ao serem enviados
        /// </summary>
        public IEnumerable<(MailAddress, Exception)> ErrorList => SentStatusList.Where(x => x.Status.HasFlag(SentStatus.Error)).Select(x => (x.Destination, x.Error));

        /// <summary>
        /// Status geral do disparo
        /// </summary>
        public SentStatus SentStatus => SentStatusList.Select(x => x.Status).Aggregate((x, y) => x | y);

        /// <summary>
        /// Lista contendo os destinatários e o status do disparo
        /// </summary>
        public IEnumerable<(MailAddress Destination, SentStatus Status, Exception Error)> SentStatusList => _status.AsEnumerable();

        /// <summary>
        /// Informações de SMTP
        /// </summary>
        public SmtpClient Smtp { get; set; } = new SmtpClient();

        /// <summary>
        /// Ação executada quando o disparo for concluido com êxito
        /// </summary>
        public Action<MailAddress, FluentMailMessage> SuccessAction { get; set; }

        /// <summary>
        /// Lista contendo os endereços de email que foram enviados com sucesso
        /// </summary>
        public IEnumerable<MailAddress> SuccessList => SentStatusList.Where(x => x.Status.HasFlag(SentStatus.Success)).Select(x => x.Destination);

        /// <summary>
        /// SMTP do Gmail
        /// </summary>
        /// <returns></returns>
        public static SmtpClient GmailSmtp() => new SmtpClient("smtp.gmail.com", 587) { EnableSsl = true };

        /// <summary>
        /// SMTP da Locaweb
        /// </summary>
        /// <returns></returns>
        public static SmtpClient LocawebSmtp() => new SmtpClient("email-ssl.com.br", 465) { EnableSsl = true };

        /// <summary>
        /// SMTP do Office365
        /// </summary>
        /// <returns></returns>
        public static SmtpClient Office365Smtp() => new SmtpClient("smtp.office365.com", 587) { EnableSsl = true };

        /// <summary>
        /// SMTP do Outlook
        /// </summary>
        /// <returns></returns>
        public static SmtpClient OutlookSmtp() => new SmtpClient("smtp-mail.outlook.com", 587) { EnableSsl = true };

        /// <summary>
        /// Cria e dispara rapidamente uma <see cref="FluentMailMessage"/>
        /// </summary>
        /// <param name="Email">Email do remetende e utilizado nas credenciais de SMTP</param>
        /// <param name="Password">Senha utilizada nas credenciais de SMTP</param>
        /// <param name="Recipient">Destinatários</param>
        /// <param name="Subject">Assunto do Email</param>
        /// <param name="Message">Corpo da Mensagem</param>
        /// <returns></returns>
        public static SentStatus QuickSend(string Email, string Password, string Recipient, string Subject, string Message) => new FluentMailMessage().WithQuickConfig(Email, Password).AddRecipient(Recipient).WithSubject(Subject).WithMessage(Message).OnError((m, a, ex) => Debug.WriteLine(ex.ToFullExceptionString())).SendAndDispose();

        /// <summary>
        /// Adciona um anexo ao email
        /// </summary>
        /// <param name="Attachment"></param>
        /// <returns></returns>
        public FluentMailMessage AddAttachment(params Attachment[] Attachment) => AddAttachment((Attachment ?? Array.Empty<Attachment>()).AsEnumerable());

        /// <summary>
        /// Adciona um anexo ao email
        /// </summary>
        public FluentMailMessage AddAttachment(IEnumerable<Attachment> Attachment)
        {
            if (Attachment != null && Attachment.Any())
                foreach (var item in Attachment)
                {
                    this.AddAttachment(item);
                }
            return this;
        }

        /// <summary>
        /// Adciona um anexo ao email
        /// </summary>
        public FluentMailMessage AddAttachment(Attachment Attachment)
        {
            if (Attachment != null && Attachment.ContentStream.Length > 0)
                this.Attachments.Add(Attachment);
            return this;
        }

        /// <summary>
        /// Adciona um anexo ao email
        /// </summary>
        public FluentMailMessage AddAttachment(Stream Attachment, string Name)
        {
            if (Attachment != null && Attachment.Length > 0 && Name.IsNotBlank())
                this.Attachments.Add(new Attachment(Attachment, Name));
            return this;
        }

        /// <summary>
        /// Adciona um anexo ao email
        /// </summary>
        public FluentMailMessage AddAttachment(byte[] Attachment, string Name)
        {
            if (Attachment != null && Attachment.Length > 0 && Name.IsNotBlank())
                using (var s = new MemoryStream(Attachment))
                    this.Attachments.Add(new Attachment(s, Name));
            return this;
        }

        /// <summary>
        /// Adciona um anexo ao email
        /// </summary>
        public FluentMailMessage AddAttachment(FileInfo Attachment)
        {
            if (Attachment != null && Attachment.Length > 0)
                this.Attachments.Add(new Attachment(Attachment.FullName));
            return this;
        }

        /// <summary>
        /// Adiciona endereços ao BCC do email
        /// </summary>
        /// <param name="Emails"></param>
        /// <returns></returns>
        public FluentMailMessage AddBlindCarbonCopy(params string[] Emails)
        {
            foreach (var email in (Emails ?? Array.Empty<string>()).SelectMany(x => x.ExtractEmails()).ToArray())
                Bcc.Add(new TemplateMailAddress(email));
            return this;
        }

        /// <summary>
        /// Adiciona endereços ao BCC do email
        /// </summary>
        /// <param name="Emails"></param>
        /// <returns></returns>
        public FluentMailMessage AddBlindCarbonCopy(params MailAddress[] Emails)
        {
            foreach (var email in Emails ?? Array.Empty<MailAddress>())
                Bcc.Add(email);
            return this;
        }

        /// <summary>
        /// Adiciona endereços ao CC do email
        /// </summary>
        /// <param name="Emails"></param>
        /// <returns></returns>
        public FluentMailMessage AddCarbonCopy(params string[] Emails)
        {
            foreach (var email in (Emails ?? Array.Empty<string>()).SelectMany(x => x.ExtractEmails()).ToArray())
                CC.Add(new TemplateMailAddress(email));
            return this;
        }

        /// <summary>
        /// Adiciona endereços ao CC do email
        /// </summary>
        /// <param name="Emails"></param>
        /// <returns></returns>
        public FluentMailMessage AddCarbonCopy(params MailAddress[] Emails)
        {
            foreach (var email in Emails ?? Array.Empty<MailAddress>())
                CC.Add(email);
            return this;
        }

        /// <summary>
        /// Adiciona um destinatário a lista de destinatários deta mensagem
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage AddRecipient<T>(string Email, T TemplateData)
        {
            To.Add(new TemplateMailAddress(Email, TemplateData));
            return this;
        }

        /// <summary>
        /// Adiciona um destinatário a lista de destinatários deta mensagem
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Email"></param>
        /// <param name="DisplayName"></param>
        /// <param name="TemplateData"></param>
        /// <returns></returns>
        public FluentMailMessage AddRecipient<T>(string Email, string DisplayName, T TemplateData)
        {
            To.Add(new TemplateMailAddress(Email, DisplayName, TemplateData));
            return this;
        }

        /// <summary>
        /// Adiciona um destinatário a lista de destinatários deta mensagem
        /// </summary>
        public FluentMailMessage AddRecipient<T>(T Data, Expression<Func<T, string>> EmailSelector, Expression<Func<T, string>> NameSelector = null)
        {
            To.Add(TemplateMailAddress.FromObject(Data, EmailSelector, NameSelector));
            return this;
        }

        /// <summary>
        /// Adiciona um destinatário a lista de destinatários deta mensagem
        /// </summary>
        public FluentMailMessage AddRecipient<T>(IEnumerable<T> Data, Expression<Func<T, string>> EmailSelector, Expression<Func<T, string>> NameSelector = null)
        {
            foreach (var m in TemplateMailAddress.FromList(Data, EmailSelector, NameSelector)) To.Add(m);
            return this;
        }

        /// <summary>
        /// Adiciona um destinatário a lista de destinatários desta mensagem
        /// </summary>
        public FluentMailMessage AddRecipient(params string[] Emails)
        {
            foreach (var email in (Emails ?? Array.Empty<string>()).SelectMany(x => x.ExtractEmails()).ToArray())
                To.Add(new TemplateMailAddress(email));
            return this;
        }

        public FluentMailMessage AddRecipient(params TemplateMailAddress[] Emails)
        {
            foreach (var email in Emails ?? Array.Empty<TemplateMailAddress>())
                To.Add(email);
            return this;
        }

        /// <summary>
        /// Configura o remetente da mensagem
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage FromEmail(string Email)
        {
            From = new MailAddress(Email);
            return this;
        }

        /// <summary>
        /// Configura o remetente da mensagem
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage FromEmail(string Email, string DisplayName)
        {
            From = new MailAddress(Email, DisplayName);
            return this;
        }

        /// <summary>
        /// Configura o remetente da mensagem
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage FromEmail(MailAddress Email)
        {
            From = Email;
            return this;
        }

        /// <summary>
        /// Função executada quando houver um disparo com erro
        /// </summary>
        /// <param name="Action"></param>
        /// <returns></returns>
        public FluentMailMessage OnError(Action<MailAddress, FluentMailMessage, Exception> Action)
        {
            ErrorAction = Action;
            return this;
        }

        /// <summary>
        /// Função executada quando houver um disparo com sucesso
        /// </summary>
        /// <param name="Action"></param>
        /// <returns></returns>
        public FluentMailMessage OnSuccess(Action<MailAddress, FluentMailMessage> Action)
        {
            SuccessAction = Action;
            return this;
        }

        /// <summary>
        /// Reinicia o status de disparo
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage ResetStatus()
        {
            _status = _status ?? new List<(MailAddress, SentStatus, Exception)>();
            _status.Clear();
            return this;
        }

        /// <summary>
        /// Envia os emails
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public FluentMailMessage Send()
        {
            ResetStatus();

            if (Smtp != null)
            {
                foreach (var item in To)
                {
                    if (item != null)
                    {
                        try
                        {
                            using (var msgIndiv = new MailMessage())
                            {
                                string msg = Body.IfBlank(Text.Empty);
                                string subj = Subject.IfBlank(Text.Empty);

                                if (item is TemplateMailAddress templateMail)
                                {
                                    var data = templateMail.TemplateData;
                                    if (data != null)
                                    {
                                        msg = msg.Inject(data);
                                        subj = subj.Inject(data);

                                        foreach (var att in templateMail.Attachments ?? new List<Attachment>())
                                            msgIndiv.Attachments.Add(att);
                                    }
                                }

                                msgIndiv.IsBodyHtml = this.IsBodyHtml;
                                msgIndiv.BodyEncoding = this.BodyEncoding;
                                msgIndiv.BodyTransferEncoding = this.BodyTransferEncoding;
                                msgIndiv.DeliveryNotificationOptions = this.DeliveryNotificationOptions;
                                msgIndiv.From = this.From;
                                msgIndiv.Body = msg;
                                msgIndiv.Subject = subj;
                                msgIndiv.To.Add(item);

                                msgIndiv.SubjectEncoding = this.SubjectEncoding;
                                msgIndiv.HeadersEncoding = this.HeadersEncoding;
                                msgIndiv.Priority = this.Priority;
                                msgIndiv.Sender = this.Sender;
                                msgIndiv.Headers.Add(this.Headers);

                                foreach (var email in this.CC)
                                {
                                    msgIndiv.CC.Add(item);
                                }

                                foreach (var email in this.ReplyToList)
                                {
                                    msgIndiv.ReplyToList.Add(item);
                                }

                                foreach (var email in this.CC)
                                {
                                    msgIndiv.CC.Add(item);
                                }

                                foreach (var email in this.Bcc)
                                {
                                    msgIndiv.Bcc.Add(item);
                                }

                                foreach (var att in this.Attachments)
                                {
                                    msgIndiv.Attachments.Add(att);
                                }

                                foreach (var att in this.AlternateViews)
                                {
                                    msgIndiv.AlternateViews.Add(att);
                                }

                                Smtp.Send(msgIndiv);

                                _status.Add((item, SentStatus.Success, null));

                                SuccessAction?.Invoke(item, this);
                            }
                        }
                        catch (Exception ex)
                        {
                            _status.Add((item, SentStatus.Error, ex));

                            ErrorAction?.Invoke(item, this, ex);
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentException("Smtp is null", nameof(Smtp));
            }

            return this;
        }

        /// <summary>
        /// Envia os emails
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public SentStatus SendAndDispose()
        {
            Send();
            var _ss = SentStatus;
            this.Dispose();
            return _ss;
        }

        /// <summary>
        /// Retorna o corpo da mensagem
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Body;

        /// <summary>
        /// Configura o SMTP do Gmail para este disparo
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage UseGmailSmtp()
        {
            Smtp = GmailSmtp();
            return this;
        }

        /// <summary>
        /// Configura o SMTP da Locaweb para este disparo
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage UseLocawebSmtp()
        {
            Smtp = LocawebSmtp();
            return this;
        }

        /// <summary>
        /// Configura o SMTP do Office365 para este disparo
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage UseOffice365Smtp()
        {
            Smtp = Office365Smtp();
            return this;
        }

        /// <summary>
        /// Configura o SMTP do Outlook para este disparo
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage UseOutlookSmtp()
        {
            Smtp = OutlookSmtp();
            return this;
        }

        /// <summary>
        /// Utiliza um template para o corpo da mensagem
        /// </summary>
        /// <param name="TemplateOrFilePathOrUrl"></param>
        /// <returns></returns>
        public FluentMailMessage UseTemplate(string TemplateOrFilePathOrUrl) => UseTemplate(TemplateOrFilePathOrUrl, Text.Empty);

        /// <summary>
        /// Utiliza um template para o corpo da mensagem
        /// </summary>
        /// <param name="TemplateOrFilePathOrUrl"></param>
        /// <returns></returns>
        public FluentMailMessage UseTemplate(string TemplateOrFilePathOrUrl, string MessageTemplate) => UseTemplate(TemplateOrFilePathOrUrl, new { BodyText = MessageTemplate });
        public FluentMailMessage UseTemplate(HtmlTag Template, string MessageTemplate) => UseTemplate(Template?.OuterHtml ?? Text.Empty, new { BodyText = MessageTemplate });
        public FluentMailMessage UseTemplate(HtmlTag Template) => UseTemplate(Template?.OuterHtml ?? Text.Empty, Text.Empty);
        public FluentMailMessage UseTemplate<T>(HtmlTag Template, T MessageTemplate) where T : class => UseTemplate(Template.OuterHtml, MessageTemplate);

        public FluentMailMessage UseTemplate(FileInfo Template) => UseTemplate(Template?.FullName ?? Text.Empty, Text.Empty);
        public FluentMailMessage UseTemplate(FileInfo Template, string MessageTemplate) => UseTemplate(Template?.FullName, MessageTemplate);
        public FluentMailMessage UseTemplate<T>(FileInfo Template, T MessageTemplate) where T : class => UseTemplate(Template?.FullName ?? Text.Empty, MessageTemplate);

        public FluentMailMessage UseTemplate(DirectoryInfo TemplateDirectory, string TemplateFileName) => UseTemplate(Path.Combine(TemplateDirectory?.FullName ?? Text.Empty, TemplateFileName ?? Text.Empty), Text.Empty);
        public FluentMailMessage UseTemplate(DirectoryInfo TemplateDirectory, string TemplateFileName, string MessageTemplate) => UseTemplate(Path.Combine(TemplateDirectory?.FullName ?? Text.Empty, TemplateFileName ?? Text.Empty), MessageTemplate);
        public FluentMailMessage UseTemplate<T>(DirectoryInfo TemplateDirectory, string TemplateFileName, T MessageTemplate) where T : class => UseTemplate(Path.Combine(TemplateDirectory?.FullName ?? Text.Empty, TemplateFileName ?? Text.Empty), MessageTemplate);


        /// <summary>
        /// Utiliza um template para o corpo da mensagem
        /// </summary>
        /// <param name="TemplateOrFilePathOrUrl"></param>
        /// <returns></returns>
        public FluentMailMessage UseTemplate<T>(string TemplateOrFilePathOrUrl, T MessageTemplate) where T : class
        {
            if (TemplateOrFilePathOrUrl.IsFilePath())
            {
                if (File.Exists(TemplateOrFilePathOrUrl)) TemplateOrFilePathOrUrl = File.ReadAllText(TemplateOrFilePathOrUrl);
            }

            if (TemplateOrFilePathOrUrl.IsURL())
            {
                TemplateOrFilePathOrUrl = Web.DownloadString(TemplateOrFilePathOrUrl);
            }

            if (MessageTemplate != null)
            {
                TemplateOrFilePathOrUrl = TemplateOrFilePathOrUrl.Inject(MessageTemplate);
            }

            return WithMessage(TemplateOrFilePathOrUrl);
        }

        /// <summary>
        /// Configura as credenciais do SMTP
        /// </summary>
        /// <param name="Credentials"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// Ocorre quando <paramref name="Credentials"/> é nulo
        /// </exception>
        /// <exception cref="Exception"></exception>
        public FluentMailMessage WithCredentials(NetworkCredential Credentials)
        {
            if (Smtp == null) throw new ArgumentException("SMTP is null, Define SMTP before defining credentials", nameof(Smtp));
            Smtp.Credentials = Credentials ?? throw new ArgumentNullException(nameof(Credentials));
            if (From == null || From.Address.IsBlank())
            {
                From = new MailAddress(Credentials.UserName);
            }

            return this;
        }

        /// <summary>
        /// Configura as credenciais do SMTP
        /// </summary>
        public FluentMailMessage WithCredentials(string Login, string Password)
        {
            if (Smtp == null || Smtp.Host.IsBlank())
            {
                return WithQuickConfig(Login, Password);
            }
            else
            {
                return WithCredentials(new NetworkCredential(Login, Password));
            }
        }

        /// <summary>
        /// Configura a mensagem a ser enviada
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage WithMessage(string Text)
        {
            Body = Text;
            return this;
        }

        /// <summary>
        /// Configura a mensagem a ser enviada
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage WithMessage(HtmlTag Text)
        {
            IsBodyHtml = true;
            Body = Text?.ToString() ?? "";
            return this;
        }

        /// <summary>
        /// Configura o SMTP para este disparo a partir de um email e senha
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage WithQuickConfig(string Email, string Password)
        {
            var domain = Email.GetDomain();

            switch (domain)
            {
                case "outlook.com":
                case "outlook.com.br":
                case "hotmail.com":
                    UseOutlookSmtp();
                    break;

                case "office365.com":
                    UseOffice365Smtp();
                    break;

                case "gmail.com":
                    UseGmailSmtp();
                    break;

                default:
                    WithSmtp($"smtp.{domain}", 587, false);
                    break;
            }

            Debug.WriteLine($"Using {Smtp.Host}");

            WithCredentials(Email, Password);

            return this;
        }

        /// <summary>
        /// Configura o SMTP para este disparo
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage WithSmtp(SmtpClient Client)
        {
            Smtp = Client;
            return this;
        }

        /// <summary>
        /// Configura o SMTP para este disparo
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage WithSmtp(string Host) => WithSmtp(new SmtpClient(Host));

        /// <summary>
        /// Configura o SMTP para este disparo
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage WithSmtp(string Host, int Port) => WithSmtp(new SmtpClient(Host, Port));

        /// <summary>
        /// Configura o SMTP para este disparo
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage WithSmtp(string Host, int Port, bool UseSSL) => WithSmtp(new SmtpClient(Host, Port) { EnableSsl = UseSSL });

        /// <summary>
        /// Configura o assunto do email
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage WithSubject(string Text)
        {
            Subject = Text;
            return this;
        }


        public FluentMailMessage AddAttachmentFromData<T>(Expression<Func<T, IEnumerable<Attachment>>> AttachmentSelector)
        {
            Misc.AddAttachmentFromData(To.Where(x => x is TemplateMailAddress).Cast<TemplateMailAddress>(), AttachmentSelector);
            return this;
        }

        public FluentMailMessage AddAttachmentFromData<T>(Expression<Func<T, Attachment>> AttachmentSelector)
        {
            Misc.AddAttachmentFromData(To.Where(x => x is TemplateMailAddress).Cast<TemplateMailAddress>(), AttachmentSelector);
            return this;
        }

    }

    /// <summary>
    /// Um destinatário de email contendo informações que serão aplicadas em um template
    /// </summary>
    public class TemplateMailAddress : MailAddress
    {
        public TemplateMailAddress(string address, object TemplateData = null) : base(address)
        {
            this.TemplateData = TemplateData;
        }

        public TemplateMailAddress(string address, string displayName, object TemplateData = null) : base(address, displayName)
        {
            this.TemplateData = TemplateData;
        }

        public TemplateMailAddress(string address, string displayName, Encoding displayNameEncoding, object TemplateData = null) : base(address, displayName, displayNameEncoding)
        {
            this.TemplateData = TemplateData;
        }

        public virtual object TemplateData { get; set; }


        public List<Attachment> Attachments { get; set; }

        public TemplateMailAddress AddAttachment(string fileName) => AddAttachment(new Attachment(fileName));
        public TemplateMailAddress AddAttachment(FileInfo file) => AddAttachment(file?.FullName);
        public TemplateMailAddress AddAttachment(Attachment attachment)
        {
            Attachments = Attachments ?? new List<Attachment>();
            Attachments.Add(attachment);
            return this;
        }

        public static IEnumerable<TemplateMailAddress> FromList<T>(IEnumerable<T> Data, Expression<Func<T, string>> EmailSelector, Expression<Func<T, string>> NameSelector = null) => (Data ?? Array.Empty<T>()).AsEnumerable().Select(x => FromObject(x, EmailSelector, NameSelector)).Where(x => x != null);

        public static TemplateMailAddress FromObject<T>(T Data, Expression<Func<T, string>> EmailSelector, Expression<Func<T, string>> NameSelector = null)
        {
            if (Data != null)
            {
                var name = Text.Empty;
                var email = Text.Empty;

                if (EmailSelector != null)
                {
                    email = EmailSelector.Compile().Invoke(Data);
                }

                if (NameSelector != null)
                {
                    name = NameSelector.Compile().Invoke(Data);
                }

                if (name.IsBlank())
                {
                    return new TemplateMailAddress(email, Data);
                }
                else
                {
                    return new TemplateMailAddress(email, name, Data);
                }
            }
            return null;
        }
    }
}