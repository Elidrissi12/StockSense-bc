using Microsoft.EntityFrameworkCore;
using StockManagement.Application.Interfaces;
using StockManagement.Domain.Entities;
using StockManagement.Infrastructure.DbContext;

namespace StockManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly StockDbContext _db;

    public UserRepository(StockDbContext db)
    {
        _db = db;
    }

    public Task<bool> ExistsByEmailAsync(string email) =>
        _db.Users.AnyAsync(u => u.Email == email);

    public Task<User?> GetByEmailAsync(string email) =>
        _db.Users.FirstOrDefaultAsync(u => u.Email == email);

    public Task<User?> GetByIdAsync(Guid id) =>
        _db.Users.FirstOrDefaultAsync(u => u.Id == id);

    public async Task AddAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }
}

