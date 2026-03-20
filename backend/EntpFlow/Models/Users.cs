using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EntpFlow.Models;

public class User
{
    public int UserId { get; set; }
    [Required]
    public string? EmployeeId {get; set;}

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public int? DepartmentId { get; set; }

    public bool IsActive { get; set; } = true;
    public int RoleId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Department? Department { get; set; }
    public Role? Role { get; set; }

    public ICollection<ProjectMember>? ProjectMemberships { get; set; }

}