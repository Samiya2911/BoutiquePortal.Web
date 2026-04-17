using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoutiquePortal.Model.Models
{
    public class City
    {
        public int CityId { get; set; }
        public string CityName { get; set; }
        public string CityCode { get; set; }
        public int StateId { get; set; }
        public bool IsActive { get; set; }
        public string StateName { get; set; }
        public string CountryName { get; set; }
    }
}
