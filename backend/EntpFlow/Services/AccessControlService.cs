using EntpFlow.Data;
using EntpFlow.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EntpFlow.Services;

public class AccessControlService : IAccessControlService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public AccessControlService(
        ApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public bool IsAdmin()
    {
        return _currentUser.Role == "Admin";
    }

    public bool IsManager()
    {
        return _currentUser.Role == "Manager";
    }

    public bool CanManageDepartment(int? targetDepartmentId)
    {
        if (IsAdmin())
            return true;

        if (!IsManager())
            return false;

        if (!_currentUser.DepartmentId.HasValue || !targetDepartmentId.HasValue)
            return false;

        return _currentUser.DepartmentId.Value == targetDepartmentId.Value;
    }

    public async Task<bool> CanManageProjectAsync(int projectId)
    {
        if (IsAdmin())
            return true;

        if (!IsManager())
            return false;

        var projectDepartmentId = await _context.Projects
            .AsNoTracking()
            .Where(p => p.Id == projectId)
            .Select(p => p.DepartmentId)
            .FirstOrDefaultAsync();

        if (projectDepartmentId == 0)
            return false;

        return CanManageDepartment(projectDepartmentId);
    }

    public async Task<bool> CanManageProjectMemberAsync(int projectMemberId)
    {
        if (IsAdmin())
            return true;

        if (!IsManager())
            return false;

        var departmentId = await _context.ProjectMembers
            .AsNoTracking()
            .Where(pm => pm.UserId == projectMemberId)
            .Select(pm => pm.Project != null ? pm.Project.DepartmentId : (int?)null)
            .FirstOrDefaultAsync();

        return CanManageDepartment(departmentId);
    }

    public async Task<bool> CanManageTaskAsync(int taskId)
    {
        if (IsAdmin())
            return true;

        if (!IsManager())
            return false;

        var departmentId = await _context.Tasks
            .AsNoTracking()
            .Where(t => t.Id == taskId)
            .Select(t => t.Project != null ? t.Project.DepartmentId : (int?)null)
            .FirstOrDefaultAsync();

        return CanManageDepartment(departmentId);
    }
}