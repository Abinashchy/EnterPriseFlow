namespace EntpFlow.DTOs.ProjectMembers;

public class GetMembersDto
{
    public int ProjectId { get; set; }

    public string? ProjectName { get; set; }

    public int UserId { get; set; }

    public string? UserName { get; set; }
}