using EntpFlow.Models;

namespace EntpFlow.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}