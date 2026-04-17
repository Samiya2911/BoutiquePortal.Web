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
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        // ================== GET ALL ==================
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            using var conn = new SqlConnection(_connectionString);

            return await conn.QueryAsync<Product>(
                "sp_Product_GetAll",
                commandType: CommandType.StoredProcedure
            );
        }

        // ================== GET BY ID ==================
        public async Task<Product> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);

            return await conn.QueryFirstOrDefaultAsync<Product>(
                "sp_Product_GetById",
                new { ProductId = id },
                commandType: CommandType.StoredProcedure
            ) ?? new Product();
        }

        // ================== ADD ==================
        public async Task<int> AddAsync(Product entity)
        {
            using var conn = new SqlConnection(_connectionString);

            var p = new DynamicParameters();
            p.Add("@ProductName", entity.ProductName);
            p.Add("@Price", entity.Price);
            p.Add("@DiscountPrice", entity.DiscountPrice);
            p.Add("@Description", entity.Description);
            p.Add("@CategoryId", entity.CategoryId);
            p.Add("@SubCategoryId", entity.SubCategoryId);
            p.Add("@VendorId", entity.VendorId);
            p.Add("@BrandName", entity.BrandName);
            p.Add("@Quantity", entity.Quantity);
            p.Add("@ProductImage", entity.ProductImage);
            p.Add("@IsActive", entity.IsActive);

            return await conn.ExecuteScalarAsync<int>(
                "sp_Product_Insert",
                p,
                commandType: CommandType.StoredProcedure
            );
        }

        // ================== UPDATE ==================
        public async Task<int> UpdateAsync(Product entity)
        {
            using var conn = new SqlConnection(_connectionString);

            var p = new DynamicParameters();
            p.Add("@ProductId", entity.ProductId);
            p.Add("@ProductName", entity.ProductName);
            p.Add("@Price", entity.Price);
            p.Add("@DiscountPrice", entity.DiscountPrice);
            p.Add("@Description", entity.Description);
            p.Add("@CategoryId", entity.CategoryId);
            p.Add("@SubCategoryId", entity.SubCategoryId);
            p.Add("@Quantity", entity.Quantity);
            p.Add("@ProductImage", entity.ProductImage);
            p.Add("@IsActive", entity.IsActive);

            return await conn.ExecuteScalarAsync<int>(
                "sp_Product_Update",
                p,
                commandType: CommandType.StoredProcedure
            );
        }

        // ================== DELETE ==================
        public async Task<int> DeleteAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);

            return await conn.ExecuteScalarAsync<int>(
                "sp_Product_Delete",
                new { ProductId = id },
                commandType: CommandType.StoredProcedure
            );
        }

        // ================== GET BY VENDOR ==================
        public async Task<IEnumerable<Product>> GetByVendorId(int vendorId)
        {
            using var conn = new SqlConnection(_connectionString);

            return await conn.QueryAsync<Product>(
                "sp_Product_GetByVendor",
                new { VendorId = vendorId },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
