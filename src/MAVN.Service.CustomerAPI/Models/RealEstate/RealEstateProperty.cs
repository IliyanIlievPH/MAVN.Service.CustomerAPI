using System.Collections.Generic;

namespace MAVN.Service.CustomerAPI.Models.RealEstate
{
    public class RealEstateProperty
    {
        public string Name { get; set; }

        public List<RealEstateInstalments> Instalments { get; set; }
    }
}
