using BoutiquePortal.Services.Interfaces;
using BoutiquePortal.Web.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BoutiquePortal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthFilter]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
            => _orderService = orderService;

        // ======= INDEX =======
        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllAsync();
            return View(orders);
        }

        // ======= DETAILS =======
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        // ======= UPDATE STATUS (AJAX) =======
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> UpdateStatus(int orderId,
        //    string orderStatus, string paymentStatus)
        //{
        //    await _orderService.UpdateStatusAsync(orderId, orderStatus, paymentStatus);
        //    TempData["Success"] = "Order status updated!";
        //    return RedirectToAction(nameof(Details), new { id = orderId });
        //}

        // ======= UPDATE STATUS =======
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(
            int orderId,
            string orderStatus,
            string paymentStatus)
        {
            // ======= GET CURRENT ORDER BEFORE UPDATE =======
            var existingOrder = await _orderService.GetByIdAsync(orderId);

            // ======= UPDATE STATUS IN DB =======
            await _orderService.UpdateStatusAsync(
                orderId, orderStatus, paymentStatus);

            // ======= CHECK IF CONDITION MET =======
            // Trigger quantity decrease ONLY when:
            // 1. Order just became "Shipped"
            // 2. Payment is "Paid"
            // 3. Was NOT already "Shipped" before (prevent double decrease)

            bool justShipped = orderStatus == "Shipped"
                            && paymentStatus == "Paid"
                            && existingOrder?.OrderStatus != "Shipped";

            if (justShipped)
            {
                //  Decrease product quantities
                int updated = await _orderService
                    .DecreaseProductQuantityAsync(orderId);

                //  Auto mark out-of-stock products inactive
                await _orderService.UpdateStockStatusAsync();

                TempData["Success"] = updated > 0
                    ? $"Order status updated! Stock decreased for {updated} product(s)."
                    : "Order status updated!";
            }
            else
            {
                TempData["Success"] = "Order status updated successfully!";
            }

            return RedirectToAction(nameof(Details), new { id = orderId });
        }
        // ======= DELETE =======
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _orderService.DeleteAsync(id);
            return Ok();
        }
    }
}