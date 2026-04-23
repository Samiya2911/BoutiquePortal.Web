using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoutiquePortal.Model.Models;
using BoutiquePortal.Repositories.Interfaces;
using BoutiquePortal.Services.Interfaces;
//using BoutiquePortal.Web.Helpers;
using Microsoft.AspNetCore.Http;

namespace BoutiquePortal.Services.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _repo;

        public CartService(ICartRepository repo) => _repo = repo;

        public Task<IEnumerable<CartDbItem>> GetCartAsync(int customerId)
            => _repo.GetByCustomerAsync(customerId);

        public Task<int> AddToCartAsync(
            int customerId, int productId, int quantity)
            => _repo.AddOrUpdateAsync(customerId, productId, quantity);

        public Task<int> UpdateQuantityAsync(
            int customerId, int productId, int quantity)
            => _repo.UpdateQuantityAsync(customerId, productId, quantity);

        public Task<int> RemoveItemAsync(int customerId, int productId)
            => _repo.RemoveItemAsync(customerId, productId);

        public Task<int> ClearCartAsync(int customerId)
            => _repo.ClearAsync(customerId);

        // ✅ Sync Session → DB (called on logout)

        public async Task<List<CartItem>> GetCartItemsForSessionAsync(
            int customerId)
        {
            var dbCart = await _repo.GetByCustomerAsync(customerId);

            return dbCart.Select(d => new CartItem
            {
                ProductId = d.ProductId,
                ProductName = d.ProductName,
                BrandName = d.BrandName ?? string.Empty,
                ProductImage = d.ProductImage ?? string.Empty,
                Price = d.Price,
                DiscountPrice = d.DiscountPrice,
                Quantity = d.Quantity,
                VendorId = d.VendorId
            }).ToList();
        }


        //public async Task SyncSessionToDbAsync(int customerId, ISession session)
        //{
        //    var sessionCart = CartHelper.GetCart(session);

        //    foreach (var item in sessionCart)
        //    {
        //        await _repo.AddOrUpdateAsync(
        //            customerId, item.ProductId, item.Quantity);
        //    }
        //}

        //// ✅ Load DB → Session (called on login)
        //public async Task LoadDbToSessionAsync(int customerId, ISession session)
        //{
        //    var dbCart = await _repo.GetByCustomerAsync(customerId);

        //    // Convert DB cart to Session CartItems
        //    var sessionItems = dbCart.Select(d => new CartItem
        //    {
        //        ProductId = d.ProductId,
        //        ProductName = d.ProductName,
        //        BrandName = d.BrandName ?? string.Empty,
        //        ProductImage = d.ProductImage ?? string.Empty,
        //        Price = d.Price,
        //        DiscountPrice = d.DiscountPrice,
        //        Quantity = d.Quantity,
        //        VendorId = d.VendorId
        //    }).ToList();

        //    // Save to session
        //    CartHelper.SaveCart(session, sessionItems);
        //}
    }
}