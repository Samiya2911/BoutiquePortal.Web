using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoutiquePortal.Model.Helpers;
using BoutiquePortal.Repositories.Interfaces;
using BoutiquePortal.Services.Interfaces;

namespace BoutiquePortal.Services.Services
{
    public class PasswordResetService : IPasswordResetService
    {
        private readonly IPasswordResetRepository _resetRepo;
        private readonly IAuthRepository _authRepo;
        private readonly ICustomerRepository _customerRepo;

        public PasswordResetService(
            IPasswordResetRepository resetRepo,
            IAuthRepository authRepo,
            ICustomerRepository customerRepo)
        {
            _resetRepo = resetRepo;
            _authRepo = authRepo;
            _customerRepo = customerRepo;
        }

        // ======= GENERATE TOKEN =======
        public async Task<(bool success, string message, string token)>
            GenerateTokenAsync(string email, string userType)
        {
            // Check email exists
            bool emailExists = false;

            if (userType == "Vendor")
            {
                var vendor = await _authRepo.GetVendorByEmailAsync(email);
                emailExists = vendor != null;
            }
            else if (userType == "Customer")
            {
                var customer = await _customerRepo.GetByEmailAsync(email);
                emailExists = customer != null;
            }

            if (!emailExists)
                return (false, "No account found with this email.", string.Empty);

            // Generate secure random token
            string token = Guid.NewGuid().ToString("N")
                         + Guid.NewGuid().ToString("N");  // 64 char token

            DateTime expiry = DateTime.Now.AddMinutes(30);

            await _resetRepo.InsertTokenAsync(email, userType, token, expiry);

            return (true, "Token generated successfully.", token);
        }

        // ======= RESET PASSWORD =======
        public async Task<(bool success, string message)>
            ResetPasswordAsync(string token, string newPassword)
        {
            var resetToken = await _resetRepo.GetByTokenAsync(token);

            if (resetToken == null)
                return (false, "Invalid or expired reset link. Please request a new one.");

            if (resetToken.ExpiryTime < DateTime.Now)
                return (false, "This reset link has expired. Please request a new one.");

            string hashedPassword = PasswordHelper.Hash(newPassword);

            // Update password based on user type
            if (resetToken.UserType == "Vendor")
            {
                await _authRepo.UpdateVendorPasswordByEmailAsync(
                    resetToken.Email, hashedPassword);
            }
            else if (resetToken.UserType == "Customer")
            {
                await _customerRepo.UpdatePasswordByEmailAsync(
                    resetToken.Email, hashedPassword);
            }

            // Mark token as used
            await _resetRepo.MarkUsedAsync(token);

            return (true, "Password reset successfully! You can now login.");
        }
    }
}