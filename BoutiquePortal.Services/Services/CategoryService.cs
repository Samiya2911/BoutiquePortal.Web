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
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;

        public CategoryService(ICategoryRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Category>> GetAllAsync() => _repo.GetAllAsync();
        public Task<Category> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task<int> AddAsync(Category entity) => _repo.AddAsync(entity);
        public Task<int> UpdateAsync(Category entity) => _repo.UpdateAsync(entity);
        public Task<int> DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}
