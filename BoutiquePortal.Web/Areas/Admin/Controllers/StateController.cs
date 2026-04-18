using BoutiquePortal.Model.Models;
using BoutiquePortal.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BoutiquePortal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StateController : Controller
    {
        private readonly IStateService _service;
        private readonly ICountryService _countryService;

        public StateController(IStateService service, ICountryService countryService)
        {
            _service = service;
            _countryService = countryService;
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
            ViewBag.Country = await _countryService.GetAllAsync();

            if (id.HasValue && id.Value > 0)
            {
                var data = await _service.GetByIdAsync(id.Value);
                return View(data ?? new State());
            }

            return View(new State { IsActive = true });
        }

        // ================== AddEdit POST ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(State model)
        {
            model.StateName = Request.Form["StateName"].ToString().Trim();
            model.StateCode = Request.Form["StateCode"].ToString();
            model.CountryId = int.TryParse(Request.Form["CountryId"], out int cid) ? cid : 0;
            model.StateId = int.TryParse(Request.Form["StateId"], out int id) ? id : 0;
            model.IsActive = Request.Form["IsActive"].ToString().Contains("true");

            ModelState.Clear();

            if (string.IsNullOrWhiteSpace(model.StateName))
                ModelState.AddModelError("StateName", "State name is required");

            if (string.IsNullOrWhiteSpace(model.StateCode))
                ModelState.AddModelError("StateCode", "State code is required");

            if (model.CountryId == 0)
                ModelState.AddModelError("CountryId", "Country is required");

            if (!ModelState.IsValid)
            {
                ViewBag.Country = await _countryService.GetAllAsync();
                return View(model);
            }

            if (model.StateId == 0)
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