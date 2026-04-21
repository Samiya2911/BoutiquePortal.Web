using BoutiquePortal.Model.Models;
using BoutiquePortal.Model.ViewModels;
using BoutiquePortal.Repositories.Interfaces;
using BoutiquePortal.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoutiquePortal.Services.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repo;
        public OrderService(IOrderRepository repo) => _repo = repo;

        public Task<IEnumerable<Order>> GetAllAsync()
            => _repo.GetAllAsync();

        public Task<Order?> GetByIdAsync(int orderId)
            => _repo.GetByIdAsync(orderId);

        public Task<IEnumerable<VendorOrderVM>> GetByVendorAsync(int vendorId)
            => _repo.GetByVendorAsync(vendorId);

        public Task<int> AddAsync(Order order)
            => _repo.AddAsync(order);

        public Task<int> AddItemAsync(OrderItem item)
            => _repo.AddItemAsync(item);

        public Task<int> UpdateStatusAsync(int orderId,
            string orderStatus, string paymentStatus)
            => _repo.UpdateStatusAsync(orderId, orderStatus, paymentStatus);

        public Task<int> DeleteAsync(int orderId)
            => _repo.DeleteAsync(orderId);

        public Task<VendorEarningsVM> GetEarningsAsync(int vendorId)
            => _repo.GetEarningsAsync(vendorId);

        public Task<IEnumerable<Order>> GetByCustomerAsync(int customerId)
    => _repo.GetByCustomerAsync(customerId);
    }
}
