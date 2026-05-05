using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoutiquePortal.Model.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int Quantity { get; set; } = 1;
        public int VendorId { get; set; }
        public string? SelectedSize { get; set; }

        // Calculated property
        public decimal ActualPrice =>
            DiscountPrice.HasValue && DiscountPrice > 0
            ? DiscountPrice.Value
            : Price;

        public decimal SubTotal => ActualPrice * Quantity;
    }
}
