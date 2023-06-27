using System.Diagnostics;

namespace OnlineShopPoc
{
    public class SalesNotificatorBackgroundService : BackgroundService
    {
        private readonly ILogger<SalesNotificatorBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly int _attemptsLimit;

        public SalesNotificatorBackgroundService(
            ILogger<SalesNotificatorBackgroundService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            ArgumentException.ThrowIfNullOrEmpty(nameof(configuration));
            _attemptsLimit = configuration.GetValue<int>("SalesEmailAttemptCount");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var localServiceProvider = scope.ServiceProvider;
            var emailSender = localServiceProvider.GetRequiredService<IEmailSender>();
            var users = new User[]
            {
                new ("ptykhina.khristi@mail.ru")
            };

            var sw = Stopwatch.StartNew();
            foreach (var user in users)
                await SendEmailWithRetries(emailSender, sw, user);

            async Task SendEmailWithRetries(IEmailSender emailSender, Stopwatch sw, User user)
            {
                for (int attempt = 1; attempt <= _attemptsLimit; attempt++)
                {
                    try
                    {
                        sw.Restart();
                        await emailSender.SendEmailAsync(user.email, "Промоакции", "Список акций");
                        _logger.LogInformation("Email sent to {email} in {ElapsedMilliseconds} ms",
                            user.email, sw.ElapsedMilliseconds);
                        break;
                    }
                    catch (Exception e) when (attempt < _attemptsLimit)
                    {
                        _logger.LogError(e, "Ошибка отправки сообщения: {Email}",
                        user.email);
                    }
                    catch (Exception e)
                    {
                        _logger.LogWarning(e, "Повторная отправка сообщения: {Email}",
                        user.email);
                    }
                }
            }
        }
    }
    public record User(string email);
}
