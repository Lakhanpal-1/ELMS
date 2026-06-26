using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeLeaveManagementSystem.Models.ViewModels
{
    public class LeaveRequestApplyViewModel
    {
        [Required]
        [Display(Name = "Leave Type")]
        public int LeaveTypeId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; } = DateTime.Today;

        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        public List<SelectListItem> LeaveTypeOptions { get; set; } = new();

        /// <summary>Remaining balance per leave type for this year, used by the page's JS to show a live preview.</summary>
        public Dictionary<int, int> RemainingDaysByLeaveType { get; set; } = new();
    }

    public class LeaveDecisionViewModel
    {
        public int LeaveRequestId { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }
    }
}
