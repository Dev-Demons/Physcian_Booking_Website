using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Binke.ViewModels
{
    public class AppointmentViewModel
    {
        public string PatientName { get; set; }
        public string SlotDate { get; set; }
        public string SlotTime { get; set; }
    }
}
