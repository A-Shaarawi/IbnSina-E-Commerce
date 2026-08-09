
namespace IbnSina.Domain.Entities
{
    public class Product
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int Quantity { get; private set; }
        public decimal Price { get; private set; }
        public int CategoryId { get; private set; }
        public Category Category { get; private set; }
        public bool IsInStock => Quantity > 0;
        private Product() { }
        public Product(string name, string description, int quantity, decimal price, int categoryId )
        {
            SetName(name);
            Description = description;
            SetQuantity(quantity);
            SetPrice(price);
            SetCategoryId(categoryId);
        }
        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name cannot be empty.");
            Name = name;
        }
        private void SetQuantity(int quantity)
        {
            if (quantity < 0)
                throw new ArgumentException("Quantity cannot be negative.");
            Quantity = quantity;
        }
        public void SetPrice(decimal price)
        {
            if (price < 0)
                throw new ArgumentException("Price cannot be negative.");
            Price = price;
        }
        public void SetDescription(string description)
        {
            Description = description;
        }
        public void SetCategoryId(int categoryId)
        {
            if (categoryId <= 0)
                throw new ArgumentException("Category ID must be a positive integer.");
            CategoryId = categoryId;
        }
        public void IncreaseStock(int amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount to increase must be positive.");
            Quantity += amount;
        }
        public void DecreaseStock(int amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount to decrease must be positive.");
            if (Quantity - amount < 0)
                throw new InvalidOperationException("Cannot decrease stock below zero.");
            Quantity -= amount;
        }
    }
}
