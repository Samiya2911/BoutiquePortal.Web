using BoutiquePortal.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoutiquePortal.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(int customerId);
        Task<Customer?> GetByEmailAsync(string email);
        Task<int> AddAsync(Customer customer);
        Task<int> UpdateAsync(Customer customer);
        Task<int> UpdatePasswordAsync(int customerId, string newPassword);
        Task<int> ToggleStatusAsync(int customerId, bool isActive);
        Task<int> DeleteAsync(int customerId);
    }
}
