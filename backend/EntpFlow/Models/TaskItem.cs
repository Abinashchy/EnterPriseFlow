using System.ComponentModel.DataAnnotations.Schema;

namespace EntpFlow.Models;

public class TaskItem
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public enum WorkStatus
{
    Created,
    Assigned,
    InProgress,
    CodeReview,
    ReadyForTesting,
    Completed,
    Cancelled,
    Done
}

    public WorkStatus Status { get; set; } = WorkStatus.Created;   //Make this enum 

    public string? Priority { get; set; }

    public int ProjectId { get; set; }

    public int? AssignedTo { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // navigation
    public Project? Project { get; set; }

    [ForeignKey("AssignedTo")]
    public User? AssignedUser { get; set; }

    [ForeignKey("CreatedBy")]
    public User? CreatedByUser { get; set; }
    public ICollection<TaskComment>? Comments { get; set; }


}