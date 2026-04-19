using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoutiquePortal.Model.ViewModels
{
    public class VendorEarningsVM
    {
        public decimal TotalEarnings { get; set; }
        public decimal ThisMonthEarnings { get; set; }
        public decimal PendingAmount { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public List<MonthlyEarning> MonthlyBreakdown { get; set; }
    }

    public class MonthlyEarning
    {
        public string Month { get; set; }
        public decimal Amount { get; set; }
        public int OrderCount { get; set; }
    }
}
