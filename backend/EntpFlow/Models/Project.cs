using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace EntpFlow.Models;

public class Project
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int CreatedBy { get; set; }
    public int DepartmentId { get; set; }

    public Department? Department { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    
    [ForeignKey("CreatedBy")]
    public User? CreatedByUser { get; set; }

    public ICollection<ProjectMember>? Members { get; set; }

    public ICollection<TaskItem>? Tasks { get; set; }
}