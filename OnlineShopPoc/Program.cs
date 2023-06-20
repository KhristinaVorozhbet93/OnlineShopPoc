using OnlineShopPoc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ICatalog,InMemoryCatalog>();
builder.Services.AddScoped<IEmailSender, MailKitSmtpEmailSender>();
builder.Services.AddHostedService<AppStartedNotificatorBackgroundServer>();
builder.Services.AddHostedService<SalesNotificatorBackgroundService>();

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
app.MapPost("/add_product", AddProduct);
app.MapGet("/get_product", GetProductById);
app.MapPost("/delete_product", DeleteProduct);
app.MapPost("/update_product", UpdateProduct);
app.MapPost("/clear_products", ClearCatalog);

//REST
app.MapGet("/products", GetProducts);
app.MapGet("/productById", GetProductById);
app.MapPost("/products/product", AddProduct);
app.MapDelete("/products/{productId}", DeleteProduct);
app.MapPut("/products/{productId}", UpdateProduct);

List<Product> GetProducts(ICatalog catalog)
{
    return catalog.GetProducts();
}
IResult AddProduct(Product product, ICatalog catalog)
{
    if (product is null) throw new ArgumentException(nameof(product));
    catalog.AddProduct(product);
    return Results.Created($"/products/{product.Id}", product);
}
Product GetProductById(Guid id, ICatalog catalog)
{
    return catalog.GetProduct(id);
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

app.Run();

