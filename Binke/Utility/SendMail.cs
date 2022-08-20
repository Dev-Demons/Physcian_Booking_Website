using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Binke.Utility
{
    public class SendMail
    {
        public static async Task SendEmailAsync(string to, string cc, string bcc, string replyTo, string stHtmlBody, string subject, string stAttachmentFile = null)
        {
            try
            {
                var uname = RequestHelpers.GetConfigValue("SMTP_USERNAME");
                var email = RequestHelpers.GetConfigValue("SMTP_SENDER_EMAIL");
                var password = RequestHelpers.GetConfigValue("SMTP_PASSWORD");
                var host = RequestHelpers.GetConfigValue("SMTP_HOST");
                var port = Convert.ToInt32(RequestHelpers.GetConfigValue("SMTP_PORT"));
                var enableSsl = Convert.ToBoolean(RequestHelpers.GetConfigValue("SMTP_ENABLESSL"));
                var companyName = RequestHelpers.GetConfigValue("CompanyName");
                var companySite = RequestHelpers.GetConfigValue("CompanySite");

                //bcc = RequestHelpers.GetConfigValue("CompanyEmailBCC");
                var message = new MailMessage { From = new MailAddress(email, companyName) };
                message.To.Add(new MailAddress(to));
                if (!string.IsNullOrEmpty(cc)) message.CC.Add(cc);
                if (!string.IsNullOrEmpty(bcc)) message.Bcc.Add(bcc);
                if (!string.IsNullOrEmpty(replyTo))
                {
                    message.ReplyToList.Add(new MailAddress(replyTo));
                }

                message.IsBodyHtml = true;
                // Subject and multipart/alternative Body
                message.Subject = subject;
                message.Body = stHtmlBody;
                message.Priority = MailPriority.High;
                if (!string.IsNullOrEmpty(stAttachmentFile))
                {
                    if (File.Exists(stAttachmentFile))
                    {
                        Attachment oAttachmentPdf = new Attachment(stAttachmentFile);
                        message.Attachments.Add(oAttachmentPdf);
                    }
                }

                var loginInfo = new NetworkCredential(uname, password);
                // Init SmtpClient and send
                var smtpClient = new SmtpClient(host, port)
                {
                    //EnableSsl = enableSsl,
                    UseDefaultCredentials = false,
                    Credentials = loginInfo,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };
                //ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                await smtpClient.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, "SendEmailSendGrid");
            }

        }

        public static void SendEmail(string to, string cc, string bcc, string replyTo, string stHtmlBody, string subject, string stAttachmentFile = null)
        {
            try
            {
                var uname = RequestHelpers.GetConfigValue("SMTP_USERNAME");
                var email = RequestHelpers.GetConfigValue("SMTP_SENDER_EMAIL");
                var password = RequestHelpers.GetConfigValue("SMTP_PASSWORD");
                var host = RequestHelpers.GetConfigValue("SMTP_HOST");
                var port = Convert.ToInt32(RequestHelpers.GetConfigValue("SMTP_PORT"));
                var enableSsl = Convert.ToBoolean(RequestHelpers.GetConfigValue("SMTP_ENABLESSL"));
                var companyName = RequestHelpers.GetConfigValue("CompanyName");
                var companySite = RequestHelpers.GetConfigValue("CompanySite");

                //bcc = RequestHelpers.GetConfigValue("CompanyEmailBCC");
                var message = new MailMessage { From = new MailAddress(email, companyName) };
                message.To.Add(new MailAddress(to));
                if (!string.IsNullOrEmpty(cc)) message.CC.Add(cc);
                if (!string.IsNullOrEmpty(bcc)) message.Bcc.Add(bcc);
                if (!string.IsNullOrEmpty(replyTo))
                {
                    message.ReplyToList.Add(new MailAddress(replyTo));
                }

                message.IsBodyHtml = true;
                // Subject and multipart/alternative Body
                message.Subject = subject;
                message.Body = stHtmlBody;
                message.Priority = MailPriority.High;
                if (!string.IsNullOrEmpty(stAttachmentFile))
                {
                    if (File.Exists(stAttachmentFile))
                    {
                        Attachment oAttachmentPdf = new Attachment(stAttachmentFile);
                        message.Attachments.Add(oAttachmentPdf);
                    }
                }

                var loginInfo = new NetworkCredential(uname, password);
                // Init SmtpClient and send
                var smtpClient = new SmtpClient(host, port)
                {
                    //EnableSsl = enableSsl,
                    UseDefaultCredentials = false,
                    Credentials = loginInfo,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };
                //ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
             smtpClient.Send(message);
            }
            catch (Exception ex)

            {
                Common.LogError(ex, "SendEmailSendGrid");
            }
        }
    }
}
