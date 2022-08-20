using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Binke.ViewModels
{
    public class FacilityBasicInformation : BaseViewModel
    {
        public string OrganisationName { get; set; }
        public bool? EnabledBooking { get; set; }
        public string Location { get; set; }

        public string Speciality { get; set; }

        public int FacilityId { get; set; }

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

        //[StringLength(6, ErrorMessage = "The {0} must be at least {2} characters long.")]
        public string PhoneExt { get; set; }

        public string PhoneNumber { get; set; }

        public string FaxNumber { get; set; }

        public byte? FacilityTypeId { get; set; }
        public int OrganizationTypeID { get; set; }

        public string FacilityName { get; set; }

        public string FacilityType { get; set; }

        public string ProfilePicture { get; set; }

        public string Email { get; set; }
  
        public AddressViewModel AddressView { get; set; }
    }

    public class FacilityTypeViewModel : BaseViewModel
    {
        public byte FacilityTypeId { get; set; }

        [Required(ErrorMessage = "Please select facility option.")]
        public byte FacilityOptionId { get; set; }

        public string FacilityOption { get; set; }

        [Required(ErrorMessage = "Please enter facility type."),
         StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string FacilityTypeName { get; set; }
    }

    public class FacilityOptionViewModel : BaseViewModel
    {
        public byte FacilityOptionId { get; set; }
        
        [Required(ErrorMessage = "Please enter facility Option."),
         StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string FacilityOptionName { get; set; }
    }
}
