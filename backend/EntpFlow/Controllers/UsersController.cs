using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EntpFlow.Data;
using EntpFlow.Models;
using EntpFlow.Services.Users;
using Microsoft.AspNetCore.Authorization;

namespace EntpFlow.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _service;

    public UsersController(UserService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var users =  await _service.GetUsers();
        if (users == null)
            return NotFound();

        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _service.GetUserById(id);
        if (user == null)
            return NotFound();
    
        return Ok(user);

 
    }

    // [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(CreateUser user)
    {
        Console.Write("Creating request");
        await _service.CreateUser(user);

        return Ok();

    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, UpdateUser user)
    {
        try
        {
            var oldUser = await _service.GetUserById(id);
            if (oldUser == null)
                return NotFound();

            await _service.UpdateUser(id, user);

        }
        catch(Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error Updating data");
        }
        return Ok(user);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _service.GetUserById(id);


        if (user == null)
            return NotFound();

        await _service.DeleteUser(id);
        return Ok(user);
        

        
    }
}