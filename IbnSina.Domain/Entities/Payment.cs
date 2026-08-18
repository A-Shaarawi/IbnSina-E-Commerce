namespace IbnSina.Domain.Entities;

public enum PaymentStatus
{
    Pending,
    Succeeded,
    Failed
}

public class Payment
{
    public int Id { get; private set; }
    public int OrderId { get; private set; }
    public string StripePaymentIntentId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Payment() { }

    public Payment(int orderId, string stripePaymentIntentId, decimal amount)
    {
        OrderId = orderId;
        StripePaymentIntentId = stripePaymentIntentId;
        Amount = amount;
        Status = PaymentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkSucceeded() => Status = PaymentStatus.Succeeded;
    public void MarkFailed() => Status = PaymentStatus.Failed;
}