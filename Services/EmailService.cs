using MimeKit;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace StudioMgn.Services
{
    public class EmailService : IEmailSender
    {
        public EmailService(ILogger<EmailService> logger)
        {
            Logger = logger;
        }

        public ILogger<EmailService> Logger { get; }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Моя студия", "studio-mgn@yandex.ru"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Cc.Add(new MailboxAddress("", "jeisonj2019@gmail.com"));
            emailMessage.Subject = subject;
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = message;
            emailMessage.Body = bodyBuilder.ToMessageBody();


            using (var client = new SmtpClient())
            {
                client.CheckCertificateRevocation = false;
                try
                {
                    await client.ConnectAsync("smtp.yandex.ru", 587, false);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync("studio-mgn@yandex.ru", "tpfadlluhgchbwlr");
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                    Logger.LogInformation($"=== электронное сообщение успешно отправлено на адрес {email}");

                }
                catch (System.Exception)
                {

                    throw;
                }
            }

        }
    }
}
