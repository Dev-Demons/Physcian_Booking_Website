using System;
using System.ComponentModel.DataAnnotations;

namespace Binke.ViewModels
{
    public class DrugViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "Please enter drug name."),
        StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string DrugName { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Please enter unitory price."),DataType(DataType.Currency)]
        public float UnitoryPrice { get; set; }

        [Required(ErrorMessage = "Please enter unitory price."), DataType(DataType.Currency)]
        public float SellingPrice { get; set; }

        [Required(ErrorMessage = "Please enter drug name."),
        StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string ManufactureName { get; set; }

        [Required(ErrorMessage = "Please enter expiry date.")]
        public string ExpiryDate { get; set; }

        public int PharmacyId { get; set; }
    }

    public class PharmacyProfileViewModel
    {
        public int OrganisationId { get; set; }

        public int? UserId { get; set; }

        [Required(ErrorMessage = "Organisation type id is required")]
        public int OrganizationTypeID { get; set; }

        [Required(ErrorMessage = "Organisation name is required")]
        [StringLength(200)]
        public string OrganisationName { get; set; }

        [System.Web.Mvc.Remote("ValidateNPI", "Pharmacy", ErrorMessage = "NPI already exists")]
        [StringLength(10)]
        public string NPI { get; set; }

        [StringLength(10)]
        public string OrganisationSubpart { get; set; }

        [StringLength(200)]
        public string AliasBusinessName { get; set; }

        [StringLength(200)]
        public string LogoFilePath { get; set; }

        [StringLength(15)]
        public string OrganizatonEIN { get; set; }

        public DateTime? EnumerationDate { get; set; }

        [Required]
        [StringLength(10)]
        public string Status { get; set; }

        [StringLength(10)]
        public string AuthorisedOfficialCredential { get; set; }

        [StringLength(50)]
        public string AuthorizedOfficialFirstName { get; set; }

        [StringLength(50)]
        public string AuthorizedOfficialLastName { get; set; }

        [StringLength(10)]
        public string AuthorizedOfficialTelephoneNumber { get; set; }

        [StringLength(50)]
        public string AuthorizedOfficialTitleOrPosition { get; set; }

        [StringLength(10)]
        public string AuthorizedOfficialNamePrefix { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }
        public bool? EnabledBooking { get; set; }

    }

    public class PharmacyOfficeLocationViewModel : BaseViewModel
    {
        public int PharmacyId { get; set; }
        public string PharmacyName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string FullAddress { get; set; }

    }

    public class PharmacyStateLicenseViewModel : BaseViewModel
    {
        public int DocOrgStateLicenseId { get; set; }
        public int PharmacyId { get; set; }
        public string PharmacyName { get; set; }
        public string HealthCareProviderTaxonomyCode { get; set; }
        public string ProviderLicenseNumber { get; set; }
        public string ProviderLicenseNumberStateCode { get; set; }
        public bool? HealthcareProviderPrimaryTaxonomySwitch { get; set; }
        public int? UserTypeId { get; set; }
    }

    public class PharmacyImagesViewModel : BaseViewModel
    {
        public int ImageId { get; set; }
        public int PharmacyId { get; set; }
        public string PharmacyName { get; set; }
        public string ImagePath { get; set; }
    }

    public class PharmacyReviewViewModel : BaseViewModel
    {
        public long ReviewId { get; set; }
        public int PharmacyId { get; set; }
        public string PharmacyName { get; set; }
        public string Description { get; set; }
        public int Rating { get; set; }
        public string CreatedUserName { get; set; }
    }

    public class PharmacyAmenityOptionViewModel : BaseViewModel
    {
        public int AmenityId { get; set; }
        public int PharmacyId { get; set; }
        public string PharmacyName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsOption { get; set; }
    }

    public class PharmacyOpeningHourViewModel : BaseViewModel
    {
        public int OpeningHourID { get; set; }
        public int PharmacyId { get; set; }
        public string PharmacyName { get; set; }
        public DateTime? CalendarDate { get; set; }
        public int? WeekDay { get; set; }
        public int? SlotDuration { get; set; }
        public string StartDateTime { get; set; }
        public string EndDateTime { get; set; }
        public string WeekDayName { get; set; }
        public bool? IsHoliday { get; set; }
        public string Comments { get; set; }
    }
}
