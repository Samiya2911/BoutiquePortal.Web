using BoutiquePortal.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoutiquePortal.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(int customerId);
        Task<Customer?> GetByEmailAsync(string email);
        Task<(bool success, string message, Customer? customer)> LoginAsync(string email, string password);
        Task<(bool success, string message)> RegisterAsync(Customer customer);
        Task<(bool success, string message)> UpdateProfileAsync(Customer customer);
        Task<(bool success, string message)> UpdatePasswordAsync(int customerId, string currentPassword, string newPassword);
        Task<int> ToggleStatusAsync(int customerId, bool isActive);
        Task<int> DeleteAsync(int customerId);
    }
}
