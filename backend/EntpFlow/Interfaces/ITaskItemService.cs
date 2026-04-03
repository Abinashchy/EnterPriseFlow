using EntpFlow.DTOs.Tasks;

namespace EntpFlow.Interfaces;

public interface ITaskItemService
{

    Task<IEnumerable<GetTasksDto>> GetTasks(int? projectId  = null);
    Task<GetTasksDto> CreateTask(CreateTasksDto dto);
    Task<bool> UpdateTask(int id, UpdateTaskDto dto);
    Task<bool> DeleteTask(int id);
}