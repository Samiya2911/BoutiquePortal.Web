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
    public class PasswordResetRepository : IPasswordResetRepository
    {
        private readonly string _conn;

        public PasswordResetRepository(IConfiguration cfg)
            => _conn = cfg.GetConnectionString("DefaultConnection")!;

        public async Task<int> InsertTokenAsync(
            string email, string userType,
            string token, DateTime expiry)
        {
            using var conn = new SqlConnection(_conn);
            return await conn.ExecuteScalarAsync<int>(
                "sp_PasswordReset_Insert",
                new
                {
                    Email = email,
                    UserType = userType,
                    Token = token,
                    ExpiryTime = expiry
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<PasswordResetToken?> GetByTokenAsync(string token)
        {
            using var conn = new SqlConnection(_conn);
            return await conn.QueryFirstOrDefaultAsync<PasswordResetToken>(
                "sp_PasswordReset_GetByToken",
                new { Token = token },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> MarkUsedAsync(string token)
        {
            using var conn = new SqlConnection(_conn);
            return await conn.ExecuteScalarAsync<int>(
                "sp_PasswordReset_MarkUsed",
                new { Token = token },
                commandType: CommandType.StoredProcedure);
        }
    }
}