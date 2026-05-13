using BoutiquePortal.Model.ViewModels;
using VendorModel = BoutiquePortal.Model.Models.Vendor;
using Microsoft.AspNetCore.Mvc;
//using BoutiquePortal.Model.Models;
using BoutiquePortal.Services.Interfaces;

namespace BoutiquePortal.Web.Areas.Vendor.Controllers
{
    [Area("Vendor")]
    public class VendorAccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ICountryService _countryService;
        private readonly IStateService _stateService;
        private readonly ICityService _cityService;
        private readonly IPasswordResetService _resetService;

        public VendorAccountController(IAuthService authService,
            ICountryService countryService,
            IStateService stateService,
            ICityService cityService,
            IPasswordResetService resetService)
        {
            _authService = authService;
            _countryService = countryService;
            _stateService = stateService;
            _cityService = cityService;
            _resetService = resetService;
        }

        // ======= VENDOR LOGIN =======
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(VendorLoginVM model)
        {
            if (!ModelState.IsValid) return View(model);

            // var (success, message, vendor) = await _authService.ValidateVendorAsync(model.Email, model.Password);
            
            (bool success, string message, VendorModel? vendor) =
                 await _authService.ValidateVendorAsync(model.Email, model.Password);

            if (!success)
            {
                ModelState.AddModelError("", message);
                return View(model);
            }

            HttpContext.Session.SetString("Role", "Vendor");
            HttpContext.Session.SetString("VendorName", vendor!.VendorName);
            HttpContext.Session.SetString("BrandName", vendor.BrandName);
            HttpContext.Session.SetInt32("VendorId", vendor.VendorId);

            return RedirectToAction("Index", "Dashboard", new { area = "Vendor" });
        }

        // ======= VENDOR REGISTER =======
        public async Task<IActionResult> Register()
        {
            ViewBag.Country = (await _countryService.GetAllAsync()).ToList();
            return View();
        }

        // ======= VENDOR REGISTER POST =======
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(VendorRegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Country = (await _countryService.GetAllAsync()).ToList();
                return View(model);
            }

            var vendor = new VendorModel
            {
                VendorName = model.VendorName,
                BrandName = model.BrandName,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                Password = model.Password,
                CountryId = model.CountryId,
                StateId = model.StateId,
                CityId = model.CityId
            };

            // var (success, message) = await _authService.RegisterVendorAsync(vendor);
            (bool success, string message) =
                await _authService.RegisterVendorAsync(vendor);

            if (!success)
            {
                ModelState.AddModelError("", message);
                ViewBag.Country = (await _countryService.GetAllAsync()).ToList();
                return View(model);
            }

            TempData["SuccessMsg"] = message;
            return RedirectToAction("Login", "VendorAccount", new { area = "Vendor" });
            // return RedirectToAction("Login");
        }

        // AJAX helpers (same pattern as Admin VendorController)
        public async Task<JsonResult> GetStatesByCountry(int countryId)
            => Json(await _stateService.GetByCountryId(countryId));

        public async Task<JsonResult> GetCitiesByState(int stateId)
            => Json(await _cityService.GetByStateId(stateId));

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/Home/Index");
            //return RedirectToAction("Login", "VendorAccount", new { area = "Vendor" });
        }


        // ======= FORGOT PASSWORD  =======

        // ======= FORGOT PASSWORD GET =======
        public IActionResult ForgotPassword() => View();

        // ======= FORGOT PASSWORD POST =======
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var (success, message, token) =
                await _resetService.GenerateTokenAsync(model.Email, "Vendor");

            if (!success)
            {
                ModelState.AddModelError("", message);
                return View(model);
            }

            // Show reset link directly (no email server needed)
            ViewBag.ResetLink = $"/Vendor/VendorAccount/ResetPassword?token={token}";
            ViewBag.TokenGenerated = true;
            return View(model);
        }

        // ======= RESET PASSWORD GET =======
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login");

            return View(new ResetPasswordVM { Token = token });
        }

        // ======= RESET PASSWORD POST =======
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var (success, message) =
                await _resetService.ResetPasswordAsync(
                    model.Token, model.NewPassword);

            if (!success)
            {
                ModelState.AddModelError("", message);
                return View(model);
            }

            TempData["SuccessMsg"] = message;
            return RedirectToAction("Login");
        }


    }
}
