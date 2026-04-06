using EntpFlow.Data;
using EntpFlow.DTOs.Tasks;
using EntpFlow.Interfaces;
using EntpFlow.Models;
using EntpFlow.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using static EntpFlow.Models.TaskItem;

namespace EntpFlow.Tests;

public class TaskItemServiceTests
{
    private static ApplicationDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        var context = new ApplicationDbContext(options);
        return context;
    }

    private static Mock<ICurrentUserService> MockCurrentUser(int userId = 1, string role = "Admin", int? deptId = 1)
    {
        var mock = new Mock<ICurrentUserService>();
        mock.Setup(x => x.UserId).Returns(userId);
        mock.Setup(x => x.Role).Returns(role);
        mock.Setup(x => x.DepartmentId).Returns(deptId);
        mock.Setup(x => x.IsAuthenticated).Returns(true);
        return mock;
    }

    private static Mock<IAccessControlService> MockAccessControl(bool allow = true)
    {
        var mock = new Mock<IAccessControlService>();
        mock.Setup(x => x.CanManageProjectAsync(It.IsAny<int>())).ReturnsAsync(allow);
        mock.Setup(x => x.CanManageTaskAsync(It.IsAny<int>())).ReturnsAsync(allow);
        return mock;
    }

    private static void SeedData(ApplicationDbContext context)
    {
        context.Departments.Add(new Department { Id = 1, Name = "IT" });
        context.Users.Add(new User { UserId = 1, Name = "Admin", Email = "admin@test.com", PasswordHash = "hash", EmployeeId = "E001", DepartmentId = 1 });
        context.Users.Add(new User { UserId = 2, Name = "User", Email = "user@test.com", PasswordHash = "hash", EmployeeId = "E002", DepartmentId = 1 });
        context.Projects.Add(new Project { Id = 1, Name = "TestProject", CreatedBy = 1, DepartmentId = 1 });
        context.Tasks.AddRange(
            new TaskItem { Id = 1, Title = "Task 1", Status = WorkStatus.Created, Priority = "High", ProjectId = 1, CreatedBy = 1, AssignedTo = 2 },
            new TaskItem { Id = 2, Title = "Task 2", Status = WorkStatus.InProgress, Priority = "Low", ProjectId = 1, CreatedBy = 1, AssignedTo = 1 }
        );
        context.SaveChanges();
    }

    [Fact]
    public async Task GetTasks_AdminGetsAllTasks()
    {
        using var context = CreateContext(nameof(GetTasks_AdminGetsAllTasks));
        SeedData(context);
        var service = new TaskItemService(context, MockCurrentUser().Object, MockAccessControl().Object);

        var result = (await service.GetTasks()).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetTasks_FiltersByProjectId()
    {
        using var context = CreateContext(nameof(GetTasks_FiltersByProjectId));
        SeedData(context);
        var service = new TaskItemService(context, MockCurrentUser().Object, MockAccessControl().Object);

        var result = (await service.GetTasks(1)).ToList();

        Assert.Equal(2, result.Count);
        Assert.All(result, t => Assert.Equal(1, t.ProjectId));
    }

    [Fact]
    public async Task GetTasks_ManagerSeesOnlyDepartmentTasks()
    {
        using var context = CreateContext(nameof(GetTasks_ManagerSeesOnlyDepartmentTasks));
        SeedData(context);
        var service = new TaskItemService(context, MockCurrentUser(role: "Manager", deptId: 1).Object, MockAccessControl().Object);

        var result = (await service.GetTasks()).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetTasks_ReturnsEmptyForUnknownRole()
    {
        using var context = CreateContext(nameof(GetTasks_ReturnsEmptyForUnknownRole));
        SeedData(context);
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(x => x.Role).Returns("Guest");
        currentUser.Setup(x => x.DepartmentId).Returns((int?)null);
        var service = new TaskItemService(context, currentUser.Object, MockAccessControl().Object);

        var result = (await service.GetTasks()).ToList();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetTasks_MapsProjectNameAndAssignedToName()
    {
        using var context = CreateContext(nameof(GetTasks_MapsProjectNameAndAssignedToName));
        SeedData(context);
        var service = new TaskItemService(context, MockCurrentUser().Object, MockAccessControl().Object);

        var result = (await service.GetTasks()).ToList();

        Assert.Equal("TestProject", result[0].ProjectName);
        Assert.NotNull(result[0].AssignedToName);
    }

    [Fact]
    public async Task CreateTask_CreatesAndReturnsMappedDto()
    {
        using var context = CreateContext(nameof(CreateTask_CreatesAndReturnsMappedDto));
        SeedData(context);
        var service = new TaskItemService(context, MockCurrentUser().Object, MockAccessControl().Object);

        var dto = new CreateTasksDto
        {
            Title = "New Task",
            Description = "Desc",
            Status = WorkStatus.Created,
            Priority = "Medium",
            ProjectId = 1,
            AssignedTo = 2,
        };

        var result = await service.CreateTask(dto);

        Assert.Equal("New Task", result.Title);
        Assert.Equal(3, context.Tasks.Count());
    }

    [Fact]
    public async Task CreateTask_ThrowsIfNotAllowed()
    {
        using var context = CreateContext(nameof(CreateTask_ThrowsIfNotAllowed));
        SeedData(context);
        var service = new TaskItemService(context, MockCurrentUser().Object, MockAccessControl(allow: false).Object);

        var dto = new CreateTasksDto { Title = "Denied", ProjectId = 1, Status = WorkStatus.Created };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.CreateTask(dto));
    }

    [Fact]
    public async Task CreateTask_ThrowsIfAssignedUserNotFound()
    {
        using var context = CreateContext(nameof(CreateTask_ThrowsIfAssignedUserNotFound));
        SeedData(context);
        var service = new TaskItemService(context, MockCurrentUser().Object, MockAccessControl().Object);

        var dto = new CreateTasksDto { Title = "Task", ProjectId = 1, Status = WorkStatus.Created, AssignedTo = 999 };

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.CreateTask(dto));
    }

    [Fact]
    public async Task UpdateTask_UpdatesFieldsAndReturnsTrue()
    {
        using var context = CreateContext(nameof(UpdateTask_UpdatesFieldsAndReturnsTrue));
        SeedData(context);
        var service = new TaskItemService(context, MockCurrentUser().Object, MockAccessControl().Object);

        var dto = new UpdateTaskDto { Title = "Updated", Description = "New desc", Status = WorkStatus.InProgress, Priority = "Low", AssignedTo = 1 };
        var result = await service.UpdateTask(1, dto);

        Assert.True(result);
        var task = await context.Tasks.FindAsync(1);
        Assert.Equal("Updated", task!.Title);
        Assert.Equal(WorkStatus.InProgress, task.Status);
    }

    [Fact]
    public async Task UpdateTask_ReturnsFalseIfNotFound()
    {
        using var context = CreateContext(nameof(UpdateTask_ReturnsFalseIfNotFound));
        var service = new TaskItemService(context, MockCurrentUser().Object, MockAccessControl().Object);

        var result = await service.UpdateTask(999, new UpdateTaskDto { Title = "X", Status = WorkStatus.Created });

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateTask_ThrowsIfNotAllowed()
    {
        using var context = CreateContext(nameof(UpdateTask_ThrowsIfNotAllowed));
        SeedData(context);
        var service = new TaskItemService(context, MockCurrentUser().Object, MockAccessControl(allow: false).Object);

        var dto = new UpdateTaskDto { Title = "Hack", Status = WorkStatus.Created };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.UpdateTask(1, dto));
    }

    [Fact]
    public async Task DeleteTask_RemovesAndReturnsTrue()
    {
        using var context = CreateContext(nameof(DeleteTask_RemovesAndReturnsTrue));
        SeedData(context);
        var service = new TaskItemService(context, MockCurrentUser().Object, MockAccessControl().Object);

        var result = await service.DeleteTask(1);

        Assert.True(result);
        Assert.Single(context.Tasks);
    }

    [Fact]
    public async Task DeleteTask_ReturnsFalseIfNotFound()
    {
        using var context = CreateContext(nameof(DeleteTask_ReturnsFalseIfNotFound));
        var service = new TaskItemService(context, MockCurrentUser().Object, MockAccessControl().Object);

        var result = await service.DeleteTask(999);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteTask_ThrowsIfNotAllowed()
    {
        using var context = CreateContext(nameof(DeleteTask_ThrowsIfNotAllowed));
        SeedData(context);
        var service = new TaskItemService(context, MockCurrentUser().Object, MockAccessControl(allow: false).Object);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.DeleteTask(1));
    }
}
