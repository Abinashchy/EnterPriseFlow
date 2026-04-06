using EntpFlow.Data;
using EntpFlow.Models;
using EntpFlow.Services;
using Microsoft.EntityFrameworkCore;

namespace EntpFlow.Tests;

public class TaskCommentServiceTests
{
    private static ApplicationDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new ApplicationDbContext(options);
    }

    private static void SeedComments(ApplicationDbContext context)
    {
        context.Users.Add(new User { UserId = 1, Name = "Admin", Email = "admin@test.com", PasswordHash = "hash", EmployeeId = "E001" });
        context.Users.Add(new User { UserId = 2, Name = "User", Email = "user@test.com", PasswordHash = "hash", EmployeeId = "E002" });
        context.TaskComments.AddRange(
            new TaskComment { Id = 1, TaskId = 10, UserId = 1, Comment = "First comment" },
            new TaskComment { Id = 2, TaskId = 10, UserId = 2, Comment = "Second comment" },
            new TaskComment { Id = 3, TaskId = 20, UserId = 1, Comment = "Other task comment" }
        );
        context.SaveChanges();
    }

    [Fact]
    public async Task GetComments_ReturnsCommentsForTask()
    {
        using var context = CreateContext(nameof(GetComments_ReturnsCommentsForTask));
        SeedComments(context);
        var service = new TaskCommentService(context);

        var result = (await service.GetComments(10)).ToList();

        Assert.Equal(2, result.Count);
        Assert.All(result, c => Assert.Equal(10, c.TaskId));
    }

    [Fact]
    public async Task GetComments_ReturnsEmptyForNonexistentTask()
    {
        using var context = CreateContext(nameof(GetComments_ReturnsEmptyForNonexistentTask));
        SeedComments(context);
        var service = new TaskCommentService(context);

        var result = (await service.GetComments(999)).ToList();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetComments_IncludesUserNavigation()
    {
        using var context = CreateContext(nameof(GetComments_IncludesUserNavigation));
        SeedComments(context);
        var service = new TaskCommentService(context);

        var result = (await service.GetComments(10)).ToList();

        Assert.NotNull(result[0].User);
        Assert.Equal("Admin", result[0].User!.Name);
    }

    [Fact]
    public async Task AddComment_InsertsComment()
    {
        using var context = CreateContext(nameof(AddComment_InsertsComment));
        var service = new TaskCommentService(context);

        await service.AddComment(new TaskComment { TaskId = 10, UserId = 1, Comment = "New comment" });

        Assert.Single(context.TaskComments);
        Assert.Equal("New comment", context.TaskComments.First().Comment);
    }

    [Fact]
    public async Task UpdateComment_UpdatesExistingComment()
    {
        using var context = CreateContext(nameof(UpdateComment_UpdatesExistingComment));
        SeedComments(context);
        var service = new TaskCommentService(context);

        await service.UpdateComment(1, "Updated comment", 1);

        var comment = await context.TaskComments.FindAsync(1);
        Assert.Equal("Updated comment", comment!.Comment);
    }

    [Fact]
    public async Task UpdateComment_ThrowsIfWrongUser()
    {
        using var context = CreateContext(nameof(UpdateComment_ThrowsIfWrongUser));
        SeedComments(context);
        var service = new TaskCommentService(context);

        await Assert.ThrowsAsync<Exception>(() => service.UpdateComment(1, "hack", 999));
    }

    [Fact]
    public async Task UpdateComment_ThrowsIfNotFound()
    {
        using var context = CreateContext(nameof(UpdateComment_ThrowsIfNotFound));
        var service = new TaskCommentService(context);

        await Assert.ThrowsAsync<Exception>(() => service.UpdateComment(999, "test", 1));
    }

    [Fact]
    public async Task DeleteComment_RemovesComment()
    {
        using var context = CreateContext(nameof(DeleteComment_RemovesComment));
        SeedComments(context);
        var service = new TaskCommentService(context);

        await service.DeleteComment(1, 1);

        Assert.Null(await context.TaskComments.FindAsync(1));
    }

    [Fact]
    public async Task DeleteComment_ThrowsIfWrongUser()
    {
        using var context = CreateContext(nameof(DeleteComment_ThrowsIfWrongUser));
        SeedComments(context);
        var service = new TaskCommentService(context);

        await Assert.ThrowsAsync<Exception>(() => service.DeleteComment(1, 999));
    }

    [Fact]
    public async Task DeleteComment_ThrowsIfNotFound()
    {
        using var context = CreateContext(nameof(DeleteComment_ThrowsIfNotFound));
        var service = new TaskCommentService(context);

        await Assert.ThrowsAsync<Exception>(() => service.DeleteComment(999, 1));
    }
}
