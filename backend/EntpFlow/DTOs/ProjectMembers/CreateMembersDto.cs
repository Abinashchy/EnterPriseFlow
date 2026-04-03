using System.ComponentModel.DataAnnotations;

namespace EntpFlow.DTOs.ProjectMembers;

public class CreateMembersDto
{
    [Required]
    public int ProjectId { get; set; }

    [Required]
    public int UserId { get; set; }
}