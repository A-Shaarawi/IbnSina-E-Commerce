using IbnSina.Application.Interfaces;
using IbnSina.Domain.Entities;
using IbnSina.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IbnSina.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllAsync(string? search, int? categoryId, bool activeOnly)
    {
        var query = _context.Products.Include(p => p.Category).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p =>
                EF.Functions.Like(p.Name, $"%{search}%") ||
                (p.Description != null && EF.Functions.Like(p.Description, $"%{search}%")));
        }
        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }
        if (activeOnly)
        {
            query = query.Where(p => p.StockQuantity > 0);
        }
        return await query.ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Product product)
    {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }
}