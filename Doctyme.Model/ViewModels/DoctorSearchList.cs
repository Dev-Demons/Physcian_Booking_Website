using Doctyme.Model.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using static Doctyme.Model.Utility.Extension;

namespace Doctyme.Model.ViewModels
{
    public class SearchDoctorResult
    {
        public IList<DoctorSearchList> listDoctors;
        public long CountOfAllowNewPatients => listDoctors.LongCount(d => d.IsAllowNewPatient == 1);
        public long CountOfNtPcp => listDoctors.LongCount(d => d.IsNtPcp == 1);
        public long CountOfPrimaryCare => listDoctors.LongCount(d => d.IsPrimaryCare == 1);

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecord { get; set; }
    }

    public class FeaturedDoctors
    {
        public int ReferenceId { get; set; }
    }

    public class FeaturedFacilities
    {
        public int ReferenceId { get; set; }
    }

    public class DoctorDetails
    {
        public int DoctorId { get; set; }
        public string SpecitiesIds { get; set; }
        public string FacilityIds { get; set; }
        public string RatingNos { get; set; }
        public int ReviewCount { get; set; }
        public int OfficeCount { get; set; }
        public int InsuaranceCount { get; set; }
        public string ListSpecities { get; set; }

    }
    public class CityStateZipDetail
    {
        public int Id { get; set; }
        public string ZipCityState { get; set; }

    }
    public class IpInfo
    {

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("loc")]
        public string Loc { get; set; }

        [JsonProperty("org")]
        public string Org { get; set; }

        [JsonProperty("postal")]
        public string Postal { get; set; }

        [JsonProperty("zip")]
        public string zip { get; set; }

        [JsonProperty("region_code")]
        public string region_code { get; set; }
    }

    public class DoctorSearchList
    {
        public int DoctorId { get; set; }
        public string Npi { get; set; }
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
                        return $"{dateTimeSpan.Years} Years experience";
                    }
                }
                return string.Empty;
            }
        }
        public string Credential { get; set; }
        public string FullName { get; set; }
        public string ProfilePicture { get; set; }
        public string Address1 { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public double DistanceCount { get; set; }
        public string SpecitiesIds { get; set; }
        public List<string> Specities { get; set; }
        public string FacilityIds { get; set; }
        public List<OrgFacitiesInfo> Facility { get; set; }
        public int EnabledBooking { get; set; }
        public int IsPrimaryCare { get; set; }
        public int IsAllowNewPatient { get; set; }
        public int IsNtPcp { get; set; }
        public string RatingNos { get; set; }
        public decimal ReviewStars { get { return Extension.GetReviewStars(RatingNos); } }
        public int ReviewCount { get; set; }
        public int OfficeCount { get; set; }
        public int InsuranceCount { get; set; }
        public string FullAddress { get { return Address1; } }
        public int TotalRecordCount { get; set; }
        public string OrgName { get; set; }
    }
    public class OrgFacitiesInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string OrgNpi { get; set; }
        public int? OrgType { get; set; }

        public string OrgTypeUrl
        {
            get
            {
                if (OrgType.HasValue && OrgType > 0)
                {
                    string url = string.Empty;
                    switch (OrgType)
                    {
                        case 1006:
                            url = "/Profile/Facility/";
                            break;
                        case 1007:
                            url = "/Profile/SeniorCare/";
                            break;
                        case 1005:
                        default:
                            url = "/Profile/Pharmacy/";
                            break;

                    }
                    if (!string.IsNullOrEmpty(url))
                        return url = url + Name.Replace("&", " ").Replace(" ", "-").Replace("--", "-") + "-" + OrgNpi;
                    else
                        return url = "#";

                }
                return string.Empty;
            }
        }
    }

    public class SearchPharmacyResult
    {
        public IList<PharmacySearchList> listPharmacys;
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecord { get; set; }
    }
    public class PharmacySearchList
    {
        public int PharmacyId { get; set; }
        public string PharmayName { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public string FullName { get; set; }
        public string ProfilePicture { get; set; }
        public string PhoneNumber { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
    }

    public class SearchSeniorCareResult
    {
        public IList<SeniorCareSearchList> listSeniorCare;
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecord { get; set; }
    }

    public class SeniorCareSearchList
    {
        public int SeniorCareId { get; set; }
        public string SeniorCareName { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public string FullName { get; set; }
        public string ProfilePicture { get; set; }
        public string PhoneNumber { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
    }

    public class SearchFacilityResult
    {
        public IList<FacilitySearchList> listFacility;
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecord { get; set; }
    }


    public class OrganisationsWithDistance
    {
        public int OrganisationId { get; set; }
        public decimal Lat { get; set; }

        public decimal Long { get; set; }
        public double DistanceCount { get; set; }
    }

    public class DoctorWithDistance
    {
        public int DoctorId { get; set; }
        public decimal Lat { get; set; }

        public decimal Long { get; set; }
        public double DistanceCount { get; set; }
    }

    public class FacilitySearchList
    {
        public int FacilityId { get; set; }
        public string FacilityName { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public string FullName { get; set; }
        public string ProfilePicture { get; set; }
        public string PhoneNumber { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
    }

    public class DrpLanguageModel
    {
        public short LanguageId { get; set; }
        public string LanguageName { get; set; }
    }

    public class KeyValueModel
    {
        public int Key { get; set; }
        public string Value { get; set; }
    }

    public class DrpKeyValueModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
    }
}
