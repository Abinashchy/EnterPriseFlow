using System.ComponentModel.DataAnnotations;
using static EntpFlow.Models.TaskItem;

namespace EntpFlow.DTOs.Tasks;

public class UpdateTaskDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public WorkStatus Status { get; set; } = WorkStatus.Created;

    public string? Priority { get; set; }

    public int? AssignedTo { get; set; }
}