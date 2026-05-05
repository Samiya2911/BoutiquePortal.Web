using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoutiquePortal.Model.Models;

namespace BoutiquePortal.Repositories.Interfaces
{
    public interface IProductSizeRepository
    {
        Task<IEnumerable<ProductSize>> GetByProductAsync(int productId);
        Task<int> AddAsync(int productId, string sizeName, int quantity);
        Task<int> UpdateAsync(int sizeId, string sizeName, int quantity);
        Task<int> DeleteAsync(int sizeId);
        Task<int> DeleteByProductAsync(int productId);
        Task<int> DecreaseQuantityAsync(int orderId);
    }
}