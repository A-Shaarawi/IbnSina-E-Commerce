namespace IbnSina.Domain.Entities;

public enum OrderStatus
{
    Pending,
    Completed,
    Cancelled
}

public class Order
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public OrderStatus Status { get; private set; }

    private readonly List<OrderItem> _orderItems = new();
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    public decimal TotalAmount => _orderItems.Sum(oi => oi.Subtotal);

    private Order() { }

    public Order(int userId)
    {
        SetUserId(userId);
        OrderDate = DateTime.UtcNow;
        Status = OrderStatus.Pending;
    }

    private void SetUserId(int userId)
    {
        if (userId <= 0)
            throw new ArgumentException("User ID must be a positive integer.");
        UserId = userId;
    }

    public void AddItem(OrderItem item)
    {
        _orderItems.Add(item);
    }

    public void Complete()
    {
        Status = OrderStatus.Completed;
    }

    public void Cancel()
    {
        Status = OrderStatus.Cancelled;
    }
}