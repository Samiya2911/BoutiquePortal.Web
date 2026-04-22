using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoutiquePortal.Model.ViewModels
{
    public class AdminDashboardVM
    {
        // ======= STATS CARDS =======
        public int TotalVendors { get; set; }
        public int PendingVendors { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal ThisMonthRevenue { get; set; }

        // ======= CHARTS =======
        public List<MonthlyRevenueData> MonthlyRevenue { get; set; } = new();
        public List<OrderStatusData> OrderStatusBreakdown { get; set; } = new();
        public List<TopProductData> TopProducts { get; set; } = new();

        // ======= RECENT ORDERS =======
        public List<RecentOrderData> RecentOrders { get; set; } = new();
    }

    public class MonthlyRevenueData
    {
        public string MonthName { get; set; } = string.Empty;
        public int MonthNum { get; set; }
        public int YearNum { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }

    public class OrderStatusData
    {
        public string OrderStatus { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class TopProductData
    {
        public string ProductName { get; set; } = string.Empty;
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class RecentOrderData
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
    }
}