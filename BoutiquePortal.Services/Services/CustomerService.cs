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
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repo;

        public CustomerService(ICustomerRepository repo) => _repo = repo;

        // ======= GET ALL =======
        public Task<IEnumerable<Customer>> GetAllAsync()
            => _repo.GetAllAsync();

        // ======= GET BY ID =======
        public Task<Customer?> GetByIdAsync(int customerId)
            => _repo.GetByIdAsync(customerId);

        // ======= GET BY EMAIL =======
        public Task<Customer?> GetByEmailAsync(string email)
            => _repo.GetByEmailAsync(email);

        // ======= LOGIN =======
        public async Task<(bool success, string message, Customer? customer)> LoginAsync(
            string email, string password)
        {
            var customer = await _repo.GetByEmailAsync(email);

            if (customer == null || customer.Password != password)
                return (false, "Invalid email or password.", null);

            if (!customer.IsActive)
                return (false, "Your account has been deactivated.", null);

            return (true, "Success", customer);
        }

        // ======= REGISTER =======
        public async Task<(bool success, string message)> RegisterAsync(Customer customer)
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
        public async Task<(bool success, string message)> UpdateProfileAsync(Customer customer)
        {
            var result = await _repo.UpdateAsync(customer);
            return result > 0
                ? (true, "Profile updated successfully!")
                : (false, "Failed to update profile.");
        }

        // ======= UPDATE PASSWORD =======
        public async Task<(bool success, string message)> UpdatePasswordAsync(
            int customerId, string currentPassword, string newPassword)
        {
            var customer = await _repo.GetByIdAsync(customerId);

            if (customer == null)
                return (false, "Customer not found.");

            if (customer.Password != currentPassword)
                return (false, "Current password is incorrect.");

            await _repo.UpdatePasswordAsync(customerId, newPassword);
            return (true, "Password updated successfully!");
        }

        // ======= TOGGLE STATUS =======
        public Task<int> ToggleStatusAsync(int customerId, bool isActive)
            => _repo.ToggleStatusAsync(customerId, isActive);

        // ======= DELETE =======
        public Task<int> DeleteAsync(int customerId)
            => _repo.DeleteAsync(customerId);
    }
}