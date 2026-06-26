using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeLeaveManagementSystem.Models.ViewModels
{
    /// <summary>Used by Admin to create a brand-new employee (creates the Identity login too).</summary>
    public class EmployeeCreateViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Initial Password")]
        public string Password { get; set; } = string.Empty;

        public string? Designation { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Joining")]
        public DateTime DateOfJoining { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        public List<SelectListItem> DepartmentOptions { get; set; } = new();
    }

    /// <summary>Used by Admin to edit an existing employee's profile (no password / login change here).</summary>
    public class EmployeeEditViewModel
    {
        public int EmployeeId { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Designation { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Joining")]
        public DateTime DateOfJoining { get; set; }

        [Required]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        public List<SelectListItem> DepartmentOptions { get; set; } = new();
    }
}
