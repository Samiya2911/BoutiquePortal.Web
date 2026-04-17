using BoutiquePortal.Model.Models;
using BoutiquePortal.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BoutiquePortal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _service;
        private readonly ICategoryService _categoryService;
        private readonly ISubCategoryService _subCategoryService;
        private readonly IVendorService _vendorService;

        public ProductController(
            IProductService service,
            ICategoryService categoryService,
            ISubCategoryService subCategoryService,
            IVendorService vendorService)
        {
            _service = service;
            _categoryService = categoryService;
            _subCategoryService = subCategoryService;
            _vendorService = vendorService;
        }

        // ================== INDEX ==================
        public async Task<IActionResult> Index()
        {
            var list = await _service.GetAllAsync();
            return View(list);
        }

        // ================== ADD EDIT (GET) ==================
        [HttpGet]
        public async Task<IActionResult> AddEdit(int? id)
        {
            // Dropdowns
            ViewBag.Categories = (await _categoryService.GetAllAsync()).ToList();
            ViewBag.Vendors = (await _vendorService.GetAllAsync()).ToList();

            if (id.HasValue && id.Value > 0)
            {
                var existing = await _service.GetByIdAsync(id.Value);

                // Load subcategory based on selected category (EDIT MODE)
                if (existing != null && existing.CategoryId > 0)
                {
                    ViewBag.SubCategories = (await _subCategoryService
                        .GetByCategoryId(existing.CategoryId)).ToList();
                }

                return View(existing ?? new Product());
            }

            return View(new Product { IsActive = true });
        }

        // ================== ADD EDIT (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(Product model)
        {
            // Manual binding (same like your Category/City)
            model.ProductName = Request.Form["ProductName"].ToString().Trim();
           // model.ProductCode = Request.Form["ProductCode"].ToString();
            model.Description = Request.Form["Description"].ToString();

            model.CategoryId = int.TryParse(Request.Form["CategoryId"], out int cid) ? cid : 0;
            model.SubCategoryId = int.TryParse(Request.Form["SubCategoryId"], out int sid) ? sid : 0;
            model.VendorId = int.TryParse(Request.Form["VendorId"], out int vid) ? vid : 0;

            model.Price = decimal.TryParse(Request.Form["Price"], out decimal price) ? price : 0;
            model.Quantity = int.TryParse(Request.Form["Quantity"], out int qty) ? qty : 0;

            model.ProductId = int.TryParse(Request.Form["ProductId"], out int pid) ? pid : 0;
            model.IsActive = Request.Form["IsActive"].ToString().Contains("true");

            ModelState.Clear();

            // ================== VALIDATION ==================
            if (string.IsNullOrWhiteSpace(model.ProductName))
                ModelState.AddModelError("ProductName", "Product name is required");

            if (model.CategoryId == 0)
                ModelState.AddModelError("CategoryId", "Category is required");

            if (model.SubCategoryId == 0)
                ModelState.AddModelError("SubCategoryId", "SubCategory is required");

            if (model.VendorId == 0)
                ModelState.AddModelError("VendorId", "Vendor is required");

            if (model.Price <= 0)
                ModelState.AddModelError("Price", "Price must be greater than 0");

            if (model.Quantity < 0)
                ModelState.AddModelError("Quantity", "Quantity invalid");

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = (await _categoryService.GetAllAsync()).ToList();
                ViewBag.Vendors = (await _vendorService.GetAllAsync()).ToList();
                ViewBag.SubCategories = (await _subCategoryService
                    .GetByCategoryId(model.CategoryId)).ToList();

                return View(model);
            }

            // ================== SAVE ==================
            if (model.ProductId == 0)
                await _service.AddAsync(model);
            else
                await _service.UpdateAsync(model);

            return RedirectToAction(nameof(Index));
        }

        // ================== DELETE ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok();
        }

        // ================== AJAX ==================

        // 🔥 CATEGORY → SUBCATEGORY
        public async Task<JsonResult> GetSubCategoryByCategory(int categoryId)
        {
            var data = await _subCategoryService.GetByCategoryId(categoryId);
            return Json(data);
        }
    }
}