using EntpFlow.Data;
using EntpFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace EntpFlow.Services;

public class RoleService
{
    private readonly ApplicationDbContext _context;

    public RoleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Role>> GetRoles()
    {
        return await _context.Roles.ToListAsync();
    }

    public async Task<Role?> GetRoleById(int id)
    {
        return await _context.Roles.FindAsync(id);
    }

    public async Task CreateRole(Role role)
    {
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteRole(int id)
    {
        var role = await _context.Roles.FindAsync(id);

        if (role == null) return;

        _context.Roles.Remove(role);

        await _context.SaveChangesAsync();
    }
}