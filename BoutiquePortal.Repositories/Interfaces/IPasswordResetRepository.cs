using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoutiquePortal.Model.Models;

namespace BoutiquePortal.Repositories.Interfaces
{
    public interface IPasswordResetRepository
    {
        Task<int> InsertTokenAsync(string email,
            string userType, string token, DateTime expiry);
        Task<PasswordResetToken?> GetByTokenAsync(string token);
        Task<int> MarkUsedAsync(string token);
    }
}
