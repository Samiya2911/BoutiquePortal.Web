using BoutiquePortal.Model.Models;
using BoutiquePortal.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace BoutiquePortal.Repositories.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly string _connectionString;

        public CategoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        // ================== GET ALL ==================
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            using var conn = new SqlConnection(_connectionString);

            return await conn.QueryAsync<Category>(
                "sp_GetAllCategories",
                commandType: CommandType.StoredProcedure
            );
        }

        // ================== GET BY ID ==================
        public async Task<Category> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);

            return await conn.QueryFirstOrDefaultAsync<Category>(
                "sp_GetCategoryById",
                new { CategoryId = id },
                commandType: CommandType.StoredProcedure
            ) ?? new Category();
        }

        // ================== ADD ==================
        public async Task<int> AddAsync(Category entity)
        {
            using var conn = new SqlConnection(_connectionString);

            var p = new DynamicParameters();
            p.Add("@CategoryName", entity.CategoryName);
            p.Add("@Description", entity.Description);
            p.Add("@CategoryImage", entity.CategoryImage);   //  IMAGE
            p.Add("@IsActive", entity.IsActive);
            p.Add("@CreatedDate", entity.CreatedDate);

            return await conn.ExecuteScalarAsync<int>(
                "sp_AddCategory",
                p,
                commandType: CommandType.StoredProcedure
            );
        }

        // ================== UPDATE ==================
        public async Task<int> UpdateAsync(Category entity)
        {
            using var conn = new SqlConnection(_connectionString);

            var p = new DynamicParameters();
            p.Add("@CategoryId", entity.CategoryId);
            p.Add("@CategoryName", entity.CategoryName);
            p.Add("@Description", entity.Description);
            p.Add("@CategoryImage", entity.CategoryImage);   //  IMAGE
            p.Add("@IsActive", entity.IsActive);

            return await conn.ExecuteScalarAsync<int>(
                "sp_UpdateCategory",
                p,
                commandType: CommandType.StoredProcedure
            );
        }

        // ================== DELETE ==================
        public async Task<int> DeleteAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);

            return await conn.ExecuteScalarAsync<int>(
                "sp_DeleteCategory",
                new { CategoryId = id },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}