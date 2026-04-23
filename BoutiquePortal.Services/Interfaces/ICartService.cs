using BoutiquePortal.Model.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoutiquePortal.Services.Interfaces
{
    public interface ICartService
    {
        Task<IEnumerable<CartDbItem>> GetCartAsync(int customerId);
        Task<int> AddToCartAsync(int customerId, int productId, int quantity);
        Task<int> UpdateQuantityAsync(int customerId, int productId, int quantity);
        Task<int> RemoveItemAsync(int customerId, int productId);
        Task<int> ClearCartAsync(int customerId);
        Task<List<CartItem>> GetCartItemsForSessionAsync(int customerId);

        // Task SyncSessionToDbAsync(int customerId, ISession session);
        //Task LoadDbToSessionAsync(int customerId, ISession session);
    }
}