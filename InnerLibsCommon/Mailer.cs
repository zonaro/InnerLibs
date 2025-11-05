using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Text;
using Extensions;
using Extensions.ComplexText;
using Extensions.Web;
using static System.Net.Mime.MediaTypeNames;

namespace Extensions.Mail
{
    /// <summary>
    /// Wrapper for <see cref="System.Net.Mail"/> in FluentAPI with Template configurations and helper methods.
    /// Uses objects of type <see cref="Dictionary{string,object}"/> for template objects.
    /// </summary>
    public class FluentMailMessage : FluentMailMessage<Dictionary<string, object>>
    {
        #region Public Methods

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Recipients"></param>
        /// <param name="EmailSelector"></param>
        /// <param name="NameSelector"></param>
        /// <returns></returns>
        public static FluentMailMessage<T> FromData<T>(IEnumerable<T> Recipients, Expression<Func<T, string>> EmailSelector, Expression<Func<T, string>> NameSelector) where T : class => new FluentMailMessage<T>().AddRecipient(EmailSelector, NameSelector, Recipients?.ToArray() ?? Array.Empty<T>());

        public static FluentMailMessage<T> FromData<T>(IEnumerable<T> Recipients, Expression<Func<T, string>> EmailSelector) where T : class => new FluentMailMessage<T>().AddRecipient(EmailSelector, Recipients?.ToArray() ?? Array.Empty<T>());

        /// <summary>
        /// Creates a <see cref="FluentMailMessage{T}"/> instance from the specified data.
        /// </summary>
        /// <typeparam name="T">The type of the data.</typeparam>
        /// <param name="EmailSelector">An expression that selects the email address from the data.</param>
        /// <param name="NameSelector">An expression that selects the name from the data.</param>
        /// <param name="Recipients">The data objects representing the recipients.</param>
        /// <returns>A <see cref="FluentMailMessage{T}"/> instance.</returns>
        /// <remarks>
        /// This method creates a <see cref="FluentMailMessage{T}"/> instance by extracting the email address and name from the specified data objects.
        /// The <paramref name="EmailSelector"/> expression is used to select the email address property from the data objects,
        /// and the <paramref name="NameSelector"/> expression is used to select the name property.
        /// The <paramref name="Recipients"/> parameter represents the data objects that will be used as recipients.
        /// </remarks>
        public static FluentMailMessage<T> FromData<T>(Expression<Func<T, string>> EmailSelector, Expression<Func<T, string>> NameSelector, params T[] Recipients) where T : class => FromData(Recipients?.AsEnumerable(), EmailSelector, NameSelector);

        /// <summary>
        /// Creates a new instance of the <see cref="FluentMailMessage{T}"/> class from the specified data.
        /// </summary>
        /// <typeparam name="T">The type of the data.</typeparam>
        /// <param name="EmailSelector">The expression that selects the email address from the data.</param>
        /// <param name="Recipients">The array of data objects representing the recipients.</param>
        /// <returns>An instance of the <see cref="FluentMailMessage{T}"/> class.</returns>
        /// <remarks>
        /// This method creates a new instance of the <see cref="FluentMailMessage{T}"/> class using the specified data.
        /// The <paramref name="EmailSelector"/> expression is used to select the email address from the data.
        /// The <paramref name="Recipients"/> array represents the recipients of the email.
        /// </remarks>
        public static FluentMailMessage<T> FromData<T>(Expression<Func<T, string>> EmailSelector, params T[] Recipients) where T : class => FromData(Recipients?.AsEnumerable(), EmailSelector);

        /// <summary>
        /// Creates a new instance of the <see cref="FluentMailMessage{T}"/> class using the specified recipient, email selector, and name selector.
        /// </summary>
        /// <typeparam name="T">The type of the recipient.</typeparam>
        /// <param name="Recipient">The recipient object.</param>
        /// <param name="EmailSelector">The expression that selects the email address from the recipient object.</param>
        /// <param name="NameSelector">The expression that selects the name from the recipient object.</param>
        /// <returns>A new instance of the <see cref="FluentMailMessage{T}"/> class.</returns>
        /// <remarks>
        /// This method is used to create a new mail message with a single recipient using the specified expressions to extract the email address and name from the recipient object.
        /// </remarks>
        public static FluentMailMessage<T> FromData<T>(T Recipient, Expression<Func<T, string>> EmailSelector, Expression<Func<T, string>> NameSelector) where T : class => FromData(EmailSelector, NameSelector, new[] { Recipient });

        /// <summary>
        /// Creates a new instance of the <see cref="FluentMailMessage{T}"/> class from the specified data.
        /// </summary>
        /// <typeparam name="T">The type of the recipient.</typeparam>
        /// <param name="Recipient">The recipient object.</param>
        /// <param name="EmailSelector">The expression that selects the email address from the recipient object.</param>
        /// <returns>A new instance of the <see cref="FluentMailMessage{T}"/> class.</returns>
        /// <remarks>
        /// This method is used to create a new <see cref="FluentMailMessage{T}"/> instance by specifying the recipient object and an expression that selects the email address from the recipient object.
        /// </remarks>
        public static FluentMailMessage<T> FromData<T>(T Recipient, Expression<Func<T, string>> EmailSelector) where T : class => FromData(EmailSelector, new[] { Recipient });

