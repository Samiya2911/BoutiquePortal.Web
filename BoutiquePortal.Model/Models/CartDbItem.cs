using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoutiquePortal.Model.Models
{
    public class CartDbItem
    {
        public int CartId { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedDate { get; set; }

        // JOIN fields from Product
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string? ProductImage { get; set; }
        public string? BrandName { get; set; }
        public int VendorId { get; set; }
        public int StockQuantity { get; set; }

        // Calculated
        public decimal ActualPrice =>
            DiscountPrice.HasValue && DiscountPrice > 0
            ? DiscountPrice.Value : Price;

        public decimal SubTotal => ActualPrice * Quantity;
    }
}