using System.Collections.Generic;
using System.Linq;

namespace Binke.Model.ViewModels
{
    public class SearchDoctorResult
    {
        public IList<DoctorSearchList> listDoctors;
        public long CountOfAllowNewPatients => listDoctors.LongCount(d => d.IsAllow == 1);
        public long CountOfNtPcp => listDoctors.LongCount(d => d.IsNtPcp == 1);
        public long CountOfPrimaryCare => listDoctors.LongCount(d => d.IsPrimaryCare == 1);

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecord { get; set; }
    }


    public class DoctorSearchList
    {
        public int DoctorId { get; set; }
        public string Suffix { get; set; }
        public string ProfilePicture { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Npi { get; set; }
        public string Education { get; set; }
        public string ShortDesc { get; set; }
        public string LongDesc { get; set; }
        public int IsAllow { get; set; }
        public int IsNtPcp { get; set; }
        public int IsPrimaryCare { get; set; }
        public string SpecitiesIds { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public List<string> Specities { get; set; }
        public string FacilityIds { get; set; }
        public List<string> Facility { get; set; }
        public int IsAllowNewPatient { get; set; }
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
}