        /// <summary>
        /// Represents a Simple Mail Transfer Protocol (SMTP) client that can be used to send email with Gmail Provider.
        /// </summary>
        public static SmtpClient GmailSmtp() => new SmtpClient("smtp.gmail.com", 587) { EnableSsl = true };

        /// <summary>
        /// Represents a Simple Mail Transfer Protocol (SMTP) client that sends email using Locaweb Provider.
        /// </summary>
        public static SmtpClient LocawebSmtp() => new SmtpClient("email-ssl.com.br", 587) { EnableSsl = true };

        /// <summary>
        /// Represents a Simple Mail Transfer Protocol (SMTP) client that can be used to send email using Office365 Provider.
        /// </summary>
        public static SmtpClient Office365Smtp() => new SmtpClient("smtp.office365.com", 587) { EnableSsl = true };

        /// <summary>
        /// Represents a Simple Mail Transfer Protocol (SMTP) client that can be used to send email using Outlook Provider.
        /// </summary>
        public static SmtpClient OutlookSmtp() => new SmtpClient("smtp-mail.outlook.com", 587) { EnableSsl = true };

        /// <summary>
        /// Represents a Simple Mail Transfer Protocol (SMTP) client that can be used to send email using Hostinger Provider.
        /// </summary>
        public static SmtpClient HostingerSmtp() => new SmtpClient("smtp.hostinger.com", 465) { EnableSsl = true };

        public static SentStatus QuickSend(string Email, string Password, string Recipient, string Subject, string Message, Dictionary<string, object> TemplateData = null) => QuickSend<Dictionary<string, object>>(Email, Password, Recipient, Subject, Message, TemplateData);

        public static SentStatus QuickSend<T>(string Email, string Password, string Recipient, string Subject, string Message, T TemplateData) where T : class => new FluentMailMessage<T>().WithQuickConfig(Email, Password).AddRecipient(Recipient, TemplateData).WithSubject(Subject).WithMessage(Message).OnError((m, a, ex) => Util.WriteDebug(ex.ToFullExceptionString())).SendAndDispose();

        /// <summary>
        /// Cria e dispara rapidamente uma <see cref="FluentMailMessage"/>
        /// </summary>
        /// <param name="Email">Email do remetende e utilizado nas credenciais de SMTP</param>
        /// <param name="Password">Senha utilizada nas credenciais de SMTP</param>
        /// <param name="SmtpHost">Host SMTP</param>
        /// <param name="SmtpPort">Porta SMTP</param>
        /// <param name="UseSSL">SSL do SMTP</param>
        /// <param name="Recipient">Destinatários</param>
        /// <param name="Subject">Assunto do Email</param>
        /// <param name="Message">Corpo da Mensagem</param>
        /// <returns></returns>
        public static SentStatus QuickSend(string Email, string Password, string SmtpHost, int SmtpPort, bool UseSSL, string Recipient, string Subject, string Message, Dictionary<string, object> TemplateData = null) => QuickSend<Dictionary<string, object>>(Email, Password, SmtpHost, SmtpPort, UseSSL, Recipient, Subject, Message, TemplateData);

        /// <inheritdoc cref="QuickSend"/>
        public static SentStatus QuickSend<T>(string Email, string Password, string SmtpHost, int SmtpPort, bool UseSSL, string Recipient, string Subject, string Message, T TemplateData) where T : class => new FluentMailMessage<T>().WithSmtp(SmtpHost, SmtpPort, UseSSL).WithCredentials(Email, Password).AddRecipient(Recipient, TemplateData).WithSubject(Subject).WithMessage(Message).OnError((m, a, ex) => Util.WriteDebug(ex.ToFullExceptionString())).SendAndDispose();

        #endregion Public Methods
    }

    /// <summary>
    /// Wrapper for <see cref="System.Net.Mail"/> in FluentAPI with Template configurations and helper methods.
    /// Uses objects of type <typeparamref name="T"/> for template objects.
    /// </summary>
    public class FluentMailMessage<T> : MailMessage where T : class
    {
        #region Private Fields

        private List<(TemplateMailAddress<T> Template, SentStatus Status, Exception Error)> _status = new List<(TemplateMailAddress<T> Template, SentStatus Status, Exception Error)>();

        #endregion Private Fields

        #region Private Methods

        public bool CaseSensitive { get; set; } = false;

        public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

        public FluentMailMessage<T> WithCulture(string culture)
        {
            Culture = new CultureInfo(culture);
            return this;
        }
        public FluentMailMessage<T> WithCulture(CultureInfo culture)
        {
            this.Culture = culture ?? CultureInfo.InvariantCulture;
            return this;
        }

        public FluentMailMessage<T> WithCaseSensitive(bool CaseSensitive)
        {
            this.CaseSensitive = CaseSensitive;
            return this;
        }

