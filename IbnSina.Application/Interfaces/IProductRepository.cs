using IbnSina.Domain.Entities;

namespace IbnSina.Application.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(string? search, int? categoryId, bool activeOnly);
    Task<Product?> GetByIdAsync(int id);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Product product);
}