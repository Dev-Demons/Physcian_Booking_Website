using System.ComponentModel.DataAnnotations;

namespace Binke.ViewModels
{
    public class PharmacyBasicInformation : BaseViewModel
    {
        public int PharmacyId { get; set; }

        [Required(ErrorMessage = "Please enter first name."),
         StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter middle name."),
         StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Please enter last name."),
         StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string LastName { get; set; }

        public string FullName { get; set; }

        public string PhoneExt { get; set; }

        public string PhoneNumber { get; set; }

        public string FaxNumber { get; set; }

        public string PharmacyName { get; set; }

        public string ProfilePicture { get; set; }

        public string Email { get; set; }

        public string NPI { get; set; }

        public int totRows { get; set; }

        public AddressViewModel AddressView { get; set; }
    }
}
