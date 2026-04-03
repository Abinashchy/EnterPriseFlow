using EntpFlow.Data;
using EntpFlow.Models;
using EntpFlow.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EntpFlow.Interfaces;

namespace EntpFlow.Services.Users
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ICurrentUserService _currentUser;

        public UserService(ApplicationDbContext context, IPasswordHasher passwordHasher, ICurrentUserService currentUser)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<UserDto>> GetVisibleUsersAsync()
        {
            if(_currentUser.Role =="Admin"){
            var users =  await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Department)
            .Select(u => new UserDto
            {
                UserId = u.UserId,
                EmployeeId = u.EmployeeId,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role.Name,
                Department = u.Department.Name,
                IsActive = u.IsActive
            })
            .ToListAsync();
            
            return users;
            }
            if(_currentUser.Role =="Manager" && _currentUser.DepartmentId.HasValue){
                var deptId = _currentUser.DepartmentId.Value;
                var users =  await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Department)
                .Where(u => u.DepartmentId == deptId)
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    EmployeeId = u.EmployeeId,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role.Name,
                    Department = u.Department.Name,
                    IsActive = u.IsActive
                })
                .ToListAsync();
                return users;
            }
            return Enumerable.Empty<UserDto>();

        }
        

        public async Task<UserDto?> GetUserById(int id)
        {
        var user =  await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Department)
            .Select(u => new UserDto
            {
                UserId = u.UserId,
                EmployeeId = u.EmployeeId,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role.Name,
                Department = u.Department.Name,
                IsActive = u.IsActive
            })
            .FirstAsync(u => u.UserId == id);
            return user;
        }

        public async Task CreateUser(CreateUser user)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(x => x.Name == user.Role);
            var department = await _context.Departments.FirstOrDefaultAsync(x => x.Name == user.Department);
            if(role == null || department == null)
                return;
            var deptId = department.Id;

            var roleId = role.Id;
            var NewUser = new User()
            {
            
                EmployeeId = user.EmployeeId,
                Name = user.Name,
                Email = user.Email,
                PasswordHash = _passwordHasher.Hash(user.Password),
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

        if(UpdatedUser == null || role == null || department == null)
            return;

        UpdatedUser.Name = user.Name;
        UpdatedUser.Email = user.Email;
        UpdatedUser.RoleId = role.Id;
        UpdatedUser.DepartmentId = department.Id;
        UpdatedUser.IsActive = user.IsActive;

        await _context.SaveChangesAsync();
      
        }
    }
}
