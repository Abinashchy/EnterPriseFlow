using EntpFlow.Data;
using EntpFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace EntpFlow.Services
{
    public class ProjectService
    {

        private readonly ApplicationDbContext _context;

        public ProjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Project>> GetProjects()
        {
            return await _context.Projects.ToListAsync();
        }

        public async Task<Project?> GetProjectById(int id)
        {
            return await _context.Projects.FindAsync(id);
        }

        public async Task CreateProject(Project project)
        {
            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null) return;

            _context.Projects.Remove(project);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateProject(Project project)
        {
            _context.Entry(project).State = EntityState.Modified;

            await _context.SaveChangesAsync();
      
        }



    }
}
