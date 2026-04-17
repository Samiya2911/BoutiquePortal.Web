using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoutiquePortal.Model.Models;

namespace BoutiquePortal.Services.Interfaces
{
    public interface ICountryService
    {
        Task<IEnumerable<Country>> GetAllAsync();
        Task<Country> GetByIdAsync(int id);
        Task<int> AddAsync(Country country);
        Task<int> UpdateAsync(Country country);
        Task<int> DeleteAsync(int id);
    }
}
