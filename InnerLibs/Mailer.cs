using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Text;
using InnerLibs.LINQ;

namespace InnerLibs.Mail
{
    public class TemplateMailAddress : MailAddress
    {


        public static IEnumerable<TemplateMailAddress> FromList<T>(IEnumerable<T> Data, Expression<Func<T, string>> EmailSelector, Expression<Func<T, string>> NameSelector = null)
        {
            return (Data ?? new T[] { }.AsEnumerable()).Select(x => FromObject(x, EmailSelector, NameSelector)).Where(x => x != null);
        }
        public static TemplateMailAddress FromObject<T>(T Data, Expression<Func<T, string>> EmailSelector, Expression<Func<T, string>> NameSelector = null)
        {
            if (Data != null)
            {
                var name = "";
                var email = "";

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

        public virtual object TemplateData { get; set; }

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


    }

    public class FluentMailMessage : MailMessage
    {
        public SmtpClient Smtp { get; set; } = new SmtpClient();
        public Action<MailAddress, FluentMailMessage> SuccessAction { get; set; }
        public Action<MailAddress, FluentMailMessage, Exception> ErrorAction { get; set; }

        private List<MailAddress> _SuccessList = new List<MailAddress>();
        private List<MailAddress> _ErrorList = new List<MailAddress>();

        public string SentStatus
        {
            get
            {
                if (SuccessList.Any() && ErrorList.Any() == false) return "SUCCESS";
                if (SuccessList.Any() == false && ErrorList.Any() == false) return "NOT_SENT";
                return "PARTIAL_SUCCESS";
            }
        }

        public FluentMailMessage ResetStatus()
        {
            _SuccessList = _SuccessList ?? new List<MailAddress>();
            _ErrorList = _ErrorList ?? new List<MailAddress>();
            _SuccessList.Clear();
            _ErrorList.Clear();
            return this;
        }

        public IEnumerable<MailAddress> SuccessList => _SuccessList;

        public IEnumerable<MailAddress> ErrorList => _ErrorList;

        public static SmtpClient GmailSmtp() => new SmtpClient("smtp.gmail.com", 587) { EnableSsl = true };

        public static SmtpClient OutlookSmtp() => new SmtpClient("smtp-mail.outlook.com", 587) { EnableSsl = true };

        public static SmtpClient Office365Smtp() => new SmtpClient("smtp.office365.com", 587) { EnableSsl = true };

        public static SmtpClient LocawebSmtp() => new SmtpClient("email-ssl.com.br", 465) { EnableSsl = true };

        public FluentMailMessage WithSmtp(SmtpClient Client)
        {
            Smtp = Client;
            return this;
        }

        public FluentMailMessage WithSmtp(string Host) => WithSmtp(new SmtpClient(Host));

        public FluentMailMessage WithSmtp(string Host, int Port) => WithSmtp(new SmtpClient(Host, Port));

        public FluentMailMessage WithSmtp(string Host, int Port, bool UseSSL) => WithSmtp(new SmtpClient(Host, Port) { EnableSsl = UseSSL });

        public FluentMailMessage UseGmailSmtp()
        {
            Smtp = GmailSmtp();
            return this;
        }

        public FluentMailMessage UseLocawebSmtp()
        {
            Smtp = LocawebSmtp();
            return this;
        }

        public FluentMailMessage UseOutlookSmtp()
        {
            Smtp = OutlookSmtp();
            return this;
        }

        public FluentMailMessage UseOffice365Smtp()
        {
            Smtp = Office365Smtp();
            return this;
        }

        public FluentMailMessage WithMessage(string Text, bool IsHtml = true)
        {
            Body = Text;
            IsBodyHtml = IsHtml;
            return this;
        }

        public FluentMailMessage WithSubject(string Text)
        {
            Subject = Text;
            return this;
        }



        public FluentMailMessage UseTemplate(string TemplateOrFilePathOrUrl) => UseTemplate(TemplateOrFilePathOrUrl, null);


        public FluentMailMessage UseTemplate(string TemplateOrFilePathOrUrl, string MessageTemplate)
        {
            if (TemplateOrFilePathOrUrl.IsFilePath())
            {
                if (File.Exists(TemplateOrFilePathOrUrl)) TemplateOrFilePathOrUrl = File.ReadAllText(TemplateOrFilePathOrUrl);
            }

            if (TemplateOrFilePathOrUrl.IsURL())
            {
                using (var client = new WebClient())
                {
                    TemplateOrFilePathOrUrl = client.DownloadString(TemplateOrFilePathOrUrl);
                }
            }

            if (MessageTemplate.IsNotBlank())
            {
                TemplateOrFilePathOrUrl.Inject(new { BodyText = MessageTemplate });
            }

            return WithMessage(TemplateOrFilePathOrUrl, true);
        }


        public FluentMailMessage FromEmail(string Email)
        {
            From = new MailAddress(Email);
            return this;
        }

        public FluentMailMessage FromEmail(string Email, string DisplayName)
        {
            From = new MailAddress(Email, DisplayName);
            return this;
        }

        public FluentMailMessage FromEmail(MailAddress Email)
        {
            From = Email;
            return this;
        }

        public FluentMailMessage AddRecipient<T>(string Email, T TemplateData)
        {
            To.Add(new TemplateMailAddress(Email, TemplateData));
            return this;
        }
        public FluentMailMessage AddRecipient<T>(string Email, string DisplayName, T TemplateData)
        {
            To.Add(new TemplateMailAddress(Email, DisplayName, TemplateData));
            return this;
        }

        public FluentMailMessage AddRecipient<T>(T Data, Expression<Func<T, string>> EmailSelector, Expression<Func<T, string>> NameSelector = null)
        {
            To.Add(TemplateMailAddress.FromObject(Data, EmailSelector, NameSelector));
            return this;
        }

        public FluentMailMessage AddRecipient<T>(IEnumerable<T> Data, Expression<Func<T, string>> EmailSelector, Expression<Func<T, string>> NameSelector = null)
        {
            foreach (var m in TemplateMailAddress.FromList(Data, EmailSelector, NameSelector)) To.Add(m);
            return this;
        }

        public FluentMailMessage AddRecipient(params string[] Emails)
        {
            foreach (var email in Emails ?? Array.Empty<string>())
                To.Add(new TemplateMailAddress(email));
            return this;
        }

        public FluentMailMessage AddRecipient(params TemplateMailAddress[] Emails)
        {
            foreach (var email in Emails ?? Array.Empty<TemplateMailAddress>())
                To.Add(email);
            return this;
        }

        public FluentMailMessage AddCarbonCopy(params string[] Emails)
        {
            foreach (var email in Emails ?? Array.Empty<string>())
                CC.Add(new TemplateMailAddress(email));
            return this;
        }

        public FluentMailMessage AddCarbonCopy(params MailAddress[] Emails)
        {
            foreach (var email in Emails ?? Array.Empty<MailAddress>())
                CC.Add(email);
            return this;
        }

        public FluentMailMessage AddBlindCarbonCopy(params string[] Emails)
        {
            foreach (var email in Emails ?? Array.Empty<string>())
                Bcc.Add(new TemplateMailAddress(email));
            return this;
        }

        public FluentMailMessage AddBlindCarbonCopy(params MailAddress[] Emails)
        {
            foreach (var email in Emails ?? Array.Empty<MailAddress>())
                Bcc.Add(email);
            return this;
        }

        public FluentMailMessage WithCredentials(NetworkCredential Credentials)
        {
            if (Credentials is null)
                throw new ArgumentNullException("Credentials");
            if (Smtp is null)
                throw new Exception("SMTP is null");
            Smtp.Credentials = Credentials;
            if (From is null || From.Address.IsBlank())
            {
                From = new MailAddress(Credentials.UserName);
            }

            return this;
        }

        public FluentMailMessage WithCredentials(string Login, string Password) => WithCredentials(new NetworkCredential(Login, Password));


        public FluentMailMessage AddAttachment(params Attachment[] Attachment) => AddAttachment((Attachment ?? Array.Empty<Attachment>()).AsEnumerable());


        public FluentMailMessage AddAttachment(IEnumerable<Attachment> Attachment)
        {
            if (Attachment != null && Attachment.Any())
                foreach (var item in Attachment)
                {
                    this.AddAttachment(item);
                }
            return this;
        }


        public FluentMailMessage AddAttachment(Attachment Attachment)
        {
            if (Attachment != null && Attachment.ContentStream.Length > 0)
                this.Attachments.Add(Attachment);
            return this;
        }

        public FluentMailMessage AddAttachment(Stream Attachment, string Name)
        {
            if (Attachment != null && Attachment.Length > 0 && Name.IsNotBlank())
                this.Attachments.Add(new Attachment(Attachment, Name));
            return this;
        }

        public FluentMailMessage AddAttachment(byte[] Attachment, string Name)
        {
            if (Attachment != null && Attachment.Length > 0 && Name.IsNotBlank())
                using (var s = new MemoryStream(Attachment))
                    this.Attachments.Add(new Attachment(s, Name));
            return this;
        }

        public FluentMailMessage AddAttachment(FileInfo Attachment)
        {
            if (Attachment != null && Attachment.Length > 0)
                this.Attachments.Add(new Attachment(Attachment.FullName));
            return this;
        }

        public FluentMailMessage OnSuccess(Action<MailAddress, FluentMailMessage> Action)
        {
            SuccessAction = Action;
            return this;
        }

        public FluentMailMessage OnError(Action<MailAddress, FluentMailMessage, Exception> Action)
        {
            ErrorAction = Action;
            return this;
        }

        public FluentMailMessage Send()
        {
            _SuccessList = _SuccessList ?? new List<MailAddress>();
            _ErrorList = _ErrorList ?? new List<MailAddress>();
            _SuccessList.Clear();
            _ErrorList.Clear();

            if (Smtp != null)
            {
                foreach (var item in To)
                {
                    if (item != null)
                    {
                        string msg = Body.IfBlank("");
                        string subj = Subject.IfBlank("");
                        if (ReferenceEquals(item.GetType(), typeof(TemplateMailAddress)))
                        {
                            var data = ((TemplateMailAddress)item)?.TemplateData;
                            if (data != null)
                            {
                                msg = msg.Inject(data);
                                subj = subj.Inject(data);
                            }
                        }

                        try
                        {
                            var lista = new[] { item.Address }.ToList();
                            if (CC.Any())
                            {
                                lista.AddRange((IEnumerable<string>)CC);
                            }

                            string emails = lista.SelectJoinString(x => x.ToString(), ",");
                            Smtp.Send(From.ToString(), emails, subj, msg);
                            if (Bcc.Any())
                            {
                                Smtp.Send(From.ToString(), Bcc.SelectJoinString(x => x.ToString(), ","), subj, msg);
                            }

                            if (SuccessAction != null)
                            {
                                SuccessAction.Invoke(item, this);
                            }


                            _SuccessList.Add(item);
                        }

                        catch (Exception ex)
                        {

                            _ErrorList.Add(item);

                            if (ErrorAction != null)
                            {
                                ErrorAction.Invoke(item, this, ex);
                            }

                        }
                    }
                }
            }
            else
            {
                throw new ArgumentException("Smtp is null");
            }

            return this;
        }

        public override string ToString() => Body;
    }
}