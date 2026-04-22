using BoutiquePortal.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoutiquePortal.Services.Interfaces
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardVM> GetFullDashboardAsync();
    }
}
