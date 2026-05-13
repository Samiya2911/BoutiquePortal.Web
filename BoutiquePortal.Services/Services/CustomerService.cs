//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using BoutiquePortal.Model.Models;
//using BoutiquePortal.Repositories.Interfaces;
//using BoutiquePortal.Services.Interfaces;
//using BoutiquePortal.Model.Helpers;

//namespace BoutiquePortal.Services.Services
//{
//    public class CustomerService : ICustomerService
//    {
//        private readonly ICustomerRepository _repo;

//        public CustomerService(ICustomerRepository repo) => _repo = repo;

//        // ======= GET ALL =======
//        public Task<IEnumerable<Customer>> GetAllAsync()
//            => _repo.GetAllAsync();

//        // ======= GET BY ID =======
//        public Task<Customer?> GetByIdAsync(int customerId)
//            => _repo.GetByIdAsync(customerId);

//        // ======= GET BY EMAIL =======
//        public Task<Customer?> GetByEmailAsync(string email)
//            => _repo.GetByEmailAsync(email);

//        // ======= LOGIN =======
//        public async Task<(bool success, string message, Customer? customer)> LoginAsync(
//            string email, string password)
//        {
//            var customer = await _repo.GetByEmailAsync(email);

//            if (customer == null || customer.Password != password)
//                return (false, "Invalid email or password.", null);

//            if (!customer.IsActive)
//                return (false, "Your account has been deactivated.", null);

//            return (true, "Success", customer);
//        }

//        // ======= REGISTER =======
//        public async Task<(bool success, string message)> RegisterAsync(Customer customer)
//        {
//            var existing = await _repo.GetByEmailAsync(customer.Email);
//            if (existing != null)
//                return (false, "This email is already registered.");

//            var result = await _repo.AddAsync(customer);

//            if (result == -1)
//                return (false, "This email is already registered.");

//            return (true, "Registration successful! You can now login.");
//        }

//        // ======= UPDATE PROFILE =======
//        public async Task<(bool success, string message)> UpdateProfileAsync(Customer customer)
//        {
//            var result = await _repo.UpdateAsync(customer);
//            return result > 0
//                ? (true, "Profile updated successfully!")
//                : (false, "Failed to update profile.");
//        }

//        // ======= UPDATE PASSWORD =======
//        public async Task<(bool success, string message)> UpdatePasswordAsync(
//               int customerId, string currentPassword, string newPassword)
//        {
//            //  Use dedicated method that returns password
//            var customer = await _repo.GetByIdWithPasswordAsync(customerId);

//            if (customer == null)
//                return (false, "Customer not found.");

//            //  Trim to avoid any whitespace issues
//            string storedPassword = customer.Password?.Trim() ?? string.Empty;
//            string enteredPassword = currentPassword?.Trim() ?? string.Empty;

//            if (storedPassword != enteredPassword)
//                return (false, "Current password is incorrect.");

//            await _repo.UpdatePasswordAsync(customerId, newPassword.Trim());
//            return (true, "Password updated successfully!");
//        }


//        // ======= TOGGLE STATUS =======
//        public Task<int> ToggleStatusAsync(int customerId, bool isActive)
//            => _repo.ToggleStatusAsync(customerId, isActive);

//        // ======= DELETE =======
//        public Task<int> DeleteAsync(int customerId)
//            => _repo.DeleteAsync(customerId);
//    }
//}


using BoutiquePortal.Model.Helpers;   
using BoutiquePortal.Model.Models;
using BoutiquePortal.Repositories.Interfaces;
using BoutiquePortal.Services.Interfaces;

namespace BoutiquePortal.Services.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repo;

        public CustomerService(ICustomerRepository repo) => _repo = repo;

        public Task<IEnumerable<Customer>> GetAllAsync()
            => _repo.GetAllAsync();

        public Task<Customer?> GetByIdAsync(int customerId)
            => _repo.GetByIdAsync(customerId);

        public Task<Customer?> GetByEmailAsync(string email)
            => _repo.GetByEmailAsync(email);

        // ======= LOGIN =======
        public async Task<(bool success, string message, Customer? customer)>
            LoginAsync(string email, string password)
        {
            var customer = await _repo.GetByEmailAsync(email);

            if (customer == null)
                return (false, "Invalid email or password.", null);

            // Verify — handles both plain text and hashed
            if (!PasswordHelper.Verify(password, customer.Password))
                return (false, "Invalid email or password.", null);

            if (!customer.IsActive)
                return (false, "Your account has been deactivated.", null);

            // Re-save as hashed if still plain text
            if (!PasswordHelper.IsHashed(customer.Password))
            {
                await _repo.UpdatePasswordAsync(
                    customer.CustomerId,
                    PasswordHelper.Hash(password));
            }

            return (true, "Success", customer);
        }

        // ======= REGISTER =======
        public async Task<(bool success, string message)>
            RegisterAsync(Customer customer)
        {
            var existing = await _repo.GetByEmailAsync(customer.Email);
            if (existing != null)
                return (false, "This email is already registered.");

            var result = await _repo.AddAsync(customer);
            if (result == -1)
                return (false, "This email is already registered.");

            return (true, "Registration successful! You can now login.");
        }

        // ======= UPDATE PROFILE =======
        public async Task<(bool success, string message)>
            UpdateProfileAsync(Customer customer)
        {
            var result = await _repo.UpdateAsync(customer);
            return result > 0
                ? (true, "Profile updated successfully!")
                : (false, "Failed to update profile.");
        }

        // ======= UPDATE PASSWORD =======
        public async Task<(bool success, string message)>
            UpdatePasswordAsync(
                int customerId,
                string currentPassword,
                string newPassword)
        {
            // Use GetByIdWithPassword to get password field
            var customer = await _repo.GetByIdWithPasswordAsync(customerId);

            if (customer == null)
                return (false, "Customer not found.");

            // Verify current password (handles plain + hashed)
            if (!PasswordHelper.Verify(currentPassword, customer.Password))
                return (false, "Current password is incorrect.");

            // Save new password as hash
            await _repo.UpdatePasswordAsync(
                customerId,
                PasswordHelper.Hash(newPassword));

            return (true, "Password updated successfully!");
        }

        public Task<int> ToggleStatusAsync(int customerId, bool isActive)
            => _repo.ToggleStatusAsync(customerId, isActive);

        public Task<int> DeleteAsync(int customerId)
            => _repo.DeleteAsync(customerId);
    }
}