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
    public class ProductSizeRepository : IProductSizeRepository
    {
        private readonly string _conn;

        public ProductSizeRepository(IConfiguration cfg)
            => _conn = cfg.GetConnectionString("DefaultConnection")!;

        public async Task<IEnumerable<ProductSize>> GetByProductAsync(int productId)
        {
            using var conn = new SqlConnection(_conn);
            return await conn.QueryAsync<ProductSize>(
                "sp_ProductSize_GetByProduct",
                new { ProductId = productId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> AddAsync(int productId, string sizeName, int quantity)
        {
            using var conn = new SqlConnection(_conn);
            return await conn.ExecuteScalarAsync<int>(
                "sp_ProductSize_Insert",
                new { ProductId = productId, SizeName = sizeName, Quantity = quantity },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> UpdateAsync(int sizeId, string sizeName, int quantity)
        {
            using var conn = new SqlConnection(_conn);
            return await conn.ExecuteScalarAsync<int>(
                "sp_ProductSize_Update",
                new { SizeId = sizeId, SizeName = sizeName, Quantity = quantity },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteAsync(int sizeId)
        {
            using var conn = new SqlConnection(_conn);
            return await conn.ExecuteScalarAsync<int>(
                "sp_ProductSize_Delete",
                new { SizeId = sizeId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteByProductAsync(int productId)
        {
            using var conn = new SqlConnection(_conn);
            return await conn.ExecuteScalarAsync<int>(
                "sp_ProductSize_DeleteByProduct",
                new { ProductId = productId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DecreaseQuantityAsync(int orderId)
        {
            using var conn = new SqlConnection(_conn);
            return await conn.ExecuteScalarAsync<int>(
                "sp_ProductSize_DecreaseQuantity",
                new { OrderId = orderId },
                commandType: CommandType.StoredProcedure);
        }
    }
}