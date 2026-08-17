using IbnSina.Application.Interfaces;
using IbnSina.Domain.Entities;
using IbnSina.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IbnSina.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Order>> GetAllAsync(int userId)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.UserId == userId)
            .ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(int id, int userId)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
    }

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
    }
}