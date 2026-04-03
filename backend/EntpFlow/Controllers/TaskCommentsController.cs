using EntpFlow.Models;
using EntpFlow.Services;
using Microsoft.AspNetCore.Mvc;

namespace EntpFlow.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskCommentsController : ControllerBase
{
    private readonly TaskCommentService _service;

    public TaskCommentsController(TaskCommentService service)
    {
        _service = service;
    }

    [HttpGet("{taskId}")]
    public async Task<IActionResult> GetComments(int taskId)
    {
        var comments = await _service.GetComments(taskId);
        var result = comments.Select(c => new
        {
            c.Id,
            c.TaskId,
            c.UserId,
            c.Comment,
            c.CreatedAt,
            UserName = c.User?.Name ?? "Unknown"
        });
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddComment(TaskComment comment)
    {
        await _service.AddComment(comment);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateComment(int id, [FromBody] UpdateCommentRequest request)
    {
        await _service.UpdateComment(id, request.Comment, request.UserId);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(int id, [FromQuery] int userId)
    {
        await _service.DeleteComment(id, userId);
        return NoContent();
    }
}

public class UpdateCommentRequest
{
    public string Comment { get; set; } = string.Empty;
    public int UserId { get; set; }
}