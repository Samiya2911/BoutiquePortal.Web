using BoutiquePortal.Model.Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace BoutiquePortal.Web.Helpers
{
    public static class CartHelper
    {
        private const string CartKey = "ShoppingCart";

        // ======= GET CART =======
        public static List<CartItem> GetCart(ISession session)
        {
            var cartJson = session.GetString(CartKey);
            if (string.IsNullOrEmpty(cartJson))
                return new List<CartItem>();

            return JsonSerializer.Deserialize<List<CartItem>>(cartJson)
                   ?? new List<CartItem>();
        }

        // ======= SAVE CART =======
        public static void SaveCart(ISession session, List<CartItem> cart)
        {
            session.SetString(CartKey, JsonSerializer.Serialize(cart));
        }

        // ======= ADD ITEM =======
        public static void AddItem(ISession session, CartItem newItem)
        {
            var cart = GetCart(session);
            var existing = cart.FirstOrDefault(c => c.ProductId == newItem.ProductId);

            if (existing != null)
                existing.Quantity += newItem.Quantity; // increase qty
            else
                cart.Add(newItem);

            SaveCart(session, cart);
        }

        // ======= REMOVE ITEM =======
        public static void RemoveItem(ISession session, int productId)
        {
            var cart = GetCart(session);
            cart.RemoveAll(c => c.ProductId == productId);
            SaveCart(session, cart);
        }

        // ======= UPDATE QUANTITY =======
        public static void UpdateQuantity(ISession session, int productId, int quantity)
        {
            var cart = GetCart(session);
            var item = cart.FirstOrDefault(c => c.ProductId == productId);

            if (item != null)
            {
                if (quantity <= 0)
                    cart.RemoveAll(c => c.ProductId == productId);
                else
                    item.Quantity = quantity;
            }

            SaveCart(session, cart);
        }

        // ======= CLEAR CART =======
        public static void ClearCart(ISession session)
        {
            session.Remove(CartKey);
        }

        // ======= GET COUNT =======
        public static int GetCartCount(ISession session)
        {
            return GetCart(session).Sum(c => c.Quantity);
        }

        // ======= GET TOTAL =======
        public static decimal GetCartTotal(ISession session)
        {
            return GetCart(session).Sum(c => c.SubTotal);
        }
    }
}