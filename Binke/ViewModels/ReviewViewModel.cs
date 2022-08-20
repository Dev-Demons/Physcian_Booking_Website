using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Binke.ViewModels
{
    public class ReviewViewModel : BaseViewModel
    {
        public int? ReviewId { get; set; }

        [Required(ErrorMessage = "Please enter subject."),
         StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Please enter review.")]
        public string Review { get; set; }

        [Required(ErrorMessage = "Please enter rating.")]
        public int Rating { get; set; }
        
        public int DoctorId { get; set; }
    }
}
