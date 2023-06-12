namespace OnlineShopPoc
{
    public class Catalog
    {
        private List<Product> _products;
        public Catalog()
        {
            _products = GenerateProducts(10); 
        }

        public List<Product> GetProducts()
        {
            return _products;
        }

        public void AddProduct(Product product)
        {
            if (product is null) throw new ArgumentException(nameof(product));
            _products.Add(product);
        }
        public Product GetProduct(Guid id)
        {
            foreach (var product in _products)
            {
                if (product.Id == id)
                {
                    return product; 
                }
            }
            throw new ArgumentException($"Продукта с ID={id} не существует!") ;    
        }
        public bool DeleteProduct(Guid productId)
        {
            foreach (var product in _products)
            {
                if (product.Id == productId)
                {
                    return _products.Remove(product);
                }
            }
            return false; 
        }

        public void UpdateProduct(Guid productId, Product newProduct)
        {
            if (newProduct is null) throw new ArgumentException(nameof(newProduct));
            foreach (var product in _products)
            {
                if (product.Id == productId)
                {
                    product.Name = newProduct.Name;
                    product.Price = newProduct.Price;
                    product.ExpiredAt = newProduct.ExpiredAt;
                    product.ProducedAt = newProduct.ProducedAt;
                    product.Description = newProduct.Description;
                }
                else
                {
                    throw new ArgumentException($"Продукта с ID={productId} не существует!");
                }
            }
        }

        public void ClearCatalog()
        {
            _products.Clear();
        }
        private static List<Product> GenerateProducts(int count)
        {
            var random = new Random();
            var products = new Product[count];

            // Массив реальных названий товаров
            var productNames = new string[]
            {
            "Молоко",
            "Хлеб",
            "Яблоки",
            "Макароны",
            "Сахар",
            "Кофе",
            "Чай",
            "Рис",
            "Масло подсолнечное",
            "Сыр"
            };



            for (int i = 0; i < count; i++)
            {
                var name = productNames[i];
                var price = random.Next(50, 500);
                var producedAt = DateTime.Now.AddDays(-random.Next(1, 30));
                var expiredAt = producedAt.AddDays(random.Next(1, 365));


                products[i] = new Product(name, price)
                {
                    Id = Guid.NewGuid(), 
                    Description = "Описание " + name,
                    ProducedAt = producedAt,
                    ExpiredAt = expiredAt
                };
            }



            return products.ToList();
        }
    }
}
