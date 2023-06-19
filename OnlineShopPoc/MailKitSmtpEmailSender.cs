using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System.Net;

namespace OnlineShopPoc
{
    public class MailKitSmtpEmailSender : IEmailSender
    {
        private readonly string _emailPassword = Environment.GetEnvironmentVariable("emailPassword") ??
            throw new ArgumentNullException(nameof(_emailPassword)); 
        public async Task SendEmailAsync(string recepientEmail, string subject, string message)
        {
            ArgumentNullException.ThrowIfNull(recepientEmail);
            ArgumentNullException.ThrowIfNull(subject);
            ArgumentNullException.ThrowIfNull(message);


            var emailMessage = new MimeMessage
            {
                Subject = subject,
                Body = new TextPart(TextFormat.Plain)
                {
                    Text = message,
                },
                From = { MailboxAddress.Parse("asp2023pv112@rodion-m.ru") },
                To = { MailboxAddress.Parse(recepientEmail) },
            };

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.beget.com", 25);
                await client.AuthenticateAsync("asp2023pv112@rodion-m.ru", _emailPassword);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}
