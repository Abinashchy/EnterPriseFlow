using EntpFlow.Data;
using EntpFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace EntpFlow.Services;

public class TaskItemService
{
    private readonly ApplicationDbContext _context;

    public TaskItemService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaskItem>> GetTasks(int projectId)
    {
        return await _context.Tasks
            .Where(x => x.ProjectId == projectId)
            .Include(x => x.AssignedUser)
            .ToListAsync();
    }

    public async Task CreateTask(TaskItem task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateTask(TaskItem task)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTask(int id)
    {
        var task = await _context.Tasks.FindAsync(id);

        if (task == null) return;

        _context.Tasks.Remove(task);

        await _context.SaveChangesAsync();
    }
}