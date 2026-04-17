using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoutiquePortal.Model.Models;
using System.Collections.Generic;

namespace BoutiquePortal.Repositories.Interfaces
{
    public interface ISubCategoryRepository
    {
        Task<IEnumerable<SubCategory>> GetAllAsync();
        Task<SubCategory> GetByIdAsync(int id);
        Task<int> AddAsync(SubCategory entity);
        Task<int> UpdateAsync(SubCategory entity);
        Task<int> DeleteAsync(int id);
    }
}
