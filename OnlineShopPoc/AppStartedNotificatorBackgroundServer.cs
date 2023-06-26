namespace OnlineShopPoc
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
                    var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

                    await emailSender.SendEmailAsync
                        ("ptykhina.khristi@mail.ru", "Приложение запущено", "Приложение запущено!");
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
            }
        }
    }
}

