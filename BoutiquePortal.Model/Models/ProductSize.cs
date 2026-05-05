using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoutiquePortal.Model.Models
{
    public class ProductSize
    {
        public int SizeId { get; set; }
        public int ProductId { get; set; }
        public string SizeName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
