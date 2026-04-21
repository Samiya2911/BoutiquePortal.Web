using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BoutiquePortal.Model.ViewModels
{
    public class CheckoutVM
    {
        // ======= SHIPPING INFO =======
        [Required(ErrorMessage = "Full name is required")]
        public string ShippingName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required")]
        public string ShippingPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        public string ShippingCity { get; set; } = string.Empty;

        [Required(ErrorMessage = "State is required")]
        public string ShippingState { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pincode is required")]
        public string ShippingPincode { get; set; } = string.Empty;

        // ======= PAYMENT INFO =======
        [Required(ErrorMessage = "Please select payment method")]
        public string PaymentMethod { get; set; } = "COD";

        // UPI details (shown only if UPI selected)
        public string? UpiId { get; set; }

        // Card details (shown only if Card selected)
        public string? CardNumber { get; set; }
        public string? CardExpiry { get; set; }
        public string? CardCvv { get; set; }
        public string? CardName { get; set; }

        // ======= ORDER SUMMARY (filled from cart in controller) =======
        public decimal SubTotal { get; set; }
        public decimal DeliveryCharge { get; set; } = 100m;
        public decimal GrandTotal => SubTotal + DeliveryCharge;
    }
}