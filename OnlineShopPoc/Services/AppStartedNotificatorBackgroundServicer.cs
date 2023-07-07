using System.Diagnostics;
using OnlineShopPoc.Interfaces;

namespace OnlineShopPoc.Services
{
    public class AppStartedNotificatorBackgroundServer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AppStartedNotificatorBackgroundServer> _logger;

        public AppStartedNotificatorBackgroundServer(IServiceProvider serviceProvider,
            ILogger<AppStartedNotificatorBackgroundServer> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _logger.LogInformation("Сервер запущен!");
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await using (var scope = _serviceProvider.CreateAsyncScope())
                {
                    var email = "ptykhina.khristi@mail.ru";
                    var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

                    await emailSender.SendEmailAsync(email, "Промоакции", "Список акций");
                    _logger.LogInformation("Email sent to {email} in {ElapsedMilliseconds} ms",
                        email);
                }
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}

