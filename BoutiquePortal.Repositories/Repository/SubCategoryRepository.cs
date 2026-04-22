using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoutiquePortal.Model.Models;
using BoutiquePortal.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;


namespace BoutiquePortal.Repositories.Repository
{
    public class SubCategoryRepository : ISubCategoryRepository
    {
        private readonly string _connectionString;

        public SubCategoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        // ================== GET ALL ==================
        public async Task<IEnumerable<SubCategory>> GetAllAsync()
        {
            using var conn = new SqlConnection(_connectionString);

            return await conn.QueryAsync<SubCategory>(
                "sp_SubCategories_GetAll",
                commandType: CommandType.StoredProcedure
            );
        }

        // ================== GET BY ID ==================
        public async Task<SubCategory> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);

            return await conn.QueryFirstOrDefaultAsync<SubCategory>(
                "sp_SubCategory_GetById",
                new { SubCategoryId = id },
                commandType: CommandType.StoredProcedure
            ) ?? new SubCategory();
        }

        // ================== ADD ==================
        public async Task<int> AddAsync(SubCategory entity)
        {
            using var conn = new SqlConnection(_connectionString);

            var p = new DynamicParameters();
            p.Add("@SubCategoryName", entity.SubCategoryName);
            p.Add("@Description", entity.Description);
            p.Add("@SubCategoryImage", entity.SubCategoryImage);
            p.Add("@CategoryId", entity.CategoryId);
            p.Add("@IsActive", entity.IsActive);
            p.Add("@CreatedDate", entity.CreatedDate);

            return await conn.ExecuteScalarAsync<int>(
                "sp_SubCategory_Insert",
                p,
                commandType: CommandType.StoredProcedure
            );
        }

        // ================== UPDATE ==================
        public async Task<int> UpdateAsync(SubCategory entity)
        {
            using var conn = new SqlConnection(_connectionString);

            var p = new DynamicParameters();
            p.Add("@SubCategoryId", entity.SubCategoryId);
            p.Add("@SubCategoryName", entity.SubCategoryName);
            p.Add("@Description", entity.Description);
            p.Add("@SubCategoryImage", entity.SubCategoryImage);
            p.Add("@CategoryId", entity.CategoryId);
            p.Add("@IsActive", entity.IsActive);

            return await conn.ExecuteScalarAsync<int>(
                "sp_SubCategory_Update",
                p,
                commandType: CommandType.StoredProcedure
            );
        }

        // ================== DELETE ==================
        public async Task<int> DeleteAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);

            return await conn.ExecuteScalarAsync<int>(
                "sp_SubCategory_Delete",
                new { SubCategoryId = id },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<SubCategory>> GetByCategoryId(int categoryId)
        {
            using var conn = new SqlConnection(_connectionString);
            return await conn.QueryAsync<SubCategory>(
                "sp_SubCategory_GetByCategory",
                new { CategoryId = categoryId },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
