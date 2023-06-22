using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.Net;

namespace OnlineShopPoc
{
    public class MailKitSmtpEmailSender : IEmailSender, IAsyncDisposable
    {
        private readonly SmtpClient _smtpClient = new();
        private readonly SmtpConfig _smtpConfig;

        public MailKitSmtpEmailSender(IOptions<SmtpConfig> options)
        {
            ArgumentNullException.ThrowIfNull(options); 
           _smtpConfig= options.Value;
        }
        public async ValueTask DisposeAsync()
        {
            await _smtpClient.DisconnectAsync(true);
            _smtpClient.Dispose(); 
        }

        private async Task EnsureConnectAndAuthenticateAsync()
        {
            if (!_smtpClient.IsConnected)
            {
                await _smtpClient.ConnectAsync(_smtpConfig.Host, _smtpConfig.Port);
            }
            if (!_smtpClient.IsAuthenticated)
            {
                await _smtpClient.AuthenticateAsync(_smtpConfig.UserName, _smtpConfig.Password);
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
                From = { MailboxAddress.Parse(_smtpConfig.UserName) },
                To = { MailboxAddress.Parse(recepientEmail) },
            };

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            await EnsureConnectAndAuthenticateAsync();
            await _smtpClient.SendAsync(emailMessage);
        }
    }
}
