namespace OnlineShopPoc
{
    public class AppStartedNotificatorBackgroundServer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public AppStartedNotificatorBackgroundServer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
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
