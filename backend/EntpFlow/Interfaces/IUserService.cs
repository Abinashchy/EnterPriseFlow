using EntpFlow.DTOs;

namespace EntpFlow.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetVisibleUsersAsync();
    Task<UserDto?> GetUserById(int id);
    Task CreateUser(CreateUser dto);
    Task UpdateUser(int id, UpdateUser dto);
    Task DeleteUser(int id);
}