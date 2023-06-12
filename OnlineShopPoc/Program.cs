using OnlineShopPoc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(
   options =>
   {
       options.SerializerOptions.WriteIndented = true;
   });
var app = builder.Build();
Catalog catalog = new Catalog();

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

List<Product> GetProducts()
{
    return catalog.GetProducts();
}
IResult AddProduct(Product product)
{
    if (product is null) throw new ArgumentException(nameof(product));
    catalog.AddProduct(product);
    return Results.Created($"/products/{product.Id}", product); 
}
Product GetProductById(Guid id)
{
    return catalog.GetProduct(id);
}
void DeleteProduct(Guid productId)
{
    catalog.DeleteProduct(productId);
}
void UpdateProduct(Guid productId, Product newProduct)
{
    if (newProduct is null) throw new ArgumentException(nameof(newProduct));
    catalog.UpdateProduct(productId, newProduct);
}
void ClearCatalog()
{
    catalog.ClearCatalog();
}

app.Run();

