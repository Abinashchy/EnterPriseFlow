using Microsoft.EntityFrameworkCore;
using EntpFlow.Models;

namespace EntpFlow.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Tables
    public DbSet<Department> Departments { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<Project> Projects { get; set; }

    public DbSet<ProjectMember> ProjectMembers { get; set; }

    public DbSet<TaskItem> Tasks { get; set; }

    public DbSet<TaskComment> TaskComments { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //-----------------------------------------
        // Composite Keys
        //-----------------------------------------

        // modelBuilder.Entity<User>()
        //     .HasIndex(u => u.EmployeeId)
        //     .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.EmployeeId)
            .IsUnique();

        modelBuilder.Entity<ProjectMember>()
            .HasOne(pm => pm.User)
            .WithMany(u => u.ProjectMemberships)
            .HasForeignKey(pm => pm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProjectMember>()
            .HasKey(pm => new { pm.ProjectId, pm.UserId });



        //     //-----------------------------------------
        //     // User ↔ Department
        //     //-----------------------------------------

        //     modelBuilder.Entity<User>()
        //         .HasOne(u => u.Department)
        //         .WithMany(d => d.Users)
        //         .HasForeignKey(u => u.DepartmentId)
        //         .OnDelete(DeleteBehavior.Restrict);

        //     //-----------------------------------------
        //     // Project ↔ Department
        //     //-----------------------------------------

        //     modelBuilder.Entity<Project>()
        //         .HasOne(p => p.Department)
        //         .WithMany(d => d.Projects)
        //         .HasForeignKey(p => p.DepartmentId);

        //     //-----------------------------------------
        //     // Project ↔ User (CreatedBy)
        //     //-----------------------------------------

        //     modelBuilder.Entity<Project>()
        //         .HasOne(p => p.CreatedByUser)
        //         .WithMany()
        //         .HasForeignKey(p => p.CreatedBy)
        //         .OnDelete(DeleteBehavior.Restrict);

        //     //-----------------------------------------
        //     // ProjectMembers relationships
        //     //-----------------------------------------

        //     modelBuilder.Entity<ProjectMember>()
        //         .HasOne(pm => pm.Project)
        //         .WithMany(p => p.Members)
        //         .HasForeignKey(pm => pm.ProjectId);

        modelBuilder.Entity<ProjectMember>()
            .HasOne(pm => pm.User)
            .WithMany(u => u.ProjectMemberships)
            .HasForeignKey(pm => pm.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        //     //-----------------------------------------
        //     // Task ↔ Project
        //     //-----------------------------------------

        //     modelBuilder.Entity<TaskItem>()
        //         .HasOne(t => t.Project)
        //         .WithMany(p => p.Tasks)
        //         .HasForeignKey(t => t.ProjectId);

        //     //-----------------------------------------
        //     // Task ↔ Assigned User
        //     //-----------------------------------------

        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.AssignedUser)
            .WithMany()
            .HasForeignKey(t => t.AssignedTo)
            .OnDelete(DeleteBehavior.Restrict);

        //     //-----------------------------------------
        //     // Task ↔ Creator
        //     //-----------------------------------------

        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.CreatedByUser)
            .WithMany()
            .HasForeignKey(t => t.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TaskItem>()
            .Property(t => t.Status)
            .HasConversion<string>();

        //     //-----------------------------------------
        //     // TaskComments
        //     //-----------------------------------------

        modelBuilder.Entity<TaskComment>()
            .HasOne(c => c.Task)
            .WithMany(t => t.Comments)
            .HasForeignKey(c => c.TaskId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TaskComment>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Role>()
        .HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "Manager" },
            new Role { Id = 3, Name = "Employee" }
        );

        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, Name = "Engineering" },
            new Department { Id = 2, Name = "HR" }
        );


    }
}