using BoutiquePortal.Model.Models;
using BoutiquePortal.Services.Interfaces;
using BoutiquePortal.Web.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace BoutiquePortal.Web.Areas.Shop.Controllers
{
    [Area("Shop")]
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;    

        public CartController(
            IProductService productService,
            ICartService cartService)             
        {
            _productService = productService;
            _cartService = cartService;
        }

        // ======= VIEW CART =======
        public IActionResult Index()
        {
            var cart = CartHelper.GetCart(HttpContext.Session);
            return View(cart);
        }

        // ======= ADD TO CART =======
        public async Task<IActionResult> Add(int id, int qty = 1)
        {
            // Must be logged in as Customer
            if (HttpContext.Session.GetString("Role") != "Customer")
            {
                TempData["CartMsg"] = "Please login to add items to cart.";
                return RedirectToAction("Login", "CustomerAccount",
                    new { area = "Customer" });
            }

            var product = await _productService.GetDetailAsync(id);

            if (product == null)
            {
                TempData["CartMsg"] = "Product not found.";
                return RedirectToAction("Index", "Shop", new { area = "Shop" });
            }

            if (product.Quantity <= 0)
            {
                TempData["CartMsg"] = "Sorry, this product is out of stock.";
                return RedirectToAction("Details", "Product",
                    new { area = "Shop", id });
            }

            var cartItem = new CartItem
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                BrandName = product.BrandName ?? string.Empty,
                ProductImage = product.ProductImage ?? string.Empty,
                Price = product.Price,
                DiscountPrice = product.DiscountPrice,
                Quantity = qty,
                VendorId = product.VendorId
            };

            CartHelper.AddItem(HttpContext.Session, cartItem);

            int customerId = HttpContext.Session.GetInt32("CustomerId") ?? 0;
            if (customerId > 0)
            {
                await _cartService.AddToCartAsync(
                    customerId, product.ProductId, qty);
            }

            TempData["CartMsg"] = $"'{product.ProductName}' added to cart!";
            return RedirectToAction(nameof(Index));
        }

        // ======= UPDATE QUANTITY (AJAX) =======
        //[HttpPost]
        //[IgnoreAntiforgeryToken]
        //public IActionResult UpdateQty(int productId, int quantity)
        //{
        //    CartHelper.UpdateQuantity(HttpContext.Session, productId, quantity);

        //    int customerId = HttpContext.Session.GetInt32("CustomerId") ?? 0;
        //    if (customerId > 0)
        //    {
        //        await _cartService.UpdateQuantityAsync(
        //            customerId, productId, quantity);
        //    }


        //    var cart = CartHelper.GetCart(HttpContext.Session);
        //    var item = cart.FirstOrDefault(c => c.ProductId == productId);
        //    var total = CartHelper.GetCartTotal(HttpContext.Session);

        //    return Json(new
        //    {
        //        success = true,
        //        subTotal = item?.SubTotal.ToString("N2") ?? "0.00",
        //        cartTotal = total.ToString("N2"),
        //        cartCount = CartHelper.GetCartCount(HttpContext.Session)
        //    });
        //}

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UpdateQty(int productId, int quantity)
        {
            CartHelper.UpdateQuantity(HttpContext.Session, productId, quantity);

            int customerId = HttpContext.Session.GetInt32("CustomerId") ?? 0;
            if (customerId > 0)
            {
                await _cartService.UpdateQuantityAsync(customerId, productId, quantity);
            }

            var cart = CartHelper.GetCart(HttpContext.Session);
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            var total = CartHelper.GetCartTotal(HttpContext.Session);

            return Json(new
            {
                success = true,
                subTotal = item?.SubTotal.ToString("N2") ?? "0.00",
                cartTotal = total.ToString("N2"),
                cartCount = CartHelper.GetCartCount(HttpContext.Session)
            });
        }


        // ======= REMOVE ITEM (AJAX) =======

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Remove(int productId)
        {
            CartHelper.RemoveItem(HttpContext.Session, productId);

            int customerId = HttpContext.Session.GetInt32("CustomerId") ?? 0;
            if (customerId > 0)
            {
                await _cartService.RemoveItemAsync(customerId, productId);
            }

            var total = CartHelper.GetCartTotal(HttpContext.Session);
            var count = CartHelper.GetCartCount(HttpContext.Session);

            return Json(new
            {
                success = true,
                cartTotal = total.ToString("N2"),
                cartCount = count
            });
        }

        //[HttpPost]
        //[IgnoreAntiforgeryToken]
        //public IActionResult Remove(int productId)
        //{
        //    CartHelper.RemoveItem(HttpContext.Session, productId);

        //    int customerId = HttpContext.Session.GetInt32("CustomerId") ?? 0;
        //    if (customerId > 0)
        //    {
        //        await _cartService.RemoveItemAsync(customerId, productId);
        //    }

        //    var total = CartHelper.GetCartTotal(HttpContext.Session);
        //    var count = CartHelper.GetCartCount(HttpContext.Session);

        //    return Json(new
        //    {
        //        success = true,
        //        cartTotal = total.ToString("N2"),
        //        cartCount = count
        //    });
        //}

        // ======= CLEAR CART =======
        //public IActionResult Clear()    
        //{
        //    CartHelper.ClearCart(HttpContext.Session);
        //    return RedirectToAction(nameof(Index));
        //}
        public async Task<IActionResult> Clear()
        {
            // ✅ Clear Session
            CartHelper.ClearCart(HttpContext.Session);

            // ✅ Clear DB if logged in
            int customerId = HttpContext.Session.GetInt32("CustomerId") ?? 0;
            if (customerId > 0)
            {
                await _cartService.ClearCartAsync(customerId);
            }

            return RedirectToAction(nameof(Index));
        }

        // ======= GET CART COUNT (AJAX for header) =======
        public IActionResult GetCount()
        {
            return Json(new
            {
                count = CartHelper.GetCartCount(HttpContext.Session),
                total = CartHelper.GetCartTotal(HttpContext.Session).ToString("N2")
            });
        }
    }
}