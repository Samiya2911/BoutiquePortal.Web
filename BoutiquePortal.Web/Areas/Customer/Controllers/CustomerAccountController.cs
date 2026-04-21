using Microsoft.AspNetCore.Mvc;
//using BoutiquePortal.Model.Models;
using BoutiquePortal.Model.ViewModels;
using BoutiquePortal.Services.Interfaces;
using CustomerModel = BoutiquePortal.Model.Models.Customer;

namespace BoutiquePortal.Web.Areas.Customer.Controllers
{
    
    [Area("Customer")]
    public class CustomerAccountController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerAccountController(ICustomerService customerService)
            => _customerService = customerService;

        // ======= LOGIN GET =======
        public IActionResult Login() => View();

        // ======= LOGIN POST =======
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(CustomerLoginVM model)
        {
            if (!ModelState.IsValid) return View(model);

            (bool success, string message, CustomerModel? customer) =
                await _customerService.LoginAsync(model.Email, model.Password);

            if (!success)
            {
                ModelState.AddModelError("", message);
                return View(model);
            }

            HttpContext.Session.SetString("Role", "Customer");
            HttpContext.Session.SetString("CustomerName", customer!.FullName);
            HttpContext.Session.SetString("CustomerEmail", customer.Email);
            HttpContext.Session.SetInt32("CustomerId", customer.CustomerId);
            return RedirectToAction("Index", "Home", new { area = "Shop" });
            // return RedirectToAction("Index", "Dashboard", new { area = "Customer" });
        }

        // ======= REGISTER GET =======
        public IActionResult Register() => View();

        // ======= REGISTER POST =======
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(CustomerRegisterVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var customer = new CustomerModel
            {
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                Password = model.Password,
                CreatedDate = DateTime.Now
            };

            (bool success, string message) =
                await _customerService.RegisterAsync(customer);

            if (!success)
            {
                ModelState.AddModelError("", message);
                return View(model);
            }

            TempData["SuccessMsg"] = message;
            return RedirectToAction("Login");
        }

        // ======= LOGOUT =======
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "CustomerAccount", new { area = "Customer" });
        }
    }
}
