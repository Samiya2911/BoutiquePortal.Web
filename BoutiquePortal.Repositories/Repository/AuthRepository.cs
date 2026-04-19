using BoutiquePortal.Model.Models;
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
    public class AuthRepository : IAuthRepository
    {
        private readonly string _connectionString;

        public AuthRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<Admin?> GetAdminByEmailAsync(string email)
        {
            using var conn = new SqlConnection(_connectionString);
            return await conn.QueryFirstOrDefaultAsync<Admin>(
                "sp_Admin_GetByEmail",
                new { Email = email },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<Vendor?> GetVendorByEmailAsync(string email)
        {
            using var conn = new SqlConnection(_connectionString);
            return await conn.QueryFirstOrDefaultAsync<Vendor>(
                "sp_Vendor_GetByEmail",
                new { Email = email },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<int> RegisterVendorAsync(Vendor vendor)
        {
            using var conn = new SqlConnection(_connectionString);
            var p = new DynamicParameters();
            p.Add("@VendorName", vendor.VendorName);
            p.Add("@Email", vendor.Email);
            p.Add("@Password", vendor.Password);
            p.Add("@BrandName", vendor.BrandName);
            p.Add("@Phone", vendor.Phone);
            p.Add("@Address", vendor.Address);
            p.Add("@CountryId", vendor.CountryId);
            p.Add("@StateId", vendor.StateId);
            p.Add("@CityId", vendor.CityId);
            p.Add("@IsApproved", false);   // always false on self-register
            p.Add("@IsActive", true);

            // reuse your existing sp_Vendor_Insert
            return await conn.ExecuteScalarAsync<int>(
                "sp_Vendor_Insert",
                p,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
