using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Doctyme.Model;
using Doctyme.Model.ViewModels;
using Newtonsoft.Json;

namespace Binke.ViewModels
{
    public class BaseViewModel
    {
        public int Id { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsEnable { get; set; }
    }

    public class KeyValuePairModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
    }

    public class SelectIdValueModel
    {
        public int Id { get; set; }
        public string StrId { get; set; }
        public string Value { get; set; }
    }

    public class DateSlotModel : AsSerializeable
    {
        public string Date { get; set; }
        public List<TimeSlotModel> TimeSlots { get; set; }
        public string DayOfWeek { get; set; }
    }

    public class DateSlotRecords
    {
        public int Total { get; set; }
        public List<object> DateSlotModel { get; set; }
        public DoctorDetail Doctor { get; set; }
        public FacilityDetail Facility { get; set; }
        public SeniorCareDetail SeniorCare { get; set; }
    }

    public class DoctorDetail
    {
        public int DoctorId { get; set; }
        public int? UserId { get; set; }
        public virtual ApplicationUser DoctorUser { get; set; }
        public List<AddressViewModel> Address { get; set; }
    }

    public class FacilityDetail
    {
        public int FacilityId { get; set; }
        public virtual ApplicationUser FacilityUser { get; set; }
        public List<AddressViewModel> Address { get; set; }
    }

    public class SeniorCareDetail
    {
        public int SeniorCareId { get; set; }
        public virtual ApplicationUser SeniorCareUser { get; set; }
        public List<AddressViewModel> Address { get; set; }
    }

    public abstract class AsSerializeable
    {
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class TimeSlotModel
    {
        public int Id { get; set; }
        public string Time { get; set; }
        public bool IsBook { get; set; }
    }

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

    public class SearchViewModel
    {
        public int AcceptingNewPatients { get; set; }
        public int NearTermPcpAvailability { get; set; }
        public int PrimaryCare { get; set; }
        public int Male { get; set; }
        public int Female { get; set; }
        public AgeGroupsSeen AgeGroupsSeen { get; set; }
        public IpInfo IpInfo { get; set; }
        public Location Location { get; set; }
        public List<KeyValuePairModel> Insurance { get; set; }

    }
    public class Location
    {
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
    }
    public class Facility : BaseViewModel
    {
        public string FacilityName { get; set; }
        public string ProfilePicture { get; set; }
    }

    public class AgeGroup
    {
        public int Age { get; set; }
    }

    public class TimeSlotRequest
    {
        public int DoctorId { get; set; }
        public int FacilityId { get; set; }
        public int SeniorCareId { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
