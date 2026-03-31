using BCrypt.Net;
using StockManagement.Application.Interfaces;

namespace StockManagement.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        // BCrypt est une fonction lente, adaptée au stockage de mots de passe.
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}

