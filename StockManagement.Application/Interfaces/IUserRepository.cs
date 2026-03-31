using StockManagement.Domain.Entities;

namespace StockManagement.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
    Task AddAsync(User user);
    Task<bool> ExistsByEmailAsync(string email);
}

