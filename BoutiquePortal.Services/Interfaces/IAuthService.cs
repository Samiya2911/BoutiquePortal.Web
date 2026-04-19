using BoutiquePortal.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoutiquePortal.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Admin?> ValidateAdminAsync(string email, string password);
        Task<(bool success, string message, Vendor? vendor)> ValidateVendorAsync(string email, string password);
        Task<(bool success, string message)> RegisterVendorAsync(Vendor vendor);
    }
}
