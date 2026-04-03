namespace EntpFlow.Interfaces;

public interface IAccessControlService
{
    bool IsAdmin();
    bool IsManager();
    bool CanManageDepartment(int? targetDepartmentId);

    Task<bool> CanManageProjectAsync(int projectId);
    Task<bool> CanManageProjectMemberAsync(int projectMemberId);
    Task<bool> CanManageTaskAsync(int taskId);
}