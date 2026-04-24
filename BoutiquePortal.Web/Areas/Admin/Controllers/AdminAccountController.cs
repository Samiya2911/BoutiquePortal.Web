using Microsoft.AspNetCore.Mvc;
using BoutiquePortal.Model.ViewModels;
using BoutiquePortal.Services.Interfaces;

namespace BoutiquePortal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminAccountController : Controller
    {
        private readonly IAuthService _authService;
        public AdminAccountController(IAuthService authService)
        {
            _authService = authService;
        }
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AdminLoginVM model)
        {
            if (!ModelState.IsValid)
            {
                // See exactly what validation errors exist
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                TempData["Debug"] = string.Join(", ", errors);
                return View(model);
            }

            // See if admin is found in DB
            var admin = await _authService.ValidateAdminAsync(model.Email, model.Password);

            TempData["Debug"] = admin == null
                ? $"Admin NOT found for email: {model.Email}"
                : $"Admin found: {admin.AdminName}";

            if (admin == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            HttpContext.Session.SetString("Role", "Admin");
            HttpContext.Session.SetString("AdminName", admin.AdminName);
            HttpContext.Session.SetInt32("AdminId", admin.AdminId);

            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            //return RedirectToAction("Login", "AdminAccount", new { area = "Admin" });
            return Redirect("/Home/Index");
        }
    }
}
