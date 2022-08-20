using Doctyme.Model.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using static Doctyme.Model.Utility.Extension;
using Newtonsoft.Json;
using Doctyme.Model.ViewModels;

namespace Doctyme.Model.ViewModels
{
    public class OrganizationViewModel
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
        public string OpeningTime { get; set; }
        public int MaxDays { get; set; }
        public int Maxslots { get; set; }
        public int CalenderDatesCount { get; set; }
        public int OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public int UserTypeId { get; set; }
        public virtual ICollection<Qualification> Qualifications { get; set; }

        public virtual ICollection<SiteImage> DoctorImages { get; set; }

        public List<Experiences> Experiences { get; set; }

        public List<Languages> DoctorLanguages { get; set; }

        public List<Certifications> DoctorBoardCertifications { get; set; }
        public virtual ICollection<SocialMedia> SocialMediaLinks { get; set; }
        public List<Reviews> Reviews { get; set; }
        public virtual ICollection<OpeningHour> OpeningHours { get; set; }
        public List<SlotsDates> lstslotsDates { get; set; }
        public List<SlotTimes> lstslotTimes { get; set; }
        public List<OrgAddress> lstOrgAddress { get; set; }
        public List<DoctorAffiliations> lstDoctorAffiliations { get; set; }
        public List<InsuranceAccepted> lstInsuranceAccepted { get; set; }
        public List<DoctorAdvertisements> lstDoctorAdvertisements { get; set; }
        public List<OrgAmenityOptions> lstOrgAmenityOptions { get; set; }
        public string OpeningDay { get; set; }
        public string ShortDescription { get; set; }
        public DateTime? PracticeStartDate { get; set; }
        public string PracticeStartDateStr
        {
            get
            {
                if (PracticeStartDate.HasValue)
                {
                    var dateTimeSpan = DateTimeSpan.CompareDates(PracticeStartDate.Value, DateTime.Now);
                    if (dateTimeSpan.Years == 0 && dateTimeSpan.Months == 1)
                        return $"{dateTimeSpan.Months} Month experience";
                    else if (dateTimeSpan.Years == 0 && dateTimeSpan.Months > 1)
                        return $"{dateTimeSpan.Months} Months experience";
                    else if (dateTimeSpan.Years > 1)
                    {
                        //if (dateTimeSpan.Months > 1)
                        //    return $"{dateTimeSpan.Years} Years {dateTimeSpan.Months} Months experience";
                        //else
                        //    return $"{dateTimeSpan.Years} Years experience";
                        return $"{dateTimeSpan.Years}";
                    }
                }
                return string.Empty;
            }
        }
        public string RatingNos { get; set; }
        public int Rating { get; set; }
        public decimal ReviewStars { get { return Extension.GetReviewStars(RatingNos); } }
        public int ReviewCount { get; set; }
        public string Speciality { get; set; }
        public string NPI { get; set; }
        public string ReturUrl { get; set; }
        public string SlotFor { get; set; }
        public bool? IsClaimed { get; set; }
        public string TaxonomyCode { get; set; }
        public string DoctorName { get; set; }
    }
    public class SlotsDates
    {
        public string SlotDate { get; set; }
        public int WeekNumber { get; set; }
        public string WeekName { get; set; }
        public int DoctorId { get; set; }
        public string CalenderDate { get; set; }
        public int OrganizationId { get; set; }
        public string StartDateTime { get; set; }
        public string EndDateTime { get; set; }
        public int IsHoliday { get; set; }
        public int IsActive { get; set; }
        public int IsDeleted { get; set; }
        public int SlotDuration { get; set; }
        public Int64 Rno { get; set; }
        public int MaxSlots { get; set; }
        public int MaxCount { get; set; }
        public string DisplayDate { get; set; }
        public string displayweek { get; set; }
    }
    public class SlotTimes
    {
        public DateTime CalenderDate { get; set; }
        public int WeeKNumber { get; set; }
        public int DoctorId { get; set; }
        public int OrganizationID { get; set; }
        public int IsHoliday { get; set; }
        public int IsActive { get; set; }
        public int IsDeleted { get; set; }
        public int SlotDuration { get; set; }
        public string SlotSatrtTime { get; set; }
        public string SlotEndTIme { get; set; }
        public string TimeSlot { get; set; }
        public Int64 Rno { get; set; }
        public Int64 slotTImesRno { get; set; }
        public string SlotDate { get; set; }
        public bool IsBooked { get; set; }

    }
    public class OrgAddress
    {
        public int OrganisationId { get; set; }
        public int DoctorId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public int AddressId { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public int CityStateZipCodeID { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string LocationType { get; set; }
        public decimal Lat { get; set; }
        public decimal Long { get; set; }
        public decimal Xaxis { get; set; }
        public decimal Yaxis { get; set; }
        public decimal Zaxis { get; set; }
        public string WorldRegion { get; set; }
        public string Country { get; set; }
        public string LocationText { get; set; }
        public string Location { get; set; }
        public bool Decommissioned { get; set; }
        public string OrganisationName { get; set; }
        public string TaxonomyCode { get; set; }
        public string LogoFilePath { get; set; }
        public int AddressTypeId { get; set; }//Added by Reena
    }
    public class Certifications
    {
        public int BoardCertificationId { get; set; }
        public string CertificationName { get; set; }
        public string Description { get; set; }
    }
    public class Languages
    {
        public int LanguageId { get; set; }
        public string LanguageName { get; set; }
    }
    public class Reviews
    {
        public long ReviewId { get; set; }
        public string Description { get; set; }
        public int Rating { get; set; }
        public string CreatedDate { get; set; }
        public string UserName { get; set; }
    }
    public class Experiences
    {
        public int ExperienceId { get; set; }
        public int DoctorId { get; set; }
        public string Designation { get; set; }
        public string Organization { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Description { get; set; }
    }
    public class DoctorAffiliations
    {
        public int DoctorAffiliationId { get; set; }
        public int DoctorId { get; set; }
        public int OrganisationId { get; set; }
        public string OrganisationName { get; set; }
    }

    public class InsuranceAccepted
    {
        public int InsurancePlanId { get; set; }
        public string InsuranceProviderName { get; set; }
        public string InsurancePlanName { get; set; }
        public string InsurancePlanDetails { get; set; }
    }

    public class DoctorAdvertisements
    {
        public int AdvertisementId { get; set; }
        public int ReferenceId { get; set; }
        public int PaymentTypeId { get; set; }
        public string Title { get; set; }
        public string ImagePath { get; set; }
        public string PaymentTypeName { get; set; }
        public string PaymentTypeDescription { get; set; }
    }

    public class OrgStateLicenseModel
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

        public string ReferenceName { get; set; }
        public int OrganisationId { get; set; }
        public int TotalRecordCount { get; set; }

        public int UserTypeId { get; set; }
        public int OrganizationTypeId { get; set; }
    }

    public class OrgStateLicenseListModel
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

    public class OrgStateLicenseUpdateModel : BaseModel
    {
        public int DocOrgStateLicenseId { get; set; }

        [Required(ErrorMessage ="Taxonomy Code is required")]
        public string HealthCareProviderTaxonomyCode { get; set; }
        
        [Required(ErrorMessage = "License Number is required")]
        public string ProviderLicenseNumber { get; set; }
        public string ProviderLicenseNumberStateCode { get; set; }
        public bool? HealthcareProviderPrimaryTaxonomySwitch { get; set; }

        [Required(ErrorMessage = "Organization Name is required! Should be select correct Organization")]
        [Range(1, int.MaxValue, ErrorMessage = "Organization Name is required! Should be select correct Organization")]
        public int OrganisationId { get; set; }

        public string OrganisationName { get; set; }

        public int UserTypeId { get; set; }
        public int OrganizationTypeId { get; set; }


    }

    public class OrgSiteImageModel : BaseModel
    {
        public int SiteImageId { get; set; }
        public int ReferenceId { get; set; }

        public string ImagePath { get; set; }

        public bool IsProfile { get; set; }

        public string ReferenceName { get; set; }

        public string Name { get; set; }

        public int TotalRecordCount { get; set; }
        public int UserTypeId { get; set; }
        public int OrganizationTypeId { get; set; }
    }

    public class OrgSiteImageListModel : BaseModel
    {
        public int SiteImageId { get; set; }
        public int OrganisationId { get; set; }
        public string ImagePath { get; set; }
        public bool IsProfile { get; set; }
        public string OrganisationName { get; set; }

        public string Name { get; set; }

        public int TotalRecordCount { get; set; }
    }

    public class OrgSiteImageUpdateModel :BaseModel
    {
        public int SiteImageId { get; set; }

        [Required(ErrorMessage = "Organization Name is required! Should be select correct Organization")]
        [Range(1, int.MaxValue, ErrorMessage = "Organization Name is required! Should be select correct Organization")]
        public int OrganisationId { get; set; }

        [Required(ErrorMessage = "Image path cannot be empty!")]
        public string ImagePath { get; set; }
        public bool IsProfile { get; set; }
        public string OrganisationName { get; set; }
        //public int OrganizatonTypeID { get; set; }

        public string Name { get; set; }
        public int UserTypeId { get; set; }
        public int OrganizationTypeId { get; set; }

        [JsonConverter(typeof(Base64FileJsonConverter))]
        public byte[] Image { get; set; }
    }

    public class OrgInsuranceModel :BaseModel
    {
        public int DocOrgInsuranceId { get; set; }
        public int ReferenceId { get; set; }

        public int InsurancePlanId { get; set; }
        public int InsuranceProviderId { get; set; }

        public string InsuranceIdentifierId { get; set; }
        public string StateId { get; set; }

        public string ReferenceName { get; set; }
        public string Name { get; set; }

        public int InsuranceTypeId { get; set; }
        public string TypeName { get; set; }

        public int TotalRecordCount { get; set; }
        public int UserTypeId { get; set; }
        public int OrganizationTypeId { get; set; }
    }

    public class OrgInsuranceListModel:BaseModel
    {
        public int DocOrgInsuranceId { get; set; }
        public int OrganisationId { get; set; }

        public int InsurancePlanId { get; set; }
        public int InsuranceProviderId { get; set; }

        public string InsuranceIdentifierId { get; set; }
        public string StateId { get; set; }

        public string OrganisationName { get; set; }
        public string Name { get; set; }

        public int InsuranceTypeId { get; set; }
        public string TypeName { get; set; }
        public int TotalRecordCount { get; set; }
    }

    public class OrgInsuranceUpdateModel :BaseModel
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

        public string OrganisationName { get; set; }
        public string Name { get; set; }

        [Required(ErrorMessage = "Insurance Type Is Requried")]
        public int InsuranceTypeId { get; set; }

        public string TypeName { get; set; }
        public int UserTypeId { get; set; }
        public int OrganizationTypeId { get; set; }
    }

    public class OrgOpeningHoursModel:BaseModel
    {
        public int OpeningHourID { get; set; }
        public int? DoctorID { get; set; }
        public int? OrganizationID { get; set; }
        public int? WeekDay { get; set; }

        public string StartDateTime { get; set; }
        public DateTime? CalendarDate { get; set; }
        public int? SlotDuration { get; set; }

        public bool? IsHoliday { get; set; }

        public string EndDateTime { get; set; }

        public string Comments { get; set; }

        public string OrganisationName { get; set; }

        public int TotalRecordCount { get; set; }
        public int UserTypeId { get; set; }
        public int OrganizationTypeId { get; set; }
    }

    public class OrgOpeningHoursListModel : BaseModel
    {
        public int OpeningHourID { get; set; }
        public int? DoctorID { get; set; }

        public int? WeekDay { get; set; }
        public string WeekDayName { get; set; }
        public DateTime? CalendarDate { get; set; }
        public int? SlotDuration { get; set; }

        public string StartDateTime { get; set; }

        public bool? IsHoliday { get; set; }

        public string EndDateTime { get; set; }

        public int? OrganizationID { get; set; }

        public string OrganisationName { get; set; }
        public int UserTypeId { get; set; }
        public int OrganizationTypeId { get; set; }
        public int TotalRecordCount { get; set; }
    }

    public class OrgOpeningHoursUpdateModel:BaseModel
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

        public int[] DayNo { get; set; }

        public string[] StartTime { get; set; }

        public string[] EndTime { get; set; }

        public int[] SlotDuration { get; set; }

        public string[] Comments { get; set; }

        public bool[] IsHoliday { get; set; }

        public bool[] IsActive { get; set; }

        public bool[] IsDeleted { get; set; }

    }

    public class OrganisationFeaturedModel : BaseModel
    {
        public int FeaturedId { get; set; }

        public int ReferenceId { get; set; }
        public int? OrganisationId { get; set; }
        public string ReferenceName { get; set; }

        public string ProfileImage { get; set; }

        public string Description { get; set; }

        public int? TotalImpressions { get; set; }
        public int? PaymentTypeID { get; set; }
        public int? AdvertisementLocationID { get; set; }

        public string Title { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int? CityStateZipCodeID { get; set; }
        public int TotalRecordCount { get; set; }
        public int UserTypeId { get; set; }
        public int? OrganizationTypeId { get; set; }


    }

    public class OrganisationFeaturedListModel : BaseModel
    {
        public int FeaturedId { get; set; }

        public int ReferenceId { get; set; }
        public int? OrganisationId { get; set; }
        public string OrganisationName { get; set; }

        public int? DoctorId { get; set; }
        public string DoctorName { get; set; }
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

        public int? CityStateZipCodeID { get; set; }

        public int TotalRecordCount { get; set; }
    }

    public class OrganisationFeaturedAddEditModel:BaseModel
    {
        public int FeaturedId { get; set; }

        [Required(ErrorMessage = "Organization Name is required!")]
        [Range(1, int.MaxValue, ErrorMessage = "Organization Name is required!")]
        public int OrganisationId { get; set; }

        [StringLength(200)]
        public string OrganisationName { get; set; }

        public string ProfileImage { get; set; }

        [JsonConverter(typeof(Base64FileJsonConverter))]
        public byte[] Image { get; set; }

        [StringLength(200)]
        public string Title { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }

        public int? AdvertisementLocationID { get; set; }

        [Required(ErrorMessage = "Start Date is required!")]
        public DateTime? FeaturdStartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? TotalImpressions { get; set; }
        public int? PaymentTypeID { get; set; }

        [Required(ErrorMessage = "Zip code, City and State is required!")]
        public int? CityStateZipCodeID { get; set; }

        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Long { get; set; }

        public int UserTypeId { get; set; }
        public int? OrganizationTypeId { get; set; }
    }

    public class OrganizationAmenityOptionsModel:BaseModel
    {

        public int OrganizationAmenityOptionID { get; set; }
        public int OrganizationID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ImagePath { get; set; }

        public bool IsOption { get; set; }

        public string OrganisationName { get; set; }
        public int TotalRecordCount { get; set; }
        public int UserTypeId { get; set; }
        public int OrganizationTypeId { get; set; }

    }

    public class OrganizationAmenityOptionListModel : BaseModel
    {

        public int OrganizationAmenityOptionID { get; set; }
        public int OrganizationID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ImagePath { get; set; }

        public bool IsOption { get; set; }

        public string OrganisationName { get; set; }
        public int TotalRecordCount { get; set; }
        public int UserTypeId { get; set; }
        public int OrganizationTypeId { get; set; }

    }

    public class OrganizationAmenityOptionsUpdateModel:BaseModel
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

        [JsonConverter(typeof(Base64FileJsonConverter))]
        public byte[] Image { get; set; }

        public bool IsOption { get; set; }
        public string OrganisationName { get; set; }
        public int UserTypeId { get; set; }
        public int OrganizationTypeId { get; set; }
    }

    public class OrganisationTaxonomyModel:BaseModel
    {
        public int DocOrgTaxonomyID { get; set; }
        public int ReferenceID { get; set; }
        public int ParentID { get; set; }
        public int TaxonomyID { get; set; }

        public string Taxonomy_Code { get; set; }
        public string Specialization { get; set; }

        public string OrganisationName { get; set; }

        public string IsActiveString { get; set; }
        public string Description { get; set; }
        public int TotalRecordCount { get; set; }
        public int UserTypeId { get; set; }
        public int OrganizationTypeId { get; set; }
    }

    public class OrganisationTaxonomyListModel:BaseModel
    {
        public int DocOrgTaxonomyID { get; set; }
        public int ReferenceID { get; set; }
        public int TaxonomyID { get; set; }
        public int? ParentID { get; set; }
        public string Taxonomy_Code { get; set; }
        public string Specialization { get; set; }

        public int OrganisationId { get; set; }

        public string OrganisationName { get; set; }

        public string IsActiveString { get; set; }
        public string Description { get; set; }
        public int TotalRecordCount { get; set; }
        public int UserTypeId { get; set; }
        public int OrganizationTypeId { get; set; }
    }

    public class OrganisationTaxonomyUpdateModel:BaseModel
    {
        public int DocOrgTaxonomyID { get; set; }

        [Required(ErrorMessage = "Taxonomy code is required! Should be select valid Taxonomy Code!")]
        [Range(1, int.MaxValue, ErrorMessage = "Taxonomy code is required! Should be select valid Taxonomy Code!")]
        public int TaxonomyID { get; set; }
        public string Taxonomy_Code { get; set; }
        public string Specialization { get; set; }

        [Required(ErrorMessage = "Organization Name is required! Should be select correct Organization")]
        [Range(1, int.MaxValue, ErrorMessage = "Organization Name is required! Should be select correct Organization")]
        public int OrganisationId { get; set; }

        public string OrganisationName { get; set; }

        public int UserTypeId { get; set; }
        public int OrganizationTypeId { get; set; }

    }

    public class OrgSocialMediaModel:BaseModel
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

        public int OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public int TotalRecordCount { get; set; }
        public int UserTypeId { get; set; }
        public int OrganizationTypeId { get; set; }
    }

    public class OrgSocialMediaListModel:BaseModel
    {
        public int SocialMediaId { get; set; }

        public string Facebook { get; set; }

        public string Twitter { get; set; }

        public string LinkedIn { get; set; }

        public string Instagram { get; set; }

        public string Youtube { get; set; }

        public string Pinterest { get; set; }

        public string Tumblr { get; set; }
        public string IsActiveString { get; set; }

        public int OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public int TotalRecordCount { get; set; }
        public int UserTypeId { get; set; }
        public int OrganizationTypeId { get; set; }
    }

    public class OrgSocialMediaUpdateModel:BaseModel
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

        [Required(ErrorMessage = "Organization Name is required!")]
        [Range(1, int.MaxValue, ErrorMessage = "Organization Name is required!")]
        public int OrganisationId { get; set; }

        public string OrganisationName { get; set; }
        public int TotalRecordCount { get; set; }
        public int UserTypeId { get; set; }
        public int OrganizationTypeId { get; set; }
    }

    //public class OrgBookingModel:BaseModel
    //{
    //    public int SlotId { get; set; }
    //    public int ReferenceId { get; set; }
    //    public int? DoctorId { get; set; }
    //    public string DoctorName { get; set; }
    //    public string Credential { get; set; }

    //    public string SlotDate { get; set; }
    //    public string SlotTime { get; set; }

    //    public int? BookedFor { get; set; }

    //    public bool IsBooked { get; set; }
    //    public bool IsEmailReminder { get; set; }
    //    public bool IsTextReminder { get; set; }
    //    public bool IsInsuranceChanged { get; set; }
    //    public int InsuranceTypeId { get; set; }
    //    public int InsurancePlanId { get; set; }
    //    public int AddressId { get; set; }

    //    public string Description { get; set; }

    //    public string OrganisationName { get; set; }

    //    public int OrganisationId { get; set; }

    //    public string FirstName { get; set; }
    //    public string MiddleName { get; set; }
    //    public string LastName { get; set; }
    //    public string Email { get; set; }
    //    public string PhoneNumber { get; set; }


    //    public string Address1 { get; set; }
    //    public string Address2 { get; set; }
    //    public string ZipCode { get; set; }
    //    public string City { get; set; }
    //    public string State { get; set; }
    //    public string Country { get; set; }
    //    public int? CityStateZipCodeID { get; set; }
    //    public int AddressTypeID { get; set; }

    //    public int TotalRecordCount { get; set; }
    //}

    public class OrgBookingListModel:BaseModel
    {
        public int SlotId { get; set; }
        public int OrganisationId { get; set; }
        public int? DoctorId { get; set; }

        public string DoctorName { get; set; }

        public string SlotDate { get; set; }
        public string SlotTime { get; set; }

        public int? BookedFor { get; set; }

        public bool IsBooked { get; set; }
        public bool IsEmailReminder { get; set; }
        public bool IsTextReminder { get; set; }
        public bool IsInsuranceChanged { get; set; }

        public int InsurancePlanId { get; set; }
        public int AddressId { get; set; }

        public string Description { get; set; }

        public string OrganisationName { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }


        public string FullAddress { get; set; }
        public int? CityStateZipCodeID { get; set; }
        public int AddressTypeID { get; set; }

        public int TotalRecordCount { get; set; }
        public int UserTypeId { get; set; }
        public int OrganizationTypeId { get; set; }
    }

    public class OrgBookingUpdateModel:BaseModel
    {
        public int SlotId { get; set; }
        [Required(ErrorMessage = "Organization Name is required! Should be select correct Organization")]
        [Range(1, int.MaxValue, ErrorMessage = "Organization Name is required! Should be select correct Organization")]
        public int OrganisationId { get; set; }

        [Required(ErrorMessage = "Doctor Name is required! Should be select correct Doctor")]
        [Range(1, int.MaxValue, ErrorMessage = "Doctor Name is required! Should be select correct Doctor")]
        public int? DoctorId { get; set; }

        [Required(ErrorMessage = "Booking date is required!")]
        [StringLength(10)]
        //[System.Web.Mvc.Remote("ValidateBookingDate", "Pharmacy", ErrorMessage = "Booking date should be greater than or equal to today's date!")]
        public string SlotDate { get; set; }

        [Required(ErrorMessage = "Booking time is required!")]
        [StringLength(10)]
        //[System.Web.Mvc.Remote("ValidateBookingTime", "Pharmacy", AdditionalFields = "SlotDate,OrganisationId,SlotId", ErrorMessage = "Booking already exists!")]
        public string SlotTime { get; set; }

        [Required(ErrorMessage = "Patient Name is required!")]
        [Range(1, int.MaxValue, ErrorMessage = "Patient Name is required! Should be select correct Patient Name")]
        public int? BookedFor { get; set; }

        public bool IsBooked { get; set; }
        public bool IsEmailReminder { get; set; }
        public bool IsTextReminder { get; set; }
        public bool IsInsuranceChanged { get; set; }

        [Required(ErrorMessage = "Insurance Type is required! Should be select Insurance Type")]
        public int InsuranceTypeId { get; set; }

        [Required(ErrorMessage = "Insurance Plan is required! Should be select Insurance Plan")]
        [Range(1, int.MaxValue, ErrorMessage = "Insurance Plan is required! Should be select Insurance Plan")]
        public int InsurancePlanId { get; set; }

        [Required(ErrorMessage = "Address is required! Should be select Correct Address")]
        public int AddressId { get; set; }

        public string Description { get; set; }

        public string OrganisationName { get; set; }


        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }


        public string FullAddress { get; set; }
        public int? CityStateZipCodeID { get; set; }
        public int AddressTypeID { get; set; }
        public int UserTypeId { get; set; }
        public int OrganizationTypeId { get; set; }
    }

    public class OrganisationProfileModel
    {
        public int OrganisationId { get; set; }

        public int? UserId { get; set; }

        [Required(ErrorMessage = "Organization type id is required")]
        public int OrganizationTypeID { get; set; }

        [Required(ErrorMessage = "Organization name is required")]
        [StringLength(200)]
        public string OrganisationName { get; set; }

        public int? UserTypeID { get; set; }

        //[System.Web.Mvc.Remote("ValidateNPI", "Pharmacy", AdditionalFields = "OrganisationId", ErrorMessage = "NPI already exists")]
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

        public int TotalRecordCount { get; set; }
    }

    public class OrganisationProfileListModel : BaseModel
    {
        public int Id { get; set; }
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

    public class OrganisationAddressListModel : BaseModel
    {
        public int OrganisationId { get; set; }
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
        public int ReferenceId { get; set; }
        public int CityStateZipCodeID { get; set; }
        public string IsActiveString { get; set; }

        public int TotalRecordCount { get; set; }
    }

    public class OrganisationOfficeLocationModel : BaseModel
    {
        public int OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string FullAddress { get; set; }

    }

    public class CostModel:BaseModel
    {
        public int CostID { get; set; }

        [Required(ErrorMessage ="Organization Name is required")]
        public int ReferenceID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal? Price { get; set; }

        public string CreatedDateString { get; set; }
        public string UpdatedDateString { get; set; }

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

        [JsonConverter(typeof(Base64FileJsonConverter))]
        public byte[] Image { get; set; }

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

        //[Required(ErrorMessage = "One or more order items required!")]
        //public string DragIds { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }

        public int OrganizationTypeID { get; set; }

        public List<OrgPatientOrderDetailsViewModel> OrderDetails { get; set; }
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

    public class OrgPatientInfoViewModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }

    public class InsurancePlanDropDownModel
    {
        public int InsurancePlanId { get; set; }
        public int InsuranceTypeId { get; set; }
        [StringLength(200)]
        public string Name { get; set; }
    }

    public class OrgGetDrugInfoViewModel
    {
        public int DrugId { get; set; }
        public string DrugName { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }

    }
}