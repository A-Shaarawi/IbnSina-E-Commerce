using IbnSina.Application.Interfaces;
using IbnSina.Domain.Entities;
using IbnSina.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IbnSina.Infrastructure.Repositories
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly AppDbContext _context;

        public CartItemRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<CartItem>> GetAllAsync(int userId)
        {
            return await _context.CartItems.Where(c => c.UserId == userId).Include(c => c.Product).ToListAsync();
        }
        public async Task<CartItem?> GetByIdAsync(int id, int userId)
        {
            return await _context.CartItems
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
        }
        public async Task AddAsync(CartItem cartItem)
        {
            await _context.CartItems.AddAsync(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CartItem cartItem)
        {
            _context.CartItems.Update(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task ClearAsync(int userId)
        {
            var items = await _context.CartItems.Where(c => c.UserId == userId).ToListAsync();
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
    }
}
