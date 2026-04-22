using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoutiquePortal.Model.ViewModels;
using BoutiquePortal.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BoutiquePortal.Repositories.Repository
{
    public class AdminDashboardRepository : IAdminDashboardRepository
    {
        private readonly string _conn;

        public AdminDashboardRepository(IConfiguration cfg)
            => _conn = cfg.GetConnectionString("DefaultConnection")!;

        public async Task<AdminDashboardVM> GetStatsAsync()
        {
            using var conn = new SqlConnection(_conn);
            return await conn.QueryFirstOrDefaultAsync<AdminDashboardVM>(
                "sp_Admin_DashboardStats",
                commandType: CommandType.StoredProcedure)
                ?? new AdminDashboardVM();
        }

        public async Task<List<MonthlyRevenueData>> GetMonthlyRevenueAsync()
        {
            using var conn = new SqlConnection(_conn);
            var result = await conn.QueryAsync<MonthlyRevenueData>(
                "sp_Admin_MonthlyRevenue",
                commandType: CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<List<OrderStatusData>> GetOrderStatusBreakdownAsync()
        {
            using var conn = new SqlConnection(_conn);
            var result = await conn.QueryAsync<OrderStatusData>(
                "sp_Admin_OrderStatusBreakdown",
                commandType: CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<List<TopProductData>> GetTopProductsAsync()
        {
            using var conn = new SqlConnection(_conn);
            var result = await conn.QueryAsync<TopProductData>(
                "sp_Admin_TopProducts",
                commandType: CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<List<RecentOrderData>> GetRecentOrdersAsync()
        {
            using var conn = new SqlConnection(_conn);
            var result = await conn.QueryAsync<RecentOrderData>(
                "sp_Admin_RecentOrders",
                commandType: CommandType.StoredProcedure);
            return result.ToList();
        }
    }
}