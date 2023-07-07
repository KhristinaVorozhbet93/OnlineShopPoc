using OnlineShopPoc.Interfaces;
using Polly;
using Polly.Retry;

namespace OnlineShopPoc.Decorators
{
    public class EmailSenderRetryDecorator
    {
        private readonly IEmailSender _emailSenderImplementation;
        private readonly ILogger<EmailSenderRetryDecorator> _logger;
        private readonly int _attemptLimit;
        private readonly AsyncRetryPolicy? _policy;
        CancellationTokenSource tokenSource;

        public EmailSenderRetryDecorator(IEmailSender emailSenderImplementation,
            ILogger<EmailSenderRetryDecorator> logger,
            IConfiguration configuration)
        {
            _emailSenderImplementation = emailSenderImplementation
                ?? throw new ArgumentNullException(nameof(emailSenderImplementation));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));
            _attemptLimit = configuration.GetValue<int>("SalesEmailAttemptCount");
            tokenSource = new CancellationTokenSource();

            _policy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(retryCount: _attemptLimit,
                                    sleepDurationProvider: (retryCount) => TimeSpan.FromSeconds(retryCount * 2),
                                    onRetry: (exception, sleepDuration, attemptNumber) =>
                                    {
                                        _logger.LogWarning(exception,
                                    "Ошибка! Сообщение отправить повторно через {SleepDuration}.Попытка номер: {RetryCount}.", sleepDuration, attemptNumber);
                                    });

        }

        public async Task SendEmailAsync(string recepientEmail, string subject, string message)
        {
            var sendResult = await _policy!.ExecuteAndCaptureAsync(
                _ => _emailSenderImplementation.SendEmailAsync(recepientEmail, subject, message), tokenSource.Token);

            if (sendResult.Outcome == OutcomeType.Failure)
            {
                _logger.LogError(sendResult.FinalException, "Сообщение не отправлено!Ошибка!");
            }
        }
    }
}

