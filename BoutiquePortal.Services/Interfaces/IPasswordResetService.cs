using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoutiquePortal.Services.Interfaces
{
    public interface IPasswordResetService
    {
        Task<(bool success, string message, string token)>
            GenerateTokenAsync(string email, string userType);

        Task<(bool success, string message)>
            ResetPasswordAsync(string token, string newPassword);
    }
}
