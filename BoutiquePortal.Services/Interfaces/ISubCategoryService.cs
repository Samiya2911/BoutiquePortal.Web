using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoutiquePortal.Model.Models;

namespace BoutiquePortal.Services.Interfaces
{
    public interface ISubCategoryService
    {
        Task<IEnumerable<SubCategory>> GetAllAsync();
        Task<SubCategory> GetByIdAsync(int id);
        Task<int> AddAsync(SubCategory entity);
        Task<int> UpdateAsync(SubCategory entity);
        Task<int> DeleteAsync(int id);
        Task<IEnumerable<SubCategory>> GetByCategoryId(int categoryId);

    }
}
