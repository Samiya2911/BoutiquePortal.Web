using BoutiquePortal.Model.Models;
using BoutiquePortal.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace BoutiquePortal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CityController : Controller
    {
        private readonly ICityService _service;
        private readonly IStateService _stateService;
        private readonly ICountryService _countryService;

        public CityController(ICityService service, IStateService stateService, ICountryService countryService)
        {
            _service = service;
            _stateService = stateService;
            _countryService = countryService;
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
            ViewBag.Countries = (await _countryService.GetAllAsync()).ToList();

            if (id.HasValue && id.Value > 0)
            {
                var existing = await _service.GetByIdAsync(id.Value);
                return View(existing ?? new City());
            }

            return View(new City { IsActive = true });
        }

        // ================== AddEdit POST ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(City model)
        {
            model.CityName = Request.Form["CityName"].ToString().Trim();
            model.CityCode = Request.Form["CityCode"].ToString();
            model.StateId = int.TryParse(Request.Form["StateId"], out int sid) ? sid : 0;
            model.CityId = int.TryParse(Request.Form["CityId"], out int id) ? id : 0;
            model.IsActive = Request.Form["IsActive"].ToString().Contains("true");

            ModelState.Clear();

            if (string.IsNullOrWhiteSpace(model.CityName))
            {
                ModelState.AddModelError("CityName", "City name is required");
            }

            if (string.IsNullOrWhiteSpace(model.CityCode))
            { 
                ModelState.AddModelError("CityCode", "City code is required");
            }

            if (model.StateId == 0)
            { 
                ModelState.AddModelError("StateId", "State is required");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Countries = (await _countryService.GetAllAsync()).ToList();
                return View(model);
            }

            if (model.CityId == 0)
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

        // // ================== AJAX ==================

        //public async Task<JsonResult> GetStatesByCountry(int countryId)
        //{
        //    var data = await _stateService.GetByCountryId(countryId);
        //    return Json(data);
        //}

        //public async Task<JsonResult> GetCitiesByState(int stateId)
        //{
        //    var data = await _service.GetByStateId(stateId);
        //    return Json(data);
        //}
    }
}