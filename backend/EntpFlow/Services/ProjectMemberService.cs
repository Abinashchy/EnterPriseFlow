using EntpFlow.Data;
using EntpFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace EntpFlow.Services;

public class ProjectMemberService
{
    private readonly ApplicationDbContext _context;

    public ProjectMemberService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProjectMember>> GetProjectMembers(int projectId)
    {
        return await _context.ProjectMembers
            .Where(x => x.ProjectId == projectId)
            .Include(x => x.User)
            .ToListAsync();
    }

    public async Task AddMember(ProjectMember member)
    {
        _context.ProjectMembers.Add(member);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveMember(int projectId, int userId)
    {
        var member = await _context.ProjectMembers
            .FirstOrDefaultAsync(x => x.ProjectId == projectId && x.UserId == userId);

        if (member == null) return;

        _context.ProjectMembers.Remove(member);

        await _context.SaveChangesAsync();
    }
}