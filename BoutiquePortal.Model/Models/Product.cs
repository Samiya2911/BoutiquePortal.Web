using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace BoutiquePortal.Model.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        // 🔹 BASIC INFO
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(150)]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        public decimal Price { get; set; }

        public decimal? DiscountPrice { get; set; }

        public string? Description { get; set; }

        // 🔹 CATEGORY RELATION
        public int CategoryId { get; set; }
        public int? SubCategoryId { get; set; }

        // 🔹 VENDOR RELATION (IMPORTANT)
        public int VendorId { get; set; }

        // 🔹 BRAND (AUTO FROM VENDOR)
        public string? BrandName { get; set; }

        // 🔹 STOCK
        public int Quantity { get; set; }

        // 🔹 IMAGE
        public string? ProductImage { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        // 🔹 STATUS
        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // 🔹 DISPLAY PURPOSE (JOIN DATA)
        public string? CategoryName { get; set; }
        public string? SubCategoryName { get; set; }
        public string? VendorName { get; set; }
    }
}