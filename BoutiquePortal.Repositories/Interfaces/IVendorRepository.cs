using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoutiquePortal.Model.Models;

namespace BoutiquePortal.Repositories.Interfaces
{
    public interface IVendorRepository
    {
        Task<IEnumerable<Vendor>> GetAllAsync();
        Task<Vendor> GetByIdAsync(int id);
        Task<int> AddAsync(Vendor vendor);
        Task<int> UpdateAsync(Vendor vendor);
        Task<int> DeleteAsync(int id);
    }

}
