using Doctyme.Model.Utility;
using Doctyme.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Doctyme.Model.Utility.Extension;

namespace Doctyme.Model
{
    [Table("DoctorBoardCertification")]
    public class DoctorBoardCertification : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DoctorBoardCertificationId { get; set; }

        public short BoardCertificationId { get; set; }
        [ForeignKey("BoardCertificationId")]
        public virtual BoardCertifications BoardCertification { get; set; }

        public int DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }

    }
    public class DoctorBoardCertificationModel : BaseModel
    {
        public int DoctorBoardCertificationId { get; set; }
        public int BoardCertificationId { get; set; }
        public string Name { get; set; }
        public string BoardCertificationName { get; set; }
        public string Description { get; set; }
        public bool IsViewMode { get; set; }
        public string DoctorName { get; set; }
        public int DoctorId { get; set; }
        public List<BoardCertifications> BoardCertificationsList { get; set; }
        public bool IsActive { get; set; }
        public int TotalRows { get; set; }

    }

    public class DoctorQualificationViewModel : BaseModel
    {
        public int QualificationId { get; set; }

        public string Institute { get; set; }
        public string Degree { get; set; }
        public string PassingYear { get; set; }
        public string Notes { get; set; }

        public bool IsViewMode { get; set; }
        public string DoctorName { get; set; }
        public int DoctorId { get; set; }
        public List<BoardCertifications> BoardCertificationsList { get; set; }
        public int TotalRows { get; set; }

    }
    public class DoctorViewModel : BaseModel
    {
        DateTime? _enumerationDate;
        bool _soleProprietor;
        bool _enabledBooking;
        DateTime? _practiceStartDate;

        public int UserId { get; set; }
        public int UserTypeID { get; set; }
        public string NamePrefix { get; set; }
        public string NameSuffix { get; set; }
        [Required(ErrorMessage = "Credential is required!")]
        public string Credential { get; set; }

        [Required(ErrorMessage = "First Name is required!")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required!")]
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Status { get; set; }
        public DateTime? EnumerationDate { get { return _enumerationDate; } set { _enumerationDate = value.HasValue ? value.Value : new Nullable<DateTime>(); } }
        public string LogoFilePath { get; set; }
        [System.Web.Mvc.Remote("ValidateNPI", "Pharmacy", AdditionalFields = "DoctorId", ErrorMessage = "NPI already exists")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "NPI length is 10 digits")]
        [Required(ErrorMessage = "NPI is required!")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Accept only number")]
        public string NPI { get; set; }
        public string Education { get; set; }
       
        public string ShortDescription { get; set; }
   
        public string LongDescription { get; set; }
        public bool? SoleProprietor { get { return _soleProprietor; } set { _soleProprietor = value.HasValue ? value.Value : false; } }
        public bool IsAllowNewPatient { get; set; }
        public bool IsNtPcp { get; set; }
        public string Language { get; set; }
        public string OtherNames { get; set; }

        public bool IsPrimaryCare { get; set; }

        [StringLength(10)]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Accept only number")]
        public string PhoneNumber { get; set; }
        public bool? EnabledBooking { get { return _enabledBooking; } set { _enabledBooking = value.HasValue ? value.Value : false; } }
        public string Keywords { get; set; }
        public DateTime? PracticeStartDate { get { return _practiceStartDate; } set { _practiceStartDate = value.HasValue ? value.Value : new Nullable<DateTime>(); } }

        public bool IsViewMode { get; set; }
        public string DoctorName { get; set; }
        public int DoctorId { get; set; }
        public List<BoardCertifications> BoardCertificationsList { get; set; }
        public int TotalRows { get; set; }
        public List<DropDownModel> LanguagesList { get; set; }
        //public decimal Long { get; set; }
        //public decimal Lat { get; set; }
        public double DistanceCount { get; set; }
        public string Address1 { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

    }

    public class DoctorListViewModel : BaseModel
    {
        public int UserId { get; set; }
        public int UserTypeID { get; set; }
        public string NamePrefix { get; set; }
        public string NameSuffix { get; set; }

        public string Credential { get; set; }

        [Required(ErrorMessage = "First Name is required!")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required!")]
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Status { get; set; }
        public DateTime? EnumerationDate { get; set; }
        public string LogoFilePath { get; set; }
        [Required(ErrorMessage = "NPI is required!")]
        public string NPI { get; set; }
        public string Education { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }
        public bool SoleProprietor { get; set; }
        public bool IsAllowNewPatient { get; set; }
        public bool IsNtPcp { get; set; }
        public string Language { get; set; }
        public string OtherNames { get; set; }

        public bool IsPrimaryCare { get; set; }

        public bool EnabledBooking { get; set; }
        public string Keywords { get; set; }
        public DateTime? PracticeStartDate { get; set; }

        public bool IsViewMode { get; set; }
        public string DoctorName { get; set; }
        public int DoctorId { get; set; }
        public List<BoardCertifications> BoardCertificationsList { get; set; }
        public int TotalRows { get; set; }
        public List<DropDownModel> LanguagesList { get; set; }







        //public int DoctorId { get; set; }
        public string Suffix { get; set; }
        public string ProfilePicture { get; set; }
        //public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Npi { get; set; }
        //public string Education { get; set; }
        public string ShortDesc { get; set; }
        public string LongDesc { get; set; }

        //public DateTime? PracticeStartDate { get; set; }
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

        public int IsAllow { get; set; }
        //public int IsNtPcp { get; set; }
        //public int IsPrimaryCare { get; set; }
        public string SpecitiesIds { get; set; }
        //public string Address1 { get; set; }
        //public string Address2 { get; set; }
        //public string City { get; set; }
        //public string Country { get; set; }
        //public string State { get; set; }
        //public string ZipCode { get; set; }
        public List<string> Specities { get; set; }
        public string FacilityIds { get; set; }
        public List<KeyValueModel> Facility { get; set; }
        //public int IsAllowNewPatient { get; set; }
        //public string Credential { get; set; }
        //public int EnabledBooking { get; set; }
        public int Experience { get; set; }
        public int OfficeCount { get; set; }
        public int InsuranceCount { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public bool IsLatLong { get; set; }
        public int DistanceCount { get; set; }

        public string RatingNos { get; set; }
        public int Rating { get; set; }
        public decimal ReviewStars { get { return Extension.GetReviewStars(RatingNos); } }
        public int ReviewCount { get; set; }

        //public string FullAddress { get { return Address1; } }

    }
    public class DoctorExperienceViewModel : BaseModel
    {
        public int ExperienceId { get; set; }

        public string Designation { get; set; }
        public string Organization { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool IsViewMode { get; set; }
        public string DoctorName { get; set; }
        public int DoctorId { get; set; }

        public bool IsActive { get; set; }
        public int TotalRows { get; set; }

    }
    public class DoctorAffiliationViewModel : BaseModel
    {

        public int DoctorAffiliationId { get; set; }

        [Required(ErrorMessage = "Affiliation Name is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Affiliation Name is required")]
        public int OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public string Description { get; set; }
        public bool IsViewMode { get; set; }
        public string DoctorName { get; set; }
        public int DoctorId { get; set; }
        public int? AddressId { get; set; }
        public string FullAddress { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public List<DropDownModel> OrganizationList { get; set; }
        public bool IsActive { get; set; }
        public int TotalRows { get; set; }

    }
    public class DoctorInsurancesViewModel : BaseModel
    {
        [Required(ErrorMessage = "InsurancePlan is required")]
        [Range(1, int.MaxValue, ErrorMessage = "InsurancePlan is required!")]
        public int InsurancePlanId { get; set; }
        public string InsuranceIdentifierId { get; set; }
        public string StateId { get; set; }
        public int DocOrgInsuranceId { get; set; }
        public int ReferenceId { get; set; }
        public int UserTypeId { get; set; }
        public bool IsViewMode { get; set; }
        public string DoctorName { get; set; }
        public int DoctorId { get; set; }
        public List<DropDownModel> InsurancesPlanList { get; set; }
        public List<DropDownModel> ReferencesList { get; set; }
        public List<DropDownModel> StateList { get; set; }
        public int TotalRows { get; set; }
        public string InsurancePlanName { get; set; }
        public string ReferenceName { get; set; }

    }
    public class DoctorLanguageViewModel : BaseModel
    {
        public int DoctorLanguageId { get; set; }

        [Required(ErrorMessage = "Language is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Language is required")]
        public int LanguageId { get; set; }

        public int DoctorId { get; set; }

        public string LanguageName { get; set; }

        public string DoctorName { get; set; }
        public int TotalRows { get; set; }
        public List<DropDownModel> LanguagesList { get; set; }
        public bool IsViewMode { get; set; }
        public string LanguageCode { get; set; }

    }

    public class DoctorBookingViewModel : BaseModel
    {
        public int SlotId { get; set; }
        public DateTime SlotDate { get; set; }
        public string SlotTime { get; set; }
        public int ReferenceId { get; set; }
        public string ReferenceName { get; set; }
        public string PatientName { get; set; }
        public int BookedFor { get; set; }
        public bool IsBooked { get; set; }
        public bool IsEmailReminder { get; set; }
        public bool IsTextReminder { get; set; }
        public bool IsInsuranceChanged { get; set; }
        public int InsurancePlanId { get; set; }
        public int DoctorId { get; set; }

        public string LanguageName { get; set; }

        public string DoctorName { get; set; }
        public int TotalRows { get; set; }
        public List<DropDownModel> LanguagesList { get; set; }
        public bool IsViewMode { get; set; }
        public string LanguageCode { get; set; }

    }
}
