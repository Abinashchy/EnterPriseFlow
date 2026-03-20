using EntpFlow.Models;
using EntpFlow.Services;
using Microsoft.AspNetCore.Mvc;

namespace EntpFlow.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly RoleService _service;

    public RolesController(RoleService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
    {
        var roles = await _service.GetRoles();

        return Ok(roles);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole(Role role)
    {
        await _service.CreateRole(role);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        await _service.DeleteRole(id);

        return NoContent();
    }
}