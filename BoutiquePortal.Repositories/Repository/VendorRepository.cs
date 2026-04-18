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
    public class VendorRepository : IVendorRepository
    {
        private readonly string _connectionString;

        public VendorRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        // ================== GET ALL ==================
        public async Task<IEnumerable<Vendor>> GetAllAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            return await conn.QueryAsync<Vendor>(
                "sp_Vendor_GetAll",
                commandType: CommandType.StoredProcedure
            );
        }

        // ================== GET BY ID ==================
        public async Task<Vendor> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            return await conn.QueryFirstOrDefaultAsync<Vendor>(
                "sp_Vendor_GetById",
                new { VendorId = id },
                commandType: CommandType.StoredProcedure
            ) ?? new Vendor();
        }

        // ================== ADD ==================
        public async Task<int> AddAsync(Vendor entity)
        {
            using var conn = new SqlConnection(_connectionString);
            var p = new DynamicParameters();
            p.Add("@VendorName", entity.VendorName);
            p.Add("@Email", entity.Email);
            p.Add("@Password", entity.Password);
            p.Add("@BrandName", entity.BrandName);
            p.Add("@Phone", entity.Phone);
            p.Add("@Address", entity.Address);
            p.Add("@CountryId", entity.CountryId);
            p.Add("@StateId", entity.StateId);
            p.Add("@CityId", entity.CityId);
            p.Add("@IsApproved", entity.IsApproved);
            p.Add("@IsActive", entity.IsActive);

            return await conn.ExecuteScalarAsync<int>(
                "sp_Vendor_Insert",
                p,
                commandType: CommandType.StoredProcedure
            );
        }

        // ================== UPDATE ==================
        public async Task<int> UpdateAsync(Vendor entity)
        {
            using var conn = new SqlConnection(_connectionString);
            var p = new DynamicParameters();
            p.Add("@VendorId", entity.VendorId);
            p.Add("@VendorName", entity.VendorName);
            p.Add("@Email", entity.Email);
            p.Add("@Password", entity.Password);
            p.Add("@BrandName", entity.BrandName);
            p.Add("@Phone", entity.Phone);
            p.Add("@Address", entity.Address);
            p.Add("@CountryId", entity.CountryId);
            p.Add("@StateId", entity.StateId);
            p.Add("@CityId", entity.CityId);
            p.Add("@IsApproved", entity.IsApproved);
            p.Add("@IsActive", entity.IsActive);

            return await conn.ExecuteScalarAsync<int>(
                "sp_Vendor_Update",
                p,
                commandType: CommandType.StoredProcedure
            );
        }

        // ================== DELETE ==================
        public async Task<int> DeleteAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            return await conn.ExecuteScalarAsync<int>(
                "sp_Vendor_Delete",
                new { VendorId = id },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
