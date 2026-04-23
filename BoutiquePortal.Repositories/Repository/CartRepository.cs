using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoutiquePortal.Model.Models;
using BoutiquePortal.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BoutiquePortal.Repositories.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly string _conn;

        public CartRepository(IConfiguration cfg)
            => _conn = cfg.GetConnectionString("DefaultConnection")!;

        public async Task<IEnumerable<CartDbItem>> GetByCustomerAsync(int customerId)
        {
            using var conn = new SqlConnection(_conn);
            return await conn.QueryAsync<CartDbItem>(
                "sp_Cart_GetByCustomer",
                new { CustomerId = customerId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> AddOrUpdateAsync(
            int customerId, int productId, int quantity)
        {
            using var conn = new SqlConnection(_conn);
            return await conn.ExecuteScalarAsync<int>(
                "sp_Cart_AddOrUpdate",
                new
                {
                    CustomerId = customerId,
                    ProductId = productId,
                    Quantity = quantity
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> UpdateQuantityAsync(
            int customerId, int productId, int quantity)
        {
            using var conn = new SqlConnection(_conn);
            return await conn.ExecuteScalarAsync<int>(
                "sp_Cart_UpdateQuantity",
                new
                {
                    CustomerId = customerId,
                    ProductId = productId,
                    Quantity = quantity
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> RemoveItemAsync(int customerId, int productId)
        {
            using var conn = new SqlConnection(_conn);
            return await conn.ExecuteScalarAsync<int>(
                "sp_Cart_RemoveItem",
                new
                {
                    CustomerId = customerId,
                    ProductId = productId
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> ClearAsync(int customerId)
        {
            using var conn = new SqlConnection(_conn);
            return await conn.ExecuteScalarAsync<int>(
                "sp_Cart_Clear",
                new { CustomerId = customerId },
                commandType: CommandType.StoredProcedure);
        }
    }
}