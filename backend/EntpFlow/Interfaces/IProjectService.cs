using EntpFlow.DTOs.Projects;

namespace EntpFlow.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<GetProjectDto>> GetProjects();
    Task<GetProjectDto?> GetProjectById(int id);
    Task<GetProjectDto> CreateProject(CreateProjectDto dto);
    Task<bool> UpdateProject(int id, UpdateProjectDto dto);
    Task<bool> DeleteProject(int id);
}