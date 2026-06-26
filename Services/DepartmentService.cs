using EmployeeLeaveManagementSystem.Models;
using EmployeeLeaveManagementSystem.Repositories;

namespace EmployeeLeaveManagementSystem.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IRepository<Department> _departmentRepository;
        private readonly IRepository<Employee> _employeeRepository;

        public DepartmentService(IRepository<Department> departmentRepository, IRepository<Employee> employeeRepository)
        {
            _departmentRepository = departmentRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<List<Department>> GetAllAsync()
        {
            var departments = await _departmentRepository.GetAllAsync();
            var employees = await _employeeRepository.GetAllAsync();
            var employeesByDept = employees.GroupBy(e => e.DepartmentId).ToDictionary(g => g.Key, g => g.ToList());

            foreach (var department in departments)
                department.Employees = employeesByDept.TryGetValue(department.DepartmentId, out var list) ? list : new List<Employee>();

            return departments;
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null)
                return null;

            department.Employees = await _employeeRepository.FindAsync(e => e.DepartmentId == id);
            return department;
        }

        public async Task<ServiceResult> CreateAsync(Department department)
        {
            var existing = await _departmentRepository.FindAsync(d =>
                d.DepartmentName.ToLower() == department.DepartmentName.Trim().ToLower());
            if (existing.Count > 0)
                return ServiceResult.Failure("A department with this name already exists.");

            department.DepartmentName = department.DepartmentName.Trim();
            await _departmentRepository.AddAsync(department);
            await _departmentRepository.SaveChangesAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> UpdateAsync(Department department)
        {
            var existingDept = await _departmentRepository.GetByIdAsync(department.DepartmentId);
            if (existingDept == null)
                return ServiceResult.Failure("Department not found.");

            var duplicate = await _departmentRepository.FindAsync(d =>
                d.DepartmentId != department.DepartmentId &&
                d.DepartmentName.ToLower() == department.DepartmentName.Trim().ToLower());
            if (duplicate.Count > 0)
                return ServiceResult.Failure("A department with this name already exists.");

            existingDept.DepartmentName = department.DepartmentName.Trim();
            _departmentRepository.Update(existingDept);
            await _departmentRepository.SaveChangesAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null)
                return ServiceResult.Failure("Department not found.");

            var employeesInDept = await _employeeRepository.FindAsync(e => e.DepartmentId == id);
            if (employeesInDept.Count > 0)
                return ServiceResult.Failure(
                    $"Cannot delete '{department.DepartmentName}' — it still has {employeesInDept.Count} employee(s) assigned. Reassign or remove them first.");

            _departmentRepository.Remove(department);
            await _departmentRepository.SaveChangesAsync();
            return ServiceResult.Success();
        }
    }
}
