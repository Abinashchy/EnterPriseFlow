
using EntpFlow.Models;
using EntpFlow.Services;
using Microsoft.AspNetCore.Mvc;

namespace EntpFlow.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    private readonly ProjectService _service;

    public ProjectController(ProjectService service)
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
        return proj;
    }

    public async Task<IActionResult> CreateProject(Project project)
    {
        await _service.CreateProject(project);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(int id, Project project)
    {
        project.Id = id;
        try
        {
            var oldProject = await _service.GetProjectById(id);
            if (oldProject == null)
                return NotFound();

            await _service.UpdateProject(project);

        }
        catch(Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error Updating data");
        }
        return Ok(project);
    }

        [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var proj = await _service.GetProjectById(id);


        if (proj == null)
            return NotFound();

        await _service.DeleteProject(id);
        return Ok(proj);
        
    }





}