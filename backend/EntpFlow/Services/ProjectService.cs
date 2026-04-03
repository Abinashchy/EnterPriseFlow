using EntpFlow.Data;
using EntpFlow.DTOs.Projects;
using EntpFlow.Interfaces;
using EntpFlow.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EntpFlow.Services
{
    public class ProjectService : IProjectService
    {

        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IAccessControlService _accessControl;

        public ProjectService(ApplicationDbContext context, ICurrentUserService currentUser, IAccessControlService accessControl)
        {
            _context = context;
            _currentUser = currentUser;
            _accessControl = accessControl;
        }

        public async Task<IEnumerable<GetProjectDto>> GetProjects()
        {
            IQueryable<Project> query = _context.Projects
            .Include(p => p.Department)
            .Include(p => p.CreatedByUser);

        if (_currentUser.Role == "Admin")
        {
            return await query
                .Select(MapProject())
                .ToListAsync();
        }

        if (_currentUser.Role == "Manager" && _currentUser.DepartmentId.HasValue)
        {
            var departmentId = _currentUser.DepartmentId.Value;

            return await query
                .Where(p => p.DepartmentId == departmentId)
                .Select(MapProject())
                .ToListAsync();
        }
        return Enumerable.Empty<GetProjectDto>();
            
        }

        public async Task<GetProjectDto?> GetProjectById(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Department)
                .Include(p => p.CreatedByUser)
                .Where(p => p.Id == id)
                .Select(MapProject())
                .FirstOrDefaultAsync();

            if (project == null)
                return null;

            if (_accessControl.CanManageDepartment(project.DepartmentId))
                return project;

            return null; 
        }

        public async Task<GetProjectDto> CreateProject(CreateProjectDto dto)
        {
            if (!_currentUser.UserId.HasValue)
                throw new UnauthorizedAccessException("User not authenticated.");

            if (!_accessControl.CanManageDepartment(dto.DepartmentId))
                throw new UnauthorizedAccessException("You cannot create a project in this department.");

            var project = new Project
            {
                Name = dto.Name,
                Description = dto.Description,
                DepartmentId = dto.DepartmentId,
                CreatedBy = _currentUser.UserId.Value,
                CreatedAt = DateTime.UtcNow
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            if (_currentUser.Role == "Manager")
            {
                var alreadyMember = await _context.ProjectMembers.AnyAsync(pm =>
                    pm.ProjectId == project.Id && pm.UserId == _currentUser.UserId.Value);

                if (!alreadyMember)
                {
                    _context.ProjectMembers.Add(new ProjectMember
                    {
                        ProjectId = project.Id,
                        UserId = _currentUser.UserId.Value
                    });

                    await _context.SaveChangesAsync();
                }
            }

            var createdProject = await _context.Projects
                .Include(p => p.Department)
                .Include(p => p.CreatedByUser)
                .Where(p => p.Id == project.Id)
                .Select(MapProject())
                .FirstAsync();

            return createdProject;
        }

        public async Task<bool> DeleteProject(int id)
        {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
            return false;

        var canManage = await _accessControl.CanManageProjectAsync(id);
        if (!canManage)
            throw new UnauthorizedAccessException("You cannot delete this project.");

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

        return true;
        }

        public async Task<bool> UpdateProject(int id, UpdateProjectDto dto)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return false;

            var canManageExistingProject = await _accessControl.CanManageProjectAsync(id);
            if (!canManageExistingProject)
                throw new UnauthorizedAccessException("You cannot update this project.");

            if (!_accessControl.CanManageDepartment(dto.DepartmentId))
                throw new UnauthorizedAccessException("You cannot assign this project to that department.");

            project.Name = dto.Name;
            project.Description = dto.Description;
            project.DepartmentId = dto.DepartmentId;

            await _context.SaveChangesAsync();
            return true;
      
        }

        private static Expression<Func<Project, GetProjectDto>> MapProject()
        {
            return p => new GetProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                DepartmentId = p.DepartmentId,
                DepartmentName = p.Department != null ? p.Department.Name : null,
                CreatedBy = p.CreatedBy,
                CreatedByName = p.CreatedByUser != null ? p.CreatedByUser.Name : null,
                CreatedAt = p.CreatedAt
            };
        }

    }
}
