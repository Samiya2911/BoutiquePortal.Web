using BoutiquePortal.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BoutiquePortal.Web.Areas.Shop.Controllers
{
    [Area("Shop")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IProductSizeService _sizeService;

        public ProductController(
            IProductService productService,
            ICategoryService categoryService,
            IProductSizeService sizeService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _sizeService = sizeService;
        }

        // ======= SINGLE PRODUCT DETAIL =======
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetDetailAsync(id);
            var categories = await _categoryService.GetAllAsync();

            if (product == null) return NotFound();

            ViewBag.Categories = categories.ToList();

            ViewBag.ProductSizes = await _sizeService.GetByProductAsync(id);

            return View(product);
        }
    }
}