using BoutiquePortal.Model.Helpers;
using BoutiquePortal.Model.Models;
using BoutiquePortal.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoutiquePortal.Repositories.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly string _conn;

        public CustomerRepository(IConfiguration cfg)
            => _conn = cfg.GetConnectionString("DefaultConnection")!;

        // ======= GET ALL =======
        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            using var conn = new SqlConnection(_conn);
            return await conn.QueryAsync<Customer>(
                "sp_Customer_GetAll",
                commandType: CommandType.StoredProcedure);
        }

        // ======= GET BY ID =======
        public async Task<Customer?> GetByIdAsync(int customerId)
        {
            using var conn = new SqlConnection(_conn);
            return await conn.QueryFirstOrDefaultAsync<Customer>(
                "sp_Customer_GetById",
                new { CustomerId = customerId },
                commandType: CommandType.StoredProcedure);
        }

        // ======= GET BY EMAIL =======
        public async Task<Customer?> GetByEmailAsync(string email)
        {
            using var conn = new SqlConnection(_conn);
            return await conn.QueryFirstOrDefaultAsync<Customer>(
                "sp_Customer_GetByEmail",
                new { Email = email },
                commandType: CommandType.StoredProcedure);
        }

        // ======= ADD =======
        public async Task<int> AddAsync(Customer customer)
        {
            using var conn = new SqlConnection(_conn);
            var p = new DynamicParameters();
            p.Add("@FullName", customer.FullName);
            p.Add("@Email", customer.Email);
            //p.Add("@Password", customer.Password);
            p.Add("@Password", PasswordHelper.Hash(customer.Password)); // ✅ HASH
            p.Add("@Phone", customer.Phone);

            return await conn.ExecuteScalarAsync<int>(
                "sp_Customer_Insert",p,
                commandType: CommandType.StoredProcedure);
        }

        // ======= UPDATE PROFILE =======
        public async Task<int> UpdateAsync(Customer customer)
        {
            using var conn = new SqlConnection(_conn);
            var p = new DynamicParameters();
            p.Add("@CustomerId", customer.CustomerId);
            p.Add("@FullName", customer.FullName);
            p.Add("@Phone", customer.Phone);

            return await conn.ExecuteScalarAsync<int>(
                "sp_Customer_Update",
                p,
                commandType: CommandType.StoredProcedure);
        }

        // ======= UPDATE PASSWORD =======
        public async Task<int> UpdatePasswordAsync(int customerId, string newPassword)
        {
            using var conn = new SqlConnection(_conn);
            return await conn.ExecuteScalarAsync<int>(
                "sp_Customer_UpdatePassword",
                new { CustomerId = customerId, NewPassword = newPassword },
                commandType: CommandType.StoredProcedure);
        }

        // ======= TOGGLE STATUS =======
        public async Task<int> ToggleStatusAsync(int customerId, bool isActive)
        {
            using var conn = new SqlConnection(_conn);
            return await conn.ExecuteScalarAsync<int>(
                "sp_Customer_ToggleStatus",
                new { CustomerId = customerId, IsActive = isActive },
                commandType: CommandType.StoredProcedure);
        }

       
        public async Task<Customer?> GetByIdWithPasswordAsync(int customerId)
        {
            using var conn = new SqlConnection(_conn);
            return await conn.QueryFirstOrDefaultAsync<Customer>(
                "sp_Customer_GetByIdWithPassword",
                new { CustomerId = customerId },
                commandType: CommandType.StoredProcedure);
        }

        // ======= DELETE =======
        public async Task<int> DeleteAsync(int customerId)
        {
            using var conn = new SqlConnection(_conn);
            return await conn.ExecuteScalarAsync<int>(
                "sp_Customer_Delete",
                new { CustomerId = customerId },
                commandType: CommandType.StoredProcedure);
        }
    }
}