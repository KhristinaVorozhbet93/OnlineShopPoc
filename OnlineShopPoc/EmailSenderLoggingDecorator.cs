namespace OnlineShopPoc
{
    public class EmailSenderLoggingDecorator: IEmailSender
    {
        private readonly IEmailSender _emailSender;
        ILogger<EmailSenderLoggingDecorator> _logger;
        public EmailSenderLoggingDecorator(IEmailSender emailSender,
            ILogger<EmailSenderLoggingDecorator> logger)
        {
            _emailSender = emailSender ?? throw new  ArgumentNullException(nameof(emailSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            _logger.LogInformation($"Sending to {email} Subject: {subject}, Message: {message}");
            await _emailSender.SendEmailAsync(email, subject, message);
        }
    }
}
