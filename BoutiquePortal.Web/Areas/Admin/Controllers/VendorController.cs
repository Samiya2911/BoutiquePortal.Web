//using BoutiquePortal.Model.Models;
using VendorModel = BoutiquePortal.Model.Models.Vendor;
using BoutiquePortal.Services.Interfaces;
using BoutiquePortal.Web.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BoutiquePortal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthFilter]
    public class VendorController : Controller
    {
        private readonly IVendorService _service;
        private readonly ICountryService _countryService;
        private readonly IStateService _stateService;
        private readonly ICityService _cityService;

        public VendorController(
            IVendorService service,
            ICountryService countryService,
            IStateService stateService,
            ICityService cityService)
        {
            _service = service;
            _countryService = countryService;
            _stateService = stateService;
            _cityService = cityService;
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
            ViewBag.Country = (await _countryService.GetAllAsync()).ToList();

            if (id.HasValue && id.Value > 0)
            {
                var existing = await _service.GetByIdAsync(id.Value);
                return View(existing ?? new VendorModel());
            }

            return View(new VendorModel { IsActive = true });
        }

        // ================== AddEdit POST ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(VendorModel model)
        {
            // ================== MANUAL BINDING ==================
            model.VendorName = Request.Form["VendorName"].ToString().Trim();
            model.Email = Request.Form["Email"].ToString().Trim();
            model.Password = Request.Form["Password"].ToString();
            model.BrandName = Request.Form["BrandName"].ToString().Trim();
            model.Phone = Request.Form["Phone"].ToString().Trim();
            model.Address = Request.Form["Address"].ToString().Trim();
            model.VendorId = int.TryParse(Request.Form["VendorId"], out int vid) ? vid : 0;
            model.CountryId = int.TryParse(Request.Form["CountryId"], out int cid) ? cid : 0;
            model.StateId = int.TryParse(Request.Form["StateId"], out int sid) ? sid : 0;
            model.CityId = int.TryParse(Request.Form["CityId"], out int ctid) ? ctid : 0;
            model.IsApproved = Request.Form["IsApproved"].ToString().Contains("true");
            model.IsActive = Request.Form["IsActive"].ToString().Contains("true");

            // ================== VALIDATION ==================
            ModelState.Clear();

            if (string.IsNullOrWhiteSpace(model.VendorName))
                ModelState.AddModelError("VendorName", "Vendor name is required");

            if (string.IsNullOrWhiteSpace(model.Email))
                ModelState.AddModelError("Email", "Email is required");

            if (string.IsNullOrWhiteSpace(model.Password))
                ModelState.AddModelError("Password", "Password is required");

            if (string.IsNullOrWhiteSpace(model.BrandName))
                ModelState.AddModelError("BrandName", "Brand name is required");

            if (string.IsNullOrWhiteSpace(model.Phone))
                ModelState.AddModelError("Phone", "Phone is required");

            if (model.CountryId == 0)
                ModelState.AddModelError("CountryId", "Please select a country");

            if (model.StateId == 0)
                ModelState.AddModelError("StateId", "Please select a state");

            if (model.CityId == 0)
                ModelState.AddModelError("CityId", "Please select a city");

            if (!ModelState.IsValid)
            {
                ViewBag.Country = (await _countryService.GetAllAsync()).ToList();
                return View(model);
            }

            // ================== SAVE ==================
            int result;

            if (model.VendorId == 0)
            {
                model.CreatedDate = DateTime.Now;
                result = await _service.AddAsync(model);

                if (result == -1)
                {
                    ModelState.AddModelError("Phone", "This phone number is already registered");
                    ViewBag.Country = (await _countryService.GetAllAsync()).ToList();
                    return View(model);
                }

                TempData["Success"] = "Vendor added successfully!";
            }
            else
            {
                result = await _service.UpdateAsync(model);

                if (result == -1)
                {
                    ModelState.AddModelError("Phone", "This phone number is already registered");
                    ViewBag.Country = (await _countryService.GetAllAsync()).ToList();
                    return View(model);
                }

                TempData["Success"] = "Vendor updated successfully!";
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

        // ================== AJAX: States by Country ==================
        public async Task<JsonResult> GetStatesByCountry(int countryId)
        {
            var data = await _stateService.GetByCountryId(countryId);
            return Json(data);
        }

        // ================== AJAX: Cities by State ==================
        public async Task<JsonResult> GetCitiesByState(int stateId)
        {
            var data = await _cityService.GetByStateId(stateId);
            return Json(data);
        }
    }
}
