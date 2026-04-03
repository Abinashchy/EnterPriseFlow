using EntpFlow.DTOs.Tasks;
using EntpFlow.Interfaces;
using EntpFlow.Models;
using EntpFlow.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EntpFlow.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TaskItemsController : ControllerBase
{
    private readonly ITaskItemService _service;

    public TaskItemsController(ITaskItemService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetTasks()
    {
        var tasks = await _service.GetTasks();
        return Ok(tasks);
    }

    [HttpGet("projects/{projectId}")]
    public async Task<IActionResult> GetTasksByProjectId(int projectId)
    {
        var tasks = await _service.GetTasks(projectId);
        return Ok(tasks);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask(CreateTasksDto task)
    {
        await _service.CreateTask(task);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, UpdateTaskDto task)
    {
        await _service.UpdateTask(id, task);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        await _service.DeleteTask(id);
        return NoContent();
    }
}