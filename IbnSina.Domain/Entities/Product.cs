
namespace IbnSina.Domain.Entities
{
    public class Product
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int StockQuantity { get; private set; }
        public decimal Price { get; private set; }
        public int CategoryId { get; private set; }
        public Category Category { get; private set; }
        public bool IsInStock => StockQuantity > 0;
        public DateTime? CreatedAt { get; private set; }
        private Product() { }
        public Product(string name, string description, int quantity, decimal price, int categoryId)
        {
            SetName(name);
            Description = description;
            SetStockQuantity(quantity);
            SetPrice(price);
            SetCategoryId(categoryId);
            CreatedAt = DateTime.UtcNow;
        }
        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name cannot be empty.");
            Name = name;
        }
        private void SetStockQuantity(int stockQuantity)
        {
            if (stockQuantity < 0)
                throw new ArgumentException("Stock quantity cannot be negative.");
            StockQuantity = stockQuantity;
        }
        private void SetPrice(decimal price)
        {
            if (price < 0)
                throw new ArgumentException("Price cannot be negative.");
            Price = price;
        }
        private void SetCategoryId(int categoryId)
        {
            if (categoryId <= 0)
                throw new ArgumentException("Category ID must be a positive integer.");
            CategoryId = categoryId;
        }
        public void IncreaseStock(int amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount to increase must be positive.");
            StockQuantity += amount;
        }
        public void DecreaseStock(int amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount to decrease must be positive.");
            if (StockQuantity - amount < 0)
                throw new InvalidOperationException("Cannot decrease stock below zero.");
            StockQuantity -= amount;
        }
        public void UpdateDetails(string name, string? description, decimal price, int categoryId)
        {
            SetName(name);
            Description = description;
            SetPrice(price);
            SetCategoryId(categoryId);
        }
        public void PatchDetails(string? name, string? description, decimal? price, int? categoryId)
        {
            if (name != null) SetName(name);
            if (description != null) Description = description;
            if (price.HasValue) SetPrice(price.Value);
            if (categoryId.HasValue) SetCategoryId(categoryId.Value);
        }

    }
}
