namespace OnlineShopPoc
{
    public class AppStartedNotificatorBackgroundServer : BackgroundService
    {
        //private readonly IEmailSender _emailSender;
        //public AppStartedNotificatorBackgroundServer(IEmailSender emailSender)
        //{
        //    _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
        //}
        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    //return _emailSender.SendEmailAsync
        //    //("ptykhina.khristi@mail.ru", "Приложение запущено", "Приложение запущено!");

        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        await _emailSender.SendEmailAsync
        //        ("ptykhina.khristi@mail.ru", "Приложение запущено", "Приложение запущено!");
        //        await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        //    }
        //}

        private readonly IServiceProvider _serviceProvider;
        public AppStartedNotificatorBackgroundServer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
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
