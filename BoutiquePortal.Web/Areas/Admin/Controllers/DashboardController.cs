using BoutiquePortal.Services.Interfaces;
using BoutiquePortal.Web.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BoutiquePortal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthFilter]
    public class DashboardController : Controller
    {
        private readonly IAdminDashboardService _dashboardService;

        public DashboardController(IAdminDashboardService dashboardService)
            => _dashboardService = dashboardService;

        public async Task<IActionResult> Index()
        {
            var vm = await _dashboardService.GetFullDashboardAsync();
            return View(vm);
        }
    }
}