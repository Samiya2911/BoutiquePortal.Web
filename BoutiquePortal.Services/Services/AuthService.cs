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
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repo;

        public AuthService(IAuthRepository repo) => _repo = repo;

        public async Task<Admin?> ValidateAdminAsync(string email, string password)
        {
            var admin = await _repo.GetAdminByEmailAsync(email);
            if (admin == null || admin.Password != password) return null;
            return admin;
        }

        public async Task<(bool success, string message, Vendor? vendor)> ValidateVendorAsync(string email, string password)
        {
            var vendor = await _repo.GetVendorByEmailAsync(email);

            if (vendor == null || vendor.Password != password)
                return (false, "Invalid email or password.", null);

            if (!vendor.IsApproved)
                return (false, "Your account is pending admin approval.", null);

            if (!vendor.IsActive)
                return (false, "Your account has been deactivated.", null);

            return (true, "Success", vendor);
        }

        public async Task<(bool success, string message)> RegisterVendorAsync(Vendor vendor)
        {
            var existing = await _repo.GetVendorByEmailAsync(vendor.Email);
            if (existing != null)
                return (false, "This email is already registered.");

            vendor.CreatedDate = DateTime.Now;
            vendor.IsApproved = false;
            vendor.IsActive = true;

            await _repo.RegisterVendorAsync(vendor);
            return (true, "Registration successful! Please wait for admin approval.");
        }
    }
}