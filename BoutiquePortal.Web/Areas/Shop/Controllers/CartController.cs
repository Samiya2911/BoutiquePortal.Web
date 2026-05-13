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

        public async Task<IActionResult> Add(int id, int qty = 1, string? size = null)
        {
            if (HttpContext.Session.GetString("Role") != "Customer")
            {
                TempData["CartMsg"] = "Please login to add items to cart.";
                return RedirectToAction("Login", "CustomerAccount",
                    new { area = "Customer" });
            }

            var product = await _productService.GetDetailAsync(id);
            if (product == null || product.Quantity <= 0)
            {
                TempData["CartMsg"] = product == null
                    ? "Product not found."
                    : "Sorry, this product is out of stock.";
                return RedirectToAction("Index", "Shop", new { area = "Shop" });
            }

            // ✅ Step 1: Add to session
            var cartItem = new CartItem
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                BrandName = product.BrandName ?? string.Empty,
                ProductImage = product.ProductImage ?? string.Empty,
                Price = product.Price,
                DiscountPrice = product.DiscountPrice,
                Quantity = qty,
                VendorId = product.VendorId,
                SelectedSize = size
            };
            CartHelper.AddItem(HttpContext.Session, cartItem);

            //  Sync ENTIRE session cart to DB
            //           (replaces DB with exact session quantities)
            int customerId = HttpContext.Session.GetInt32("CustomerId") ?? 0;
            if (customerId > 0)
            {
                await SyncCartToDbAsync(customerId);
            }

            TempData["CartMsg"] = $"'{product.ProductName}'"
        + (size != null ? $" (Size: {size})" : "")
        + " added to cart!";
                
            return RedirectToAction(nameof(Index));
        }

        // ✅ Helper: sync whole session cart to DB cleanly
        private async Task SyncCartToDbAsync(int customerId)
        {
            var sessionCart = CartHelper.GetCart(HttpContext.Session);

            // Clear DB cart first
            await _cartService.ClearCartAsync(customerId);

            // Rewrite from session
            foreach (var item in sessionCart)
            {
                await _cartService.AddToCartAsync(
                    customerId, item.ProductId, item.Quantity);
            }
        }

        // ======= UPDATE QUANTITY (AJAX) ======

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UpdateQty(int productId, int quantity)
        {
            CartHelper.UpdateQuantity(HttpContext.Session, productId, quantity);

            int customerId = HttpContext.Session.GetInt32("CustomerId") ?? 0;
            if (customerId > 0)
            {
                await SyncCartToDbAsync(customerId);
                // await _cartService.UpdateQuantityAsync(customerId, productId, quantity);
            }

            var cart = CartHelper.GetCart(HttpContext.Session);
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            var total = CartHelper.GetCartTotal(HttpContext.Session);

            //  Make sure SubTotal is calculated correctly
            decimal itemSubTotal = item != null
                ? item.ActualPrice * item.Quantity
                : 0;

            return Json(new
            {
                success = true,
                subTotal = itemSubTotal.ToString("N2"),
                //cartTotal = total.ToString("N2"),
                cartTotal = CartHelper.GetCartTotal(HttpContext.Session).ToString("N2"),
                cartCount = CartHelper.GetCartCount(HttpContext.Session)
                //success = true,
                //subTotal = item?.SubTotal.ToString("N2") ?? "0.00",
                //cartTotal = total.ToString("N2"),
                //cartCount = CartHelper.GetCartCount(HttpContext.Session)
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

        // ======= CLEAR CART =======
        
        public async Task<IActionResult> Clear()
        {
            //  Clear Session
            CartHelper.ClearCart(HttpContext.Session);

            // Clear DB if logged in
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