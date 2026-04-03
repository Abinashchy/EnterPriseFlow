using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EntpFlow.Data;
using EntpFlow.Interfaces;
using EntpFlow.Models;
using EntpFlow.Services.Users;
using Microsoft.AspNetCore.Authorization;

namespace EntpFlow.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;
    private readonly ICurrentUserService _currentUser;

    public UsersController(IUserService service, ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var users =  await _service.GetVisibleUsersAsync();
        if (users == null)
            return NotFound();

        return Ok(users);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _service.GetUserById(id);
        if (user == null)
            return NotFound();
    
        return Ok(user);

 
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<User>> CreateUser(CreateUser user)
    {
        Console.Write("Creating request \n ");
        await _service.CreateUser(user);

        return Ok();

    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _service.GetUserById(id);


        if (user == null)
            return NotFound();

        await _service.DeleteUser(id);
        return Ok(user);
    }

        [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            _currentUser.UserId,
            _currentUser.Email,
            _currentUser.Role,
            _currentUser.DepartmentId,
            _currentUser.EmployeeId
        });
    }
}