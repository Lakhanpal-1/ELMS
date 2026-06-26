using EmployeeLeaveManagementSystem.Models;
using EmployeeLeaveManagementSystem.Repositories;

namespace EmployeeLeaveManagementSystem.Services
{
    public class LeaveTypeService : ILeaveTypeService
    {
        private readonly IRepository<LeaveType> _leaveTypeRepository;
        private readonly IRepository<LeaveRequest> _leaveRequestRepository;

        public LeaveTypeService(IRepository<LeaveType> leaveTypeRepository, IRepository<LeaveRequest> leaveRequestRepository)
        {
            _leaveTypeRepository = leaveTypeRepository;
            _leaveRequestRepository = leaveRequestRepository;
        }

        public async Task<List<LeaveType>> GetAllAsync() => await _leaveTypeRepository.GetAllAsync();

        public async Task<LeaveType?> GetByIdAsync(int id) => await _leaveTypeRepository.GetByIdAsync(id);

        public async Task<ServiceResult> CreateAsync(LeaveType leaveType)
        {
            if (leaveType.DefaultDaysPerYear < 0)
                return ServiceResult.Failure("Default days per year cannot be negative.");

            var existing = await _leaveTypeRepository.FindAsync(t =>
                t.Name.ToLower() == leaveType.Name.Trim().ToLower());
            if (existing.Count > 0)
                return ServiceResult.Failure("A leave type with this name already exists.");

            leaveType.Name = leaveType.Name.Trim();
            await _leaveTypeRepository.AddAsync(leaveType);
            await _leaveTypeRepository.SaveChangesAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> UpdateAsync(LeaveType leaveType)
        {
            if (leaveType.DefaultDaysPerYear < 0)
                return ServiceResult.Failure("Default days per year cannot be negative.");

            var existingType = await _leaveTypeRepository.GetByIdAsync(leaveType.LeaveTypeId);
            if (existingType == null)
                return ServiceResult.Failure("Leave type not found.");

            var duplicate = await _leaveTypeRepository.FindAsync(t =>
                t.LeaveTypeId != leaveType.LeaveTypeId &&
                t.Name.ToLower() == leaveType.Name.Trim().ToLower());
            if (duplicate.Count > 0)
                return ServiceResult.Failure("A leave type with this name already exists.");

            existingType.Name = leaveType.Name.Trim();
            existingType.DefaultDaysPerYear = leaveType.DefaultDaysPerYear;
            _leaveTypeRepository.Update(existingType);
            await _leaveTypeRepository.SaveChangesAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var leaveType = await _leaveTypeRepository.GetByIdAsync(id);
            if (leaveType == null)
                return ServiceResult.Failure("Leave type not found.");

            var requestsUsingType = await _leaveRequestRepository.FindAsync(r => r.LeaveTypeId == id);
            if (requestsUsingType.Count > 0)
                return ServiceResult.Failure(
                    $"Cannot delete '{leaveType.Name}' — it is referenced by {requestsUsingType.Count} leave request(s).");

            _leaveTypeRepository.Remove(leaveType);
            await _leaveTypeRepository.SaveChangesAsync();
            return ServiceResult.Success();
        }
    }
}
