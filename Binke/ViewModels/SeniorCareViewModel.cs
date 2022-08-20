using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Binke.ViewModels
{
    public class SeniorCareBasicInformation : BaseViewModel
    {
        public int SeniorCareId { get; set; }

        [Required(ErrorMessage = "Please enter prefix.")]
        public string Prefix { get; set; }

        [Required(ErrorMessage = "Please enter suffix.")]
        public string Suffix { get; set; }

        [Required(ErrorMessage = "Please enter first name."),
         StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string FirstName { get; set; }

        public string FullName { get; set; }

        public string ProfilePicture { get; set; }

        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter middle name."),
         StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Please enter last name."),
         StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please select Gender.")]
        public string Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        //[StringLength(6, ErrorMessage = "The {0} must be at least {2} characters long.")]
        public string PhoneExt { get; set; }

        public string PhoneNumber { get; set; }

        public string FaxNumber { get; set; }

        public int? UserId { get; set; }

        [Required(ErrorMessage = "Please enter Senior Care Name.")]
        public string SeniorCareName { get; set; }

        [Required(ErrorMessage = "Please enter summary."),
         StringLength(300, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 50)]
        public string Summary { get; set; }

        [Required(ErrorMessage = "Please enter long description."),
        StringLength(1000, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 50)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please enter long Amenities."),
         StringLength(1000, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 50)]
        public string Amenities { get; set; }

        public AddressViewModel AddressView { get; set; }

        public SocialMediaViewModel SocialMediaViewModel { get; set; }
    }

    public class SeniorCareLanguageViewModel : BaseViewModel
    {
        public int SeniorCareLanguageId { get; set; }

        public int LanguageId { get; set; }

        public int SeniorCareId { get; set; }

        public string LanguageName { get; set; }

        public string SeniorCareName { get; set; }

    }

    public class SeniorCareImageViewModel : BaseViewModel
    {
        public int SeniorCareImageId { get; set; }

        public int SeniorCareId { get; set; }

        public HttpPostedFileBase[] Image { get; set; }

        public string ImagePath { get; set; }
    }


    public class CostViewModel 
    {
        public int CostID { get; set; }

        public int ReferenceID { get; set; }
        public int UserTypeID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal? Price { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string CreatedDateString { get; set; }
        public string UpdatedDateString { get; set; }

        public int TotalRecordCount { get; set; }

    }

}
