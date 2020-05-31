using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using ModernPlayerManagementAPI.Services.Interfaces;

namespace ModernPlayerManagementAPI.Services
{
    public class MailService : IMailService
    {
        private readonly IConfiguration configuration;
        private readonly ISmtpClient smtpClient;

        public MailService(IConfiguration configuration)
        {
            this.configuration = configuration;
            smtpClient = new SmtpClient();
            smtpClient.Connect("smtp.gmail.com", 587, false);
            smtpClient.Authenticate("modern.player.management@gmail.com", "mpm@2020");
        }

        public void SendMail(string username, string email, string subject, string body)
        {
            var mail = new MimeMessage();

            mail.To.Add(new MailboxAddress(username, email));
            mail.From.Add(new MailboxAddress("MPM", "modern.player.management@gmail.com"));
            mail.Subject = subject;
            mail.Body = new TextPart(TextFormat.Html) {Text = body};

            this.smtpClient.Send(mail);
        }

        ~MailService()
        {
            this.smtpClient.Disconnect(true);
        }
    }
}