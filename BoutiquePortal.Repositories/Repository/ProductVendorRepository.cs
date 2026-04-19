using BoutiquePortal.Model.Models;
using BoutiquePortal.Repositories.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace BoutiquePortal.Repositories.Repository
{
    public class ProductVendorRepository : IProductVendorRepository
    {
        private readonly string _conn;

        public ProductVendorRepository(IConfiguration cfg)
            => _conn = cfg.GetConnectionString("DefaultConnection")!;

        // ======= GET ALL BY VENDOR =======
        public async Task<IEnumerable<Product>> GetByVendorAsync(int vendorId)
        {
            using var conn = new SqlConnection(_conn);
            return await conn.QueryAsync<Product>(
                "sp_Product_GetByVendor",
                new { VendorId = vendorId },
                commandType: CommandType.StoredProcedure);
        }

        // ======= GET BY ID (vendor security check) =======
        public async Task<Product?> GetByIdForVendorAsync(int productId, int vendorId)
        {
            using var conn = new SqlConnection(_conn);
            return await conn.QueryFirstOrDefaultAsync<Product>(
                "sp_Product_GetByIdForVendor",
                new { ProductId = productId, VendorId = vendorId },
                commandType: CommandType.StoredProcedure);
        }

        // ======= ADD =======
        public async Task<int> AddAsync(Product p)
        {
            using var conn = new SqlConnection(_conn);
            var dp = new DynamicParameters();
            dp.Add("@ProductName", p.ProductName);
            dp.Add("@Price", p.Price);
            dp.Add("@DiscountPrice", p.DiscountPrice);
            dp.Add("@Description", p.Description);
            dp.Add("@CategoryId", p.CategoryId);
            dp.Add("@SubCategoryId", p.SubCategoryId);
            dp.Add("@VendorId", p.VendorId);
            dp.Add("@BrandName", p.BrandName);
            dp.Add("@Quantity", p.Quantity);
            dp.Add("@ProductImage", p.ProductImage);
            dp.Add("@IsActive", p.IsActive);

            return await conn.ExecuteScalarAsync<int>(
                "sp_Product_InsertByVendor", dp,
                commandType: CommandType.StoredProcedure);
        }

        // ======= UPDATE =======
        public async Task<int> UpdateAsync(Product p)
        {
            using var conn = new SqlConnection(_conn);
            var dp = new DynamicParameters();
            dp.Add("@ProductId", p.ProductId);
            dp.Add("@VendorId", p.VendorId);   // security
            dp.Add("@ProductName", p.ProductName);
            dp.Add("@Price", p.Price);
            dp.Add("@DiscountPrice", p.DiscountPrice);
            dp.Add("@Description", p.Description);
            dp.Add("@CategoryId", p.CategoryId);
            dp.Add("@SubCategoryId", p.SubCategoryId);
            dp.Add("@BrandName", p.BrandName);
            dp.Add("@Quantity", p.Quantity);
            dp.Add("@ProductImage", p.ProductImage);
            dp.Add("@IsActive", p.IsActive);

            return await conn.ExecuteScalarAsync<int>(
                "sp_Product_UpdateByVendor", dp,
                commandType: CommandType.StoredProcedure);
        }

        // ======= DELETE =======
        public async Task<int> DeleteAsync(int productId, int vendorId)
        {
            using var conn = new SqlConnection(_conn);
            return await conn.ExecuteScalarAsync<int>(
                "sp_Product_DeleteByVendor",
                new { ProductId = productId, VendorId = vendorId },
                commandType: CommandType.StoredProcedure);
        }
    }
}
