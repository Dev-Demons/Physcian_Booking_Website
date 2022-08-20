using Doctyme.Model.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Doctyme.Model.ViewModels
{
    public class ProfileViewModel
    {
        private DateTime? _enumerationDate;
        private string _organisationName;
        private int _numberOfYears;
        public int OrganisationId { get; set; }
        public int OrganizationTypeID { get; set; }
        public string OrganisationName { get { if (_organisationName != null) return _organisationName; else return string.Empty; } set { _organisationName = value; } }
        public DateTime? EnumerationDate { get { if (_enumerationDate != null) return _enumerationDate; else return DateTime.Now; } set { _enumerationDate = value; } }

        public int NumberOfYears { get {
                if (_enumerationDate.HasValue)
                {
                    DateTime PresentYear = DateTime.Now;
                    TimeSpan ts = PresentYear - EnumerationDate.Value;
                    DateTime NosYears = DateTime.MinValue.AddDays(ts.Days);
                    return NosYears.Year-1;
                } 
                else
                {
                    return 0;
                } } set { _numberOfYears = value; } }
        public string OrganisationDescription { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        public string TaxonomyName { get; set; }

        public List<string> lstTaxonomyName { get { return Extension.ToStringList(TaxonomyName); } }
        public string ZipCode { get; set; }

        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string OpeningTime { get; set; }
        public int MaxDays { get; set; }
        public int Maxslots { get; set; }
        public int CalenderDatesCount { get; set; }
        public string RatingNos { get; set; }
        public int Rating { get; set; }
        public decimal ReviewStars { get { return Extension.GetReviewStars(RatingNos); } }
        public int ReviewCount { get; set; }

        public List<SlotsDates> lstslotsDates { get; set; }
        public List<SlotTimes> lstslotTimes { get; set; }

        public List<Reviews> lstReviews { get; set; }
        public List<SiteImages> lstSiteImages { get; set; }
        public List<Cost> lstCost { get; set; }
        public List<OrganizationAmenityOptions> lstOrgAmenOpt { get; set; }
        public virtual ICollection<OpeningHour> OpeningHours { get; set; }

        public virtual ICollection<SocialMedia> SocialMediaLinks { get; set; }

        public List<OrgAmenityOptions> lstOrgAmenityOptions { get; set; }
        public bool IsClaimed { get; set; }
        public string TaxonomyCode { get; set; }
        public string WebSite { get; set; }
        public List<ProviderAdvertisements> lstProviderAdvertisements { get; set; } //Added by Reena
        public string ImagePath { get; set; }
        public string NPI { get; set; }
        public int AddressId { get; set; }

    }
    public class SlotConfirmation
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string FullName { get; set; }
        public int AddressId { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        //[Required]
        public int CityId { get; set; }
        [Required]
        public string City { get; set; }
        //[Required]
        public int StateId { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string ZipCode { get; set; }
        public string Country { get; set; }
        [Required]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Please enter PhoneNumber as 0123456789, 012-345-6789, (012)-345-6789.")]
        public virtual string PhoneNumber { get; set; }
        [Required]
        public string DOB { get; set; }
        public int OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public string OrgaizationAddress { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string DoctorImg { get; set; }
        public string SlotTime { get; set; }
        public string SlotDate { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Enter Valid Email.")]
        public string UserEmail { get; set; }
        public string InsuranceProviderId { get; set; }
        public string InsuranceProviderName { get; set; }
        public string InsurancePlanId { get; set; }
        public string InsurancePlanName { get; set; }
        public string InsuranceTypeId { get; set; }
        public string InsuranceTypeName { get; set; }
        public short IsEmailReminder { get; set; }
        public short IsTextReminder { get; set; }
        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }
        public short IsWhantCallUs { get; set; }
        [Required]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public string DoctorProfileImage { get; set; }
        public string FullOrgAddress { get; set; }
        public string FullOrgName { get; set; }
        public string NPI { get; set; }
        public string ReturUrl { get; set; }
        public string SlotFor { get; set; }
    }

    public class SlotConfirmationMessage
    {
        public string ErrorMessage { get; set; }
        public int UserId { get; set; }
    }
    public class OrganizationAmenityOptions
    {
        public int OrganizationID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public bool IsOption { get; set; }
    }
    public class SiteImages
    {
        public string ImagePath { set; get; }
        public bool IsProfile { set; get; }
    }
    public class ClaimPractice
    {
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }

        public string DocumentName { get; set; }
        public int DocumentTypeId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Distance { get; set; }
        public string ClaimType { get; set; }
        public string ReturnUrl { get; set; }
        public bool Isagree { get; set; }
        public string Npi { get; set; }
        public string Country { get; set; }
        public int ReferenceId { get; set; }


        public HttpPostedFileBase PostedFile { get; set; }
        [Required(ErrorMessage = "Please select file.")]
        public string FileName { get; set; }
    }
    public class Cost
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
    public class OrgAmenityOptions
    {
        public int OrganizationID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public bool IsOption { get; set; }
    }

    public class ProviderAdvertisements //Added by Reena
    {
        public int AdvertisementId { get; set; }
        public int ReferenceId { get; set; }
        public int PaymentTypeId { get; set; }
        public string Title { get; set; }
        public string ImagePath { get; set; }
        public string PaymentTypeName { get; set; }
        public string PaymentTypeDescription { get; set; }
        public int OrganisationId { get; set; }
        public decimal Lat { get; set; }

        public decimal Long { get; set; }
        public double DistanceCount { get; set; }
    }
}
