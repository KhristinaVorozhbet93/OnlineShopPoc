using System.Collections.Concurrent;

namespace OnlineShopPoc
{
    public class Catalog
    {
        private ConcurrentDictionary<string, Product> _products;
        public Catalog()
        {
            _products = GenerateProducts(10); 
        }

        public ConcurrentDictionary<string, Product> GetProducts()
        {
            return _products;
        }

        public Task AddProduct(string key, Product product)
        {
            if (product is null) throw new ArgumentException(nameof(product));
            if (!_products.TryAdd(key, product))
            {
                throw new ArgumentException(nameof(key));
            }
        }
        public Product GetProduct(Guid id)
        {
            foreach (var product in _products)
            {
                if (product.Value.Id == id)
                {
                    return product.Value;
                }
            }
            throw new ArgumentException(nameof(id));
        }
        public Task DeleteProduct(Guid productId)
        {
            foreach (var product in _products)
            {
                if (product.Value.Id == productId)
                {
                    _products.TryRemove(product);
                }
            }
            throw new ArgumentException(nameof(productId));
        }

        public Task UpdateProduct(Guid productId, Product newProduct)
        {
            if (newProduct is null) throw new ArgumentException(nameof(newProduct));
            foreach (var product in _products)
            {
                if (product.Value.Id == productId)
                {
                    product.Value.Name = newProduct.Name;
                    product.Value.Price = newProduct.Price;
                    product.Value.ExpiredAt = newProduct.ExpiredAt;
                    product.Value.ProducedAt = newProduct.ProducedAt;
                    product.Value.Description = newProduct.Description;
                }
            }
            throw new ArgumentException(nameof(productId));
        }

        public void ClearCatalog()
        {
            _products.Clear();
        }
        private static ConcurrentDictionary<string,Product> GenerateProducts(int count)
        {
            var random = new Random();
            var productDictionary = new ConcurrentDictionary<string, Product>();
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

            for (int i = 0; i < products.Length; i++)
            {
                productDictionary.TryAdd(Guid.NewGuid().ToString(), products[i]);
            }
            return productDictionary;
        }
    }
}
