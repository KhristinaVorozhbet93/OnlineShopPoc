namespace OnlineShopPoc.Models
{
    public class Product
    {
        public Product(string name, decimal price)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException(nameof(name));
            if (string.IsNullOrWhiteSpace(name)) throw new NullReferenceException(nameof(name));

            Name = name;
            Price = price;
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime ProducedAt { get; set; }
        public DateTime ExpiredAt { get; set; }
    }
}
