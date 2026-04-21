using BoutiquePortal.Services.Interfaces;
using BoutiquePortal.Web.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BoutiquePortal.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [CustomerAuthFilter]
    public class DashboardController : Controller
    {
        private readonly IOrderService _orderService;

        public DashboardController(IOrderService orderService)
            => _orderService = orderService;

        public async Task<IActionResult> Index()
        {
            int customerId = HttpContext.Session.GetInt32("CustomerId") ?? 0;

            ViewBag.CustomerName = HttpContext.Session.GetString("CustomerName");
            ViewBag.CustomerEmail = HttpContext.Session.GetString("CustomerEmail");

            return View();
        }
    }
}