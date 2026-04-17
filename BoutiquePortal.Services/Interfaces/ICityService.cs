using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoutiquePortal.Model.Models;

namespace BoutiquePortal.Services.Interfaces
{
    public interface ICityService
    {
        Task<IEnumerable<City>> GetAllAsync();
        Task<City> GetByIdAsync(int id);
        Task<int> AddAsync(City city);
        Task<int> UpdateAsync(City city);
        Task<int> DeleteAsync(int id);
        Task<IEnumerable<City>> GetByStateId(int stateId);
    }

}
