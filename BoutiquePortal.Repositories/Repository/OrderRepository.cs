using BoutiquePortal.Model.Models;
using BoutiquePortal.Model.ViewModels;
using BoutiquePortal.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoutiquePortal.Repositories.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _conn;

        public OrderRepository(IConfiguration cfg)
            => _conn = cfg.GetConnectionString("DefaultConnection")!;

        // ======= GET ALL (Admin) =======
        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            using var conn = new SqlConnection(_conn);
            return await conn.QueryAsync<Order>(
                "sp_Order_GetAll",
                commandType: CommandType.StoredProcedure);
        }

        // ======= GET BY ID (with items) =======
        public async Task<Order?> GetByIdAsync(int orderId)
        {
            using var conn = new SqlConnection(_conn);

            using var multi = await conn.QueryMultipleAsync(
                "sp_Order_GetById",
                new { OrderId = orderId },
                commandType: CommandType.StoredProcedure);

            var order = await multi.ReadFirstOrDefaultAsync<Order>();
            if (order != null)
                order.Items = (await multi.ReadAsync<OrderItem>()).ToList();

            return order;
        }

        // ======= GET BY VENDOR =======
        public async Task<IEnumerable<VendorOrderVM>> GetByVendorAsync(int vendorId)
        {
            using var conn = new SqlConnection(_conn);
            return await conn.QueryAsync<VendorOrderVM>(
                "sp_Order_GetByVendor",
                new { VendorId = vendorId },
                commandType: CommandType.StoredProcedure);
        }

        // ======= ADD ORDER =======
        public async Task<int> AddAsync(Order order)
        {
            using var conn = new SqlConnection(_conn);
            var p = new DynamicParameters();
            p.Add("@CustomerId", order.CustomerId);
            p.Add("@TotalAmount", order.TotalAmount);
            p.Add("@ShippingName", order.ShippingName);
            p.Add("@ShippingPhone", order.ShippingPhone);
            p.Add("@ShippingAddress", order.ShippingAddress);
            p.Add("@ShippingCity", order.ShippingCity);
            p.Add("@ShippingState", order.ShippingState);
            p.Add("@ShippingPincode", order.ShippingPincode);
            p.Add("@PaymentMethod", order.PaymentMethod);
            p.Add("@PaymentStatus", order.PaymentStatus);
            p.Add("@TransactionId", order.TransactionId);

            return await conn.ExecuteScalarAsync<int>(
                "sp_Order_Insert", p,
                commandType: CommandType.StoredProcedure);
        }

        // ======= ADD ORDER ITEM =======
        public async Task<int> AddItemAsync(OrderItem item)
        {
            using var conn = new SqlConnection(_conn);
            var p = new DynamicParameters();
            p.Add("@OrderId", item.OrderId);
            p.Add("@ProductId", item.ProductId);
            p.Add("@VendorId", item.VendorId);
            p.Add("@Quantity", item.Quantity);
            p.Add("@UnitPrice", item.UnitPrice);
            p.Add("@TotalPrice", item.TotalPrice);

            return await conn.ExecuteScalarAsync<int>(
                "sp_OrderItem_Insert", p,
                commandType: CommandType.StoredProcedure);
        }

        // ======= UPDATE STATUS =======
        public async Task<int> UpdateStatusAsync(int orderId,
            string orderStatus, string paymentStatus)
        {
            using var conn = new SqlConnection(_conn);
            return await conn.ExecuteScalarAsync<int>(
                "sp_Order_UpdateStatus",
                new
                {
                    OrderId = orderId,
                    OrderStatus = orderStatus,
                    PaymentStatus = paymentStatus
                },
                commandType: CommandType.StoredProcedure);
        }

        // ======= DELETE =======
        public async Task<int> DeleteAsync(int orderId)
        {
            using var conn = new SqlConnection(_conn);
            return await conn.ExecuteScalarAsync<int>(
                "sp_Order_Delete",
                new { OrderId = orderId },
                commandType: CommandType.StoredProcedure);
        }

        // ======= VENDOR EARNINGS =======
        public async Task<VendorEarningsVM> GetEarningsAsync(int vendorId)
        {
            using var conn = new SqlConnection(_conn);
            return await conn.QueryFirstOrDefaultAsync<VendorEarningsVM>(
                "sp_Vendor_GetEarnings",
                new { VendorId = vendorId },
                commandType: CommandType.StoredProcedure)
                ?? new VendorEarningsVM();
        }

        public async Task<IEnumerable<Order>> GetByCustomerAsync(int customerId)
        {
            using var conn = new SqlConnection(_conn);
            return await conn.QueryAsync<Order>(
                "sp_Order_GetByCustomer",
                new { CustomerId = customerId },
                commandType: CommandType.StoredProcedure);
        }
    }
}
