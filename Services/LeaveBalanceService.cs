using EmployeeLeaveManagementSystem.Models;
using EmployeeLeaveManagementSystem.Repositories;

namespace EmployeeLeaveManagementSystem.Services
{
    public class LeaveBalanceService : ILeaveBalanceService
    {
        private readonly ILeaveBalanceRepository _leaveBalanceRepository;
        private readonly IRepository<LeaveType> _leaveTypeRepository;

        public LeaveBalanceService(ILeaveBalanceRepository leaveBalanceRepository, IRepository<LeaveType> leaveTypeRepository)
        {
            _leaveBalanceRepository = leaveBalanceRepository;
            _leaveTypeRepository = leaveTypeRepository;
        }

        public async Task<LeaveBalance> GetOrCreateBalanceAsync(int employeeId, int leaveTypeId, int year)
        {
            var balance = await _leaveBalanceRepository.GetAsync(employeeId, leaveTypeId, year);
            if (balance != null)
                return balance;

            var leaveType = await _leaveTypeRepository.GetByIdAsync(leaveTypeId);
            var totalDays = leaveType?.DefaultDaysPerYear ?? 0;

            balance = new LeaveBalance
            {
                EmployeeId = employeeId,
                LeaveTypeId = leaveTypeId,
                Year = year,
                TotalDays = totalDays,
                UsedDays = 0,
                RemainingDays = totalDays
            };

            await _leaveBalanceRepository.AddAsync(balance);
            await _leaveBalanceRepository.SaveChangesAsync();
            return balance;
        }

        public async Task<List<LeaveBalance>> GetBalancesForEmployeeAsync(int employeeId, int year) =>
            await _leaveBalanceRepository.GetByEmployeeIdAsync(employeeId, year);

        public async Task<int> GetRemainingDaysAsync(int employeeId, int leaveTypeId, int year)
        {
            var balance = await GetOrCreateBalanceAsync(employeeId, leaveTypeId, year);
            return balance.RemainingDays;
        }

        public async Task ApplyUsageAsync(int employeeId, int leaveTypeId, int year, int days)
        {
            var balance = await GetOrCreateBalanceAsync(employeeId, leaveTypeId, year);
            balance.UsedDays += days;
            balance.RemainingDays = balance.TotalDays - balance.UsedDays;
            _leaveBalanceRepository.Update(balance);
        }

        public async Task ReleaseUsageAsync(int employeeId, int leaveTypeId, int year, int days)
        {
            var balance = await GetOrCreateBalanceAsync(employeeId, leaveTypeId, year);
            balance.UsedDays = Math.Max(0, balance.UsedDays - days);
            balance.RemainingDays = balance.TotalDays - balance.UsedDays;
            _leaveBalanceRepository.Update(balance);
        }

        public async Task SaveChangesAsync() => await _leaveBalanceRepository.SaveChangesAsync();
    }
}
