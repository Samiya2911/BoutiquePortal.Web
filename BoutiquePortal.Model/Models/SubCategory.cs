using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoutiquePortal.Model.Models
{
    public class SubCategory
    {
        public int SubCategoryId { get; set; }

        [Required(ErrorMessage = "SubCategory name is required")]
        public string SubCategoryName { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public string? SubCategoryImage { get; set; }


        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
