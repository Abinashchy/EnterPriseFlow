using EntpFlow.DTOs.ProjectMembers;
using EntpFlow.Interfaces;
using EntpFlow.Models;
using EntpFlow.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EntpFlow.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectMembersController : ControllerBase
{
    private readonly IProjectMember _service;


    public ProjectMembersController(IProjectMember service)
    {
        _service = service;
    }

    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetMembers(int projectId)
    {
        var members = await _service.GetProjectMembers(projectId);
        return Ok(members);
    }

    [HttpPost]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<IActionResult> AddMember(CreateMembersDto member)
    {
        await _service.AddMember(member);
        return Ok();
    }

    [HttpDelete("project/{projectId}/user/{userId}")]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<IActionResult> RemoveMember(int projectId, int userId)
    {
        await _service.RemoveMember(projectId, userId);
        return NoContent();
    }
}