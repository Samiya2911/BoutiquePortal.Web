using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoutiquePortal.Model.Models;
using BoutiquePortal.Repositories.Interfaces;
using BoutiquePortal.Services.Interfaces;

namespace BoutiquePortal.Services.Services
{
    public class SubCategoryService : ISubCategoryService
    {
        private readonly ISubCategoryRepository _repo;

        public SubCategoryService(ISubCategoryRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<SubCategory>> GetAllAsync() => _repo.GetAllAsync();
        public Task<SubCategory> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task<int> AddAsync(SubCategory entity) => _repo.AddAsync(entity);
        public Task<int> UpdateAsync(SubCategory entity) => _repo.UpdateAsync(entity);
        public Task<int> DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}
