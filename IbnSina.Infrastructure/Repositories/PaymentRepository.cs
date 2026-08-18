using IbnSina.Application.Interfaces;
using IbnSina.Domain.Entities;
using IbnSina.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IbnSina.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _context;

    public PaymentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Payment?> GetByOrderIdAsync(int orderId)
    {
        return await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
    }

    public async Task<Payment?> GetByStripePaymentIntentIdAsync(string paymentIntentId)
    {
        return await _context.Payments.FirstOrDefaultAsync(p => p.StripePaymentIntentId == paymentIntentId);
    }

    public async Task AddAsync(Payment payment)
    {
        await _context.Payments.AddAsync(payment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Payment payment)
    {
        _context.Payments.Update(payment);
        await _context.SaveChangesAsync();
    }
}