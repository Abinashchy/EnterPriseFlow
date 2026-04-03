using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EntpFlow.Interfaces;
using EntpFlow.Models;
using Microsoft.IdentityModel.Tokens;

namespace EntpFlow.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        var jwtSection = _configuration.GetSection("Jwt");

        var key = jwtSection["Key"] ?? throw new Exception("JWT Key is missing.");
        var issuer = jwtSection["Issuer"] ?? throw new Exception("JWT Issuer is missing.");
        var audience = jwtSection["Audience"] ?? throw new Exception("JWT Audience is missing.");
        var expiryMinutes = int.Parse(jwtSection["ExpiryMinutes"] ?? "60");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role?.Name ?? string.Empty),
            new Claim("DepartmentId", user.DepartmentId?.ToString() ?? string.Empty),
            new Claim("EmployeeId", user.EmployeeId ?? string.Empty)
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}