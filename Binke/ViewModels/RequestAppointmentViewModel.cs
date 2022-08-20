using System;
using System.ComponentModel.DataAnnotations;

namespace Binke.ViewModels
{
    public class RequestAppointmentViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "Please enter your First name")]
        [MaxLength(50, ErrorMessage = "Please enter your First name")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter your Last name")]
        [MaxLength(50, ErrorMessage = "Please enter your Last name")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter your Middle name")]
        [MaxLength(50, ErrorMessage = "Please enter your Middle name")]
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Please enter your email address")]
        [EmailAddress]
        [Display(Name = "E-mail (Username)")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please select grouptype.")]
        [Display(Name = "Group Type")]
        public string GroupTypeId { get; set; }

        public string PhoneNumber { get; set; }

        public string UserType { get; set; }

        public string DateOfBirth { get; set; }
        public DateTime? BirthDate { get; set; }

        public int? PatientId { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter your confirm password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public AddressViewModel AddressView { get; set; }
        public RequestSlotViewModel RequestSlot { get; set; }
    }
    public class RequestSlotViewModel
    {
        public string AppTime { get; set; }
        public string AppDate { get; set; }
        public string Location { get; set; }
    }
}
