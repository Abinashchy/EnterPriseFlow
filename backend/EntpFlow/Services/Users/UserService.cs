using EntpFlow.Data;
using EntpFlow.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EntpFlow.Services.Users
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDto>> GetUsers()
        {
            var users =  await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Department)
            .Select(u => new UserDto
            {
                UserId = u.UserId,
                EmployeeID = u.EmployeeId,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role.Name,
                Department = u.Department.Name
            })
            .ToListAsync();
            return users;
        }

        public async Task<UserDto?> GetUserById(int id)
        {
            // return await _context.Users.FindAsync(id);
        var user =  await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Department)
            .Select(u => new UserDto
            {
                UserId = u.UserId,
                EmployeeID = u.EmployeeId,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role.Name,
                Department = u.Department.Name
            })
            .FirstAsync(u => u.UserId == id);
            return user;
        }

        public async Task CreateUser(CreateUser user)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(x => x.Name == user.Role);
            var department = await _context.Departments.FirstOrDefaultAsync(x => x.Name == user.Department);
            var deptId = department.Id;

            var roleId = role.Id;
            var NewUser = new User()
            {
            
                EmployeeId = user.EmployeeID,
                Name = user.Name,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                RoleId = roleId,
                DepartmentId = deptId
            };
            await _context.Users.AddAsync(NewUser);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null) return;

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateUser(int id, UpdateUser user)
        {
        var UpdatedUser = await _context.Users.FindAsync(id);

        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == user.Role);

        var department = await _context.Departments.FirstOrDefaultAsync(d => d.Name == user.Department);

        UpdatedUser.Name = user.Name;
        UpdatedUser.Email = user.Email;
        UpdatedUser.RoleId = role.Id;
        UpdatedUser.DepartmentId = department.Id;

        await _context.SaveChangesAsync();
      
        }
    }
}
