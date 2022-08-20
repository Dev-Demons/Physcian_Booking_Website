using System.ComponentModel.DataAnnotations;

namespace Binke.ViewModels
{
    public class QualificationViewModel : BaseViewModel
    {
        public int QualificationId { get; set; }

        [Required(ErrorMessage = "Please enter institute."),
         StringLength(200, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string Institute { get; set; }

        [Required(ErrorMessage = "Please enter degree."),
         StringLength(200, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string Degree { get; set; }

        [Required(ErrorMessage = "Please enter passing year.")]
        public short PassingYear { get; set; }

        public string Notes { get; set; }

        public int DoctorId { get; set; }
       
    }
}
