using EntpFlow.Data;
using EntpFlow.DTOs.Tasks;
using EntpFlow.Interfaces;
using EntpFlow.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
namespace EntpFlow.Services;

public class TaskItemService : ITaskItemService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IAccessControlService _accessControl;

    public TaskItemService(ApplicationDbContext context, ICurrentUserService currentUser, IAccessControlService accessControl)
    {
        _context = context;
        _currentUser = currentUser;
        _accessControl = accessControl;
    }

    public async Task<IEnumerable<GetTasksDto>> GetTasks(int? projectId  = null)
    {
        IQueryable<TaskItem> fullTaskQuery = _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedUser)
                .Include(t => t.CreatedByUser);

        IQueryable<TaskItem> query = _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.AssignedUser)
            .Include(t => t.CreatedByUser)
            .Where(t => t.ProjectId == projectId);
        
        if (projectId == null)
        {
            query = fullTaskQuery;
        }

        if (_currentUser.Role == "Admin")
        {
            return await query
                .Select(MapTask())
                .ToListAsync();
        }

        if ((_currentUser.Role == "Manager" || _currentUser.Role == "Employee") && _currentUser.DepartmentId.HasValue)
        {
            var departmentId = _currentUser.DepartmentId.Value;

            return await query
                .Where(t => t.Project != null && t.Project.DepartmentId == departmentId)
                .Select(MapTask())
                .ToListAsync();
        }

        return Enumerable.Empty<GetTasksDto>();
    }

    public async Task<GetTasksDto> CreateTask(CreateTasksDto dto)
    {
        if (!_currentUser.UserId.HasValue)
            throw new UnauthorizedAccessException("User not authenticated.");

        var allowed = await _accessControl.CanManageProjectAsync(dto.ProjectId);
        if (!allowed)
            throw new UnauthorizedAccessException("You cannot create tasks for this project.");

        if (dto.AssignedTo.HasValue)
        {
            var assignedUserExists = await _context.Users
                .AnyAsync(u => u.UserId == dto.AssignedTo.Value);

            if (!assignedUserExists)
                throw new KeyNotFoundException("Assigned user not found.");
        }

        var entity = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            Status = dto.Status,
            Priority = dto.Priority,
            ProjectId = dto.ProjectId,
            AssignedTo = dto.AssignedTo,
            CreatedBy = _currentUser.UserId.Value,
            CreatedAt = DateTime.UtcNow
        };

        _context.Tasks.Add(entity);
        await _context.SaveChangesAsync();

        return await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.AssignedUser)
            .Include(t => t.CreatedByUser)
            .Where(t => t.Id == entity.Id)
            .Select(MapTask())
            .FirstAsync();
    }

    public async Task<bool> UpdateTask(int id, UpdateTaskDto dto)
    {
        var entity = await _context.Tasks.FindAsync(id);
        if (entity == null)
            return false;

        var allowed = await _accessControl.CanManageTaskAsync(id);
        if (!allowed)
            throw new UnauthorizedAccessException("You cannot update this task.");

        if (dto.AssignedTo.HasValue)
        {
            var assignedUserExists = await _context.Users
                .AnyAsync(u => u.UserId == dto.AssignedTo.Value);

            if (!assignedUserExists)
                throw new KeyNotFoundException("Assigned user not found.");
        }

        entity.Title = dto.Title;
        entity.Description = dto.Description;
        entity.Status = dto.Status;
        entity.Priority = dto.Priority;
        entity.AssignedTo = dto.AssignedTo;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteTask(int id)
    {
        var entity = await _context.Tasks.FindAsync(id);
        if (entity == null)
            return false;

        var allowed = await _accessControl.CanManageTaskAsync(id);
        if (!allowed)
            throw new UnauthorizedAccessException("You cannot delete this task.");

        var comments = _context.TaskComments.Where(c => c.TaskId == id);
        _context.TaskComments.RemoveRange(comments);

        _context.Tasks.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

        private static Expression<Func<TaskItem, GetTasksDto>> MapTask()
    {
        return t => new GetTasksDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Status = t.Status,
            Priority = t.Priority,
            ProjectId = t.ProjectId,
            ProjectName = t.Project != null ? t.Project.Name : null,
            AssignedTo = t.AssignedTo,
            AssignedToName = t.AssignedUser != null ? t.AssignedUser.Name : null,
            CreatedBy = t.CreatedBy,
            CreatedByName = t.CreatedByUser != null ? t.CreatedByUser.Name : null,
            CreatedAt = t.CreatedAt
        };
    }
}