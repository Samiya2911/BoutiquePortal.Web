using BoutiquePortal.Model.Models;
using BoutiquePortal.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoutiquePortal.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order?> GetByIdAsync(int orderId);
        Task<IEnumerable<VendorOrderVM>> GetByVendorAsync(int vendorId);
        Task<int> AddAsync(Order order);
        Task<int> AddItemAsync(OrderItem item);
        Task<int> UpdateStatusAsync(int orderId, string orderStatus, string paymentStatus);
        Task<int> DeleteAsync(int orderId);
        Task<VendorEarningsVM> GetEarningsAsync(int vendorId);
        Task<IEnumerable<Order>> GetByCustomerAsync(int customerId);
    }
}
