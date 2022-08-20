using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Doctyme.Model.Utility;

namespace Doctyme.Model.ViewModels
{
    public class DoctorBasicInformations 
    {
        public int DoctorId { get; set; }

        public string Prefix { get; set; }

        public string Suffix { get; set; }

      
        public string FirstName { get; set; }

        public string FullName { get; set; }

        public string ProfilePicture { get; set; }

        public string Email { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string PhoneExt { get; set; }

        public string PhoneNumber { get; set; }

        public string FaxNumber { get; set; }

        public bool IsAllowNewPatient { get; set; }

        public bool IsNtPcp { get; set; }

        public bool IsPrimaryCare { get; set; }

      
        public string Npi { get; set; }

      
        public string Education { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public List<short> Speciality { get; set; }

        public List<int> IssuranceAccepted { get; set; }

        public List<int> AgeGroup { get; set; }

    }

    public class DoctorFeaturedAddEditModel : BaseModel
    {
        public int FeaturedId { get; set; }

        [Required(ErrorMessage = "Doctor Name is required!")]
        [Range(1, int.MaxValue, ErrorMessage = "Doctor Name is required!")]
        public int DoctorId { get; set; }

        [StringLength(200)]
        public string DoctorName { get; set; }

        public string ProfileImage { get; set; }

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

        [JsonConverter(typeof(Base64FileJsonConverter))]
        public byte[] ProfilePic { get; set; }
    }

    public class ImageModel
    {
        public string ImageName { get; set; }

        [JsonConverter(typeof(Base64FileJsonConverter))]
        public byte[] Image { get; set; }
    }
}
