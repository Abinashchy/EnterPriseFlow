using EntpFlow.Models;
using EntpFlow.Services;
using Microsoft.AspNetCore.Mvc;

namespace EntpFlow.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskItemsController : ControllerBase
{
    private readonly TaskItemService _service;

    public TaskItemsController(TaskItemService service)
    {
        _service = service;
    }

    [HttpGet("{projectId}")]
    public async Task<IActionResult> GetTasks(int projectId)
    {
        var tasks = await _service.GetTasks(projectId);
        return Ok(tasks);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask(TaskItem task)
    {
        await _service.CreateTask(task);
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTask(TaskItem task)
    {
        await _service.UpdateTask(task);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        await _service.DeleteTask(id);
        return NoContent();
    }
}