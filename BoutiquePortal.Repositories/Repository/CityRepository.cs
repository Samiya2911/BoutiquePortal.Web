using BoutiquePortal.Model.Models;
using BoutiquePortal.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BoutiquePortal.Repositories.Repository
{
    public class CityRepository : ICityRepository
    {
        private readonly string _connectionString;

        public CityRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        // GET ALL
        public async Task<IEnumerable<City>> GetAllAsync()
        {
            using var conn = new SqlConnection(_connectionString);

            return await conn.QueryAsync<City>(
                "sp_City_GetAll",
                commandType: CommandType.StoredProcedure
            );
        }

        // GET BY ID
        public async Task<City> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);

            return await conn.QueryFirstOrDefaultAsync<City>(
                "sp_City_GetById",
                new { CityId = id },
                commandType: CommandType.StoredProcedure
            ) ?? new City();
        }

        // ADD
        public async Task<int> AddAsync(City entity)
        {
            using var conn = new SqlConnection(_connectionString);

            var p = new DynamicParameters();
            p.Add("@CityName", entity.CityName);
            p.Add("@CityCode", entity.CityCode);
            p.Add("@StateId", entity.StateId);
            p.Add("@IsActive", entity.IsActive);

            return await conn.ExecuteScalarAsync<int>(
                "sp_City_Insert",
                p,
                commandType: CommandType.StoredProcedure
            );
        }

        // UPDATE
        public async Task<int> UpdateAsync(City entity)
        {
            using var conn = new SqlConnection(_connectionString);

            var p = new DynamicParameters();
            p.Add("@CityId", entity.CityId);
            p.Add("@CityName", entity.CityName);
            p.Add("@CityCode", entity.CityCode);
            p.Add("@StateId", entity.StateId);
            p.Add("@IsActive", entity.IsActive);

            return await conn.ExecuteScalarAsync<int>(
                "sp_City_Update",
                p,
                commandType: CommandType.StoredProcedure
            );
        }

        // DELETE
        public async Task<int> DeleteAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);

            return await conn.ExecuteScalarAsync<int>(
                "sp_City_Delete",
                new { CityId = id },
                commandType: CommandType.StoredProcedure
            );
        }

        // 🔥 GET BY STATE (FOR DROPDOWN)
        public async Task<IEnumerable<City>> GetByStateId(int stateId)
        {
            using var conn = new SqlConnection(_connectionString);

            return await conn.QueryAsync<City>(
                "sp_City_GetByState",
                new { StateId = stateId },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}