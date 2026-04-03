using EntpFlow.DTOs.ProjectMembers;

namespace EntpFlow.Interfaces;

public interface IProjectMember
{
    Task<IEnumerable<GetMembersDto>> GetProjectMembers(int projectId);
    Task<GetMembersDto> AddMember(CreateMembersDto dto);
    Task<bool> RemoveMember(int projectId, int userId);
}