using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoutiquePortal.Model.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; } = "Pending";

        // Shipping
        public string? ShippingName { get; set; }
        public string? ShippingPhone { get; set; }
        public string? ShippingAddress { get; set; }
        public string? ShippingCity { get; set; }
        public string? ShippingState { get; set; }
        public string? ShippingPincode { get; set; }

        // Payment
        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }
        public string? TransactionId { get; set; }

        public DateTime CreatedDate { get; set; }

        // JOIN display fields
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }

        // Order items (for detail view)
        public List<OrderItem> Items { get; set; } = new();
    }

    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int VendorId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        // JOIN display fields
        public string? ProductName { get; set; }
        public string? ImagePath { get; set; }
        public string? VendorName { get; set; }
        public string? BrandName { get; set; }
    }
}