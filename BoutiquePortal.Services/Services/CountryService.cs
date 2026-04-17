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
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _repo;

        public CountryService(ICountryRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Country>> GetAllAsync() => _repo.GetAllAsync();
        public Task<Country> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task<int> AddAsync(Country entity) => _repo.AddAsync(entity);
        public Task<int> UpdateAsync(Country entity) => _repo.UpdateAsync(entity);
        public Task<int> DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}