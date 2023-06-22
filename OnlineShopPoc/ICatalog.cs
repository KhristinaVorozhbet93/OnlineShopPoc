namespace OnlineShopPoc
{
    public interface ICatalog
    {
        List<Product> GetProducts(IClock clock);
        void AddProduct(Product product);
        Product GetProduct(Guid id, IClock clock);
        void DeleteProduct(Guid productId);
        void UpdateProduct(Guid productId, Product newProduct);
        void ClearCatalog();
    }
}