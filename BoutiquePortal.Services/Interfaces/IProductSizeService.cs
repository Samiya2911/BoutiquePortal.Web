using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoutiquePortal.Model.Models;

namespace BoutiquePortal.Services.Interfaces
{
    public interface IProductSizeService
    {
        Task<IEnumerable<ProductSize>> GetByProductAsync(int productId);
        Task SaveSizesAsync(int productId, List<string> names, List<int> quantities);
        Task<int> DecreaseQuantityAsync(int orderId);
    }
}