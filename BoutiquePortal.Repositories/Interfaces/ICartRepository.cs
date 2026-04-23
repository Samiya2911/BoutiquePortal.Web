using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoutiquePortal.Model.Models;

namespace BoutiquePortal.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<IEnumerable<CartDbItem>> GetByCustomerAsync(int customerId);
        Task<int> AddOrUpdateAsync(int customerId, int productId, int quantity);
        Task<int> UpdateQuantityAsync(int customerId, int productId, int quantity);
        Task<int> RemoveItemAsync(int customerId, int productId);
        Task<int> ClearAsync(int customerId);
    }
}