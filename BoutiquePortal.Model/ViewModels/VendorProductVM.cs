using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoutiquePortal.Model.ViewModels
{
    public class VendorProductVM
    {
        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; }

        public string Description { get; set; }

        [Required, Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        public int Stock { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public string ExistingImagePath { get; set; }
        public IFormFile ImageFile { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
