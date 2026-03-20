using EntpFlow.Models;
using EntpFlow.Services;
using Microsoft.AspNetCore.Mvc;

namespace EntpFlow.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectMembersController : ControllerBase
{
    private readonly ProjectMemberService _service;

    public ProjectMembersController(ProjectMemberService service)
    {
        _service = service;
    }

    [HttpGet("{projectId}")]
    public async Task<IActionResult> GetMembers(int projectId)
    {
        var members = await _service.GetProjectMembers(projectId);
        return Ok(members);
    }

    [HttpPost]
    public async Task<IActionResult> AddMember(ProjectMember member)
    {
        await _service.AddMember(member);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveMember(int projectId, int userId)
    {
        await _service.RemoveMember(projectId, userId);
        return NoContent();
    }
}