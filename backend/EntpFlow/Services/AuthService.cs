using EntpFlow.Data;
using EntpFlow.DTOs.auth;
using EntpFlow.DTOs;
using EntpFlow.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EntpFlow.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public AuthService(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IConfiguration configuration)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDTO dto)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Department)
            .FirstOrDefaultAsync(u => u.Email == dto.Email && u.IsActive);

        if (user == null)
            return null;

        var isPasswordValid = _passwordHasher.Verify(dto.Password, user.PasswordHash);
        if (!isPasswordValid)
            return null;

        var token = _tokenService.GenerateToken(user);
        var expiryMinutes = int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "60");

        return new LoginResponseDto
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
            User = new UserDto
            {
                UserId = user.UserId,
                EmployeeId = user.EmployeeId,
                Name = user.Name,
                Email = user.Email,
                // RoleId = user.RoleId,
                Role = user.Role?.Name ?? string.Empty,
                // DepartmentId = user.DepartmentId,
                Department = user.Department?.Name,
                // IsActive = user.IsActive,
                // CreatedAt = user.CreatedAt
            }
        };
    }
}