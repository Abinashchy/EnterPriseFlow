using System.ComponentModel.DataAnnotations;

namespace EntpFlow.DTOs.Projects;

public class CreateProjectDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public int DepartmentId { get; set; }

}