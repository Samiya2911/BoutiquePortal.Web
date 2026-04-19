using BoutiquePortal.Services.Interfaces;
using BoutiquePortal.Web.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BoutiquePortal.Web.Areas.Vendor.Controllers
{
    [Area("Vendor")]
    [VendorAuthFilter]
    public class EarningsController : Controller
    {
        private readonly IOrderService _orderService;

        public EarningsController(IOrderService orderService)
            => _orderService = orderService;

        public async Task<IActionResult> Index()
        {
            int vendorId = HttpContext.Session.GetInt32("VendorId") ?? 0;
            var earnings = await _orderService.GetEarningsAsync(vendorId);
            return View(earnings);
        }
    }
}