using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace Doctyme.Model.ViewModels
{
    public class PatientViewModel
    {
        public int PatientId { get; set; }

        public int UserId { get; set; }

        //public string PrimaryInsurance { get; set; }
        //public string SecondaryInsurance { get; set; }
        public string PatientName { get; set; }
        public DateTime CreatedDate { get; set; }

    }

    public class PatientBasicInformation : BaseViewModel
    {
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Please enter first name."),
         StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter middle name."),
         StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Please enter last name."),
         StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string LastName { get; set; }

        public string Gender { get; set; }

        public string FullName { get; set; }

        //[StringLength(6, ErrorMessage = "The {0} must be at least {2} characters long.")]
        public string PhoneExt { get; set; }

        public string PhoneNumber { get; set; }

        public string FaxNumber { get; set; }

        public string DateOfBirth { get; set; }

        public string PrimaryInsurance { get; set; }

        public string SecondaryInsurance { get; set; }

        public string ProfilePicture { get; set; }

        [Required(ErrorMessage = "Please enter email address")]
        [EmailAddress]
        public string Email { get; set; }

        //[Required(ErrorMessage = "Please enter password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,150}$",
         ErrorMessage = "Passwords must contain at least one lowercase letter, one uppercase letter, and one number.")]

        [DataType(DataType.Password)]

        public string Password { get; set; }

        //[Required(ErrorMessage = "Please enter confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

}
