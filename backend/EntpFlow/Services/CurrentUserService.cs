using System.Security.Claims;
using EntpFlow.Interfaces;

namespace EntpFlow.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated ?? false;

    public int? UserId
    {
        get
        {
            var value = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(value, out var id) ? id : null;
        }
    }

    public string? Email =>
        User?.FindFirstValue(ClaimTypes.Email);

    public string? Role =>
        User?.FindFirstValue(ClaimTypes.Role);

    public int? DepartmentId
    {
        get
        {
            var value = User?.FindFirstValue("DepartmentId");
            return int.TryParse(value, out var id) ? id : null;
        }
    }

    public string? EmployeeId =>
        User?.FindFirstValue("EmployeeId");
}