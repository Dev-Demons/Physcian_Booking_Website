using System;
using System.Collections.Generic;
using System.Linq;

namespace Binke.Model.ViewModels
{
    public class PatientViewModel
    {
        public int PatientId { get; set; }

        public int UserId { get; set; }

        public string PrimaryInsurance { get; set; }
        public string SecondaryInsurance { get; set; }
        public string PatientName { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
