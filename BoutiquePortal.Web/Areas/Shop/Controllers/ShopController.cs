using BoutiquePortal.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BoutiquePortal.Web.Areas.Shop.Controllers
{
    [Area("Shop")]
    public class ShopController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ISubCategoryService _subCategoryService;

        public ShopController(
            IProductService productService,
            ICategoryService categoryService,
            ISubCategoryService subCategoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _subCategoryService = subCategoryService;
        }

        // ======= PRODUCT LISTING =======
        public async Task<IActionResult> Index(int? categoryId, string? q)
        {
            var products = await _productService.GetForShopAsync(categoryId, q);
            var categories = await _categoryService.GetAllAsync();

            ViewBag.Categories = categories.ToList();
            ViewBag.SelectedCategory = categoryId;
            ViewBag.SearchQuery = q;

            return View(products);
        }
    }
}