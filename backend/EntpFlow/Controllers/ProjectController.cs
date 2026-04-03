
using EntpFlow.DTOs.Projects;
using EntpFlow.Interfaces;
using EntpFlow.Models;
using EntpFlow.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EntpFlow.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin, Manager")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _service;
    public ProjectController(IProjectService service, IAccessControlService accessControl)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
    {
        var projs = await _service.GetProjects();
        if(projs == null)
            return NotFound();

        return Ok(projs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Project>> GetSingleProject(int id)
    {
        var proj = await _service.GetProjectById(id);
        if(proj == null)
        {
            return NotFound("Project not exist");

        }
        return Ok(proj);
    }
    [HttpPost]
    // [Authorize(Roles = "Admin, Manager")]
    public async Task<IActionResult> CreateProject(CreateProjectDto project)
    {
        await _service.CreateProject(project);
        return Ok();
    }

    [HttpPut("{id}")]
    // [Authorize(Roles = "Admin, Manager")]
    public async Task<IActionResult> UpdateProject(int id, UpdateProjectDto project)
    {
        await _service.UpdateProject(id, project);
        return Ok(project);
    }

    [HttpDelete("{id}")]
    // [Authorize(Roles = "Admin, Manager")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var proj = await _service.GetProjectById(id);


        if (proj == null)
            return NotFound();

        await _service.DeleteProject(id);
        return Ok(proj);
        
    }


}