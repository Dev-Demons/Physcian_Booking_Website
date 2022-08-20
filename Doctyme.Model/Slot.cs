namespace Doctyme.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Slot")]
    public partial class Slot
    {
        public int SlotId { get; set; }

        [StringLength(10)]
        public string SlotDate { get; set; }

        [StringLength(10)]
        public string SlotTime { get; set; }

        //public DateTime SlotDate { get; set; }
        //public DateTime SlotTime { get; set; }

        public int ReferenceId { get; set; }

        public int? BookedFor { get; set; }

        public bool IsBooked { get; set; }

        public bool IsEmailReminder { get; set; }

        public bool IsTextReminder { get; set; }

        public bool IsInsuranceChanged { get; set; }

        public bool IsActive { get; set; }

        public int InsurancePlanId { get; set; }

        public int AddressId { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public int CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }
        //[ForeignKey("ReferenceId")]
        //public virtual Organisation Organisation { get; set; }

    }

    public class OrgBookingModel:BaseModel
    {
        public int SlotId { get; set; }
        public int ReferenceId { get; set; }
        public int? DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string Credential { get; set; }

        public string SlotDate { get; set; }
        public string SlotTime { get; set; }

        public int? BookedFor { get; set; }

        public bool IsBooked { get; set; }
        public bool IsEmailReminder { get; set; }
        public bool IsTextReminder { get; set; }
        public bool IsInsuranceChanged { get; set; }

        public int InsuranceTypeId { get; set; }
        public int InsurancePlanId { get; set; }
        public int AddressId { get; set; }

        public string Description { get; set; }

        public string OrganisationName { get; set; }

        public int? OrganisationId { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string PatientName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public int? CityStateZipCodeID { get; set; }
        public int AddressTypeID { get; set; }

        public int TotalRecordCount { get; set; }
        public int UserTypeId { get; set; }
        public int? OrganizationTypeId { get; set; }
    }

    public class DoctorBookingListModel
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


        public string PatientName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }


        public string FullAddress { get; set; }
        public int? CityStateZipCodeID { get; set; }
        public int AddressTypeID { get; set; }

        public int TotalRecordCount { get; set; }
    }
    public class OrganizationAddressDropDownViewModels
    {
        public int AddressId { get; set; }


        public string OrganizationAddress { get; set; }
    }
    public class OrgDoctorsDropDownViewModels
    {
        public int OrganizationId { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string Credential { get; set; }
        public string DisplayName { get; set; }
        public int DoctorAffiliationId { get; set; }
    }
    public class InsurancePlanDropDownViewModels
    {
        public int InsurancePlanId { get; set; }
        public int InsuranceTypeId { get; set; }
        [StringLength(200)]
        public string Name { get; set; }
    }
    public class DoctorBookingUpdateModel
    {
        public int SlotId { get; set; }

        public int OrganisationId { get; set; }

        [Required(ErrorMessage = "Doctor Name is required! Should be select correct Doctor")]
        [Range(1, int.MaxValue, ErrorMessage = "Doctor Name is required! Should be select correct Doctor")]
        public int? DoctorId { get; set; }

        public int? UserTypeID { get; set; }

        [Required(ErrorMessage = "Booking date is required!")]
        [StringLength(10)]
        //[System.Web.Mvc.Remote("ValidateBookingDate", "Doctor", ErrorMessage = "Booking date should be greater than or equal to today's date!")]
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

        public string PatientName { get; set; }
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

        public int UserId { get; set; }
        public string FullAddress { get; set; }
        public int? CityStateZipCodeID { get; set; }
        public int AddressTypeID { get; set; }

        public int CreatedBy { get; set; }
    }
}
