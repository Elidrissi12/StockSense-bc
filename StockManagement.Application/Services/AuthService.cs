using StockManagement.Application.Contracts;
using StockManagement.Application.Interfaces;
using StockManagement.Domain.Entities;
using StockManagement.Domain.Enums;

namespace StockManagement.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _userRepository.ExistsByEmailAsync(request.Email))
            throw new InvalidOperationException("Email déjà utilisé.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Nom = request.Nom,
            Email = request.Email,
            Role = request.Role,
            PasswordHash = _passwordHasher.HashPassword(request.Password)
        };

        await _userRepository.AddAsync(user);

        var token = await _tokenService.GenerateTokenAsync(user.Id, user.Email, user.Role);
        var expiresAtUtc = await _tokenService.GetTokenExpiryUtcAsync();

        return new AuthResponse(user.Id, token, expiresAtUtc, user.Role);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user is null)
            throw new UnauthorizedAccessException("Identifiants invalides.");

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Identifiants invalides.");

        var token = await _tokenService.GenerateTokenAsync(user.Id, user.Email, user.Role);
        var expiresAtUtc = await _tokenService.GetTokenExpiryUtcAsync();

        return new AuthResponse(user.Id, token, expiresAtUtc, user.Role);
    }
}

