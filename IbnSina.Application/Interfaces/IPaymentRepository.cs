using IbnSina.Domain.Entities;

namespace IbnSina.Application.Interfaces;

public interface IPaymentRepository
{
    Task<Payment?> GetByOrderIdAsync(int orderId);
    Task<Payment?> GetByStripePaymentIntentIdAsync(string paymentIntentId);
    Task AddAsync(Payment payment);
    Task UpdateAsync(Payment payment);
}