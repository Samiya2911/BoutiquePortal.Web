using BoutiquePortal.Model.Models;
using BoutiquePortal.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BoutiquePortal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CountryController : Controller
    {
        private readonly ICountryService _service;

        public CountryController(ICountryService service)
        {
            _service = service;
        }

        // ================== INDEX ==================
        public async Task<IActionResult> Index()
        {
            var list = await _service.GetAllAsync();
            return View(list);
        }

        // ================== AddEdit GET ==================
        public async Task<IActionResult> AddEdit(int? id)
        {
            if (id.HasValue && id.Value > 0)
            {
                var data = await _service.GetByIdAsync(id.Value);
                return View(data ?? new Country());
            }
            return View(new Country { IsActive = true });
        }

        // ================== AddEdit POST ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(Country model)
        {
            model.CountryName = Request.Form["CountryName"].ToString().Trim();
            model.CountryCode = Request.Form["CountryCode"].ToString();
            model.CountryId = int.TryParse(Request.Form["CountryId"], out int id) ? id : 0;
            model.IsActive = Request.Form["IsActive"].ToString().Contains("true");

            ModelState.Clear();

            if (string.IsNullOrWhiteSpace(model.CountryName))
                ModelState.AddModelError("CountryName", "Country name is required");

            if (string.IsNullOrWhiteSpace(model.CountryCode))
                ModelState.AddModelError("CountryCode", "Country code is required");

            if (!ModelState.IsValid)
                return View(model);

            if (model.CountryId == 0)
                await _service.AddAsync(model);
            else
                await _service.UpdateAsync(model);

            return RedirectToAction(nameof(Index));
        }

        // ================== DELETE ==================
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok();
        }
    }
}