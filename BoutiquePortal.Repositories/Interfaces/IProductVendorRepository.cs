using BoutiquePortal.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoutiquePortal.Repositories.Interfaces
{
    public interface IProductVendorRepository
    {
        Task<IEnumerable<Product>> GetByVendorAsync(int vendorId);
        Task<Product?> GetByIdForVendorAsync(int productId, int vendorId);
        Task<int> AddAsync(Product product);
        Task<int> UpdateAsync(Product product);
        Task<int> DeleteAsync(int productId, int vendorId);
    }
}
