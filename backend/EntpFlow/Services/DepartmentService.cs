using EntpFlow.Data;
using EntpFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace EntpFlow.Services
{
    public class DepartmentService
    {

        private readonly ApplicationDbContext _context;

        public DepartmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Department>> GetDepts()
        {
            return await _context.Departments.ToListAsync();
        }

        public async Task<Department?> GetDeptById(int id)
        {
            return await _context.Departments.FindAsync(id);
        }

        public async Task CreateDept(Department department)
        {
            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDept(int id)
        {
            var dept = await _context.Departments.FindAsync(id);

            if (dept == null) return;

            _context.Departments.Remove(dept);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateDept(Department dept)
        {
            _context.Entry(dept).State = EntityState.Modified;

            await _context.SaveChangesAsync();
      
        }



    }
}
