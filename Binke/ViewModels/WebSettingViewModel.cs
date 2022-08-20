using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Binke.ViewModels
{
    public class NetworkCount
    {
        public int DoctorCount { get; set; }
        public int PharmacyCount { get; set; }
        public int SeniorCareCount { get; set; }
        public int FacilitiesCount { get; set; }
        public int JsonDoctorCount { get; set; }
        public int JsonPharmacyCount { get; set; }
        public int JsonSeniorCareCount { get; set; }
        public int JsonFacilitiesCount { get; set; }
        public string MostlySearchedDrug { get; set; }
    }
}
