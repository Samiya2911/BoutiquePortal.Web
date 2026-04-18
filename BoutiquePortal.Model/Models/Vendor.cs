using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;

namespace BoutiquePortal.Model.Models
{
    public class Vendor
    {
        public int VendorId { get; set; }

        [Required(ErrorMessage = "Vendor name is required")]
        [StringLength(150)]
        public string VendorName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(300)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Brand name is required")]
        [StringLength(150)]
        public string BrandName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required")]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;   // UNIQUE enforced in DB + SP

        [StringLength(500)]
        public string? Address { get; set; }

        // 🔹 LOCATION RELATIONS
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }

        // 🔹 DISPLAY PURPOSE (JOIN DATA)
        public string? CountryName { get; set; }
        public string? StateName { get; set; }
        public string? CityName { get; set; }

        // 🔹 STATUS
        public bool IsApproved { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
