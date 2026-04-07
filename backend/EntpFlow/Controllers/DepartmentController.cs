using Microsoft.AspNetCore.Mvc;
using EntpFlow.Models;
using EntpFlow.Services.Users;
using EntpFlow.Services;

namespace EntpFlow.Controllers;
//
[ApiController]
[Route("api/[controller]")]
public class DepartmentController : ControllerBase
{
    private readonly DepartmentService _service;

    public DepartmentController(DepartmentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Department>>> GetDepts()
    {
        var depts =  await _service.GetDepts();
        if (depts == null)
            return NotFound();

        return Ok(depts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Department>> GetDept(int id)
    {
        var dept = await _service.GetDeptById(id);
        if (dept == null)
            return NotFound();
        return Ok(dept);
 
    }

    [HttpPost]
    public async Task<ActionResult<Department>> CreateDept(Department dept)
    {
        await _service.CreateDept(dept);

        return CreatedAtAction(nameof(GetDept), new {id = dept.Id}, dept);

    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDept(int id, Department dept)
    {
        dept.Id = id;
        try
        {
            var oldDept = await _service.GetDeptById(id);
            if (oldDept == null)
                return NotFound();

            await _service.UpdateDept(dept);

        }
        catch(Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error Updating data");
        }
        return Ok(dept);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDept(int id)
    {
        var dept = await _service.GetDeptById(id);


        if (dept == null)
            return NotFound();

        await _service.DeleteDept(id);
        return Ok(dept);
        
    }
}