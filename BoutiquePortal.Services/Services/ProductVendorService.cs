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
    public class ProductVendorService : IProductVendorService
    {
        private readonly IProductVendorRepository _repo;
        public ProductVendorService(IProductVendorRepository repo) => _repo = repo;

        public Task<IEnumerable<Product>> GetByVendorAsync(int vendorId)
            => _repo.GetByVendorAsync(vendorId);

        public Task<Product?> GetByIdForVendorAsync(int productId, int vendorId)
            => _repo.GetByIdForVendorAsync(productId, vendorId);

        public Task<int> AddAsync(Product p) => _repo.AddAsync(p);
        public Task<int> UpdateAsync(Product p) => _repo.UpdateAsync(p);

        public Task<int> DeleteAsync(int productId, int vendorId)
            => _repo.DeleteAsync(productId, vendorId);
    }
}
