namespace IbnSina.Domain.Entities;

public class OrderItem
{
    public int Id { get; private set; }
    public int OrderId { get; private set; }
    public int ProductId { get; private set; }
    public string ProductName { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    public decimal Subtotal => Quantity * UnitPrice;

    private OrderItem() { }

    public OrderItem(int productId, string productName, int quantity, decimal unitPrice)
    {
        SetProductId(productId);
        SetProductName(productName);
        SetQuantity(quantity);
        SetUnitPrice(unitPrice);
    }

    private void SetProductId(int productId)
    {
        if (productId <= 0)
            throw new ArgumentException("Product ID must be a positive integer.");
        ProductId = productId;
    }

    private void SetProductName(string productName)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name cannot be empty.");
        ProductName = productName;
    }

    private void SetQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");
        Quantity = quantity;
    }

    private void SetUnitPrice(decimal unitPrice)
    {
        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative.");
        UnitPrice = unitPrice;
    }
}