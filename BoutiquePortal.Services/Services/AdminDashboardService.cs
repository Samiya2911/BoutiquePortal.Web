using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoutiquePortal.Model.ViewModels;
using BoutiquePortal.Repositories.Interfaces;
using BoutiquePortal.Services.Interfaces;

namespace BoutiquePortal.Services.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IAdminDashboardRepository _repo;

        public AdminDashboardService(IAdminDashboardRepository repo)
            => _repo = repo;

        public async Task<AdminDashboardVM> GetFullDashboardAsync()
        {
            // Run all queries in parallel for speed
            var statsTask = _repo.GetStatsAsync();
            var monthlyTask = _repo.GetMonthlyRevenueAsync();
            var statusTask = _repo.GetOrderStatusBreakdownAsync();
            var topProductsTask = _repo.GetTopProductsAsync();
            var recentOrdersTask = _repo.GetRecentOrdersAsync();

            await Task.WhenAll(statsTask, monthlyTask,
                               statusTask, topProductsTask, recentOrdersTask);

            var vm = await statsTask;
            vm.MonthlyRevenue = await monthlyTask;
            vm.OrderStatusBreakdown = await statusTask;
            vm.TopProducts = await topProductsTask;
            vm.RecentOrders = await recentOrdersTask;

            return vm;
        }
    }
}