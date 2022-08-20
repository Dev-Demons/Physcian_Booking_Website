using Doctyme.Model.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Doctyme.Model.ViewModels
{
    public class FeaturedDoctorListModel
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string DoctorFullName { get; set; }

        public string DoctorURLName { get; set; }
        public string Npi { get; set; }
        public string SpecialityName { get; set; }
        public string ProfileImage { get; set; }

        public string RatingNos { get; set; }
        public int Rating { get; set; }
        public decimal ReviewStars { get { return Extension.GetReviewStars(RatingNos); } }
        public int ReviewCount { get; set; }

        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string LinkedIn { get; set; }
        public string Specialization { get; set; }
    }

    public class FeaturedFacilityListModel
    {
        public string Npi { get; set; }
        public int OrganisationId { get; set; }
        public int UserTypeId { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public string ProfileImage { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }

        public string RatingNos { get; set; }
        public int Rating { get; set; }
        public decimal ReviewStars { get { return Extension.GetReviewStars(RatingNos); } }
        public int ReviewCount { get; set; }

        public string FullAddress
        {
            get
            {
                if (!string.IsNullOrEmpty(CityName))
                    return CityName + (!string.IsNullOrEmpty(StateName) ? ", " + StateName : string.Empty);
                if (!string.IsNullOrEmpty(StateName))
                    return StateName;

                return string.Empty;
            }
        }
    }

    public class ReviewProviderListModel
    {
        public List<ReviewProviderModel> ReviewProviderList { get; set; }
        //public int PageIndex { get; set; }
        //public int PageSize { get; set; }
        public int TotalRecord { get; set; }
    }

    public class ReviewProviderModel
    {
        public Int64 ReviewId { get; set; }
        public int ReferenceId { get; set; }
        public string Description { get; set; }
        public int Rating { get; set; }
        public bool IsActive{get; set;}
        public string IsActiveString { get; set; }
        public int? DoctorId { get; set; }
    }

    public class FacilityProviderListModel
    {
        public List<FacilityProviderModel> FacilityProviderList { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecord { get; set; }
    }

    public class FacilityProviderModel
    {
        public int OrganisationId { get; set; }
        public int? OrganizationTypeID { get; set; }
        public int? UserTypeID { get; set; }
        public string Location
        {
            get
            {
                return Address1 + (!string.IsNullOrEmpty(Address1) ? ", " : "")
                    + Address2 + (!string.IsNullOrEmpty(Address2) ? ", " : "")
                    + CityName + (!string.IsNullOrEmpty(CityName) ? ", " : "")
                    + StateName + (!string.IsNullOrEmpty(StateName) ? ", " : "")
                    + Country + (!string.IsNullOrEmpty(Country) ? ", " : "")
                    + ZipCode;
            }
        }

        public string Speciality { get; set; }
        public string Email { get; set; }
        public string OrganisationName { get; set; }
        public string FullName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public bool? IsLatLong { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public bool IsActive { get; set; }
        public bool? EnabledBooking { get; set; }
    }

    public class SpecialityProviderListModel
    {
        public List<SpecialityProviderModel> SpecialityProviderModel { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecord { get; set; }
    }

    public class SpecialityProviderModel
    {
        public int TotalRecord { get; set; }
        public int TaxonomyID { get; set; }
        public string Taxonomy_Code { get; set; }
        public string Specialization { get; set; }
        public string Description { get; set; }
        public int? ParentID { get; set; }
        public bool? IsActive { get; set; }
        public string IsActiveString { get; set; }
        public string ParentSpecialization { get; set; }
        public bool? IsDeleted { get; set; }
        public string Taxonomy_Type { get; set; }
        public string Taxonomy_Level { get; set; }
        public string SpecialtyText { get; set; }
        public bool? IsSpecialty { get; set; }
        public string IsSpecialtyString { get; set; }
    }


    public class InsurancePlanProviderListModel
    {
        public List<InsurancePlanProviderModel> InsurancePlanProviderList { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecordCount { get; set; }
    }

    public class SummaryProviderModel
    {
        public int OrganisationId { get; set; }
        public string ShortDescription { get; set; }
    }
    public class InsurancePlanProviderModel
    {
        public int DocOrgInsuranceId { get; set; }
        public int ReferenceId { get; set; }
        public int InsurancePlanId { get; set; }
        public int InsuranceTypeId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string InsuranceIdentifierId { get; set; }
        public string StateId { get; set; }
        public string IsActiveString { get; set; }
        public string InsuranceTypeName { get; set; }

    }

    public class StateLicenseProviderListModel
    {
        public List<StateLicenseProviderModel> StateLicenseProviderList { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecordCount { get; set; }
    }

    public class StateLicenseProviderModel
    {
        public int DocOrgStateLicense { get; set; }
        public int ReferenceId { get; set; }
        public string HealthCareProviderTaxonomyCode { get; set; }
        public string ProviderLicenseNumber { get; set; }
        public string ProviderLicenseNumberStateCode { get; set; }
        public bool? HealthcareProviderPrimaryTaxonomySwitch { get; set; }
        public string HealthcareProviderPrimaryTaxonomySwitchString { get; set; }
        public bool? IsActive { get; set; }
        public string IsActiveString { get; set; } 
    }

    public class OpeningHourProviderListModel
    {
        public List<OpeningHoursProviderModel> OpeningHourProviderList { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecordCount { get; set; }
    }

    public class TotalRecords
    {
        public int TotalRecordCount { get; set; }
    }
    public class OpeningHoursProviderModel
    {
        public int TotalRecordCount { get; set; }
        public int OpeningHourID { get; set; }
        public int? DoctorID { get; set; }
        public int? OrganizationID { get; set; }
        public string OrganisationName { get; set; }
        public DateTime? CalendarDate { get; set; }
        public int? WeekDay { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public int[] DayNo { get; set; }
        public string[] StartTime { get; set; }
        public string[] EndTime { get; set; }
        public int[] SlotDuration { get; set; }
        public string[] Comments { get; set; }
        public bool[] IsHoliday { get; set; }
        public bool[] IsActive { get; set; }
        public string[] IsHolidayString { get; set; }
        public string[] IsActiveString { get; set; }

    }

    public class OrganizationProviderListModel
    {
        public List<OrganizationProviderModel> OrganizationProviderList { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecord { get; set; }
    }

    public class OrganizationProviderModel
    {
        public int TotalRecord { get; set; }
        public string Npi { get; set; }
        public int OrganisationId { get; set; }
        public int OrganizationTypeID { get; set; }
        public int UserTypeID { get; set; }
        public string OrganisationName { get; set; }
        public string FullName { get; set; }
        public string ProfilePicture { get; set; }

        public string OpenUntil { get; set; }
        public string AmenityOptions { get; set; }
        public List<string> AmenityOptionList => AmenityOptions.ToStringList();
        public string Amenities { get; set; }
        public List<string> AmenitiesList => Amenities.ToStringList();
        public string PhoneNumber { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public bool IsLatLong { get; set; }
        public int DistanceCount { get; set; }

        public string RatingNos { get; set; }
        public int Rating { get; set; }
        public decimal ReviewStars { get { return Extension.GetReviewStars(RatingNos); } }
        public int ReviewCount { get; set; }

        public string FullAddress
        {
            get
            {
                List<string> stringList = new List<string>();
                if (!string.IsNullOrEmpty(Address1))
                    stringList.Add(Address1);
                if (!string.IsNullOrEmpty(Address2))
                    stringList.Add(Address2);
                if (!string.IsNullOrEmpty(CityName))
                    stringList.Add(CityName);
                if (!string.IsNullOrEmpty(StateName))
                    stringList.Add(StateName);
                if (!string.IsNullOrEmpty(Country))
                    stringList.Add(Country);
                if (!string.IsNullOrEmpty(ZipCode))
                    stringList.Add(ZipCode);

                return string.Join(", ", stringList);
            }
        }
        public string TaxonomyName { get; set; }
    }

    public class DrpFacilityTypeModel
    {
        public int OrganizationTypeID { get; set; }
        public string OrganizationTypeName { get; set; }
    }

    public class OrgReviewListModel : BaseViewModel
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

    public class OrgReviewUpdateViewModel:BaseModel
    {
        public long ReviewId { get; set; }
        [Required(ErrorMessage = "Organization Name is required! Should be select correct Organization")]
        [Range(1, int.MaxValue, ErrorMessage = "Organization Name is required! Should be select correct Organization")]
        public int OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public string Description { get; set; }
        public int Rating { get; set; }
        public int UserTypeId { get; set; }
        public int OrganizationTypeId { get; set; }
    }
}
