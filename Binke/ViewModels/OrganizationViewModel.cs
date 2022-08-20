using System;
using System.ComponentModel.DataAnnotations;

namespace Binke.ViewModels
{
    public class OrganisationProfileViewModel
    {
        public int OrganisationId { get; set; }

        public int? UserId { get; set; }

        [Required(ErrorMessage = "Organization type id is required")]
        public int OrganizationTypeID { get; set; }

        [Required(ErrorMessage = "Organization name is required")]
        [StringLength(200)]
        public string OrganisationName { get; set; }

        public int? UserTypeID { get; set; }

        [MinLength(10, ErrorMessage = "NPI should be 10 digit")]
        [System.Web.Mvc.Remote("ValidateNPI", "Pharmacy", AdditionalFields = "OrganisationId", ErrorMessage = "NPI already exists")]
        [StringLength(10)]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Accept only number")]
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
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Accept only number")]
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
        public int? ApplicationUser_Id { get; set; }
        public bool? EnabledBooking { get; set; }

        [StringLength(300)]
        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public bool IsViewMode { get; set; }

        public int TotalRecordCount { get; set; }
    }

    public class OrganisationProfileListViewModel : BaseViewModel
    {
        public int PharmacyId { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

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

        public string FullAddress { get; set; }

        public bool? EnabledBooking { get; set; }

    }


