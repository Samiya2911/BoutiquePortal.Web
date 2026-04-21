using BoutiquePortal.Services.Interfaces;
using BoutiquePortal.Web.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BoutiquePortal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthFilter]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
            => _customerService = customerService;

        // ======= INDEX =======
        public async Task<IActionResult> Index()
        {
            var list = await _customerService.GetAllAsync();
            return View(list);
        }

        // ======= TOGGLE STATUS =======
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id, bool isActive)
        {
            await _customerService.ToggleStatusAsync(id, isActive);
            TempData["Success"] = isActive
                ? "Customer activated successfully!"
                : "Customer deactivated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // ======= DELETE =======
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _customerService.DeleteAsync(id);
            return Ok();
        }
    }
}