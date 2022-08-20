using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;


namespace Doctyme.Model.ViewModels
{
    public class SlotViewModel : BaseViewModel
    {
        public int SlotId { get; set; }
         
        public string SlotDate { get; set; }
         
        public string SlotTime { get; set; }

        //public DateTime SlotDate { get; set; }
        //public DateTime SlotTime { get; set; }

        public int ReferenceId { get; set; }

        public int? BookedFor { get; set; }

        public bool IsBooked { get; set; }

        public bool IsEmailReminder { get; set; }

        public bool IsTextReminder { get; set; }

        public bool IsInsuranceChanged { get; set; }

        public bool IsActive { get; set; }

        public int InsurancePlanId { get; set; }

        public int AddressId { get; set; }
        
        public string Description { get; set; }
    }
}