    public class OrganisationAddressViewModel
    {
        [Required(ErrorMessage = "Enter correct Organization name!")]
        [Range(1, int.MaxValue, ErrorMessage = "Enter correct Organization name!")]
        public int OrganisationId { get; set; }
        public int? OrganizationTypeID { get; set; }
        public int? UserTypeID { get; set; }
        public string OrganisationName { get; set; }
        public string OrganizationType { get; set; }
        public int AddressId { get; set; }
        [Required(ErrorMessage = "Select Address Type!")]
        public int AddressTypeId { get; set; }
        [StringLength(100)]
        public string Address1 { get; set; }
        [StringLength(100)]
        public string Address2 { get; set; }
        [Required(ErrorMessage = "Select zip code, city, state and country!")]
        [Range(1, int.MaxValue, ErrorMessage = "Enter correct zip code and city!")]
        public int CityStateZipCodeID { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lon { get; set; }
        [StringLength(20)]
        public string Phone { get; set; }
        [StringLength(20)]
        public string Fax { get; set; }
        [StringLength(200)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [StringLength(200)]
        public string WebSite { get; set; }
        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public string ReferenceName { get; set; }
        public int ReferenceId { get; set; }

        public int TotalRecordCount { get; set; }
    }
    public class DoctorAddressViewModel
    {
       
        public int DoctorId { get; set; }
       
        public int? UserTypeID { get; set; }
        public string OrganisationName { get; set; }
        public string OrganizationType { get; set; }
        public int AddressId { get; set; }
        [Required(ErrorMessage = "Select Address Type!")]
        public int AddressTypeId { get; set; }
        [StringLength(100)]
        public string Address1 { get; set; }
        [StringLength(100)]
        public string Address2 { get; set; }
        [Required(ErrorMessage = "Select zip code, city, state and country!")]
        [Range(1, int.MaxValue, ErrorMessage = "Enter correct zip code and city!")]
        public int CityStateZipCodeID { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lon { get; set; }
        [StringLength(20)]
        public string Phone { get; set; }
        [StringLength(20)]
        public string Fax { get; set; }
        [StringLength(200)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [StringLength(200)]
        public string WebSite { get; set; }
        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public string ReferenceName { get; set; }
        public int ReferenceId { get; set; }

        public int TotalRecordCount { get; set; }
        public bool IsViewMode { get; set; }
    }

    public class OrganisationAddressListViewModel : BaseViewModel
    {
        public int OrganisationId { get; set; }
        public int? OrganizationTypeID { get; set; }
        public int? UserTypeID { get; set; }
        public string OrganisationName { get; set; }
        public string OrganizationType { get; set; }
        public int AddressId { get; set; }
        public int AddressTypeId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string FullAddress { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lon { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public int ReferenceId { get; set; }
        public int CityStateZipCodeID { get; set; }
        public string IsActiveString { get; set; }

        public int TotalRecordCount { get; set; }
    }

    public class DoctorAddressListViewModel : BaseViewModel
    {
      
        public int? UserTypeID { get; set; }
        public string DoctorName { get; set; }
        public int DoctorId { get; set; }
        public int AddressId { get; set; }
        public int AddressTypeId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string FullAddress { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lon { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public int ReferenceId { get; set; }
        public int CityStateZipCodeID { get; set; }
        public string IsActiveString { get; set; }

        public int TotalRecordCount { get; set; }
    }


    public class OrganisationOfficeLocationViewModel : BaseViewModel
    {
        public int OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string FullAddress { get; set; }

    }

    public class CityStateZipViewModel : BaseViewModel
    {
        public int CityStateZipCodeID { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string LocationType { get; set; }
        public string WorldRegion { get; set; }
        public string Country { get; set; }
        public string LocationText { get; set; }

    }

    public class OrgAddressDropdownViewModel
    {
        public int AddressId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public int CityStateZipCodeID { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Long { get; set; }
        public string FullAddress { get; set; }
        public string OrganizationAddress { get; set; }        
    }


    public class OrganisationStateLicenseViewModel : BaseViewModel
    {
        public int DocOrgStateLicenseId { get; set; }
        public int OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public string HealthCareProviderTaxonomyCode { get; set; }
        public string ProviderLicenseNumber { get; set; }
        public string ProviderLicenseNumberStateCode { get; set; }
        public bool? HealthcareProviderPrimaryTaxonomySwitch { get; set; }
        public int? UserTypeId { get; set; }
    }

    public class OrganisationImagesViewModel : BaseViewModel
    {
        public int ImageId { get; set; }
        public int OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public string ImagePath { get; set; }
    }

    public class OrganisationReviewViewModel : BaseViewModel
    {
        public long ReviewId { get; set; }
        public int OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public string Description { get; set; }
        public int Rating { get; set; }
        public string CreatedUserName { get; set; }
    }

    public class OrganisationAmenityOptionViewModel : BaseViewModel
    {
        public int OrganisationAmenityId { get; set; }
        public int OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsOption { get; set; }
    }

    public class OrganisationOpeningHourViewModel : BaseViewModel
    {
        public int OpeningHourID { get; set; }
        public int OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public DateTime? CalendarDate { get; set; }
        public int? WeekDay { get; set; }
        public int? SlotDuration { get; set; }
        public string StartDateTime { get; set; }
        public string EndDateTime { get; set; }
        public string WeekDayName { get; set; }
        public bool? IsHoliday { get; set; }
        public string Comments { get; set; }
    }

    public class CityStateZipCodeViewModel
    {
        public int CityStateZipCodeID { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }

        public string State { get; set; }

        public string FullCityStateZipCode { get { return ZipCode + "(" + City + ", " + State + ")"; } }
    }

    public class SwitchUpdateViewModel
    {
        public bool FieldToUpdateValue { get; set; }
        public string FieldToUpdateName { get; set; }
        public string TableName { get; set; }
        public string PrimaryKeyName { get; set; }
        public int PrimaryKeyValue { get; set; }
    }

    public class CityStateInfoByZipCodeViewModel
    {
        public int CityStateZipCodeID { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Long { get; set; }

    }

    public class OrganisationAutoCompleteViewModel
    {
        public int OrganisationId { get; set; }
        public string OrganisationName { get; set; }
    }

    public class AddressTypeDropDownViewModel
    {
        public int AddressTypeId { get; set; }
        public string Name { get; set; }
    }


    public class StateCompleteViewModel
    {
        public string State { get; set; }
    }


    public class OrganisationFeaturedViewModel
    {
        public int FeaturedId { get; set; }

        public int ReferenceId { get; set; }
        public int? OrganisationId { get; set; }
        public string ReferenceName { get; set; }

        public int UserTypeID { get; set; }

        public string ProfileImage { get; set; }

        public string Description { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public int? TotalImpressions { get; set; }
        public int? PaymentTypeID { get; set; }
        public int? AdvertisementLocationID { get; set; }

        public string Title { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }


        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }


        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public int? CityStateZipCodeID { get; set; }
        public int OrganizationTypeID { get; set; }

        public int TotalRecordCount { get; set; }
    }

    public class OrganisationFeaturedListViewModel : BaseViewModel
    {
        public int FeaturedId { get; set; }

        public int ReferenceId { get; set; }
        public int OrganisationId { get; set; }
        public string OrganisationName { get; set; }

        public int UserTypeID { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string ProfileImage { get; set; }

        public string Description { get; set; }

        public int? TotalImpressions { get; set; }
        public int? PaymentTypeID { get; set; }
        public int? AdvertisementLocationID { get; set; }

        public string Title { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string strStartDate { get; set; }

        public string strEndDate { get; set; }

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }


        public int? CityStateZipCodeID { get; set; }

        public int OrganizationTypeID { get; set; }

        public int TotalRecordCount { get; set; }
    }

    public class OrganisationFeaturedAddEditViewModel
    {
        public int FeaturedId { get; set; }

        [Required(ErrorMessage = "Organization Name is required!")]
        [Range(1, int.MaxValue, ErrorMessage = "Organization Name is required!")]
        public int OrganisationId { get; set; }

        public int OrganizationTypeID { get; set; }
        public int UserTypeID { get; set; }

        [StringLength(200)]
        public string OrganisationName { get; set; }

        public string ProfileImage { get; set; }

        [StringLength(200)]
        public string Title { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }

        public int? AdvertisementLocationID { get; set; }

        [Required(ErrorMessage = "Start Date is required!")]
        public DateTime FeaturdStartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public int? TotalImpressions { get; set; }
        public int? PaymentTypeID { get; set; }


        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        [Required(ErrorMessage = "Zip code, City and State is required!")]
        public int? CityStateZipCodeID { get; set; }

        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Long { get; set; }
    }

    public class AdvertisementLocationDropDownViewModel
    {
        public int AdvertisementLocationId { get; set; }
        public string Name { get; set; }
    }


    public class OrgSocialMediaViewModel
    {
        public int SocialMediaId { get; set; }

        public int? ReferenceId { get; set; }

        public string Facebook { get; set; }

        public string Twitter { get; set; }

        public string LinkedIn { get; set; }

        public string Instagram { get; set; }

        public string Youtube { get; set; }

        public string Pinterest { get; set; }

        public string Tumblr { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }
        public int? UserTypeId { get; set; }

        public int OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public int OrganizationTypeID { get; set; }
        public int TotalRecordCount { get; set; }
    }

    public class OrgSocialMediaListViewModel
    {
        public int SocialMediaId { get; set; }

        public string Facebook { get; set; }

        public string Twitter { get; set; }

        public string LinkedIn { get; set; }

        public string Instagram { get; set; }

        public string Youtube { get; set; }

        public string Pinterest { get; set; }

        public string Tumblr { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsActive { get; set; }
        public string IsActiveString { get; set; }
        public bool IsDeleted { get; set; }
        public int? UserTypeId { get; set; }

        public int OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public int OrganizationTypeID { get; set; }
        public int TotalRecordCount { get; set; }
    }

    public class OrgSocialMediaUpdateViewModel
    {
        public int SocialMediaId { get; set; }

        public int? ReferenceId { get; set; }

        public string Facebook { get; set; }

        public string Twitter { get; set; }

        public string LinkedIn { get; set; }

        public string Instagram { get; set; }

        public string Youtube { get; set; }

        public string Pinterest { get; set; }

        public string Tumblr { get; set; }

        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public int CreatedBy { get; set; }

        public int? UserTypeID { get; set; }

        [Required(ErrorMessage = "Organization Name is required!")]
        [Range(1, int.MaxValue, ErrorMessage = "Organization Name is required!")]
        public int OrganisationId { get; set; }

        public string OrganisationName { get; set; }
        public int OrganizationTypeID { get; set; }
        public int TotalRecordCount { get; set; }
    }
    public class DocSocialMediaUpdateViewModel
    {
        public int SocialMediaId { get; set; }

        public int? ReferenceId { get; set; }

        public string Facebook { get; set; }

        public string Twitter { get; set; }

        public string LinkedIn { get; set; }

        public string Instagram { get; set; }

        public string Youtube { get; set; }

        public string Pinterest { get; set; }

        public string Tumblr { get; set; }

        public bool IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public int CreatedBy { get; set; }

        public int? UserTypeID { get; set; }

      
        public int DoctorId { get; set; }

        public string DoctorName { get; set; }
    
        public int TotalRecordCount { get; set; }
    }

    public class OrganisationSpecialityTaxonomyViewModel
    {
        public int DocOrgTaxonomyID { get; set; }
        public int ReferenceID { get; set; }
        public int? ParentID { get; set; }
        public int TaxonomyID { get; set; }
        public int? UserTypeId { get; set; }

        public string Taxonomy_Code { get; set; }
        public string Specialization { get; set; }

        public string OrganisationName { get; set; }
        public int? OrganizationTypeID { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool? IsActive { get; set; }
        public string IsActiveString { get; set; }
        public bool? IsDeleted { get; set; }

        public int? CreatedBy { get; set; }
        public string Description { get; set; }
        public int TotalRecordCount { get; set; }
    }

    public class OrganisationTaxonomyViewModel
    {
        public int DocOrgTaxonomyID { get; set; }
        public int ReferenceID { get; set; }
        public int ParentID { get; set; }
        public int TaxonomyID { get; set; }
        public int? UserTypeId { get; set; }

        public string Taxonomy_Code { get; set; }
        public string Specialization { get; set; }

        public string OrganisationName { get; set; }
        public int OrganizationTypeID { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsActive { get; set; }
        public string IsActiveString { get; set; }
        public bool IsDeleted { get; set; }

        public int CreatedBy { get; set; }
        public string Description { get; set; }
        public int TotalRecordCount { get; set; }
    }
    public class DoctorTaxonomyViewModel
    {
        public int DocOrgTaxonomyID { get; set; }
        public int ReferenceID { get; set; }
        public int ParentID { get; set; }
        public int TaxonomyID { get; set; }
        public int? UserTypeId { get; set; }

        public string Taxonomy_Code { get; set; }
        public string Specialization { get; set; }

     

        public DateTime? UpdatedDate { get; set; }

        public bool IsActive { get; set; }
        public string IsActiveString { get; set; }
        public bool IsDeleted { get; set; }

        public int CreatedBy { get; set; }
        public string Description { get; set; }
        public int TotalRecordCount { get; set; }
    }

    public class OrganisationTaxonomyListViewModel
    {
        public int DocOrgTaxonomyID { get; set; }
        public int ReferenceID { get; set; }
        public int TaxonomyID { get; set; }
        public int? UserTypeId { get; set; }
        public int? ParentID { get; set; }
        public string Taxonomy_Code { get; set; }
        public string Specialization { get; set; }

        public int OrganisationId { get; set; }

        public string OrganisationName { get; set; }
        public int OrganizationTypeID { get; set; }

        public string UpdatedDate { get; set; }

        public bool IsActive { get; set; }
        public string IsActiveString { get; set; }
        public bool IsDeleted { get; set; }

        public int CreatedBy { get; set; }
        public string Description { get; set; }
        public int TotalRecordCount { get; set; }
    }
    public class DoctorTaxonomyListViewModel
    {
        public int DocOrgTaxonomyID { get; set; }
        public int ReferenceID { get; set; }
        public int TaxonomyID { get; set; }
        public int? UserTypeId { get; set; }
        public int? ParentID { get; set; }
        public string Taxonomy_Code { get; set; }
        public string Specialization { get; set; }

        public int DoctorId { get; set; }

    

        public string UpdatedDate { get; set; }

        public bool IsActive { get; set; }
        public string IsActiveString { get; set; }
        public bool IsDeleted { get; set; }

        public int CreatedBy { get; set; }
        public string Description { get; set; }
        public int TotalRecordCount { get; set; }
    }

    public class OrganisationTaxonomyUpdateViewModel
    {
        public int DocOrgTaxonomyID { get; set; }

        [Required(ErrorMessage = "Taxonomy code is required! Should be select valid Taxonomy Code!")]
        [Range(1, int.MaxValue, ErrorMessage = "Taxonomy code is required! Should be select valid Taxonomy Code!")]
        public int TaxonomyID { get; set; }

        public int UserTypeID { get; set; }

        public string Taxonomy_Code { get; set; }
        public string Specialization { get; set; }

        [Required(ErrorMessage = "Organization Name is required! Should be select correct Organization")]
        [Range(1, int.MaxValue, ErrorMessage = "Organization Name is required! Should be select correct Organization")]
        public int OrganisationId { get; set; }

        public string OrganisationName { get; set; }
        public int OrganizationTypeID { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

    }

    public class DoctorTaxonomyUpdateViewModel
    {
        public int DocOrgTaxonomyID { get; set; }

        [Required(ErrorMessage = "Taxonomy code is required! Should be select valid Taxonomy Code!")]
        [Range(1, int.MaxValue, ErrorMessage = "Taxonomy code is required! Should be select valid Taxonomy Code!")]
        public int TaxonomyID { get; set; }

        public int UserTypeID { get; set; }

        public string Taxonomy_Code { get; set; }
        public string Specialization { get; set; }

        
        public int DoctorId { get; set; }

    
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsViewMode { get; set; }

    }

    public class TaxonomyAutoCompleteDropDownViewModel
    {
        public int TaxonomyID { get; set; }
        public int? ParentID { get; set; }
        public string Taxonomy_Code { get; set; }
        public string Specialization { get; set; }
    }

    public class TaxonomyCodeViewModel
    {
        public string Taxonomy_Code { get; set; }
        public string Specialization { get; set; }
    }


    public class OrganizationAmenityOptionsViewModel
    {

        public int OrganizationAmenityOptionID { get; set; }
        public int OrganizationID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ImagePath { get; set; }

        public bool IsOption { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsViewMode { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int CreatedBy { get; set; }

        public string OrganisationName { get; set; }
        public int OrganizationTypeID { get; set; }
        public int UserTypeID { get; set; }

        public int TotalRecordCount { get; set; }

    }

    public class OrganizationAmenityOptionListViewModel
    {

        public int? OrganizationAmenityOptionID { get; set; }
        public int? OrganizationID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ImagePath { get; set; }

        public bool IsOption { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedDateAsString { get; set; }
        // public string UpdatedDate { get; set; }
        public int? CreatedBy { get; set; }

        public string OrganisationName { get; set; }
        public int? OrganizationTypeID { get; set; }
        public int? UserTypeID { get; set; }

        public int? TotalRecordCount { get; set; }

    }

    public class OrganizationAmenityOptionsUpdateViewModel
    {

        public int OrganizationAmenityOptionID { get; set; }

        [Required(ErrorMessage = "Organization Name is required! Should be select correct Organization")]
        [Range(1, int.MaxValue, ErrorMessage = "Organization Name is required! Should be select correct Organization")]
        public int OrganizationID { get; set; }

        [Required(ErrorMessage = "Amenity Option Name is required!")]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        [StringLength(200)]
        public string ImagePath { get; set; }

        public bool IsOption { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public string UpdatedDate { get; set; }
        public int CreatedBy { get; set; }
        public bool IsViewMode { get; set; }
        public string OrganisationName { get; set; }
        public int OrganizationTypeID { get; set; }
        public int UserTypeID { get; set; }
    }

    public class OrgStateLicenseViewModel
    {
        public int DocOrgStateLicense { get; set; }
        public int ReferenceId { get; set; }
        public string HealthCareProviderTaxonomyCode { get; set; }
        public string ProviderLicenseNumber { get; set; }
        public string ProviderLicenseNumberStateCode { get; set; }
        public bool? HealthcareProviderPrimaryTaxonomySwitch { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public int? UserTypeID { get; set; }

        public string ReferenceName { get; set; }
        public int OrganisationId { get; set; }
        public int OrganizatonTypeID { get; set; }

        public int TotalRecordCount { get; set; }
    }
    public class DoctorStateLicenseViewModel
    {
        public int DocOrgStateLicense { get; set; }
        public int ReferenceId { get; set; }
        public string HealthCareProviderTaxonomyCode { get; set; }
        public string ProviderLicenseNumber { get; set; }
        public string ProviderLicenseNumberStateCode { get; set; }
        public bool? HealthcareProviderPrimaryTaxonomySwitch { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public int? UserTypeID { get; set; }

        public string ReferenceName { get; set; }
        public int DoctorId { get; set; }
     

        public int TotalRecordCount { get; set; }
    }


    public class OrgStateLicenseListViewModel
    {
        public int DocOrgStateLicenseId { get; set; }
        public int ReferenceId { get; set; }
        public string HealthCareProviderTaxonomyCode { get; set; }
        public string ProviderLicenseNumber { get; set; }
        public string ProviderLicenseNumberStateCode { get; set; }
        public bool? HealthcareProviderPrimaryTaxonomySwitch { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public string UpdatedDate { get; set; }


        public string OrganisationName { get; set; }
        public int OrganizatonTypeID { get; set; }

        public int TotalRecordCount { get; set; }
    }
    public class DoctorStateLicenseListViewModel
    {
        public int DocOrgStateLicenseId { get; set; }
        public int ReferenceId { get; set; }
        public string HealthCareProviderTaxonomyCode { get; set; }
        public string ProviderLicenseNumber { get; set; }
        public string ProviderLicenseNumberStateCode { get; set; }
        public bool? HealthcareProviderPrimaryTaxonomySwitch { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public string UpdatedDate { get; set; }


        public string ReferenceName { get; set; }
     

        public int TotalRecordCount { get; set; }
    }

    public class OrgStateLicenseUpdateViewModel
    {
        public int DocOrgStateLicenseId { get; set; }
        public string HealthCareProviderTaxonomyCode { get; set; }
        public string ProviderLicenseNumber { get; set; }
        public string ProviderLicenseNumberStateCode { get; set; }
        public bool? HealthcareProviderPrimaryTaxonomySwitch { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public string UpdatedDate { get; set; }
        public int? UserTypeID { get; set; }

        [Required(ErrorMessage = "Organization Name is required! Should be select correct Organization")]
        [Range(1, int.MaxValue, ErrorMessage = "Organization Name is required! Should be select correct Organization")]
        public int OrganisationId { get; set; }

        public string OrganisationName { get; set; }
        public int OrganizatonTypeID { get; set; }

    }
    public class DoctorStateLicenseUpdateViewModel
    {
        public int DocOrgStateLicenseId { get; set; }
        public string HealthCareProviderTaxonomyCode { get; set; }
        public string ProviderLicenseNumber { get; set; }
        public string ProviderLicenseNumberStateCode { get; set; }
        public bool? HealthcareProviderPrimaryTaxonomySwitch { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public string UpdatedDate { get; set; }
        public int? UserTypeID { get; set; }

      public bool IsViewMode { get; set; }
        public int DoctorId { get; set; }
        

    }

    public class StateDropDownViewModel
    {
        public string State { get; set; }
    }


    public class OrgOpeningHoursViewModel
    {
        public int OpeningHourID { get; set; }
        public int? DoctorID { get; set; }
        public int? OrganizationID { get; set; }
        public int? WeekDay { get; set; }

        public string StartDateTime { get; set; }
        public DateTime? CalendarDate { get; set; }
        public int? SlotDuration { get; set; }

        public bool? IsHoliday { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public string EndDateTime { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string Comments { get; set; }

        public string OrganisationName { get; set; }
        public int OrganizatonTypeID { get; set; }

        public int UserTypeID { get; set; }

        public int TotalRecordCount { get; set; }
    }

    public class DocOpeningHoursViewModel
    {
        public int OpeningHourID { get; set; }
        public int? DoctorID { get; set; }
        public int? OrganizationID { get; set; }
        public int? WeekDay { get; set; }

        public string StartDateTime { get; set; }
        public DateTime? CalendarDate { get; set; }
        public int? SlotDuration { get; set; }

        public bool? IsHoliday { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public string EndDateTime { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string Comments { get; set; }

       

        public int UserTypeID { get; set; }

        public int TotalRecordCount { get; set; }
    }

    public class OrgOpeningHoursListViewModel
    {
        public string Comments { get; set; }
        public int OpeningHourID { get; set; }
        public int? DoctorID { get; set; }

        public int? WeekDay { get; set; }
        public string WeekDayName { get; set; }
        public DateTime? CalendarDate { get; set; }
        public int? SlotDuration { get; set; }

        public string StartDateTime { get; set; }

        public bool? IsHoliday { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public string EndDateTime { get; set; }

        public string UpdatedDate { get; set; }

        public int? OrganizationID { get; set; }

        public string OrganisationName { get; set; }
        public int OrganizatonTypeID { get; set; }
        public int UserTypeID { get; set; }

        public int TotalRecordCount { get; set; }
    }

    public class DocOpeningHourViewModel
    {
        public int OpeningHourID { get; set; }
        public int? DoctorID { get; set; }
        public string DoctorName { get; set; }
        public int? WeekDay { get; set; }
        public string WeekDayName { get; set; }
        public DateTime? CalendarDate { get; set; }
        public int? SlotDuration { get; set; }

        public string StartDateTime { get; set; }
        public string Comments { get; set; }
        public bool? IsHoliday { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public string EndDateTime { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int UserTypeID { get; set; }

        public int TotalRecordCount { get; set; }
    }

    public class DocOpeningHoursListViewModel
    {
        public int OpeningHourID { get; set; }
        public int? DoctorID { get; set; }
        public string DoctorName { get; set; }
        public int? WeekDay { get; set; }
        public string WeekDayName { get; set; }
        public DateTime? CalendarDate { get; set; }
        public int? SlotDuration { get; set; }

        public string StartDateTime { get; set; }
        public string Comments { get; set; }
        public bool? IsHoliday { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public string EndDateTime { get; set; }

        public string UpdatedDate { get; set; }
        
        public int UserTypeID { get; set; }

        public int TotalRecordCount { get; set; }
    }

    public class DoctorOpeningHoursUpdateViewModel
    {
        public int OpeningHourID { get; set; }
        public int? DoctorID { get; set; }

        public int? WeekDay { get; set; }

        public DateTime? CalendarDate { get; set; }

        public string StartDateTime { get; set; }



        public string EndDateTime { get; set; }
        
        public int UserTypeID { get; set; }

        public int[] DayNo { get; set; }

        public string[] StartTime { get; set; }

        public string[] EndTime { get; set; }

        public int[] SlotDuration { get; set; }

        public string[] Comments { get; set; }

        public bool[] IsHoliday { get; set; }

        public bool[] IsActive { get; set; }

        public bool[] IsDeleted { get; set; }

    }
    public class OrgOpeningHoursUpdateViewModel
    {
        public int OpeningHourID { get; set; }
        public int? DoctorID { get; set; }

        public int? WeekDay { get; set; }

        public DateTime? CalendarDate { get; set; }

        public string StartDateTime { get; set; }



        public string EndDateTime { get; set; }

        [Required(ErrorMessage = "Organization Name is required! Should be select correct Organization")]
        [Range(1, int.MaxValue, ErrorMessage = "Organization Name is required! Should be select correct Organization")]
        public int OrganizationID { get; set; }

        public string OrganisationName { get; set; }
        public int OrganizatonTypeID { get; set; }
        public int UserTypeID { get; set; }

        public int[] DayNo { get; set; }

        public string[] StartTime { get; set; }

        public string[] EndTime { get; set; }

        public int[] SlotDuration { get; set; }

        public string[] Comments { get; set; }

        public bool[] IsHoliday { get; set; }

        public bool[] IsActive { get; set; }

        public bool[] IsDeleted { get; set; }

    }


    public class OrgReviewViewModel
    {
        public long ReviewId { get; set; }
        public int ReferenceId { get; set; }
        public string RefrenceName { get; set; }
        public string Description { get; set; }
        public int Rating { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public int OrganizatonTypeID { get; set; }

        public int UserTypeID { get; set; }

        public int TotalRecordCount { get; set; }
    }

    public class OrgReviewListViewModel : BaseViewModel
    {
        public long ReviewId { get; set; }
        public int OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public string Description { get; set; }
        public int Rating { get; set; }
        public string CreatedUserName { get; set; }


        public int OrganizatonTypeID { get; set; }

        public int UserTypeID { get; set; }

        public int TotalRecordCount { get; set; }
    }


    public class OrgReviewUpdateViewModel
    {
        public long ReviewId { get; set; }
        [Required(ErrorMessage = "Organization Name is required! Should be select correct Organization")]
        [Range(1, int.MaxValue, ErrorMessage = "Organization Name is required! Should be select correct Organization")]
        public int OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public string Description { get; set; }
        public int Rating { get; set; }

        public bool IsActive { get; set; }


        public int OrganizatonTypeID { get; set; }

        public int UserTypeID { get; set; }


    }

    public class OrgSiteImageViewModel
    {
        public int SiteImageId { get; set; }
        public int ReferenceId { get; set; }

        public string ImagePath { get; set; }

        public bool IsProfile { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string ReferenceName { get; set; }

        public int OrganizatonTypeID { get; set; }
        public int UserTypeID { get; set; }

        public string Name { get; set; }

        public int TotalRecordCount { get; set; }
    }

    public class OrgSiteImageListViewModel
    {
        public int SiteImageId { get; set; }
        public int OrganisationId { get; set; }

        public string ImagePath { get; set; }
        public bool IsProfile { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }

        public string OrganisationName { get; set; }
        public int OrganizatonTypeID { get; set; }
        public int UserTypeID { get; set; }

        public string Name { get; set; }

        public int TotalRecordCount { get; set; }
    }
    public class DocSiteImageListViewModel
    {
        public int SiteImageId { get; set; }
        public int DoctorId { get; set; }

        public string ImagePath { get; set; }
        public bool IsProfile { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }

        public string DoctorName { get; set; }
        public int OrganizatonTypeID { get; set; }
        public int UserTypeID { get; set; }

        public string Name { get; set; }

        public int TotalRecordCount { get; set; }
    }

    public class OrgSiteImageUpdateViewModel
    {
        public int SiteImageId { get; set; }

        [Required(ErrorMessage = "Organization Name is required! Should be select correct Organization")]
        [Range(1, int.MaxValue, ErrorMessage = "Organization Name is required! Should be select correct Organization")]
        public int OrganisationId { get; set; }

        [Required(ErrorMessage = "Image path cannot be empty!")]
        public string ImagePath { get; set; }
        public bool IsProfile { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public string OrganisationName { get; set; }
        public int OrganizatonTypeID { get; set; }
        public int UserTypeID { get; set; }

        [Required(ErrorMessage = "Name cannot be empty!")]
        public string Name { get; set; }
    }
    public class DocSiteImageUpdateViewModel
    {
        public int SiteImageId { get; set; }

        
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Image path cannot be empty!")]
        public string ImagePath { get; set; }
        public bool IsProfile { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

      
        public int UserTypeID { get; set; }


        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public bool IsViewMode { get; set; }
    }

    public class OrgInsuranceViewModel
    {
        public int DocOrgInsuranceId { get; set; }
        public int ReferenceId { get; set; }

        public int InsurancePlanId { get; set; }
        public int InsuranceProviderId { get; set; }

        public string InsuranceIdentifierId { get; set; }
        public string StateId { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string ReferenceName { get; set; }
        public string Name { get; set; }

        public int InsuranceTypeId { get; set; }
        public string TypeName { get; set; }

        public int OrganizatonTypeID { get; set; }
        public int UserTypeID { get; set; }

        public int TotalRecordCount { get; set; }
    }

    public class OrgInsuranceListViewModel
    {
        public int DocOrgInsuranceId { get; set; }
        public int OrganisationId { get; set; }

        public int InsurancePlanId { get; set; }
        public int InsuranceProviderId { get; set; }

        public string InsuranceIdentifierId { get; set; }
        public string StateId { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }

        public string OrganisationName { get; set; }
        public string Name { get; set; }

        public int InsuranceTypeId { get; set; }
        public string TypeName { get; set; }

        public int OrganizatonTypeID { get; set; }
        public int UserTypeID { get; set; }

        public int TotalRecordCount { get; set; }
    }

    public class OrgInsuranceUpdateViewModel
    {
        public int DocOrgInsuranceId { get; set; }
        [Required(ErrorMessage = "Organization Name is required! Should be select correct Organization")]
        [Range(1, int.MaxValue, ErrorMessage = "Organization Name is required! Should be select correct Organization")]
        public int OrganisationId { get; set; }

        [Required(ErrorMessage = "Insurance Plan Is Requried")]
        public int InsurancePlanId { get; set; }
        public int InsuranceProviderId { get; set; }

        [Required(ErrorMessage = "Insurance Identifier Id Is Requried")]
        public string InsuranceIdentifierId { get; set; }

        [Required(ErrorMessage = "State Is Requried")]
        public string StateId { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }

        public string OrganisationName { get; set; }
        public string Name { get; set; }

        [Required(ErrorMessage = "Insurance Type Is Requried")]
        public int InsuranceTypeId { get; set; }

        public string TypeName { get; set; }

        public int OrganizatonTypeID { get; set; }
        public int UserTypeID { get; set; }
    }


    public class InsurancePlanDropDownViewModel
    {
        public int InsurancePlanId { get; set; }
        public int InsuranceTypeId { get; set; }
        [StringLength(200)]
        public string Name { get; set; }
    }
    public class OrganizationAddressDropDownViewModel
    {
        public int AddressId { get; set; }
      
       
        public string OrganizationAddress { get; set; }
    }



    public class OrganisationSpecialityViewModel
    {
        public int TaxonomyID { get; set; }
        public int? ParentID { get; set; }

        public string Taxonomy_Code { get; set; }
        public string Specialization { get; set; }
        public string Description { get; set; }
        public string Taxonomy_Type { get; set; }
        public string Taxonomy_Level { get; set; }
        public string SpecialtyText { get; set; }

        public DateTime? CreateDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool? IsSpecialty { get; set; }

        public int CreatedBy { get; set; }

        public int TotalRecordCount { get; set; }

    }

    public class OrganisationSpecialityListViewModel
    {
        public int TaxonomyID { get; set; }
        public int? ParentID { get; set; }

        public string Taxonomy_Code { get; set; }
        public string Specialization { get; set; }
        public string Description { get; set; }
        public string Taxonomy_Type { get; set; }
        public string Taxonomy_Level { get; set; }
        public string SpecialtyText { get; set; }

        public string CreateDate { get; set; }
        public string UpdatedDate { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool? IsSpecialty { get; set; }

        public int CreatedBy { get; set; }

        public int TotalRecordCount { get; set; }
    }

    public class OrganisationSpecialityUpdateViewModel
    {
        public int TaxonomyID { get; set; }
        public int? ParentID { get; set; }

        [Required(ErrorMessage = "Taxonomy code is required! Enter valid Taxonomy Code!")]
        [StringLength(200)]
        [System.Web.Mvc.Remote("ValidateTaxonomyCode", "Pharmacy", AdditionalFields = "TaxonomyID", ErrorMessage = "Taxonomy Code already exists!")]
        public string Taxonomy_Code { get; set; }
        [Required(ErrorMessage = "Specialization is required!")]
        public string Specialization { get; set; }
        public string Description { get; set; }
        [StringLength(200)]
        public string Taxonomy_Type { get; set; }
        [StringLength(200)]
        public string Taxonomy_Level { get; set; }
        public string SpecialtyText { get; set; }


        public bool IsActive { get; set; }
        public bool IsSpecialty { get; set; }

        public string ParentText { get; set; }

    }

    public class CityStateZip
    {
        public string ZipCode { get; set; }
        public int CityStateZipCodeID { get; set; }

    }
    public class OrgProcedureViewModel
    {
        public int OrgProcedureId { get; set; }
        public int ReferenceID { get; set; }
        public int UserTypeID { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }


        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string ReferenceName { get; set; }

        public int OrganizatonTypeID { get; set; }


        public int TotalRecordCount { get; set; }
    }

    public class OrgProcedureListViewModel
    {
        public int OrgProcedureId { get; set; }
        public int OrganisationId { get; set; }
        public int UserTypeID { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }


        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; }
        public string UpdatedDate { get; set; }

        public string OrganisationName { get; set; }

        public int OrganizatonTypeID { get; set; }


        public int TotalRecordCount { get; set; }
    }

    public class OrgProcedureUpdateViewModel
    {
        public int OrgProcedureId { get; set; }
        [Required(ErrorMessage = "Organization Name is required! Should be select correct Organization")]
        [Range(1, int.MaxValue, ErrorMessage = "Organization Name is required! Should be select correct Organization")]
        public int OrganisationId { get; set; }
        public int UserTypeID { get; set; }

        [Required(ErrorMessage = "Procedure Name is required!")]
        [StringLength(200)]
        [System.Web.Mvc.Remote("ValidateProcedureName", "Pharmacy", AdditionalFields = "OrganisationId,OrgProcedureId", ErrorMessage = "Procedure Name already exists!")]
        public string Name { get; set; }
        
        public string Description { get; set; }


        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public bool status { get; set; }

        public string OrganisationName { get; set; }

        public int OrganizatonTypeID { get; set; }
    }


    public class OrgBookingViewModel
    {
        public int SlotId { get; set; }
        public int ReferenceId { get; set; }
        public int? DoctorId { get; set; }
        public int? UserTypeID { get; set; }
        public string FullAddress { get; set; }
        public string DoctorName { get; set; }
        public string Credential { get; set; }

        public string SlotDate { get; set; }
        public string SlotTime { get; set; }

        public int? BookedFor { get; set; }

        public bool IsBooked { get; set; }
        public bool IsEmailReminder { get; set; }
        public bool IsTextReminder { get; set; }
        public bool IsInsuranceChanged { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsViewMode { get; set; }
        public int? InsuranceTypeId { get; set; }
        public int? InsurancePlanId { get; set; }
        public int AddressId { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string OrganisationName { get; set; }
        public string OrganisationAddress { get; set; }

        public int? OrganisationId { get; set; }
        public int? OrganizatonTypeID { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }


        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public int? CityStateZipCodeID { get; set; }
        public int AddressTypeID { get; set; }

        public int TotalRecordCount { get; set; }
    }

    public class OrgBookingListViewModel
    {
        public int InsuranceTypeId { get; set; }
        public int SlotId { get; set; }
        public int OrganisationId { get; set; }
        public int? DoctorId { get; set; }
        public int? UserTypeID { get; set; }

        public string DoctorName { get; set; }

        public string SlotDate { get; set; }
        public string SlotTime { get; set; }

        public int? BookedFor { get; set; }

        public bool IsBooked { get; set; }
        public bool IsEmailReminder { get; set; }
        public bool IsTextReminder { get; set; }
        public bool IsInsuranceChanged { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public int InsurancePlanId { get; set; }
        public int AddressId { get; set; }

        public string Description { get; set; }

        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }

        public string OrganisationName { get; set; }

        public int OrganizatonTypeID { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }


        public string FullAddress { get; set; }
        public int? CityStateZipCodeID { get; set; }
        public int AddressTypeID { get; set; }

        public int TotalRecordCount { get; set; }
    }
    public class DoctorBookingListViewModel
    {
        public int SlotId { get; set; }
        
        public int? DoctorId { get; set; }
        public int? UserTypeID { get; set; }

        public string DoctorName { get; set; }

        public string SlotDate { get; set; }
        public string SlotTime { get; set; }

        public int? BookedFor { get; set; }

        public bool IsBooked { get; set; }
        public bool IsEmailReminder { get; set; }
        public bool IsTextReminder { get; set; }
        public bool IsInsuranceChanged { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public int InsurancePlanId { get; set; }
        public int AddressId { get; set; }

        public string Description { get; set; }

        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }

        public string OrganisationName { get; set; }
        public string OrganisationAddress { get; set; }

      

        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }


        public string FullAddress { get; set; }
        public int? CityStateZipCodeID { get; set; }
        public int AddressTypeID { get; set; }
        public int InsuranceTypeId { get; set; }
        public int TotalRecordCount { get; set; }
    }


    public class OrgBookingUpdateViewModel
    {
        public int SlotId { get; set; }
        [Required(ErrorMessage = "Organization Name is required! Should be select correct Organization")]
        [Range(1, int.MaxValue, ErrorMessage = "Organization Name is required! Should be select correct Organization")]
        public int OrganisationId { get; set; }

        [Required(ErrorMessage = "Doctor Name is required! Should be select correct Doctor")]
        [Range(1, int.MaxValue, ErrorMessage = "Doctor Name is required! Should be select correct Doctor")]
        public int? DoctorId { get; set; }

        public int? UserTypeID { get; set; }

        [Required(ErrorMessage = "Booking date is required!")]
        [StringLength(10)]
        [System.Web.Mvc.Remote("ValidateBookingDate", "Pharmacy", ErrorMessage = "Booking date should be greater than or equal to today's date!")]
        public string SlotDate { get; set; }

        [Required(ErrorMessage = "Booking time is required!")]
        [StringLength(10)]
        [System.Web.Mvc.Remote("ValidateBookingTime", "Pharmacy", AdditionalFields = "SlotDate,OrganisationId,SlotId", ErrorMessage = "Booking already exists!")]
        public string SlotTime { get; set; }

        [Required(ErrorMessage = "Patient Name is required!")]
        [Range(1, int.MaxValue, ErrorMessage = "Patient Name is required! Should be select correct Patient Name")]
        public int? BookedFor { get; set; }

        public bool IsBooked { get; set; }
        public bool IsEmailReminder { get; set; }
        public bool IsTextReminder { get; set; }
        public bool IsInsuranceChanged { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        [Required(ErrorMessage = "Insurance Type is required! Should be select Insurance Type")]
        public int InsuranceTypeId { get; set; }

        [Required(ErrorMessage = "Insurance Plan is required! Should be select Insurance Plan")]
        [Range(1, int.MaxValue, ErrorMessage = "Insurance Plan is required! Should be select Insurance Plan")]
        public int InsurancePlanId { get; set; }

        [Required(ErrorMessage = "Address is required! Should be select Correct Address")]
        public int AddressId { get; set; }

        [Required(ErrorMessage = "Description is Required!")] 
        public string Description { get; set; }

        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }

        public string OrganisationName { get; set; }

        public int OrganizatonTypeID { get; set; }


        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }


        public string FullAddress { get; set; }
        public int? CityStateZipCodeID { get; set; }
        public int AddressTypeID { get; set; }
    }

    public class DoctorBookingUpdateViewModel
    {
        public int SlotId { get; set; }
       
        public int OrganisationId { get; set; }

        [Required(ErrorMessage = "Doctor Name is required! Should be select correct Doctor")]
        [Range(1, int.MaxValue, ErrorMessage = "Doctor Name is required! Should be select correct Doctor")]
        public int? DoctorId { get; set; }

        public int? UserTypeID { get; set; }

        [Required(ErrorMessage = "Booking date is required!")]
        [StringLength(10)]
        //[System.Web.Mvc.Remote("ValidateBookingDate", "Doctor", ErrorMessage = "Booking date should be greater than today's date!")]
        public string SlotDate { get; set; }

        [Required(ErrorMessage = "Booking time is required!")]
        [StringLength(10)]
        //[System.Web.Mvc.Remote("ValidateBookingTime", "Doctor", AdditionalFields = "SlotDate,SlotId", ErrorMessage = "Booking already exists!")]
        public string SlotTime { get; set; }

        [Required(ErrorMessage = "Patient Name is required!")]
        [Range(1, int.MaxValue, ErrorMessage = "Patient Name is required! Should be select correct Patient Name")]
        public int? BookedFor { get; set; }

        public bool IsBooked { get; set; }
        public bool IsEmailReminder { get; set; }
        public bool IsTextReminder { get; set; }
        public bool IsInsuranceChanged { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsViewMode { get; set; }
        [Required(ErrorMessage = "Insurance Type is required! Should be select Insurance Type")]
        public int InsuranceTypeId { get; set; }

        [Required(ErrorMessage = "Insurance Plan is required! Should be select Insurance Plan")]
        [Range(1, int.MaxValue, ErrorMessage = "Insurance Plan is required! Should be select Insurance Plan")]
        public int InsurancePlanId { get; set; }

        [Required(ErrorMessage = "Address is required! Should be select Correct Address")]
        public int AddressId { get; set; }

        public string Description { get; set; }

        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }

        public string OrganisationName { get; set; }

        public int OrganizatonTypeID { get; set; }


        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }


        public string FullAddress { get; set; }
        public int? CityStateZipCodeID { get; set; }
        public int AddressTypeID { get; set; }
    }

    public class OrgDoctorsDropDownViewModel
    {
        public int OrganizationId { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string Credential { get; set; }
        public string DisplayName { get; set; }
        public int DoctorAffiliationId { get; set; }
    }


    public class PatientNameAutoCompleteViewModel
    {
        public int PatientId { get; set; }
        public int UserId { get; set; }
        public int AddressId { get; set; }
        public int AddressTypeID { get; set; }
        public int CityStateZipCodeID { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        public string SlotDate { get; set; }
    }

    public class PatientNameAutoCompleteSummaryViewModel
    {
        public int PatientId { get; set; }
        public int UserId { get; set; }
        public int AddressId { get; set; }
        public int AddressTypeID { get; set; }
        public int CityStateZipCodeID { get; set; }

        public string SlotDate { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string FullName { get; set; }
        public string FullAddress { get; set; }
    }

    public class OrgPatientOrderViewModel
    {
        public long OrderId { get; set; }
        public int PatientId { get; set; }
        public int ReferenceId { get; set; }
        public int UserTypeID { get; set; }
        public int AddressId { get; set; }

        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PrescriptionImage { get; set; }

        public decimal? TotalPrice { get; set; }
        public decimal? CouponDiscount { get; set; }
        public decimal? OtherDiscount { get; set; }
        public decimal? NetPrice { get; set; }

        public string OrderStatus { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public int? InsurancePlanId { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string ReferenceName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }

        public int OrganizationTypeID { get; set; }

        public int TotalRecordCount { get; set; }
    }

    public class OrgPatientOrderListViewModel
    {
        public long OrderId { get; set; }
        public int PatientId { get; set; }
        public int ReferenceId { get; set; }
        public int UserTypeID { get; set; }
        public int AddressId { get; set; }

        public string Date { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PrescriptionImage { get; set; }

        public decimal? TotalPrice { get; set; }
        public decimal? CouponDiscount { get; set; }
        public decimal? OtherDiscount { get; set; }
        public decimal? NetPrice { get; set; }

        public string OrderStatus { get; set; }

        public bool IsActive { get; set; }

        public int? InsurancePlanId { get; set; }

        public string UpdatedDate { get; set; }

        public string ReferenceName { get; set; }
        public string PatientName { get; set; }

        public int OrganizationTypeID { get; set; }

        public string OrgAddress { get; set; }

        public int TotalRecordCount { get; set; }
    }

    public class OrgPatientOrderUpdateViewModel
    {
        public long OrderId { get; set; }
        [Required(ErrorMessage = "Patient Name is required!")]
        [Range(1, int.MaxValue, ErrorMessage = "Patient Name is required! Should be select correct Patient Name")]
        public int PatientId { get; set; }
        public int OrganisationId { get; set; }
        public int UserTypeID { get; set; }
        [Required(ErrorMessage = "Organization address is required!")]
        public int AddressId { get; set; }

        [Required(ErrorMessage = "Order date is required!")]
        public DateTime? Date { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PrescriptionImage { get; set; }

        public decimal? TotalPrice { get; set; }
        public decimal? CouponDiscount { get; set; }
        public decimal? OtherDiscount { get; set; }
        public decimal? NetPrice { get; set; }

        [Required(ErrorMessage = "Order status required!")]
        public string OrderStatus { get; set; }

        public bool IsActive { get; set; }

        public int? InsurancePlanId { get; set; }
        public int? InsuranceTypeId { get; set; }

        public string FullName { get; set; }

        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        [Required(ErrorMessage = "One or more order items required!")]
        public string DragIds { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }

        public int OrganizationTypeID { get; set; }
    }

    public class OrgPatientOrderDetailsViewModel
    {
        public long OrderDetailId { get; set; }
        public long OrderId { get; set; }
        public int DrugId { get; set; }
        public string DrugName { get; set; }
        public string Description { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? Quantity { get; set; }
        public decimal? TotalAmount { get; set; }
    }

    public class OrgPatientOrderDrugAutocompleteViewModel
    {
        public int DrugId { get; set; }
        public string DrugName { get; set; }
        public string ShortDescription { get; set; }
    }

    public class OrgGetDrugInfoViewModel
    {
        public int DrugId { get; set; }
        public string DrugName { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }

    }

    public class OrgPatientInfoViewModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }


    //***********************************************

    public class GetOrgIDViewModel
    {
        public int OrganisationId { get; set; }
    }

    public class OrganisationSlotList
    {
        public int SlotId { get; set; }
        public string SlotDate { get; set; }
        public string SlotTime { get; set; }
        public int ReferenceId { get; set; }
        public string DoctorName { get; set; }
        public int BookedFor { get; set; }
        public string PatientName { get; set; }
        public bool IsBooked { get; set; }
        public bool IsEmailReminder { get; set; }
        public bool IsTextReminder { get; set; }
        public bool IsInsuranceChanged { get; set; }
        public bool IsActive { get; set; }
        public int InsurancePlanId { get; set; }
        public string InsurancePlanName { get; set; }
        public int AddressId { get; set; }
        public string Description { get; set; }
        public int TotalRows { get; set; }
        public string IsBookedString { get; set; }
        public string IsEmailReminderString { get; set; }
        public string IsTextReminderString { get; set; }
        public string IsInsuranceChangedString { get; set; }
        public string IsActiveString { get; set; }
        public string Address { get; set; }
    }

    public class DrpTaxonomyCodes
    {
        public string Taxonomy_Code { get; set; }
    }

    public class DrpAdvertisementType
    {
        public string AdvertisementTypeName { get; set; }
        public int  AdvertisementTypeId { get; set; }
    }

    public class DrpPaymentType
    {
        public string Name { get; set; }
        public int PaymentTypeId { get; set; }
    }
    public class DrpAddress
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string Address { get { return this.Address1 + " " + this.Address2 + " " +  this.City + " " + this.State + " " + this.Country + " " + this.ZipCode; } }
        public int AddressId { get; set; }

    }

    public class DrpUser
    {
        public string Uniquekey { set; get; }
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Please enter First name")]
        [MaxLength(50, ErrorMessage = "Please enter First name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter Last name")]
        [MaxLength(50, ErrorMessage = "Please enter Last name")]
        public string LastName { get; set; }

        [MaxLength(50, ErrorMessage = "Please enter Middle name")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Please enter email address")]
        [EmailAddress]
        public string Email { get; set; }

        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,150}$",
        // ErrorMessage = "Passwords must contain at least one lowercase letter, one uppercase letter, and one number.")]

        [DataType(DataType.Password)]

        public string Password { get; set; }


        public string ConfirmPassword { get; set; }
        public string FullName { get { return this.FirstName + " " + this.MiddleName + " " + this.LastName; } }

        public string UserName { get; set; }
        public int UserTypeId { get; set; }
    }

    public class DrpInsurancePlan
    {
        public int InsurancePlanId { get; set; }
        public string Name { get; set; }
    }
    public class DrpInsuranceType
    {
        public int InsuranceTypeId { get; set; }
        public string Name { get; set; }
    }
    public class DrpInsuranceProvider
    {
        public int InsProviderId { get; set; }
        public string InsCompanyName { get; set; }
    }

    public class DrpState
    {
        public int CityStatZipCodeID { get; set; }
        public string State { get; set; }
    }
}
