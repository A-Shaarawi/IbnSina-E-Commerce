using IbnSina.Domain.Entities;

namespace IbnSina.Application.Interfaces
{
    public interface ICartItemRepository
    {
        Task<List<CartItem>> GetAllAsync(int userId);
        Task<CartItem?> GetByIdAsync(int id, int userId);
        Task AddAsync(CartItem cartItem);
        Task UpdateAsync(CartItem cartItem);
        Task DeleteAsync(CartItem cartItem);
        Task ClearAsync(int userId);
    }
}
