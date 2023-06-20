using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System.Net;

namespace OnlineShopPoc
{
    public class MailKitSmtpEmailSender : IEmailSender, IAsyncDisposable
    {
        private readonly SmtpClient _smtpClient = new();
        private readonly string _emailPassword = Environment.GetEnvironmentVariable("emailPassword") ??
            throw new ArgumentNullException(nameof(_emailPassword));

        public async ValueTask DisposeAsync()
        {
            await _smtpClient.DisconnectAsync(true);
            _smtpClient.Dispose(); 
        }

        private async Task EnsureConnectAndAuthenticateAsync()
        {
            if (!_smtpClient.IsConnected)
            {
                await _smtpClient.ConnectAsync("smtp.beget.com", 25);
            }
            if (!_smtpClient.IsAuthenticated)
            {
                await _smtpClient.AuthenticateAsync("asp2023pv112@rodion-m.ru", _emailPassword);
            }
        }
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

            await EnsureConnectAndAuthenticateAsync();
            await _smtpClient.SendAsync(emailMessage);
        }
    }
}
