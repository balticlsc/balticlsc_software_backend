using MailKit.Net.Smtp;
using MimeKit;

namespace Baltic.Mail
{
    public class MailClient
    {
        private const string UserName = "Patgres Bachamow Sender";
        private const string UserEmail = "patgres.bachamow@wp.pl";
        private const string Password = "<password>";

        private readonly SmtpClient _client;

        public MailClient()
        {
            _client = new SmtpClient {ServerCertificateValidationCallback = (s, c, h, e) => true};
            _client.Connect("smtp.wp.pl", 465);

            _client.Authenticate(UserEmail, Password);
        }

        public void SendMessage(MimeMessage message)
        {
            message.From.Add(new MailboxAddress(UserName, UserEmail));
            _client.Send(message);
        }

        public void Disconnect()
        {
            _client.Disconnect(true);
        }
    }
}