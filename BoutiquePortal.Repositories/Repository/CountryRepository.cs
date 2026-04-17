using BoutiquePortal.Model.Models;
using BoutiquePortal.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BoutiquePortal.Repositories.Repository
{
    public class CountryRepository : ICountryRepository
    {
        private readonly string _connectionString;

        public CountryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        // GET ALL
        public async Task<IEnumerable<Country>> GetAllAsync()
        {
            using var conn = new SqlConnection(_connectionString);

            return await conn.QueryAsync<Country>(
                "sp_Country_GetAll",
                commandType: CommandType.StoredProcedure
            );
        }

        // GET BY ID
        public async Task<Country> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);

            return await conn.QueryFirstOrDefaultAsync<Country>(
                "sp_Country_GetById",
                new { CountryId = id },
                commandType: CommandType.StoredProcedure
            ) ?? new Country();
        }

        // ADD
        public async Task<int> AddAsync(Country entity)
        {
            using var conn = new SqlConnection(_connectionString);

            var p = new DynamicParameters();
            p.Add("@CountryName", entity.CountryName);
            p.Add("@CountryCode", entity.CountryCode);
            p.Add("@IsActive", entity.IsActive);

            return await conn.ExecuteScalarAsync<int>(
                "sp_Country_Insert",
                p,
                commandType: CommandType.StoredProcedure
            );
        }

        // UPDATE
        public async Task<int> UpdateAsync(Country entity)
        {
            using var conn = new SqlConnection(_connectionString);

            var p = new DynamicParameters();
            p.Add("@CountryId", entity.CountryId);
            p.Add("@CountryName", entity.CountryName);
            p.Add("@CountryCode", entity.CountryCode);
            p.Add("@IsActive", entity.IsActive);

            return await conn.ExecuteScalarAsync<int>(
                "sp_Country_Update",
                p,
                commandType: CommandType.StoredProcedure
            );
        }

        // DELETE
        public async Task<int> DeleteAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);

            return await conn.ExecuteScalarAsync<int>(
                "sp_Country_Delete",
                new { CountryId = id },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}