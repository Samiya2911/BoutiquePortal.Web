using BoutiquePortal.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoutiquePortal.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<Admin?> GetAdminByEmailAsync(string email);
        Task<Vendor?> GetVendorByEmailAsync(string email);
        Task<int> RegisterVendorAsync(Vendor vendor);

        Task UpdateAdminPasswordAsync(int adminId, string hashedPassword);
        Task UpdateVendorPasswordAsync(int vendorId, string hashedPassword);
    }
}
