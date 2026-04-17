using BoutiquePortal.Model.Models;
using BoutiquePortal.Repositories.Interfaces;
using BoutiquePortal.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoutiquePortal.Services.Services
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _repo;

        public CityService(ICityRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<City>> GetAllAsync() => _repo.GetAllAsync();
        public Task<City> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task<int> AddAsync(City entity) => _repo.AddAsync(entity);
        public Task<int> UpdateAsync(City entity) => _repo.UpdateAsync(entity);
        public Task<int> DeleteAsync(int id) => _repo.DeleteAsync(id);
        public async Task<IEnumerable<City>> GetByStateId(int stateId)
        {
            return await _repo.GetByStateId(stateId);
        }
    }
}
