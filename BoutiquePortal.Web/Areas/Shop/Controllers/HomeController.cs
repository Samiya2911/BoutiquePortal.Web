using BoutiquePortal.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BoutiquePortal.Web.Areas.Shop.Controllers
{
    [Area("Shop")]
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public HomeController(
            IProductService productService,
            ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        // ======= HOMEPAGE =======
        public async Task<IActionResult> Index()
        {
            // Latest 6 products for homepage
            var products = await _productService.GetForShopAsync(null, null);
            var categories = await _categoryService.GetAllAsync();

            ViewBag.Categories = categories.ToList();
            ViewBag.NewReleased = products.Take(6).ToList();
            ViewBag.Featured = products.Take(4).ToList();

            return View();
        }

        // ======= MY ORDERS (customer) =======
        public IActionResult MyOrders()
        {
            return RedirectToAction("Index", "Orders",
                new { area = "Customer" });
        }
    }
}