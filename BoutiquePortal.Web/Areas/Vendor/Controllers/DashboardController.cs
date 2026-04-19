using BoutiquePortal.Services.Interfaces;
using BoutiquePortal.Web.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BoutiquePortal.Web.Areas.Vendor.Controllers
{
    [Area("Vendor")]
    [VendorAuthFilter]
    public class DashboardController : Controller
    {
        private readonly IProductVendorService _productService;

        public DashboardController(IProductVendorService productService)
            => _productService = productService;

        public async Task<IActionResult> Index()
        {
            int vendorId = HttpContext.Session.GetInt32("VendorId") ?? 0;
            var products = await _productService.GetByVendorAsync(vendorId);

            ViewBag.TotalProducts = products.Count();
            ViewBag.ActiveProducts = products.Count(p => p.IsActive);
            ViewBag.VendorName = HttpContext.Session.GetString("VendorName");
            ViewBag.BrandName = HttpContext.Session.GetString("BrandName");

            return View();
        }
    }
}