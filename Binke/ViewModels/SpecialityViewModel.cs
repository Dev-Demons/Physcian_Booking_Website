using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Doctyme.Model;

namespace Binke.ViewModels
{
    public class SpecialityViewModel : BaseViewModel
    {
        public short SpecialityId { get; set; }

        [Required(ErrorMessage = "Please enter speciality name")]
        public string SpecialityName { get; set; }

        public int BoardCount { get; set; }

        public string BoardCertification { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string Description { get; set; }
        public bool IsViewMode { get; set; }
        public List<Speciality> SpecialitysList { get; set; }
    }
}
