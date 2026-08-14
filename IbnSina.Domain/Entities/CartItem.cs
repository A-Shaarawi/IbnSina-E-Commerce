

namespace IbnSina.Domain.Entities
{
    public class CartItem
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public int ProductId { get; private set; }
        public Product Product { get; private set; }
        public int Quantity { get; private set; }
        public DateTime? AddedAt { get; private set; }
        private CartItem() { }
        public CartItem(int userId, int productId, int quantity)
        {
            SetUserId(userId);
            SetProductId(productId);
            SetQuantity(quantity);
            AddedAt = DateTime.UtcNow;
        }
        private void SetUserId(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("User ID must be a positive integer.");
            UserId = userId;
        }
        private void SetProductId(int productId)
        {
            if (productId <= 0)
                throw new ArgumentException("Product ID must be a positive integer.");
            ProductId = productId;
        }

        private void SetQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");
            Quantity = quantity;
        }
        public void IncreaseQuantity(int amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount to increase must be positive.");
            Quantity += amount;
        }
        public void UpdateQuantity(int quantity)
        {
            SetQuantity(quantity);
        }
    }
}
