//using BoutiquePortal.Model.Models;
using BoutiquePortal.Model.ViewModels;
using BoutiquePortal.Services.Interfaces;
using BoutiquePortal.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using CustomerModel = BoutiquePortal.Model.Models.Customer;

namespace BoutiquePortal.Web.Areas.Customer.Controllers
{

    [Area("Customer")]
    public class CustomerAccountController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly ICartService _cartService;
        public CustomerAccountController(
           ICustomerService customerService,
           ICartService cartService)
        {
            _customerService = customerService;
            _cartService = cartService;
        }

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

            // Load DB cart into session on login
            var dbCartItems = await _cartService
                .GetCartItemsForSessionAsync(customer.CustomerId);

            if (dbCartItems.Any())
                CartHelper.SaveCart(HttpContext.Session, dbCartItems);


            //  CartHelper.SaveCart(HttpContext.Session, dbCartItems);

            return RedirectToAction("Index", "Home", new { area = "Shop" });
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

        public async Task<IActionResult> Logout()
        {
            var role = HttpContext.Session.GetString("Role");
            var customerId = HttpContext.Session.GetInt32("CustomerId") ?? 0;

            if (role == "Customer" && customerId > 0)
            {
                //  Sync session cart to DB before clearing
                var sessionCart = CartHelper.GetCart(HttpContext.Session);

                await _cartService.ClearCartAsync(customerId);

                foreach (var item in sessionCart)
                {
                    await _cartService.AddToCartAsync(
                        customerId, item.ProductId, item.Quantity);
                }
            }

            HttpContext.Session.Clear();
            return Redirect("/Home/Index");
            //  return RedirectToAction("Login", "CustomerAccount",new { area = "Customer" });
        }
    }
}
