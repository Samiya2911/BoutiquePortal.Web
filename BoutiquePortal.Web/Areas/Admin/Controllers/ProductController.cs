using BoutiquePortal.Model.Models;
using BoutiquePortal.Services.Interfaces;
using BoutiquePortal.Web.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace BoutiquePortal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthFilter]
    public class ProductController : Controller
    {
        private readonly IProductService _service;
        private readonly ICategoryService _categoryService;
        private readonly ISubCategoryService _subCategoryService;
        private readonly IVendorService _vendorService;
        private readonly IWebHostEnvironment _env;
        private readonly IProductSizeService _sizeService;
        public ProductController(
            IProductService service,
            ICategoryService categoryService,
            ISubCategoryService subCategoryService,
            IVendorService vendorService,
            IWebHostEnvironment env,
            IProductSizeService sizeService)
        {
            _service = service;
            _categoryService = categoryService;
            _subCategoryService = subCategoryService;
            _vendorService = vendorService;
            _env = env;
            _sizeService = sizeService;
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

                // Load existing sizes
                ViewBag.Sizes = (await _sizeService
                    .GetByProductAsync(id.Value)).ToList();

                return View(existing ?? new Product());
            }

            ViewBag.Sizes = new List<BoutiquePortal.Model.Models.ProductSize>();
            return View(new Product { IsActive = true });
        }

        // ================== ADD EDIT (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(Product model)
        {
            // Manual binding 
            model.ProductName = Request.Form["ProductName"].ToString().Trim();
            model.Description = Request.Form["Description"].ToString();
            model.ProductImage = Request.Form["ProductImage"].ToString();
            model.BrandName = Request.Form["BrandName"].ToString();  
            model.CategoryId = int.TryParse(Request.Form["CategoryId"], out int cid) ? cid : 0;
 
            model.SubCategoryId = int.TryParse(Request.Form["SubCategoryId"], out int scid) && scid > 0
                ? scid
                : (int?)null;


            model.VendorId = int.TryParse(Request.Form["VendorId"], out int vid) ? vid : 0;   
            model.Quantity = int.TryParse(Request.Form["Quantity"], out int qty) ? qty : 0;
            model.ProductId = int.TryParse(Request.Form["ProductId"], out int pid) ? pid : 0;
            model.IsActive = Request.Form["IsActive"].ToString().Contains("true");
            // model.Price = decimal.TryParse(Request.Form["Price"], out decimal price) ? price : 0;
            decimal.TryParse(Request.Form["Price"], out decimal price);
            decimal.TryParse(Request.Form["DiscountPrice"], out decimal discPrice);
            model.Price = price;
            model.DiscountPrice = discPrice > 0 ? discPrice : (decimal?)null;

            ModelState.Clear();

            // ================== VALIDATION ==================
            if (string.IsNullOrWhiteSpace(model.ProductName))
                ModelState.AddModelError("ProductName", "Product name is required");

            if (model.CategoryId == 0)
                ModelState.AddModelError("CategoryId", "Category is required");

            if (model.SubCategoryId == null)
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

            // ================== IMAGE UPLOAD ==================
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
            int savedProductId;

            if (model.ProductId == 0)
            {
                model.CreatedDate = DateTime.Now;
                int newId = await _service.AddAsync(model);
                savedProductId = newId;  // use newId
                TempData["Success"] = "Product added successfully!";
            }
            else
            {
                await _service.UpdateAsync(model);
                savedProductId = model.ProductId;  // use existing ID
                TempData["Success"] = "Product updated successfully!";
            }

            // Save sizes using savedProductId

            if (savedProductId > 0)
            {
                var sizeNames = Request.Form["SizeName"].ToList();
                var sizeQtys = Request.Form["SizeQty"]
                .Select(q => int.TryParse(q, out int n) ? n : 0)
                .ToList();

                if (sizeNames.Any(s => !string.IsNullOrWhiteSpace(s)))
                {
                    await _sizeService.SaveSizesAsync(savedProductId, sizeNames, sizeQtys);
                }
            }
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


        // ================== AJAX: BrandName by Vendor ==================
        public async Task<JsonResult> GetVendorBrand(int vendorId)
        {
            var vendor = await _vendorService.GetByIdAsync(vendorId);
            return Json(new { brandName = vendor?.BrandName ?? "" });
        }
    }
}