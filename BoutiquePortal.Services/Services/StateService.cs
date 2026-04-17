using BoutiquePortal.Model.Models;
using BoutiquePortal.Repositories.Interfaces;
using BoutiquePortal.Repositories.Repository;
using BoutiquePortal.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoutiquePortal.Services.Services
{
    public class StateService : IStateService
    {
        private readonly IStateRepository _repo;

        public StateService(IStateRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<State>> GetAllAsync() => _repo.GetAllAsync();
        public Task<State> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task<int> AddAsync(State entity) => _repo.AddAsync(entity);
        public Task<int> UpdateAsync(State entity) => _repo.UpdateAsync(entity);
        public Task<int> DeleteAsync(int id) => _repo.DeleteAsync(id);
        public async Task<IEnumerable<State>> GetByCountryId(int countryId)
        {
            return await _repo.GetByCountryId(countryId);
        }

    }
}
