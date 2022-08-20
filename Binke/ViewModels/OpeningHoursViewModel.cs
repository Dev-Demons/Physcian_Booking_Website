using System;
using System.ComponentModel.DataAnnotations;

namespace Binke.ViewModels
{
    public class OpeningHoursViewModel : BaseViewModel
    {

        public int OpeningHourId { get; set; }

        [Required(ErrorMessage = "Please select week day")]
        public int WeekDay { get; set; }

        public string DayOfWeek { get; set; }

        public DateTime StartTime { get; set; }
        public string Start { get; set; }

        public DateTime EndTime { get; set; }
        public string End { get; set; }

        public int? DoctorId { get; set; }
        public int? SeniorCareId { get; set; }
        
    }
}
