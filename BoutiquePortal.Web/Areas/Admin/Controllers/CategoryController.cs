using BoutiquePortal.Model.Models;
using BoutiquePortal.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace BoutiquePortal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _service;
        private readonly IWebHostEnvironment _env;

        public CategoryController(ICategoryService service, IWebHostEnvironment env)
        {
            _service = service;
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
            if (id.HasValue && id.Value > 0)
            {
                var existing = await _service.GetByIdAsync(id.Value);
                return View(existing ?? new Category());
            }
            return View(new Category { IsActive = true });
        }

        // ================== AddEdit POST ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(Category model)
        {
            // Read ALL values directly from Request.Form
            // This bypasses ModelState binding issues completely
            model.CategoryName = Request.Form["CategoryName"].ToString().Trim();
            model.Description = Request.Form["Description"].ToString();
            model.CategoryImage = Request.Form["CategoryImage"].ToString();
            model.CategoryId = int.TryParse(Request.Form["CategoryId"], out int cid) ? cid : 0;
            model.IsActive = Request.Form["IsActive"].ToString().Contains("true");

            // Manual validation since we are bypassing ModelState for these fields
            ModelState.Clear();

            if (string.IsNullOrWhiteSpace(model.CategoryName))
            {
                ModelState.AddModelError("CategoryName", "Category name is required");
            }
            else if (model.CategoryName.Length < 2)
            {
                ModelState.AddModelError("CategoryName", "Minimum 2 characters required");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value!.Errors.Count > 0)
                    .Select(x => $"{x.Key}: {x.Value!.Errors[0].ErrorMessage}");
                TempData["DebugErrors"] = string.Join(" | ", errors);
                return View(model);
            }

            // ── IMAGE HANDLING ──────────────────────────────────────
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                string folder = Path.Combine(_env.WebRootPath, "images", "category");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid() + Path.GetExtension(model.ImageFile.FileName);

                using (var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                model.CategoryImage = fileName;
            }
            

            // ── SAVE ────────────────────────────────────────────────
            if (model.CategoryId == 0)
            {
                model.CreatedDate = DateTime.Now;
                await _service.AddAsync(model);
                TempData["Success"] = "Category added successfully!";
            }
            else
            {
                await _service.UpdateAsync(model);
                TempData["Success"] = "Category updated successfully!";
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
