//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using BoutiquePortal.Model.Models;
//using BoutiquePortal.Repositories.Interfaces;
//using BoutiquePortal.Services.Interfaces;
//using BoutiquePortal.Web.Helpers;

//namespace BoutiquePortal.Services.Services
//{
//    public class AuthService : IAuthService
//    {
//        private readonly IAuthRepository _repo;

//        public AuthService(IAuthRepository repo) => _repo = repo;

//        public async Task<Admin?> ValidateAdminAsync(string email, string password)
//        {
//            var admin = await _repo.GetAdminByEmailAsync(email);
//            if (admin == null || admin.Password != password) return null;
//            return admin;
//        }

//        public async Task<(bool success, string message, Vendor? vendor)> ValidateVendorAsync(string email, string password)
//        {
//            var vendor = await _repo.GetVendorByEmailAsync(email);

//            if (vendor == null || vendor.Password != password)
//                return (false, "Invalid email or password.", null);

//            if (!vendor.IsApproved)
//                return (false, "Your account is pending admin approval.", null);

//            if (!vendor.IsActive)
//                return (false, "Your account has been deactivated.", null);

//            return (true, "Success", vendor);
//        }

//        public async Task<(bool success, string message)> RegisterVendorAsync(Vendor vendor)
//        {
//            var existing = await _repo.GetVendorByEmailAsync(vendor.Email);
//            if (existing != null)
//                return (false, "This email is already registered.");

//            vendor.CreatedDate = DateTime.Now;
//            vendor.IsApproved = false;
//            vendor.IsActive = true;

//            await _repo.RegisterVendorAsync(vendor);
//            return (true, "Registration successful! Please wait for admin approval.");
//        }
//    }
//}


using BoutiquePortal.Model.Helpers;   // ✅ ADD
using BoutiquePortal.Model.Models;
using BoutiquePortal.Repositories.Interfaces;
using BoutiquePortal.Services.Interfaces;

namespace BoutiquePortal.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repo;

        public AuthService(IAuthRepository repo) => _repo = repo;

        // ======= VALIDATE ADMIN =======
        public async Task<Admin?> ValidateAdminAsync(
            string email, string password)
        {
            var admin = await _repo.GetAdminByEmailAsync(email);
            if (admin == null) return null;

            // ✅ Verify using PasswordHelper (handles plain + hashed)
            if (!PasswordHelper.Verify(password, admin.Password))
                return null;

            // ✅ If password was plain text, re-save as hashed
            if (!PasswordHelper.IsHashed(admin.Password))
            {
                await _repo.UpdateAdminPasswordAsync(
                    admin.AdminId, PasswordHelper.Hash(password));
            }

            return admin;
        }

        // ======= VALIDATE VENDOR =======
        public async Task<(bool success, string message, Vendor? vendor)>
            ValidateVendorAsync(string email, string password)
        {
            var vendor = await _repo.GetVendorByEmailAsync(email);

            if (vendor == null || !PasswordHelper.Verify(password, vendor.Password))
                return (false, "Invalid email or password.", null);

            if (!vendor.IsApproved)
                return (false, "Your account is pending admin approval.", null);

            if (!vendor.IsActive)
                return (false, "Your account has been deactivated.", null);

            // ✅ Re-save as hashed if still plain text
            if (!PasswordHelper.IsHashed(vendor.Password))
            {
                await _repo.UpdateVendorPasswordAsync(
                    vendor.VendorId, PasswordHelper.Hash(password));
            }

            return (true, "Success", vendor);
        }

        // ======= REGISTER VENDOR =======
        public async Task<(bool success, string message)>
            RegisterVendorAsync(Vendor vendor)
        {
            var existing = await _repo.GetVendorByEmailAsync(vendor.Email);
            if (existing != null)
                return (false, "This email is already registered.");

            // ✅ Hash password before saving
            vendor.Password = PasswordHelper.Hash(vendor.Password);
            vendor.CreatedDate = DateTime.Now;
            vendor.IsApproved = false;
            vendor.IsActive = true;

            await _repo.RegisterVendorAsync(vendor);
            return (true, "Registration successful! Please wait for admin approval.");
        }
    }
}