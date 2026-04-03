using EntpFlow.Data;
using EntpFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace EntpFlow.Services;

public class TaskCommentService
{
    private readonly ApplicationDbContext _context;

    public TaskCommentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaskComment>> GetComments(int taskId)
    {
        return await _context.TaskComments
            .Where(x => x.TaskId == taskId)
            .Include(x => x.User)
            .ToListAsync();
    }

    public async Task AddComment(TaskComment comment)
    {
        _context.TaskComments.Add(comment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateComment(int id, string newComment, int userId)
    {
        var comment = await _context.TaskComments.FindAsync(id);
        if (comment == null || comment.UserId != userId)
            throw new Exception("Comment not found or unauthorized");

        comment.Comment = newComment;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteComment(int id, int userId)
    {
        var comment = await _context.TaskComments.FindAsync(id);
        if (comment == null || comment.UserId != userId)
            throw new Exception("Comment not found or unauthorized");

        _context.TaskComments.Remove(comment);
        await _context.SaveChangesAsync();
    }
}