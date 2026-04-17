using BoutiquePortal.Model.Models;
using BoutiquePortal.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BoutiquePortal.Repositories.Repository
{
    public class StateRepository : IStateRepository
    {
        private readonly string _connectionString;

        public StateRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        // GET ALL
        public async Task<IEnumerable<State>> GetAllAsync()
        {
            using var conn = new SqlConnection(_connectionString);

            return await conn.QueryAsync<State>(
                "sp_State_GetAll",
                commandType: CommandType.StoredProcedure
            );
        }

        // GET BY ID
        public async Task<State> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);

            return await conn.QueryFirstOrDefaultAsync<State>(
                "sp_State_GetById",
                new { StateId = id },
                commandType: CommandType.StoredProcedure
            ) ?? new State();
        }

        // ADD
        public async Task<int> AddAsync(State entity)
        {
            using var conn = new SqlConnection(_connectionString);

            var p = new DynamicParameters();
            p.Add("@StateName", entity.StateName);
            p.Add("@StateCode", entity.StateCode);
            p.Add("@CountryId", entity.CountryId);
            p.Add("@IsActive", entity.IsActive);

            return await conn.ExecuteScalarAsync<int>(
                "sp_State_Insert",
                p,
                commandType: CommandType.StoredProcedure
            );
        }

        // UPDATE
        public async Task<int> UpdateAsync(State entity)
        {
            using var conn = new SqlConnection(_connectionString);

            var p = new DynamicParameters();
            p.Add("@StateId", entity.StateId);
            p.Add("@StateName", entity.StateName);
            p.Add("@StateCode", entity.StateCode);
            p.Add("@CountryId", entity.CountryId);
            p.Add("@IsActive", entity.IsActive);

            return await conn.ExecuteScalarAsync<int>(
                "sp_State_Update",
                p,
                commandType: CommandType.StoredProcedure
            );
        }

        // DELETE
        public async Task<int> DeleteAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);

            return await conn.ExecuteScalarAsync<int>(
                "sp_State_Delete",
                new { StateId = id },
                commandType: CommandType.StoredProcedure
            );
        }

        // 🔥 GET BY COUNTRY (FOR DROPDOWN)
        public async Task<IEnumerable<State>> GetByCountryId(int countryId)
        {
            using var conn = new SqlConnection(_connectionString);

            return await conn.QueryAsync<State>(
                "sp_State_GetByCountry",
                new { CountryId = countryId },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}