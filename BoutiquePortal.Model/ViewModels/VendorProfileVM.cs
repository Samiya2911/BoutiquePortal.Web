using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoutiquePortal.Model.ViewModels
{
    public class VendorProfileVM
    {
        public int VendorId { get; set; }

        [Required]
        public string BusinessName { get; set; }

        [Required]
        public string OwnerName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        public string Address { get; set; }

        public string ExistingLogoPath { get; set; }
        public IFormFile LogoFile { get; set; }

        // Password change (optional)
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [DataType(DataType.Password), MinLength(6)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmNewPassword { get; set; }
    }
}
