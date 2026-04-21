using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoutiquePortal.Model.ViewModels
{
    public class CustomerProfileVM
    {
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty; // read only

        [Required(ErrorMessage = "Phone is required")]
        public string Phone { get; set; } = string.Empty;

        // Password change (optional)
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Minimum 6 characters")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string? ConfirmNewPassword { get; set; }
    }
}
