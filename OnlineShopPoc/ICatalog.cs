namespace OnlineShopPoc
{
    public interface ICatalog
    {
        List<Product> GetProducts();
        void AddProduct(Product product);
        Product GetProduct(Guid id);
        void DeleteProduct(Guid productId);
        void UpdateProduct(Guid productId, Product newProduct);
        void ClearCatalog();
    }
}