using Microsoft.EntityFrameworkCore;

namespace EntpFlow.Models;

public class ProjectMember
{
    public int ProjectId { get; set; }

    public int UserId { get; set; }

    public Project? Project { get; set; }

    public User? User { get; set; }
}