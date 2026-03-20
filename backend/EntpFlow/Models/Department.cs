namespace EntpFlow.Models;

public class Department
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<User>? Users { get; set; }
    public ICollection<Project>? Projects { get; set; }

}