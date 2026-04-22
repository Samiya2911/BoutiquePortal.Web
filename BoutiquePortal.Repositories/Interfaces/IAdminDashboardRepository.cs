using BoutiquePortal.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoutiquePortal.Repositories.Interfaces
{
    public interface IAdminDashboardRepository
    {
        Task<AdminDashboardVM> GetStatsAsync();
        Task<List<MonthlyRevenueData>> GetMonthlyRevenueAsync();
        Task<List<OrderStatusData>> GetOrderStatusBreakdownAsync();
        Task<List<TopProductData>> GetTopProductsAsync();
        Task<List<RecentOrderData>> GetRecentOrdersAsync();
    }
}