        /// <summary>
        /// Envia os emails
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private FluentMailMessage<T> Send(string TestEmail)
        {
            if (Smtp != null)
            {
                if (TestEmail.IsNotBlank() && !TestEmail.IsEmail())
                {
                    throw new ArgumentException("TestEmail is not a valid email", nameof(TestEmail));
                }

                ResetStatus();

                foreach (var mailMessage in GenerateEmails())
                {
                    TemplateMailAddress<T> newto;
                    try
                    {
                        if (TestEmail.IsEmail())
                        {
                            mailMessage.To.Clear();
                            mailMessage.Bcc.Clear();
                            mailMessage.CC.Clear();
                            mailMessage.To.Add(TestEmail);
                        }

                        Smtp.Send(mailMessage);

                        newto = new TemplateMailAddress<T>(mailMessage.To.First());
                        _status.Add((newto, SentStatus.Success, null));
                        SuccessAction?.Invoke(newto, this);
                    }
                    catch (Exception ex)
                    {
                        newto = new TemplateMailAddress<T>(mailMessage.To.First());
                        _status.Add((newto, SentStatus.Error, ex));
                        ErrorAction?.Invoke(newto, this, ex);
                    }
                    finally
                    {
                        mailMessage.Dispose();
                    }
                }
            }
            else
            {
                throw new SmtpException(SmtpStatusCode.GeneralFailure, "Smtp is null");
            }

            return this;
        }

        #endregion Private Methods

        #region Public Constructors

