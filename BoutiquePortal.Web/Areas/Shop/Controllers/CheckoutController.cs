using BoutiquePortal.Model.Models;
using BoutiquePortal.Model.ViewModels;
using BoutiquePortal.Services.Interfaces;
using BoutiquePortal.Services.Services;
using BoutiquePortal.Web.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace BoutiquePortal.Web.Areas.Shop.Controllers
{
    [Area("Shop")]
    public class CheckoutController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public CheckoutController(
            IOrderService orderService,
            IProductService productService,
            ICartService cartService)
        {
            _orderService = orderService;
            _productService = productService;
            _cartService = cartService;
        }

        // ======= CHECKOUT GET =======
        public IActionResult Index()
        {
            // Must be logged in
            if (HttpContext.Session.GetString("Role") != "Customer")
                return RedirectToAction("Login", "CustomerAccount",
                    new { area = "Customer" });

            var cart = CartHelper.GetCart(HttpContext.Session);

            // Cart must not be empty
            if (!cart.Any())
            {
                TempData["CartMsg"] = "Your cart is empty!";
                return RedirectToAction("Index", "Cart", new { area = "Shop" });
            }

            // Pre-fill customer name & phone from session
            var model = new CheckoutVM
            {
                ShippingName = HttpContext.Session
                                    .GetString("CustomerName") ?? string.Empty,
                SubTotal = CartHelper.GetCartTotal(HttpContext.Session),
                DeliveryCharge = 100m
            };

            ViewBag.Cart = cart;
            return View(model);
        }

        // ======= CHECKOUT POST — PLACE ORDER =======
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CheckoutVM model)
        {
            // Auth check
            if (HttpContext.Session.GetString("Role") != "Customer")
                return RedirectToAction("Login", "CustomerAccount",
                    new { area = "Customer" });

            int customerId = HttpContext.Session.GetInt32("CustomerId") ?? 0;
            var cart = CartHelper.GetCart(HttpContext.Session);

            if (!cart.Any())
            {
                TempData["CartMsg"] = "Your cart is empty!";
                return RedirectToAction("Index", "Cart", new { area = "Shop" });
            }

            // ======= MANUAL BINDING =======
            model.ShippingName = Request.Form["ShippingName"].ToString().Trim();
            model.ShippingPhone = Request.Form["ShippingPhone"].ToString().Trim();
            model.ShippingAddress = Request.Form["ShippingAddress"].ToString().Trim();
            model.ShippingCity = Request.Form["ShippingCity"].ToString().Trim();
            model.ShippingState = Request.Form["ShippingState"].ToString().Trim();
            model.ShippingPincode = Request.Form["ShippingPincode"].ToString().Trim();
            model.PaymentMethod = Request.Form["PaymentMethod"].ToString();
            model.UpiId = Request.Form["UpiId"].ToString().Trim();
            model.CardNumber = Request.Form["CardNumber"].ToString().Trim();
            model.CardName = Request.Form["CardName"].ToString().Trim();
            model.SubTotal = CartHelper.GetCartTotal(HttpContext.Session);

            // ======= VALIDATION =======
            ModelState.Clear();

            if (string.IsNullOrWhiteSpace(model.ShippingName))
                ModelState.AddModelError("ShippingName", "Full name is required");

            if (string.IsNullOrWhiteSpace(model.ShippingPhone))
                ModelState.AddModelError("ShippingPhone", "Phone is required");

            if (string.IsNullOrWhiteSpace(model.ShippingAddress))
                ModelState.AddModelError("ShippingAddress", "Address is required");

            if (string.IsNullOrWhiteSpace(model.ShippingCity))
                ModelState.AddModelError("ShippingCity", "City is required");

            if (string.IsNullOrWhiteSpace(model.ShippingState))
                ModelState.AddModelError("ShippingState", "State is required");

            if (string.IsNullOrWhiteSpace(model.ShippingPincode))
                ModelState.AddModelError("ShippingPincode", "Pincode is required");

            if (string.IsNullOrWhiteSpace(model.PaymentMethod))
                ModelState.AddModelError("PaymentMethod", "Select payment method");

            // Payment method specific validation
            if (model.PaymentMethod == "UPI" &&
                string.IsNullOrWhiteSpace(model.UpiId))
                ModelState.AddModelError("UpiId", "UPI ID is required");

            if (model.PaymentMethod == "Card")
            {
                if (string.IsNullOrWhiteSpace(model.CardNumber))
                    ModelState.AddModelError("CardNumber", "Card number is required");
                if (string.IsNullOrWhiteSpace(model.CardName))
                    ModelState.AddModelError("CardName", "Name on card is required");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Cart = cart;
                return View(model);
            }

            // ======= DETERMINE PAYMENT STATUS =======
            // COD = Pending, UPI/Card = Paid (simulated)
            string paymentStatus = model.PaymentMethod == "COD" ? "Pending" : "Paid";
            string transactionId = model.PaymentMethod != "COD"
                                    ? $"TXN{DateTime.Now:yyyyMMddHHmmss}"
                                    : string.Empty;

            // ======= CREATE ORDER =======
            var order = new Order
            {
                CustomerId = customerId,
                TotalAmount = model.GrandTotal,
                OrderStatus = "Pending",
                ShippingName = model.ShippingName,
                ShippingPhone = model.ShippingPhone,
                ShippingAddress = model.ShippingAddress,
                ShippingCity = model.ShippingCity,
                ShippingState = model.ShippingState,
                ShippingPincode = model.ShippingPincode,
                PaymentMethod = model.PaymentMethod,
                PaymentStatus = paymentStatus,
                TransactionId = transactionId,
                OrderDate = DateTime.Now
            };

            // ======= SAVE ORDER TO DB =======
            int orderId = await _orderService.AddAsync(order);

            if (orderId <= 0)
            {
                ModelState.AddModelError("",
                    "Failed to place order. Please try again.");
                ViewBag.Cart = cart;
                return View(model);
            }

            // ======= SAVE ORDER ITEMS =======
            foreach (var item in cart)
            {
                var orderItem = new OrderItem
                {
                    OrderId = orderId,
                    ProductId = item.ProductId,
                    VendorId = item.VendorId,
                    Quantity = item.Quantity,
                    UnitPrice = item.ActualPrice,
                    TotalPrice = item.SubTotal,
                    SelectedSize = item.SelectedSize
                };

                await _orderService.AddItemAsync(orderItem);
            }

            // ======= CLEAR CART AFTER ORDER =======
            CartHelper.ClearCart(HttpContext.Session);

            //  ADD THIS — also clear DB cart after order
            if (customerId > 0)
                await _cartService.ClearCartAsync(customerId);

            // ======= REDIRECT TO SUCCESS PAGE =======
            TempData["OrderId"] = orderId;
            TempData["PaymentMethod"] = model.PaymentMethod;
            TempData["GrandTotal"] = model.GrandTotal.ToString("N2");

            return RedirectToAction(nameof(OrderSuccess));
        }

        // ======= ORDER SUCCESS PAGE =======
        public IActionResult OrderSuccess()
        {
            if (TempData["OrderId"] == null)
                return RedirectToAction("Index", "Shop", new { area = "Shop" });

            ViewBag.OrderId = TempData["OrderId"];
            ViewBag.PaymentMethod = TempData["PaymentMethod"];
            ViewBag.GrandTotal = TempData["GrandTotal"];

            return View();
        }

        // ======= MY ORDERS =======
        public async Task<IActionResult> MyOrders()
        {
            if (HttpContext.Session.GetString("Role") != "Customer")
                return RedirectToAction("Login", "CustomerAccount",
                    new { area = "Customer" });

            int customerId = HttpContext.Session.GetInt32("CustomerId") ?? 0;
            var orders = await _orderService.GetByCustomerAsync(customerId);

            return View(orders);
        }
    }
}