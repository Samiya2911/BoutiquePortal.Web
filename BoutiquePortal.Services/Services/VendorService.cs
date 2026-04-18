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
    public class VendorService : IVendorService
    {
        private readonly IVendorRepository _repo;

        public VendorService(IVendorRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Vendor>> GetAllAsync() => _repo.GetAllAsync();
        public Task<Vendor> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task<int> AddAsync(Vendor entity) => _repo.AddAsync(entity);
        public Task<int> UpdateAsync(Vendor entity) => _repo.UpdateAsync(entity);
        public Task<int> DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}
