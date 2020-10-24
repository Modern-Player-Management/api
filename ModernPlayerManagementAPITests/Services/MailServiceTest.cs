using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
using Moq;
using Xunit;
using IMailService = ModernPlayerManagementAPI.Services.Interfaces.IMailService;
using MailService = ModernPlayerManagementAPI.Services.MailService;

namespace ModernPlayerManagementAPITests
{
    public class MailServiceTest
    {
        private List<MimeMessage> mailsSent = new List<MimeMessage>();
        private IMailService mailService;

        public MailServiceTest()
        {
            var mock = new Mock<ISmtpClient>();

            mock.Setup(mock =>
                    mock.Send(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), It.IsAny<ITransferProgress>()))
                .Callback<MimeMessage,CancellationToken,ITransferProgress>(
                    (m, c, t) => {mailsSent.Add(m);}
                );

            this.mailService = new MailService(mock.Object);
        }

        [Fact(Skip = "Removed Mail")]
        private void SendMailTest()
        {
            // Given
            var user = "Test";
            var email = "test@test.fr";
            var subject = "Test Subject";
            var body = "test Body";
            
            // When
            this.mailService.SendMail(user,email, subject,body);
            
            // Then
            Assert.Single(this.mailsSent);
            Assert.Equal("Test",this.mailsSent.First().To.First().Name);
        }
    }
}