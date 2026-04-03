using EntpFlow.Data;
using EntpFlow.DTOs.ProjectMembers;
using EntpFlow.Interfaces;
using EntpFlow.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EntpFlow.Services;

public class ProjectMemberService : IProjectMember
{
    private readonly ApplicationDbContext _context;
    private readonly IAccessControlService _accessControl;

    public ProjectMemberService(ApplicationDbContext context, IAccessControlService accessControl)
    {
        _context = context;
        _accessControl = accessControl;
    }

    public async Task<IEnumerable<GetMembersDto>> GetProjectMembers(int projectId)
    {
        var allowed = await _accessControl.CanManageProjectAsync(projectId);
        if (!allowed)
            return Enumerable.Empty<GetMembersDto>();

        return await _context.ProjectMembers
            .Include(pm => pm.Project)
            .Include(pm => pm.User)
            .Where(pm => pm.ProjectId == projectId)
            .Select(MapProjectMember())
            .ToListAsync();
    }
    

    public async Task<GetMembersDto> AddMember(CreateMembersDto dto)
    {
        var allowed = await _accessControl.CanManageProjectAsync(dto.ProjectId);
        if (!allowed)
            throw new UnauthorizedAccessException("You cannot add members to this project.");

        var userExists = await _context.Users.AnyAsync(u => u.UserId == dto.UserId);
        if (!userExists)
            throw new KeyNotFoundException("User not found.");

        var alreadyExists = await _context.ProjectMembers
            .AnyAsync(pm => pm.ProjectId == dto.ProjectId && pm.UserId == dto.UserId);

        if (alreadyExists)
            throw new InvalidOperationException("This user is already a member of the project.");

        var entity = new ProjectMember
        {
            ProjectId = dto.ProjectId,
            UserId = dto.UserId
        };

        _context.ProjectMembers.Add(entity);
        await _context.SaveChangesAsync();

        return await _context.ProjectMembers
            .Include(pm => pm.Project)
            .Include(pm => pm.User)
            .Where(pm => pm.ProjectId == dto.ProjectId && pm.UserId == dto.UserId)
            .Select(MapProjectMember())
            .FirstAsync();
    }

    public async Task<bool> RemoveMember(int projectId, int userId)
    {
        var allowed = await _accessControl.CanManageProjectAsync(projectId);
        if (!allowed)
            throw new UnauthorizedAccessException("You cannot remove members from this project.");

        var entity = await _context.ProjectMembers
            .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);

        if (entity == null)
            return false;

        _context.ProjectMembers.Remove(entity);
        await _context.SaveChangesAsync();

        return true;
    }

    private static Expression<Func<ProjectMember, GetMembersDto>> MapProjectMember()
    {
        return pm => new GetMembersDto
        {
            ProjectId = pm.ProjectId,
            ProjectName = pm.Project != null ? pm.Project.Name : null,
            UserId = pm.UserId,
            UserName = pm.User != null ? pm.User.Name : null
        };
    }
}