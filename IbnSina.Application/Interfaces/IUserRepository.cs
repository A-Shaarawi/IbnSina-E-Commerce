using IbnSina.Domain.Entities;

namespace IbnSina.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync(string? search);
        Task<User?> GetByIdAsync(int id);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
    }
}