        /// <summary>
        /// Instancia uma nova <see cref="FluentMailMessage{T}"/> vazia
        /// </summary>
        public FluentMailMessage() : base()
        {
            this.IsBodyHtml = true;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Ação executada quando ocorrer erro no disparo
        /// </summary>
        public Action<TemplateMailAddress<T>, MailMessage, Exception> ErrorAction { get; set; }

        /// <summary>
        /// Lista contendo os endereços de email que encontraram algum erro ao serem enviados
        /// </summary>
        public IEnumerable<(TemplateMailAddress<T> Template, Exception Error)> ErrorList => SentStatusList.Where(x => x.Status.HasFlag(SentStatus.Error)).Select(x => (x.Destination, x.Error));

        /// <summary>
        /// Status geral do disparo
        /// </summary>
        public SentStatus SentStatus => SentStatusList.Select(x => x.Status).Aggregate((x, y) => x | y);

        /// <summary>
        /// Lista contendo os destinatários e o status do disparo
        /// </summary>
        public IEnumerable<(TemplateMailAddress<T> Destination, SentStatus Status, Exception Error)> SentStatusList => _status.AsEnumerable();

        /// <summary>
        /// Informações de SMTP
        /// </summary>
        public SmtpClient Smtp { get; set; } = new SmtpClient();

        /// <summary>
        /// Ação executada quando o disparo for concluido com êxito
        /// </summary>
        public Action<TemplateMailAddress<T>, MailMessage> SuccessAction { get; set; }

        /// <summary>
        /// Lista contendo os endereços de email que foram enviados com sucesso
        /// </summary>
        public IEnumerable<TemplateMailAddress<T>> SuccessList => SentStatusList.Where(x => x.Status.HasFlag(SentStatus.Success)).Select(x => x.Destination);

        #endregion Public Properties

        
        #region Public Methods

        /// <summary>
        /// Adciona um anexo ao email
        /// </summary>
        /// <param name="Attachment"></param>
        /// <returns></returns>
        public FluentMailMessage<T> AddAttachment(params Attachment[] Attachment) => AddAttachment((Attachment ?? Array.Empty<Attachment>()).AsEnumerable());

        /// <summary>
        /// Cria e adciona um arquivo de texto ao email. Este arquivo é criado diretamente na mémoria e não no armazenamento.
        /// </summary>
        public FluentMailMessage<T> AddAttachment(string TextAttachment, string Name, Encoding encoding = default)
        {
            encoding = encoding ?? Encoding.UTF8;
            var bytes = encoding.GetBytes(TextAttachment);
            return AddAttachment(bytes, Name);
        }

        /// <summary>
        /// Adciona um anexo ao email
        /// </summary>
        public FluentMailMessage<T> AddAttachment(IEnumerable<Attachment> Attachment)
        {
            if (Attachment != null && Attachment.Any())
                foreach (var item in Attachment)
                    if (item != null && item.ContentStream.Length > 0)
                        this.Attachments.Add(item);
            return this;
        }

        /// <summary>
        /// Adciona um anexo ao email
        /// </summary>
        public FluentMailMessage<T> AddAttachment(Stream Attachment, string Name)
        {
            var att = Attachment.ToAttachment(Name);
            if (att != null) this.Attachments.Add(att);
            return this;
        }

        /// <summary>
        /// Adciona um anexo ao email
        /// </summary>
        public FluentMailMessage<T> AddAttachment(byte[] Attachment, string Name)
        {
            var att = Attachment.ToAttachment(Name);
            if (att != null) this.Attachments.Add(att);
            return this;
        }

        /// <summary>
        /// Adciona um anexo ao email
        /// </summary>
        public FluentMailMessage<T> AddAttachment(FileInfo Attachment)
        {
            var att = Attachment.ToAttachment();
            if (att != null) this.Attachments.Add(att);
            return this;
        }

        /// <summary>
        /// Adiciona endereços ao BCC do email
        /// </summary>
        /// <param name="Emails"></param>
        /// <returns></returns>
        public FluentMailMessage<T> AddBcc(params string[] Emails)
        {
            foreach (var email in (Emails ?? Array.Empty<string>()).SelectMany(x => x.ExtractEmails()).ToArray())
                Bcc.Add(new MailAddress(email));
            return this;
        }

        /// <summary>
        /// Adiciona endereços ao BCC do email
        /// </summary>
        /// <param name="Emails"></param>
        /// <returns></returns>
        public FluentMailMessage<T> AddBcc(params MailAddress[] Emails)
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
        public FluentMailMessage<T> AddCc(params string[] Emails)
        {
            foreach (var email in (Emails ?? Array.Empty<string>()).SelectMany(x => x.ExtractEmails()).ToArray())
                CC.Add(new MailAddress(email));
            return this;
        }

        /// <summary>
        /// Adiciona endereços ao CC do email
        /// </summary>
        /// <param name="Emails"></param>
        /// <returns></returns>
        public FluentMailMessage<T> AddCc(params MailAddress[] Emails)
        {
            foreach (var email in Emails ?? Array.Empty<MailAddress>())
                CC.Add(email);
            return this;
        }

        /// <summary>
        /// Adiciona um destinatário a lista de destinatários deta mensagem
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage<T> AddRecipient(string Email, T TemplateData) => AddRecipient(new TemplateMailAddress<T>(Email, TemplateData));

        /// <summary>
        /// Adiciona um destinatário a lista de destinatários deta mensagem
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Email"></param>
        /// <param name="DisplayName"></param>
        /// <param name="TemplateData"></param>
        /// <returns></returns>
        public FluentMailMessage<T> AddRecipient(string Email, string DisplayName, T TemplateData) => AddRecipient(new TemplateMailAddress<T>(Email, DisplayName, TemplateData));

        /// <summary>
        /// Adiciona um destinatário a lista de destinatários deta mensagem
        /// </summary>
        public FluentMailMessage<T> AddRecipient(Expression<Func<T, string>> EmailSelector, Expression<Func<T, string>> NameSelector, params T[] Data)
        {
            foreach (var x in Data ?? Array.Empty<T>())
                AddRecipient(new TemplateMailAddress<T>(x, EmailSelector, NameSelector));
            return this;
        }

        public FluentMailMessage<T> AddRecipient(Expression<Func<T, string>> EmailSelector, params T[] Data)
        {
            foreach (var x in Data ?? Array.Empty<T>())
                AddRecipient(new TemplateMailAddress<T>(x, EmailSelector));
            return this;
        }


        /// <summary>
        /// Adiciona um destinatário a lista de destinatários desta mensagem
        /// </summary>
        public FluentMailMessage<T> AddRecipient(params string[] Emails)
        {
            foreach (var email in (Emails ?? Array.Empty<string>()).SelectMany(x => x.ExtractEmails()).ToArray()) AddRecipient(new TemplateMailAddress<T>(email));
            return this;
        }

        public FluentMailMessage<T> AddRecipient(params TemplateMailAddress<T>[] Emails)
        {
            foreach (var email in Emails ?? Array.Empty<TemplateMailAddress<T>>()) To.Add(email);
            return this;
        }

     

        /// <summary>
        /// Configura o remetente da mensagem
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage<T> FromEmail(string Email)
        {
            From = new MailAddress(Email);
            return this;
        }

        /// <summary>
        /// Configura o remetente da mensagem
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage<T> FromEmail(string Email, string DisplayName)
        {
            From = new MailAddress(Email, DisplayName);
            return this;
        }

        /// <summary>
        /// Configura o remetente da mensagem
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage<T> FromEmail(MailAddress Email)
        {
            From = Email;
            return this;
        }

        /// <summary>
        /// Retorna uma lista de <see cref="FluentMailMessage{T}"/> com todas as mensagens de emails
        /// geradas por essa <see cref="FluentMailMessage{T}"/>
        /// </summary>
        public IEnumerable<MailMessage> GenerateEmails()
        {
            if (To != null)
                foreach (var item in To)
                {
                    if (item != null)
                    {
                        var msgIndiv = new MailMessage();

                        string msg = Body.IfBlank(Util.EmptyString);
                        string subj = Subject.IfBlank(Util.EmptyString);

                        if (item is TemplateMailAddress<T> templateMail)
                        {
                            if (templateMail.TemplateData != null)
                            {
                                msg = msg.Inject(templateMail.TemplateData, Culture, CaseSensitive);
                                subj = subj.Inject(templateMail.TemplateData, Culture, CaseSensitive);
                                 
                            }
                            foreach (var att in templateMail.Attachments) msgIndiv.Attachments.Add(att);
                        }

                        msgIndiv.To.Add(item);
                        msgIndiv.Body = msg;
                        msgIndiv.Subject = subj;
                        msgIndiv.From = this.From;
                        msgIndiv.Sender = this.Sender;
                        msgIndiv.IsBodyHtml = this.IsBodyHtml;
                        msgIndiv.BodyEncoding = this.BodyEncoding;
                        msgIndiv.BodyTransferEncoding = this.BodyTransferEncoding;
                        msgIndiv.DeliveryNotificationOptions = this.DeliveryNotificationOptions;
                        msgIndiv.SubjectEncoding = this.SubjectEncoding;
                        msgIndiv.HeadersEncoding = this.HeadersEncoding;
                        msgIndiv.Priority = this.Priority;
                        msgIndiv.Headers.Add(this.Headers);

                        foreach (var email in this.CC)
                        {
                            msgIndiv.CC.Add(email);
                        }

                        foreach (var email in this.ReplyToList)
                        {
                            msgIndiv.ReplyToList.Add(email);
                        }

                        foreach (var email in this.CC)
                        {
                            msgIndiv.CC.Add(email);
                        }

                        foreach (var email in this.Bcc)
                        {
                            msgIndiv.Bcc.Add(email);
                        }

                        foreach (var att in this.Attachments)
                        {
                            msgIndiv.Attachments.Add(att);
                        }

                        foreach (var alt in this.AlternateViews)
                        {
                            msgIndiv.AlternateViews.Add(alt);
                        }

                        yield return msgIndiv;
                    }
                }
        }

        /// <summary>
        /// Função executada quando houver um disparo com erro
        /// </summary>
        /// <param name="Action"></param>
        /// <returns></returns>
        public FluentMailMessage<T> OnError(Action<TemplateMailAddress<T>, MailMessage, Exception> Action)
        {
            ErrorAction = Action;
            return this;
        }

        /// <summary>
        /// Função executada quando houver um disparo com sucesso
        /// </summary>
        /// <param name="Action"></param>
        /// <returns></returns>
        public FluentMailMessage<T> OnSuccess(Action<TemplateMailAddress<T>, MailMessage> Action)
        {
            SuccessAction = Action;
            return this;
        }

        /// <summary>
        /// Reinicia o status de disparo
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage<T> ResetStatus()
        {
            _status = _status ?? new List<(TemplateMailAddress<T> Template, SentStatus Status, Exception Error)>();
            _status.Clear();
            return this;
        }

        public FluentMailMessage<T> Send() => Send(null);

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
        /// Envia os emails para um destinatário de teste.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public FluentMailMessage<T> SendTest(string Email) => Email.IsEmail() ? Send(Email) : throw new ArgumentException($"Invalid email: {Email}", nameof(Email));

        /// <summary>
        /// Retorna o corpo da mensagem
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Body;

        /// <summary>
        /// Configura o SMTP do Gmail para este disparo
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage<T> WithGmailSmtp()
        {
            Smtp = FluentMailMessage.GmailSmtp();
            return this;
        }

        /// <summary>
        /// Configura o SMTP da Locaweb para este disparo
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage<T> WithLocawebSmtp()
        {
            Smtp = FluentMailMessage.LocawebSmtp();
            return this;
        }

        /// <summary>
        /// Configura o SMTP do Office365 para este disparo
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage<T> WithOffice365Smtp()
        {
            Smtp = FluentMailMessage.Office365Smtp();
            return this;
        }

        /// <summary>
        /// Configura o SMTP do Outlook para este disparo
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage<T> WithOutlookSmtp()
        {
            Smtp = FluentMailMessage.OutlookSmtp();
            return this;
        }

        /// <summary>
        /// Configura o SMTP do Outlook para este disparo
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage<T> WithHostingerSmtp()
        {
            Smtp = FluentMailMessage.HostingerSmtp();
            return this;
        }

        /// <summary>
        /// Configura a mensagem a ser enviada. O texto da mensagem será injetado na variável {Message}
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage<T> WithMessage(string Message) => WithMessage("{Message}", new { Message });



        /// <summary>
        /// Utiliza um template para o corpo da mensagem
        /// </summary>
        /// <param name="TemplateOrFilePathOrUrl"></param>
        /// <returns></returns>
        public FluentMailMessage<T> WithMessage(string TemplateOrFilePathOrUrl, string Message) => WithMessage(TemplateOrFilePathOrUrl, new { Message, PreviewMessage = Message.GetTextPreview(100) });


        /// <summary>
        /// Utiliza um template para o corpo da mensagem
        /// </summary>
        public FluentMailMessage<T> WithMessage(FileInfo Template) => WithMessage(Template?.FullName ?? Util.EmptyString, Util.EmptyString);

        /// <summary>
        /// Utiliza um template para o corpo da mensagem
        /// </summary>
        public FluentMailMessage<T> WithMessage(FileInfo Template, string Message) => WithMessage(Template?.FullName, Message);

        /// <summary>
        /// Utiliza um template para o corpo da mensagem
        /// </summary>
        public FluentMailMessage<T> WithMessage<TMessage>(FileInfo Template, TMessage DataToInject) where TMessage : class => WithMessage(Template?.FullName ?? Util.EmptyString, DataToInject);

        /// <summary>
        /// Utiliza um template para o corpo da mensagem
        /// </summary>
        public FluentMailMessage<T> WithMessage(DirectoryInfo TemplateDirectory, string TemplateFileName) => WithMessage(Path.Combine(TemplateDirectory?.FullName ?? Util.EmptyString, TemplateFileName ?? Util.EmptyString), Util.EmptyString);

        /// <summary>
        /// Utiliza um template para o corpo da mensagem
        /// </summary>
        public FluentMailMessage<T> WithMessage(DirectoryInfo TemplateDirectory, string TemplateFileName, string Message) => WithMessage(Path.Combine(TemplateDirectory?.FullName ?? Util.EmptyString, TemplateFileName ?? Util.EmptyString), Message);

        /// <summary>
        /// Utiliza um template para o corpo da mensagem
        /// </summary>
        public FluentMailMessage<T> WithMessage<TMessage>(DirectoryInfo TemplateDirectory, string TemplateFileName, TMessage DataToInject) where TMessage : class => WithMessage(Path.Combine(TemplateDirectory?.FullName ?? Util.EmptyString, TemplateFileName ?? Util.EmptyString), DataToInject);

        /// <summary>
        /// Utiliza um template para o corpo da mensagem.
        /// </summary>
        /// <param name="TemplateOrFilePathOrUrl">Template HTML, URL ou caminho do arquivo</param>
        /// <param name="DataToInject">Dados para serem injetados no template. </param>
        /// <returns></returns>
        public FluentMailMessage<T> WithMessage<TMessage>(string TemplateOrFilePathOrUrl, TMessage DataToInject) where TMessage : class
        {
            if (TemplateOrFilePathOrUrl.IsFilePath())
            {
                if (File.Exists(TemplateOrFilePathOrUrl)) TemplateOrFilePathOrUrl = File.ReadAllText(TemplateOrFilePathOrUrl);
            }

            if (TemplateOrFilePathOrUrl.IsURL())
            {
                TemplateOrFilePathOrUrl = Util.DownloadString(TemplateOrFilePathOrUrl);
            }

            if (DataToInject != null)
            {
                TemplateOrFilePathOrUrl = TemplateOrFilePathOrUrl.Inject(DataToInject, Culture, CaseSensitive);
            }

            Body = TemplateOrFilePathOrUrl;
            return this;
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
        public FluentMailMessage<T> WithCredentials(NetworkCredential Credentials)
        {
            if (Smtp == null) throw new ArgumentException("SMTP is null, Define SMTP before defining credentials", nameof(Smtp));
            Smtp.Credentials = Credentials ?? throw new ArgumentNullException("Credentials is null", nameof(Credentials));
            if (From == null || From.Address.IsNotValid())
            {
                From = new MailAddress(Credentials.UserName);
            }

            return this;
        }

        /// <summary>
        /// Configura as credenciais do SMTP
        /// </summary>
        public FluentMailMessage<T> WithCredentials(string Login, string Password) => Smtp == null || Smtp.Host.IsNotValid()
                ? WithQuickConfig(Login, Password)
                : WithCredentials(new NetworkCredential(Login, Password));

        public FluentMailMessage<T> WithSmtpAndCredentials(string Login, string Password, string Host, int Port, bool UseSSL) => WithSmtp(Host, Port, UseSSL).WithCredentials(Login, Password);

        /// <summary>
        /// Configura o email com prioridade alta
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage<T> WithHighPriority() => WithPriority(MailPriority.High);

        /// <summary>
        /// Configura o email com prioridade baixa
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage<T> WithLowPriority() => WithPriority(MailPriority.Low);

        /// <summary>
        /// Configura o email com prioridade normal
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage<T> WithNormalPriority() => WithPriority(MailPriority.Normal);

        /// <summary>
        /// Configura a prioridade do email
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        public FluentMailMessage<T> WithPriority(MailPriority priority)
        {
            Priority = priority;
            return this;
        }

        public static SmtpClient FindSmtp(string Email)
        {
            if (Email.IsEmail() == false)
                throw new ArgumentException("Email is not valid", nameof(Email));

            var domain = Email.GetDomain();

            switch (domain)
            {
                case "outlook.com":
                case "outlook.com.br":
                case "hotmail.com":
                case "live.com":
                    return FluentMailMessage.OutlookSmtp();

                case "office365.com":
                    return FluentMailMessage.Office365Smtp();

                case "gmail.com":
                    return FluentMailMessage.GmailSmtp();

                default:
                    return new SmtpClient($"smtp.{domain}", 587) { EnableSsl = true };
            }
        }

        /// <summary>
        /// Configura o SMTP para este disparo a partir de um email e senha
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage<T> WithQuickConfig(string Email, string Password)
        {
            if (Email.IsEmail() == false)
                throw new ArgumentException("Email is not valid", nameof(Email));

            var domain = Email.GetDomain();

            var smtp = FindSmtp(Email);

            WithSmtp(smtp);

            WithCredentials(Email, Password);

            return this;
        }

        /// <summary>
        /// Configura o SMTP para este disparo
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage<T> WithSmtp(SmtpClient Client)
        {
            Smtp = Client;
            return this;
        }

        /// <summary>
        /// Configura o SMTP para este disparo
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage<T> WithSmtp(string Host) => WithSmtp(new SmtpClient(Host));

        /// <summary>
        /// Configura o SMTP para este disparo
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage<T> WithSmtp(string Host, int Port) => WithSmtp(new SmtpClient(Host, Port));

        /// <summary>
        /// Configura o SMTP para este disparo
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage<T> WithSmtp(string Host, int Port, bool UseSSL) => WithSmtp(new SmtpClient(Host, Port) { EnableSsl = UseSSL });

        /// <summary>
        /// Configura o assunto do email
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage<T> WithSubject(string Text, object DataToInject)
        {
            Subject = Text;
            if (DataToInject != null)
            {
                Subject = Subject.Inject(DataToInject, Culture, CaseSensitive);
            }
            return this;
        }

        /// <summary>
        /// Configura a mensagem a ser enviada. O texto da mensagem será injetado na variável {Message}
        /// </summary>
        /// <returns></returns>
        public FluentMailMessage<T> WithSubject(string Text) => WithSubject("{Text}", new { Text });

        /// <summary>
        /// Ajusta o timeout do SMTP em milisegundos. O valor mínimo aceito é 1000ms
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public FluentMailMessage<T> WithTimeout(int value)
        {
            value = value.SetMinValue(1000);
            return WithTimeout(TimeSpan.FromMilliseconds(value));
        }

        /// <summary>
        /// Ajusta o timeout do SMTP
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public FluentMailMessage<T> WithTimeout(TimeSpan value)
        {
            this.Timeout = value;
            return this;
        }

        public TimeSpan Timeout
        {
            get => TimeSpan.FromMilliseconds(Smtp?.Timeout ?? 100000);
            set
            {
                if (Smtp != null)
                {
                    Smtp.Timeout = (int)value.TotalMilliseconds;

                }
                else
                {
                    throw new ArgumentException("Smtp is null", nameof(Smtp));
                }
            }
        }

        #endregion Public Methods
    }

    public class TemplateMailAddress : TemplateMailAddress<Dictionary<string, object>>
    {
        #region Public Constructors

        public TemplateMailAddress(string address, Dictionary<string, object> TemplateData = null) : base(address, TemplateData)
        {
        }

        public TemplateMailAddress(string address, string displayName, Dictionary<string, object> TemplateData = null) : base(address, displayName, TemplateData)
        {
        }

        public TemplateMailAddress(string address, string displayName, Encoding displayNameEncoding, Dictionary<string, object> TemplateData = null) : base(address, displayName, displayNameEncoding, TemplateData)
        {
        }

        #endregion Public Constructors
    }

    /// <summary>
    /// Um destinatário de email contendo informações que serão aplicadas em um template
    /// </summary>
    public class TemplateMailAddress<T> : MailAddress where T : class
    {
        #region Internal Fields

        /// <summary>
        /// Lista de anexos exclusivos deste destinatário
        /// </summary>
        internal List<Attachment> anexos = new List<Attachment>();

        #endregion Internal Fields

        #region Public Constructors

        public TemplateMailAddress(MailAddress address) : base(address?.Address, address?.DisplayName)
        {
            if (address is TemplateMailAddress<T> a)
            {
                this.TemplateData = a.TemplateData;
            }
        }

        public TemplateMailAddress(MailAddress address, T TemplateData) : base(address?.Address, address?.DisplayName)
        {
            this.TemplateData = TemplateData;
        }

        public TemplateMailAddress(string address, T TemplateData) : base(address)
        {
            this.TemplateData = TemplateData;
        }

        public TemplateMailAddress(string address) : base(address)
        {
        }

        public TemplateMailAddress(string address, string displayName, T TemplateData) : base(address, displayName)
        {
            this.TemplateData = TemplateData;
        }

        public TemplateMailAddress(string address, string displayName, Encoding displayNameEncoding, T TemplateData) : base(address, displayName, displayNameEncoding)
        {
            this.TemplateData = TemplateData;
        }

        public TemplateMailAddress(T Data, Expression<Func<T, string>> EmailSelector, Expression<Func<T, string>> NameSelector) : this(Data == null ? throw new ArgumentException("Data is null", nameof(Data)) : EmailSelector?.Compile().Invoke(Data) ?? throw new ArgumentException("EmailSelector is null", nameof(EmailSelector)), (NameSelector ?? EmailSelector)?.Compile().Invoke(Data), Data)
        {
        }

        public TemplateMailAddress(T Data, Expression<Func<T, string>> EmailSelector) : this(Data == null ? throw new ArgumentException("Data is null", nameof(Data)) : EmailSelector?.Compile().Invoke(Data) ?? throw new ArgumentException("EmailSelector is null", nameof(EmailSelector)), Data)
        {
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Procura anexos em <see cref="TemplateData"/>
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Busca por propriedades <see cref="FileInfo"/> e <see cref="Attachment"/>. Arrays destes
        /// mesmos tipos serão percorridos. <see cref="DirectoryInfo"/> serão percorridos recursivamente
        /// </remarks>
        public IEnumerable<Attachment> Attachments
        {
            get
            {
                var l = new List<Attachment>();
                if (TemplateData != null)
                    foreach (var p in TemplateData.CreateDictionary())
                    {
                        if (p.Value != null)
                        {
                            p.WriteDebug();

                            var tipo = p.Value.GetTypeOf();

                            if (p.Value is Attachment att1)
                            {
                                att1.WriteDebug($"{att1.Name} from Attachment property {p.Key}");
                                l.Add(att1);
                                continue;
                            }

                            if (p.Value is FileInfo f)
                            {
                                var af = f.ToAttachment();
                                if (af != null)
                                {
                                    af.WriteDebug($"{f.Name} from FileInfo property {p.Key}");
                                    l.Add(af);
                                    continue;
                                }
                            }

                            if (p.Value is DirectoryInfo d)
                            {
                                Util.WriteDebug($"Searching files into {d.FullName}");

                                foreach (var ff in d.GetFiles("*", SearchOption.AllDirectories))
                                {
                                    var af = ff.ToAttachment();
                                    if (af != null)
                                    {
                                        af.WriteDebug($"{ff.Name} from FileInfo in {ff.Directory.FullName} from property {p.Key}");
                                        l.Add(af);
                                    }
                                }
                                continue;
                            }

                            if (p.Value.IsEnumerableNotString())
                            {
                                tipo.WriteDebug("Traversing");

                                foreach (var oo in (IEnumerable)p.Value)
                                {
                                    if (oo is Attachment att3)
                                    {
                                        att3.WriteDebug($"{att3.Name} from Attachment from {p.Key} in {tipo.Name} type property");
                                        l.Add(att3);
                                        continue;
                                    }

                                    if (oo is FileInfo f2)
                                    {
                                        var af = f2.ToAttachment();
                                        if (af != null)
                                        {
                                            af.WriteDebug($"{f2.Name} from FileInfo from {p.Key} in {tipo.Name} type property");
                                            l.Add(af);
                                        }
                                        continue;
                                    }

                                    if (oo is DirectoryInfo d2)
                                    {
                                        Util.WriteDebug($"Searching files into {d2.FullName}");
                                        foreach (var ff in d2.GetFiles("*", SearchOption.AllDirectories))
                                        {
                                            var af = ff.ToAttachment();
                                            if (af != null)
                                            {
                                                af.WriteDebug($"{ff.Name} from FileInfo in {ff.Directory.FullName} from {p.Key} in {tipo.Name} type property");
                                                l.Add(af);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                l.AddRange(anexos ?? new List<Attachment>());

                return l.RemoveWhere(x => x == null || x.ContentStream.Length <= 0);
            }
        }

        public virtual T TemplateData { get; set; }

        #endregion Public Properties

        #region Public Methods

        public static IEnumerable<TemplateMailAddress<T>> FromData(Expression<Func<T, string>> EmailSelector, Expression<Func<T, string>> NameSelector, params T[] Data) => FromData(Data?.AsEnumerable(), EmailSelector, NameSelector);

        public static IEnumerable<TemplateMailAddress<T>> FromData(IEnumerable<T> Data, Expression<Func<T, string>> EmailSelector, Expression<Func<T, string>> NameSelector) => (Data ?? Array.Empty<T>()).Select(x => new TemplateMailAddress<T>(x, EmailSelector, NameSelector)).Where(x => x != null);

        public static IEnumerable<TemplateMailAddress<T>> FromData(Expression<Func<T, string>> EmailSelector, params T[] Data) => FromData(Data?.AsEnumerable(), EmailSelector);

        public static IEnumerable<TemplateMailAddress<T>> FromData(IEnumerable<T> Data, Expression<Func<T, string>> EmailSelector) => (Data ?? Array.Empty<T>()).Select(x => new TemplateMailAddress<T>(x, EmailSelector)).Where(x => x != null);

        /// <inheritdoc cref="AddAttachment(Attachment)"/>
        public TemplateMailAddress<T> AddAttachment(params string[] files) => AddAttachment(files.Select(file => new Attachment(file)));

        /// <inheritdoc cref="AddAttachment(Attachment)"/>
        public TemplateMailAddress<T> AddAttachment(params FileInfo[] files) => AddAttachment(files.Select(file => file.ToAttachment()));

        /// <summary>
        /// Adiciona um anexo a este email
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        public TemplateMailAddress<T> AddAttachment(params Attachment[] attachments)
        {
            anexos = anexos ?? new List<Attachment>();
            foreach (var attachment in attachments ?? Array.Empty<Attachment>())
            {
                if (attachment != null && anexos.Contains(attachment))
                    anexos.Add(attachment);
            }
            return this;
        }

        public TemplateMailAddress<T> AddAttachment(IEnumerable<Attachment> attachments) => AddAttachment(attachments.ToArray());

        public TemplateMailAddress<T> RemoveAttachment(IEnumerable<Attachment> attachments) => RemoveAttachment(attachments?.ToArray());

        public TemplateMailAddress<T> RemoveAttachment(params Attachment[] attachments)
        {
            anexos = anexos ?? new List<Attachment>();
            foreach (var attachment in attachments ?? Array.Empty<Attachment>())
            {
                if (attachment != null && anexos.Contains(attachment))
                    anexos.Remove(attachment);
            }
            return this;
        }

        public TemplateMailAddress<T> RemoveAttachment(Expression<Func<Attachment, bool>> predicade)
        {
            if (predicade != null)
            {
                anexos = anexos ?? new List<Attachment>();
                anexos.RemoveWhere(predicade);
            }
            return this;
        }

        public TemplateMailAddress<T> RemoveAttachment(params int[] Indexes) => RemoveAttachment(Indexes?.Select(Index => anexos.IfNoIndex(Index)));

        #endregion Public Methods
    }

    [Flags]
    public enum SentStatus
    {
        None = 0,
        Success = 1,
        Error = 2,
        PartialSuccess = Error + Success,
    }
}