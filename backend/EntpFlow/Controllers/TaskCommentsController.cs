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
        return Ok(comments);
    }

    [HttpPost]
    public async Task<IActionResult> AddComment(TaskComment comment)
    {
        await _service.AddComment(comment);
        return Ok();
    }
}