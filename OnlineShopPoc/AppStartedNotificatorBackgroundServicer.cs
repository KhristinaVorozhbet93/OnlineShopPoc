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
            int attemptLimit = 2;
            bool isSend = false;

            while (!stoppingToken.IsCancellationRequested)
            {
                await using (var scope = _serviceProvider.CreateAsyncScope())
                {
                    var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
                    for (int i = 0; i < attemptLimit && isSend == false; i++)
                    {
                        try
                        {
                            await emailSender.SendEmailAsync
                                ("ptykhina.khristi@mail.ru", "Приложение запущено", "Приложение запущено!");
                            isSend = true;
                        }
                        catch (Exception e)
                        {
                            if (i == 0)
                            {
                                _logger.LogWarning(e, "Сообщение не отправлено!");
                            }
                            else
                            {
                                _logger.LogError(e, "Сообщение не отправлено повторно!");
                            }
                        }
                    }
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
            }
        }
    }
}

