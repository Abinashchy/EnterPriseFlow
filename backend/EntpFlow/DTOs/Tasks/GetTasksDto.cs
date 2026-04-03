using static EntpFlow.Models.TaskItem;

namespace EntpFlow.DTOs.Tasks;

public class GetTasksDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public WorkStatus Status { get; set; } = WorkStatus.Created;

    public string? Priority { get; set; }

    public int ProjectId { get; set; }

    public string? ProjectName { get; set; }

    public int? AssignedTo { get; set; }

    public string? AssignedToName { get; set; }

    public int CreatedBy { get; set; }

    public string? CreatedByName { get; set; }

    public DateTime CreatedAt { get; set; }
}