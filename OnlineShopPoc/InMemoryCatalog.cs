namespace OnlineShopPoc
{
    public class InMemoryCatalog : ICatalog
    {
        private object _productsSyncObj = new();

        private List<Product> _products;

        public InMemoryCatalog()
        {
            _products = GenerateProducts(10);
        }

        public List<Product> GetProducts(IClock clock)
        {
            ArgumentNullException.ThrowIfNull(clock);

            if (clock.GetTimeUtc().DayOfWeek == DayOfWeek.Monday)
            {
                lock (_productsSyncObj)
                {
                    List<Product> salesProducts = new();

                    for (int i = 0; i < _products.Count; i++)
                    {
                        salesProducts.Add(new Product(_products[i].Name,
                            Math.Floor(_products[i].Price - (_products[i].Price / 100 * 30)))
                        {
                            Id = _products[i].Id,
                            Description = _products[i].Description,
                            ExpiredAt = _products[i].ExpiredAt,
                            ProducedAt = _products[i].ProducedAt
                        });
                    }
                    return salesProducts;
                }
            }
            else
            {
                lock (_productsSyncObj)
                {
                    return _products;
                }
            }
            throw new ArgumentNullException(nameof(_products));
        }

        public void AddProduct(Product product)
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(product));
            lock (_productsSyncObj)
            {
                _products.Add(product);
            }
        }
        public Product GetProduct(Guid id, IClock clock)
        {
            ArgumentNullException.ThrowIfNull(clock);
            for (int i = 0; i < _products.Count; i++)
            {
                if (_products[i].Id == id)
                {
                    if (clock.GetTimeUtc().DayOfWeek == DayOfWeek.Monday)
                    {
                        lock (_productsSyncObj)
                        {
                            Product saleProduct = new Product(_products[i].Name,
                                Math.Floor(_products[i].Price - (_products[i].Price / 100 * 30)))
                            {
                                Id = _products[i].Id,
                                Description = _products[i].Description,
                                ExpiredAt = _products[i].ExpiredAt,
                                ProducedAt = _products[i].ProducedAt
                            };
                            return saleProduct;
                        }
                    }
                    else
                    {
                        return _products[i];
                    }
                }
            }
            throw new ArgumentException($"Продукта с ID={id} не существует!");
        }


        public void DeleteProduct(Guid productId)
        {
            foreach (var product in _products)
            {
                if (product.Id == productId)
                {
                    lock (_productsSyncObj)
                    {
                        _products.Remove(product);
                    }
                }
            }
            throw new ArgumentException($"Продукта с ID={productId} не существует!");
        }

        public void UpdateProduct(Guid productId, Product newProduct)
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(newProduct));
            foreach (var product in _products)
            {
                if (product.Id == productId)
                {
                    lock (_productsSyncObj)
                    {
                        product.Name = newProduct.Name;
                        product.Price = newProduct.Price;
                        product.ExpiredAt = newProduct.ExpiredAt;
                        product.ProducedAt = newProduct.ProducedAt;
                        product.Description = newProduct.Description;
                    }
                }
                else
                {
                    throw new ArgumentException($"Продукта с ID={productId} не существует!");
                }
            }
        }

        public void ClearCatalog()
        {
            lock (_productsSyncObj)
            {
                _products.Clear();
            }
        }
        private static List<Product> GenerateProducts(int count)
        {
            var random = new Random();
            var products = new Product[count];

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
