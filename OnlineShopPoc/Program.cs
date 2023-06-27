using OnlineShopPoc;
using Serilog;

Log.Logger = new LoggerConfiguration()
   .WriteTo.Console()
   .CreateBootstrapLogger();
Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((_, conf) =>
    {
        conf
            .WriteTo.Console()
            .WriteTo.File("log-.txt",
     rollingInterval: RollingInterval.Day)
        ;
    });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddOptions<SmtpConfig>()
        .BindConfiguration("SmtpConfig")
       .ValidateDataAnnotations()
       .ValidateOnStart();
    builder.Configuration.GetSection("SmtpConfig").Get<SmtpConfig>();
    builder.Services.AddSingleton<ICatalog, InMemoryCatalog>();
    builder.Services.AddSingleton<IClock, Clock>();
    builder.Services.AddScoped<IEmailSender, MailKitSmtpEmailSender>();
    //builder.Services.Decorate<IEmailSender, EmailSenderLoggingDecorator>();
    builder.Services.AddHostedService<AppStartedNotificatorBackgroundServer>();
    //builder.Services.AddHostedService<SalesNotificatorBackgroundService>();
    builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(
       options =>
       {
           options.SerializerOptions.WriteIndented = true;
       });

    var app = builder.Build();
    app.UseSwagger();
    app.UseSwaggerUI();

    //RPC
    app.MapGet("/get_products", GetProducts);
    app.MapGet("/get_product", GetProductById);
    app.MapPost("/add_product", AddProduct);
    app.MapPost("/delete_product", DeleteProduct);
    app.MapPost("/update_product", UpdateProduct);
    app.MapPost("/clear_products", ClearCatalog);

    app.MapGet("/get_date", GetDateUtc);
    app.MapPost("/send_email", SendEmail);

    //REST
    app.MapGet("/products", GetProducts);
    app.MapGet("/productById", GetProductById);
    app.MapPost("/products/product", AddProduct);
    app.MapDelete("/products/{productId}", DeleteProduct);
    app.MapPut("/products/{productId}", UpdateProduct);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Сервер рухнул!");
}
finally 
{
    Log.Information("Shut down complete");
    await Log.CloseAndFlushAsync();
}
async Task SendEmail(IEmailSender emailSender, string email, string subject, string message)
{
    ArgumentNullException.ThrowIfNull(email);
    ArgumentNullException.ThrowIfNull(subject);
    ArgumentNullException.ThrowIfNull(message);
    await emailSender.SendEmailAsync(email, subject, message);
}

List<Product> GetProducts(ICatalog catalog, IClock clock)
{
    return catalog.GetProducts(clock);
}
IResult AddProduct(Product product, ICatalog catalog)
{
    if (product is null) throw new ArgumentException(nameof(product));
    catalog.AddProduct(product);
    return Results.Created($"/products/{product.Id}", product);
}
Product GetProductById(Guid id, ICatalog catalog, IClock clock)
{
    return catalog.GetProduct(id, clock);
}
void DeleteProduct(Guid productId, ICatalog catalog)
{
    catalog.DeleteProduct(productId);
}
void UpdateProduct(Guid productId, Product newProduct, ICatalog catalog)
{
    if (newProduct is null) throw new ArgumentException(nameof(newProduct));
    catalog.UpdateProduct(productId, newProduct);
}
void ClearCatalog(ICatalog catalog)
{
    catalog.ClearCatalog();
}

DateTime GetDateUtc(IClock clock)
{
    return clock.GetTimeUtc(); 
}


