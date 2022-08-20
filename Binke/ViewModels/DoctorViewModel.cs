using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Doctyme.Model;

namespace Binke.ViewModels
{
    public class DoctorBasicInformation : BaseViewModel
    {
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Please enter prefix.")]
        public string Prefix { get; set; }

        [Required(ErrorMessage = "Please enter suffix.")]
        public string Suffix { get; set; }

        [Required(ErrorMessage = "Please enter first name."),
         StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string FirstName { get; set; }

        public string FullName { get; set; }
        public string Name { get; set; }

        public string ProfilePicture { get; set; }

        public string Email { get; set; }

        //[Required(ErrorMessage = "Please enter middle name."),
        // StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Please enter last name."),
         StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please select Gender.")]
        public string Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(6, ErrorMessage = "The {0} must be at least {2} characters long.")]
        public string PhoneExt { get; set; }

        public string PhoneNumber { get; set; }

        public string FaxNumber { get; set; }

        public bool IsAllowNewPatient { get; set; }

        public bool IsNtPcp { get; set; }

        public bool IsPrimaryCare { get; set; }

        [Required(ErrorMessage = "Please enter NPI.")]
        public string Npi { get; set; }

        [Required(ErrorMessage = "Please enter education.")]
        public string Education { get; set; }

        [Required(ErrorMessage = "Please enter short description."),
        StringLength(300, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 50)]
        public string ShortDescription { get; set; }

        [Required(ErrorMessage = "Please enter long description."),
        StringLength(1000, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 50)]
        public string LongDescription { get; set; }

        public bool? EnabledBooking { get; set; } 

        public List<short> Speciality { get; set; }

        public List<int> IssuranceAccepted { get; set; }

        public List<int> AgeGroup { get; set; }

        public AddressViewModel AddressView { get; set; }

        public SocialMediaViewModel SocialMediaViewModel { get; set; }
        public int TotalRows { get; set; }
       


    }

    public class DoctorImage : BaseViewModel
    {
        public int DoctorImageId { get; set; }

        public int DoctorId { get; set; }

        public HttpPostedFileBase[] Image { get; set; }

        public string ImagePath { get; set; }
        public string Name { get; set; }
        public bool IsProfile { get; set; }

    }

    public class DoctorAffiliationModel : BaseViewModel
    {
        public int AffiliationId { get; set; }

        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Please select Facility.")]
        public int FacilityId { get; set; }

        public string FacilityName { get; set; }
    }

    public class DoctorInsuranceAcceptedModel : BaseViewModel
    {
        public int InsuranceAcceptedId { get; set; }

        [Required(ErrorMessage = "Please enter Insurance Name.")]
        public string Name { get; set; }
    }

    public class AddressViewModel : BaseViewModel
    {
        public int AddressId { get; set; }

        [Required(ErrorMessage = "Please enter address1."),
        StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string Address1 { get; set; }

        public string Address2 { get; set; }

        [Required(ErrorMessage = "Please select city.")]
        public int CityId { get; set; }

        [Required(ErrorMessage = "Please select state.")]
        public int StateId { get; set; }

        [Required(ErrorMessage = "Please enter country."),
        StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string Country { get; set; }

        [Required(ErrorMessage = "Please enter country."),
        StringLength(9, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string ZipCode { get; set; }

        public bool IsDefault { get; set; }

        public int DoctorId { get; set; }

        public int? FacilityId { get; set; }

        public int? PatientId { get; set; }

        public int? SeniorCareId { get; set; }

        public string CityName { get; set; }

        public string StateName { get; set; }

    }

    

    public class DoctorProfileViewModel : BaseViewModel
    {
        public int DoctorId { get; set; }
        public string LongDescription { get; set; }

        public string Education { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        public string ZipCode { get; set; }

        public string Country { get; set; }

        public string FullForDoctor { get; set; }

        public virtual string PhoneNumber { get; set; }

        public string ProfilePicture { get; set; }

        public virtual ICollection<Qualification> Qualifications { get; set; }

        public virtual ICollection<SiteImage> DoctorImages { get; set; }        

        public virtual ICollection<Experience> Experiences { get; set; }

        public virtual ICollection<DoctorLanguage> DoctorLanguages { get; set; }

        public virtual ICollection<DoctorBoardCertification> DoctorBoardCertifications { get; set; }
        public virtual ICollection<SocialMedia> SocialMediaLinks { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<OpeningHour> OpeningHours { get; set; }

    }

}
