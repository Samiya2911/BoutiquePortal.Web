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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int orderId,
            string orderStatus, string paymentStatus)
        {
            await _orderService.UpdateStatusAsync(orderId, orderStatus, paymentStatus);
            TempData["Success"] = "Order status updated!";
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