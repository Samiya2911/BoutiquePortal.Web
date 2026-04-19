using BoutiquePortal.Model.Models;
using BoutiquePortal.Services.Interfaces;
using BoutiquePortal.Web.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace BoutiquePortal.Web.Areas.Vendor.Controllers
{
    [Area("Vendor")]
    [VendorAuthFilter]
    public class ProductController : Controller
    {
        private readonly IProductVendorService _service;
        private readonly ICategoryService _categoryService;
        private readonly ISubCategoryService _subCategoryService;
        private readonly IWebHostEnvironment _env;

        public ProductController(
            IProductVendorService service,
            ICategoryService categoryService,
            ISubCategoryService subCategoryService,
            IWebHostEnvironment env)
        {
            _service = service;
            _categoryService = categoryService;
            _subCategoryService = subCategoryService;
            _env = env;
        }

        // ================== INDEX ==================
        public async Task<IActionResult> Index()
        {
            int vendorId = HttpContext.Session.GetInt32("VendorId") ?? 0;
            var list = await _service.GetByVendorAsync(vendorId);
            return View(list);
        }

        // ================== ADD EDIT (GET) ==================
        [HttpGet]
        public async Task<IActionResult> AddEdit(int? id)
        {
            int vendorId = HttpContext.Session.GetInt32("VendorId") ?? 0;

            ViewBag.Categories = (await _categoryService.GetAllAsync()).ToList();

            if (id.HasValue && id.Value > 0)
            {
                var existing = await _service.GetByIdForVendorAsync(id.Value, vendorId);

                // security: vendor trying to edit someone else's product
                if (existing == null)
                {
                    TempData["Error"] = "Product not found or access denied.";
                    return RedirectToAction(nameof(Index));
                }

                // Load subcategories for selected category (edit mode)
                if (existing.CategoryId > 0)
                {
                    ViewBag.SubCategories = (await _subCategoryService
                        .GetByCategoryId(existing.CategoryId)).ToList();
                }

                return View(existing);
            }

            return View(new Product { IsActive = true });
        }

        // ================== ADD EDIT (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(Product model)
        {
            // ================== SESSION ==================
            int vendorId = HttpContext.Session.GetInt32("VendorId") ?? 0;
            string brandName = HttpContext.Session.GetString("BrandName") ?? string.Empty;

            // ================== MANUAL BINDING (same pattern as Admin) ==================
            model.ProductId = int.TryParse(Request.Form["ProductId"], out int pid) ? pid : 0;
            model.ProductName = Request.Form["ProductName"].ToString().Trim();
            model.Description = Request.Form["Description"].ToString();
            model.ProductImage = Request.Form["ProductImage"].ToString();   // existing image
            model.CategoryId = int.TryParse(Request.Form["CategoryId"], out int cid) ? cid : 0;
            model.SubCategoryId = int.TryParse(Request.Form["SubCategoryId"], out int scid) && scid > 0
                                    ? scid : (int?)null;
            model.Quantity = int.TryParse(Request.Form["Quantity"], out int qty) ? qty : 0;
            model.IsActive = Request.Form["IsActive"].ToString().Contains("true");

            // VendorId & BrandName always from session — vendor cannot change these
            model.VendorId = vendorId;
            model.BrandName = brandName;

            decimal.TryParse(Request.Form["Price"], out decimal price);
            decimal.TryParse(Request.Form["DiscountPrice"], out decimal discPrice);
            model.Price = price;
            model.DiscountPrice = discPrice > 0 ? discPrice : (decimal?)null;

            // ================== VALIDATION ==================
            ModelState.Clear();

            if (string.IsNullOrWhiteSpace(model.ProductName))
                ModelState.AddModelError("ProductName", "Product name is required");

            if (model.CategoryId == 0)
                ModelState.AddModelError("CategoryId", "Category is required");

            if (model.SubCategoryId == null)
                ModelState.AddModelError("SubCategoryId", "Sub category is required");

            if (model.Price <= 0)
                ModelState.AddModelError("Price", "Price must be greater than 0");

            if (model.Quantity < 0)
                ModelState.AddModelError("Quantity", "Quantity invalid");

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = (await _categoryService.GetAllAsync()).ToList();
                ViewBag.SubCategories = (await _subCategoryService
                    .GetByCategoryId(model.CategoryId)).ToList();
                return View(model);
            }

            // ================== IMAGE UPLOAD (same as Admin) ==================
            var imageFile = Request.Form.Files["ImageFile"];
            if (imageFile != null && imageFile.Length > 0)
            {
                string folder = Path.Combine(_env.WebRootPath, "images", "product");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                using (var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // Delete old image if updating
                if (model.ProductId > 0 && !string.IsNullOrEmpty(model.ProductImage))
                {
                    var oldPath = Path.Combine(_env.WebRootPath, "images", "product", model.ProductImage);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                model.ProductImage = fileName;
            }

            // ================== SAVE ==================
            if (model.ProductId == 0)
            {
                model.CreatedDate = DateTime.Now;
                await _service.AddAsync(model);
                TempData["Success"] = "Product added successfully!";
            }
            else
            {
                await _service.UpdateAsync(model);
                TempData["Success"] = "Product updated successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        // ================== DELETE ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            int vendorId = HttpContext.Session.GetInt32("VendorId") ?? 0;
            await _service.DeleteAsync(id, vendorId);  // security: only own products
            return Ok();
        }

        // ================== AJAX: SubCategories by Category ==================
        public async Task<JsonResult> GetSubCategoriesByCategory(int categoryId)
        {
            var data = await _subCategoryService.GetByCategoryId(categoryId);

            var result = data.Select(sc => new
            {
                subCategoryId = sc.SubCategoryId,
                subCategoryName = sc.SubCategoryName
            });

            return Json(result);
        }
    }
}