using BoutiquePortal.Model.Models;
using BoutiquePortal.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace BoutiquePortal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ISubCategoryService _service;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _env;

        public SubCategoryController(
            ISubCategoryService service,
            ICategoryService categoryService,
            IWebHostEnvironment env)
        {
            _service = service;
            _categoryService = categoryService;
            _env = env;
        }

        // ================== INDEX ==================
        public async Task<IActionResult> Index()
        {
            var list = await _service.GetAllAsync();
            return View(list);
        }

        // ================== AddEdit GET ==================
        [HttpGet]
        public async Task<IActionResult> AddEdit(int? id)
        {
            ViewBag.Categories = (await _categoryService.GetAllAsync()).ToList();

            if (id.HasValue && id.Value > 0)
            {
                var existing = await _service.GetByIdAsync(id.Value);
                return View(existing ?? new SubCategory());
            }

            return View(new SubCategory { IsActive = true });
        }

        // ================== AddEdit POST ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(SubCategory model)
        {
            // ✅ MANUAL BINDING
            model.SubCategoryName = Request.Form["SubCategoryName"].ToString().Trim();
            model.Description = Request.Form["Description"].ToString();
            model.SubCategoryImage = Request.Form["SubCategoryImage"].ToString();

            model.CategoryId = int.TryParse(Request.Form["CategoryId"], out int cid) ? cid : 0;
            model.SubCategoryId = int.TryParse(Request.Form["SubCategoryId"], out int sid) ? sid : 0;
            model.IsActive = Request.Form["IsActive"].ToString().Contains("true");

            // ================== VALIDATION ==================
            ModelState.Clear();

            if (string.IsNullOrWhiteSpace(model.SubCategoryName))
            { 
                ModelState.AddModelError("SubCategoryName", "SubCategory name is required"); 
            }
       
            if (model.CategoryId == 0)
            { 
                ModelState.AddModelError("CategoryId", "Please select category");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = (await _categoryService.GetAllAsync()).ToList();
                return View(model);
            }

            // ================== IMAGE UPLOAD ==================
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                string folder = Path.Combine(_env.WebRootPath, "images", "subcategory");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid() + Path.GetExtension(model.ImageFile.FileName);

                using (var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                // ✅ SAVE ONLY IMAGE NAME
                model.SubCategoryImage = fileName;
            }

            // ================== SAVE ==================
            if (model.SubCategoryId == 0)
            {
                model.CreatedDate = DateTime.Now;
                await _service.AddAsync(model);
                TempData["Success"] = "SubCategory added successfully!";
            }
            else
            {
                await _service.UpdateAsync(model);
                TempData["Success"] = "SubCategory updated successfully!";
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
    }
}