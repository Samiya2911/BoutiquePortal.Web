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
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;

        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Product>> GetAllAsync() => _repo.GetAllAsync();

        public Task<Product> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

        public Task<int> AddAsync(Product entity) => _repo.AddAsync(entity);

        public Task<int> UpdateAsync(Product entity) => _repo.UpdateAsync(entity);

        public Task<int> DeleteAsync(int id) => _repo.DeleteAsync(id);

        public async Task<IEnumerable<Product>> GetByVendorId(int vendorId)
        {
            return await _repo.GetByVendorId(vendorId);
        }
    }
}
