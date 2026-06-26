using EmployeeLeaveManagementSystem.Models;
using EmployeeLeaveManagementSystem.Models.ViewModels;
using EmployeeLeaveManagementSystem.Repositories;

namespace EmployeeLeaveManagementSystem.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly ILeaveBalanceService _leaveBalanceService;

        public LeaveRequestService(ILeaveRequestRepository leaveRequestRepository, ILeaveBalanceService leaveBalanceService)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _leaveBalanceService = leaveBalanceService;
        }

        public async Task<LeaveRequest?> GetByIdAsync(int id) => await _leaveRequestRepository.GetWithDetailsAsync(id);

        public async Task<List<LeaveRequest>> GetByEmployeeIdAsync(int employeeId) =>
            await _leaveRequestRepository.GetByEmployeeIdAsync(employeeId);

        public async Task<List<LeaveRequest>> GetPendingAsync() => await _leaveRequestRepository.GetPendingAsync();

        public async Task<List<LeaveRequest>> GetAllAsync() => await _leaveRequestRepository.GetAllWithDetailsAsync();

        public async Task<ServiceResult> ApplyAsync(int employeeId, LeaveRequestApplyViewModel model)
        {
            var startDate = model.StartDate.Date;
            var endDate = model.EndDate.Date;

            if (endDate < startDate)
                return ServiceResult.Failure("End date cannot be before the start date.");

            var numberOfDays = (endDate - startDate).Days + 1;

            var overlapping = await _leaveRequestRepository.HasOverlappingRequestAsync(employeeId, startDate, endDate);
            if (overlapping)
                return ServiceResult.Failure("You already have a leave request that overlaps these dates.");

            var remainingDays = await _leaveBalanceService.GetRemainingDaysAsync(employeeId, model.LeaveTypeId, startDate.Year);
            if (numberOfDays > remainingDays)
                return ServiceResult.Failure(
                    $"Insufficient leave balance — you only have {remainingDays} day(s) remaining for this leave type in {startDate.Year}.");

            var leaveRequest = new LeaveRequest
            {
                EmployeeId = employeeId,
                LeaveTypeId = model.LeaveTypeId,
                StartDate = startDate,
                EndDate = endDate,
                NumberOfDays = numberOfDays,
                Reason = model.Reason.Trim(),
                Status = "Pending",
                AppliedDate = DateTime.UtcNow
            };

            await _leaveRequestRepository.AddAsync(leaveRequest);
            await _leaveRequestRepository.SaveChangesAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> ApproveAsync(int leaveRequestId, string approvedByUserId, string? remarks)
        {
            var leaveRequest = await _leaveRequestRepository.GetByIdAsync(leaveRequestId);
            if (leaveRequest == null)
                return ServiceResult.Failure("Leave request not found.");

            if (leaveRequest.Status != "Pending")
                return ServiceResult.Failure("This request has already been processed.");

            var remainingDays = await _leaveBalanceService.GetRemainingDaysAsync(
                leaveRequest.EmployeeId, leaveRequest.LeaveTypeId, leaveRequest.StartDate.Year);
            if (leaveRequest.NumberOfDays > remainingDays)
                return ServiceResult.Failure("Cannot approve — the employee no longer has enough remaining balance.");

            leaveRequest.Status = "Approved";
            leaveRequest.ApprovedById = approvedByUserId;
            leaveRequest.ApprovedDate = DateTime.UtcNow;
            leaveRequest.Remarks = remarks;

            await _leaveBalanceService.ApplyUsageAsync(
                leaveRequest.EmployeeId, leaveRequest.LeaveTypeId, leaveRequest.StartDate.Year, leaveRequest.NumberOfDays);

            _leaveRequestRepository.Update(leaveRequest);
            await _leaveRequestRepository.SaveChangesAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> RejectAsync(int leaveRequestId, string approvedByUserId, string? remarks)
        {
            var leaveRequest = await _leaveRequestRepository.GetByIdAsync(leaveRequestId);
            if (leaveRequest == null)
                return ServiceResult.Failure("Leave request not found.");

            if (leaveRequest.Status != "Pending")
                return ServiceResult.Failure("This request has already been processed.");

            leaveRequest.Status = "Rejected";
            leaveRequest.ApprovedById = approvedByUserId;
            leaveRequest.ApprovedDate = DateTime.UtcNow;
            leaveRequest.Remarks = remarks;

            _leaveRequestRepository.Update(leaveRequest);
            await _leaveRequestRepository.SaveChangesAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> CancelAsync(int leaveRequestId, int employeeId)
        {
            var leaveRequest = await _leaveRequestRepository.GetByIdAsync(leaveRequestId);
            if (leaveRequest == null)
                return ServiceResult.Failure("Leave request not found.");

            if (leaveRequest.EmployeeId != employeeId)
                return ServiceResult.Failure("You can only cancel your own leave requests.");

            if (leaveRequest.Status != "Pending")
                return ServiceResult.Failure("Only pending requests can be cancelled.");

            _leaveRequestRepository.Remove(leaveRequest);
            await _leaveRequestRepository.SaveChangesAsync();
            return ServiceResult.Success();
        }
    }
}
