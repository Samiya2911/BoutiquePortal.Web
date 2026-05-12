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
        private readonly IProductSizeService _sizeService;
        public OrderController(IOrderService orderService,
            IProductSizeService sizeService)
        {
            _orderService = orderService;
            _sizeService = sizeService;
        }
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

        // ======= UPDATE STATUS =======
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> UpdateStatus(
        //    int orderId,
        //    string orderStatus,
        //    string paymentStatus)
        //{
        //    // ======= GET CURRENT ORDER BEFORE UPDATE =======
        //    var existingOrder = await _orderService.GetByIdAsync(orderId);

        //    // ======= UPDATE STATUS IN DB =======
        //    await _orderService.UpdateStatusAsync(
        //        orderId, orderStatus, paymentStatus);

        //    // ======= CHECK IF CONDITION MET =======
        //    // Trigger quantity decrease ONLY when:
        //    // 1. Order just became "Shipped"
        //    // 2. Payment is "Paid"
        //    // 3. Was NOT already "Shipped" before (prevent double decrease)

        //    bool justShipped = orderStatus == "Shipped"
        //                    && paymentStatus == "Paid"
        //                    && existingOrder?.OrderStatus != "Shipped";

        //    if (justShipped)
        //    {
        //        //  Decrease product quantities
        //        int updated = await _orderService
        //            .DecreaseProductQuantityAsync(orderId);


        //        //  Auto mark out-of-stock products inactive
        //        await _orderService.UpdateStockStatusAsync();

        //        // After existing DecreaseProductQuantityAsync call, add:
        //        await _sizeService.DecreaseQuantityAsync(orderId);

        //        TempData["Success"] = updated > 0
        //            ? $"Order status updated! Stock decreased for {updated} product(s)."
        //            : "Order status updated!";
        //    }
        //    else
        //    {
        //        TempData["Success"] = "Order status updated successfully!";
        //    }

        //    return RedirectToAction(nameof(Details), new { id = orderId });
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(
    int orderId,
    string orderStatus,
    string paymentStatus)
        {
            // ✅ STEP 1: Get BEFORE update
            var existingOrder = await _orderService.GetByIdAsync(orderId);

            string oldOrderStatus = existingOrder?.OrderStatus ?? "";
            string oldPaymentStatus = existingOrder?.PaymentStatus ?? "";

            // ✅ STEP 2: Update status in DB
            await _orderService.UpdateStatusAsync(
                orderId, orderStatus, paymentStatus);

            // ✅ STEP 3: Trigger when Delivered + Paid
            //            AND was NOT already Delivered+Paid before
            bool isNowDeliveredAndPaid =
                orderStatus == "Delivered" &&
                paymentStatus == "Paid";

            bool wasAlreadyDeliveredAndPaid =
                oldOrderStatus == "Delivered" &&
                oldPaymentStatus == "Paid";

            bool shouldDecrease = isNowDeliveredAndPaid
                               && !wasAlreadyDeliveredAndPaid;

            if (shouldDecrease)
            {
                // ✅ Decrease overall product quantity
                int updated = await _orderService
                    .DecreaseProductQuantityAsync(orderId);

                // ✅ Decrease size-specific quantity
                await _sizeService.DecreaseQuantityAsync(orderId);

                // ✅ Auto deactivate out-of-stock products
                await _orderService.UpdateStockStatusAsync();

                TempData["Success"] = updated > 0
                    ? $"✅ Delivered! Stock reduced for {updated} product(s)."
                    : "✅ Status updated!";
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