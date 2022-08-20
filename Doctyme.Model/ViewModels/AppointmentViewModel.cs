using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Doctyme.Model.ViewModels
{
    public class AppointmentViewModel
    {
        public string PatientName { get; set; }
        public DateTime SlotDate { get; set; }
        public string SlotTime { get; set; }
     
    }
}
