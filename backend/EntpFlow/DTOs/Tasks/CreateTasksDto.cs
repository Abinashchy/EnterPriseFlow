using static EntpFlow.Models.TaskItem;
using System.ComponentModel.DataAnnotations;

namespace EntpFlow.DTOs.Tasks;

public class CreateTasksDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public int ProjectId { get; set; }

    public int? AssignedTo { get; set; }

    public WorkStatus Status { get; set; } = WorkStatus.Created;

    public string? Priority { get; set; }
}