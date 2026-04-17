using BoutiquePortal.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoutiquePortal.Repositories.Interfaces
{
    public interface IStateRepository
    {
         Task<IEnumerable<State>> GetAllAsync();
        Task<State> GetByIdAsync(int id);
        Task<int> AddAsync(State state);
        Task<int> UpdateAsync(State state);
        Task<int> DeleteAsync(int id);
        Task<IEnumerable<State>> GetByCountryId(int countryId);
    }
}
