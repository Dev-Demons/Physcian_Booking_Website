using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Model.ViewModels
{
   public class DashboardItemCount
    {
        public int DoctorCount { get; set; }
        public int FacilityCount { get; set; }
        public int PatientCount { get; set; }
        public int SeniorCareCount { get; set; }
        public int DrugCount { get; set; }
        public int PharmacyCount { get; set; } //Added by Reena
        
    }
}
