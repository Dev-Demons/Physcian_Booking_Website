using System;
using System.ComponentModel.DataAnnotations;

namespace Binke.ViewModels
{
    public class SlotViewModel : BaseViewModel
    {
        public int SlotId { get; set; }

        //[Required(ErrorMessage = "Please enter start date")]
        public string StartDate { get; set; }

        //Required(ErrorMessage = "Please enter end date")]
        public string EndDate { get; set; }

        //[Required(ErrorMessage = "Please select slot from time")]
        public int? FromTime { get; set; }

        //[Required(ErrorMessage = "Please select slot to time")]
        public int? ToTime { get; set; }

        //[Required(ErrorMessage = "Please select slot size")]
        public int SlotSize { get; set; }

        public int DoctorId { get; set; }
        public int? SeniorCareId { get; set; }
        public int? FacilityId { get; set; }
        public string SlotTime { get; set; }
        public string SlotDate { get; set; }
        public int ReferenceId { get; set; }
        public int? BookedFor { get; set; }
        public bool IsBooked { get; set; }
        public bool IsEmailReminder { get; set; }
        public bool IsTextReminder { get; set; }
        public bool IsInsuranceChanged { get; set; }        
        public int InsurancePlanId { get; set; }
        public int AddressId { get; set; }         
        public string Description { get; set; }
    }
}
