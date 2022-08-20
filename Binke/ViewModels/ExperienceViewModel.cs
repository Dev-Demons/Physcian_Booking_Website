using System.ComponentModel.DataAnnotations;

namespace Binke.ViewModels
{
    public class ExperienceViewModel : BaseViewModel
    {
        public int ExperienceId { get; set; }

        [Required(ErrorMessage = "Please enter designation."),
         StringLength(200, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string Designation { get; set; }

        [Required(ErrorMessage = "Please enter organization."),
         StringLength(200, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string Organization { get; set; }

        [Required(ErrorMessage = "Please enter start date.")]
        public string StartDate { get; set; }

        [Required(ErrorMessage = "Please enter start date.")]
        public string EndDate { get; set; }

        public int? CityId { get; set; }

        public int? StateId { get; set; }

        public int DoctorId { get; set; }
    }
}
