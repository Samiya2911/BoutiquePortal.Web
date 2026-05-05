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
    public class ProductSizeService : IProductSizeService
    {
        private readonly IProductSizeRepository _repo;

        public ProductSizeService(IProductSizeRepository repo) => _repo = repo;

        public Task<IEnumerable<ProductSize>> GetByProductAsync(int productId)
            => _repo.GetByProductAsync(productId);

        // Save all sizes: delete existing then re-insert
        public async Task SaveSizesAsync(
            int productId, List<string> names, List<int> quantities)
        {
            // Delete all existing sizes for this product
            await _repo.DeleteByProductAsync(productId);

            // Re-insert valid sizes
            for (int i = 0; i < names.Count; i++)
            {
                var name = names[i].Trim();
                var qty = quantities.Count > i ? quantities[i] : 0;

                if (!string.IsNullOrWhiteSpace(name))
                    await _repo.AddAsync(productId, name, qty);
            }
        }

        public Task<int> DecreaseQuantityAsync(int orderId)
            => _repo.DecreaseQuantityAsync(orderId);
    }
}