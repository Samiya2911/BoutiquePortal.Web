using BoutiquePortal.Model.Models;
using BoutiquePortal.Model.ViewModels;
using BoutiquePortal.Services.Interfaces;
using BoutiquePortal.Web.Filters;
using Microsoft.AspNetCore.Mvc;

using CustomerModel = BoutiquePortal.Model.Models.Customer;

namespace BoutiquePortal.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [CustomerAuthFilter]
    public class ProfileController : Controller
    {
        private readonly ICustomerService _customerService;

        public ProfileController(ICustomerService customerService)
            => _customerService = customerService;

        // ======= INDEX GET =======
        public async Task<IActionResult> Index()
        {
            int customerId = HttpContext.Session.GetInt32("CustomerId") ?? 0;
            var customer = await _customerService.GetByIdAsync(customerId);

            if (customer == null)
                return RedirectToAction("Login", "CustomerAccount", new { area = "Customer" });

            var model = new CustomerProfileVM
            {
                CustomerId = customer.CustomerId,
                FullName = customer.FullName,
                Email = customer.Email,
                Phone = customer.Phone ?? string.Empty
            };

            return View(model);
        }

        // ======= UPDATE PROFILE POST =======
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CustomerProfileVM model)
        {
            int customerId = HttpContext.Session.GetInt32("CustomerId") ?? 0;

            // ======= MANUAL BINDING =======
            model.CustomerId = customerId;
            model.FullName = Request.Form["FullName"].ToString().Trim();
            model.Phone = Request.Form["Phone"].ToString().Trim();

            ModelState.Clear();

            if (string.IsNullOrWhiteSpace(model.FullName))
                ModelState.AddModelError("FullName", "Full name is required");

            if (string.IsNullOrWhiteSpace(model.Phone))
                ModelState.AddModelError("Phone", "Phone is required");

            if (!ModelState.IsValid)
                return View(model);

            var customer = new CustomerModel
            {
                CustomerId = customerId,
                FullName = model.FullName,
                Phone = model.Phone
            };

            (bool success, string message) =
                await _customerService.UpdateProfileAsync(customer);

            if (!success)
            {
                ModelState.AddModelError("", message);
                return View(model);
            }

            // ✅ Update session name
            HttpContext.Session.SetString("CustomerName", model.FullName);

            TempData["Success"] = message;
            return RedirectToAction(nameof(Index));
        }

        // ======= CHANGE PASSWORD POST =======
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(CustomerProfileVM model)
        {
            int customerId = HttpContext.Session.GetInt32("CustomerId") ?? 0;

            var currentPassword = Request.Form["CurrentPassword"].ToString();
            var newPassword = Request.Form["NewPassword"].ToString();
            var confirmPassword = Request.Form["ConfirmNewPassword"].ToString();

            if (newPassword != confirmPassword)
            {
                TempData["PasswordError"] = "New passwords do not match.";
                return RedirectToAction(nameof(Index));
            }

            if (newPassword.Length < 6)
            {
                TempData["PasswordError"] = "Password must be at least 6 characters.";
                return RedirectToAction(nameof(Index));
            }

            (bool success, string message) =
                await _customerService.UpdatePasswordAsync(
                    customerId, currentPassword, newPassword);

            if (!success)
                TempData["PasswordError"] = message;
            else
                TempData["PasswordSuccess"] = message;

            return RedirectToAction(nameof(Index));
        }
    }
}