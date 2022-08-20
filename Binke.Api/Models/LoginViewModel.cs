using Doctyme.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Binke.Api.Models
{
    public class ApiLoginViewModel
    {

        [Display(Name = "Email")]
        [RegularExpression(@"^ *([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,6}) *$", ErrorMessage = "Enter valid Email Address")]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public bool IsEmailLogin { get; set; }
        public Int32 Login_Method { get; set; }
    }
    public class ApiRegisterViewModel
    {
      
        [MaxLength(50, ErrorMessage = "Please enter First name")]
        public string FirstName { get; set; }

       
        [MaxLength(50, ErrorMessage = "Please enter Last name")]
        public string LastName { get; set; }

        [MaxLength(50, ErrorMessage = "Please enter Middle name")]
        public string MiddleName { get; set; }

       
        [EmailAddress]
        public string Email { get; set; }
        
        [Display(Name = "Group Type")]
        public string GroupTypeId { get; set; }

        [Required(ErrorMessage = "Please select facility type.")]
        public int FacilityTypeId { get; set; }

        //[Required(ErrorMessage = "Please select facility name.")]
        public string FacilityName { get; set; }

        public string UserType { get; set; }

        
        [MinLength(10, ErrorMessage = "NPI should be 10 digit")]
        public string Uniquekey { get; set; }

        [Required(ErrorMessage = "Please enter password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,150}$",
         ErrorMessage = "Passwords must contain at least one lowercase letter, one uppercase letter, and one number.")]

        [DataType(DataType.Password)]

        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string PhoneNumber { get; set; }
        public bool PhoneNumberVerified { get; set; }

    }

    public class NetworkCount
    {
        public int DoctorCount { get; set; }
        public int PharmacyCount { get; set; }
        public int SeniorCareCount { get; set; }
        public int FacilitiesCount { get; set; }
        public int JsonDoctorCount { get; set; }
        public int JsonPharmacyCount { get; set; }
        public int JsonSeniorCareCount { get; set; }
        public int JsonFacilitiesCount { get; set; }
        public string MostlySearchedDrug { get; set; }
    }
    public class FeaturedSpecialityViewModel : BaseViewModel
    {
        public int FeaturedSpecialityId { get; set; }

        public short SpecialityId { get; set; }

        public string SpecialityName { get; set; }

        public string Description { get; set; }

        public HttpPostedFileBase ProfilePic { get; set; }

        public string ProfilePicture { get; set; }
    }
    public class HomeViewModel
    {
        public string CityName { get; set; }
        public string PostalCode { get; set; }
        public NetworkCount NetworkCount { get; set; }
        public List<FeaturedDoctorListModel> FeaturedDoctors { get; set; }
        public List<FeaturedSpecialityViewModel> FeaturedSpecialities { get; set; }
        public List<FeaturedFacilityListModel> Facilities { get; set; }

        public int DoctorCount { get; set; }
        public int PharmacyCount { get; set; }
        public int FacilityCount { get; set; }
        public int SeniorcareCount { get; set; }
    }
}