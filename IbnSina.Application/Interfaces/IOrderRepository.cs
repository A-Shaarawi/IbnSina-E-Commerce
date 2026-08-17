using IbnSina.Domain.Entities;

using IbnSina.Domain.Entities;

namespace IbnSina.Application.Interfaces;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync(int userId);
    Task<Order?> GetByIdAsync(int id, int userId);
    Task AddAsync(Order order);
}