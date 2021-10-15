using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Text;
using InnerLibs.LINQ;

namespace InnerLibs
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
        public Action<MailAddress> SuccessAction { get; set; }
        public Action<MailAddress, Exception> ErrorAction { get; set; }

        public static SmtpClient GmailSmtp()
        {
            return new SmtpClient("smtp.gmail.com", 587) { EnableSsl = true };
        }

        public static SmtpClient OutlookSmtp()
        {
            return new SmtpClient("smtp-mail.outlook.com", 587) { EnableSsl = true };
        }

        public static SmtpClient Office365Smtp()
        {
            return new SmtpClient("smtp.office365.com", 587) { EnableSsl = true };
        }

        public FluentMailMessage WithSmtp(SmtpClient Client)
        {
            Smtp = Client;
            return this;
        }

        public FluentMailMessage WithSmtp(string Host)
        {
            return WithSmtp(new SmtpClient(Host));
        }

        public FluentMailMessage WithSmtp(string Host, int Port)
        {
            return WithSmtp(new SmtpClient(Host, Port));
        }

        public FluentMailMessage WithSmtp(string Host, int Port, bool UseSSL)
        {
            return WithSmtp(new SmtpClient(Host, Port) { EnableSsl = UseSSL });
        }

        public FluentMailMessage UseGmailSmtp()
        {
            Smtp = GmailSmtp();
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

        public FluentMailMessage UseTemplate(string TemplateOrFilePath)
        {
            if (TemplateOrFilePath.IsFilePath())
            {
                if (File.Exists(TemplateOrFilePath)) TemplateOrFilePath = File.ReadAllText(TemplateOrFilePath);
            }

            return WithMessage(TemplateOrFilePath, true);
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

        public FluentMailMessage WithCredentials(string Login, string Password)
        {
            return WithCredentials(new NetworkCredential(Login, Password));
        }

        public FluentMailMessage OnSuccess(Action<MailAddress> Action)
        {
            SuccessAction = Action;
            return this;
        }

        public FluentMailMessage OnError(Action<MailAddress, Exception> Action)
        {
            ErrorAction = Action;
            return this;
        }

        public FluentMailMessage Send()
        {
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
                                SuccessAction.Invoke(item);
                            }
                        }
                        catch (SmtpException ex)
                        {
                            if (ErrorAction != null)
                            {
                                ErrorAction.Invoke(item, ex);
                            }
                        }
                        catch (Exception ex2)
                        {
                            throw ex2;
                        }
                    }
                }
            }
            else
            {
                throw new Exception("Smtp is null");
            }

            return this;
        }

        public override string ToString()
        {
            return Body;
        }
    }
}