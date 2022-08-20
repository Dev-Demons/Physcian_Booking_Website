using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Binke.ViewModels
{
    public class PatientBasicInformation : BaseViewModel
    {
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Please enter first name."),
         StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter middle name."),
         StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Please enter last name."),
         StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please select Gender.")]
        public string Gender { get; set; }

        public string FullName { get; set; }

        //[StringLength(6, ErrorMessage = "The {0} must be at least {2} characters long.")]
        public string PhoneExt { get; set; }

        public string PhoneNumber { get; set; }

        public string FaxNumber { get; set; }

        public string DateOfBirth { get; set; }

        public string PrimaryInsurance { get; set; }

        public string SecondaryInsurance { get; set; }

        public string ProfilePicture { get; set; }

        public string Email { get; set; }

        public AddressViewModel AddressView { get; set; }
    }
    public enum PrescriptionType
    {
        New,
        Refill,
        Transfer
    }

    public enum DeliveryType
    {
        Home,
        Store
    }
    public class PatientProfile : BaseViewModel
    {
        public int UserId { get; set; }
        public int? PatientId { get; set; }

        [Required(ErrorMessage = "Please enter first name."),
         StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter middle name."),
        StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Please enter last name."),
         StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string LastName { get; set; }

        //[Required(ErrorMessage = "Please select Gender.")]
        public string Gender { get; set; }

        //public string FullName { get; set; }

        //[StringLength(6, ErrorMessage = "The {0} must be at least {2} characters long.")]
        // public string PhoneExt { get; set; }

        public string PhoneNumber { get; set; }

        //public string FaxNumber { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public int? InsuranceTypeId { get; set; }
        public int? InsurancePlanID { get; set; }
        public string HealthInsurance { get; set; }

        //public string SecondaryInsurance { get; set; }

        public string ProfilePicture { get; set; }

        [Required(ErrorMessage = "Please enter email address")]
        [EmailAddress]
        public string Email { get; set; }

        public string Address { get; set; }
        public int? AddressId { get; set; }
        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        public int? CityStateZipCodeID { get; set; }
        public int? DoctorId { get; set; }
        public string PrimaryDoctor { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }

    public class PatientBooking : BaseViewModel
    {
        public string SlotDate { get; set; }

        public string SlotTime { get; set; }

        public string InsCompanyName { get; set; }

        public string Description { get; set; }

        public int TotalRecordCount { get; set; }
    }

    public class PatientOrders
    {
        public Int64 OrderId { get; set; }
        public int? PatientId { get; set; }
        public int ReferenceId { get; set; }
        public int UserTypeID { get; set; }
        //public int AddressId { get; set; }
        public DateTime Date { get; set; }

        public string OrderDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PrescriptionImage { get; set; }
        public decimal? TotalPrice { get; set; }        
        public decimal? CouponDiscount { get; set; }
        public decimal?  OtherDiscount { get; set; }
        public decimal? NetPrice { get; set; }
        public string OrderStatus { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int TotalRecordCount { get; set; }
        public string Address { get; set; } //Added by Reena
        public string DrugName { get; set; } //Added by Reena
        public decimal? UnitPrice { get; set; } //Added by Reena
        public int Quantity { get; set; } //Added by Reena
    }

    public class PatientGraphDetails
    {
        public int Count { get; set; }
        public string Label { get; set; }
    }

    public class ConfirmPrescription
    {
        public string RXNumber { get; set; }
        public string DrugName { get; set; }
        public string DrugStrengthName { get; set; }
        public int Quantity { get; set; }
        public string InsuranceProvider { get; set; }
    }

    public class PatientPrescription
    {
        public int UserId { get; set; }
        public int? PatientId { get; set; }
        public int UserTypeID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public string Gender { get; set; }

        public string PhoneNumber { get; set; }
        public string DateOfBirth { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }
        public int? AddressId { get; set; }
        public string City { get; set; }

        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public int? CityStateZipCodeID { get; set; }

        public string BinNumber { get; set; }
        public string MemberNumber { get; set; }
        public string GroupNumber { get; set; }
        public string PcnNumber { get; set; }

        public bool IsPrimary { get; set; }
        public string RelationShipWithCardHolder { get; set; }
        public string PrimaryCardHolderName { get; set; }
        public string CardHolderDateOfBirth { get; set; }

        public int? OrgId { get; set; }
        public int? DoctorId { get; set; }
        public string InsuranceProviderID { get; set; }
        public int? InsuranceTypeId { get; set; }
        public int? InsurancePlanId { get; set; }
        public string HealthInsurance { get; set; }
        public string InsurancePlan { get; set; }
        public string InsuranceProvider { get; set; }
        public string InsuranceType { get; set; }
        public DeliveryType IsDeliveryType { get; set; }
        public int DrugStrengthId { get; set; }
        public string DrugStrengthName { get; set; }

        public PrescriptionType PrescriptionType { get; set; }

        public List<NewPrescription> lstNewPrescriptionInfo { get; set; }
        public List<ExistingPrescription> lstExistingPrescriptionInfo { get; set; }
        public List<TransferPrescription> lstTransferPrescription { get; set; }

        public PatientRefill RefillData { get; set; }//TO DO CHECK THIS PROPERTY REQUIRES OR NOT
        public string RefillDate { get; set; }

        public List<ConfirmPrescription> confirmPrescription { get; set; }
        public List<string> lstPrescriptionNumber { get; set; }

        public PharmacyAddress Pharmacy { get; set; }
        public PharmacyAddress TransferPharmacy { get; set; }
    }
    
    public class DrugStrengthByDrugId
    {
        public int DrugStrengthId { get; set; }
        public string Name { get; set; }
    }

    public class ExistingPrescription
    {
        public Int64 PatientPrescriptionID { get; set; }
        public Int64 PatientID { get; set; }
        public int PatientRefillId { get; set; }
        public string RXNumber { get; set; }
        public int DrugId { get; set; }
        public int DoctorId { get; set; }
        public Int64 DrugStrengthID { get; set; }
        public string DrugStrengthName { get; set; }
        public int PharamcyID { get; set; }
        public int InsuranceID { get; set; }
        public int Quantity { get; set; }
        public int AddressId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreateBy { get; set; }
        public int InsurancePlanID { get; set; }
        public string InsurancePlan { get; set; }
        public string InsuranceType { get; set; }
        public string InsuranceProvider { get; set; }
        public string DrugName { get; set; }
        public DateTime? RefillDate { get; set; }       
        public DeliveryType IsDeliveryType { get; set; }
    }

    public class NewPrescription
    {
        public string RXNumber { get; set; }
        public int DrugId { get; set; }
        public Int64 DoctorId { get; set; }
        public string DoctorName { get; set; }
        public int DrugStrengthId { get; set; }
        public string DrugStrengthName { get; set; }
        public int PatientID { get; set; }
        public int PharamcyID { get; set; }
        public int InsuranceID { get; set; }
        public int InsuranceTypeId { get; set; }
        public int InsurancePlanId { get; set; }
        public int InsuranceProviderId { get; set; }
        public int Quantity { get; set; }
        public Int64 AddressId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreateBy { get; set; }
        public string InsurancePlan { get; set; }
        public string InsuranceType { get; set; }
        public string InsuranceProvider { get; set; }
        public string DrugName { get; set; }
        public string RefillDate { get; set; }
        public DeliveryType IsDeliveryType { get; set; }

    }

    public class TransferPrescription
    {
        public Int64 PatientPrescriptionID { get; set; }
        public Int64 PatientID { get; set; }
        public int PatientRefillId { get; set; }
        public int  TransFromPharmacyId { get; set; }
        public string TransFromPharmacyName { get; set; }
        public int TransToPharmacyId { get; set; }
        public string TransToPharmacyName { get; set; }
        public string TransFromPharmacyPhone { get; set; }        
        public string TransferPrescriptionNumber { get; set; }
        public int DoctorId { get; set; }
        public string RXNumber { get; set; }
        public int DrugId { get; set; }
        public string DrugName { get; set; }       
        public Int64 DrugStrengthID { get; set; }
        public string DrugStrengthName { get; set; }
        public int PharamcyID { get; set; }
        public int InsuranceID { get; set; }
        public int Quantity { get; set; }
        public int AddressId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreateBy { get; set; }
        public int InsurancePlanID { get; set; }
        public string InsurancePlan { get; set; }
        public string InsuranceType { get; set; }
        public string InsuranceProvider { get; set; }
        public string RefillDate { get; set; }
        public DeliveryType IsDeliveryType { get; set; }
    }
 
    public class PatientRefill
    {
        public int? PatientID { get; set; }
        public int? ReferenceId { get; set; }
        public int? UserType { get; set; }
        public DeliveryType IsDeliveryType { get; set; }
        public int? AddressId { get; set; }
        public int? RXNumber { get; set; }
        public DateTime? RefillDate { get; set; }
        public int? IsDeleted { get; set; }
        public int? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? ModifiedBy { get; set; }
    }

    public class PharmacyAddress
    {
        public int OrgId { get; set; }
        public string OrgName { get; set; }
        public string NPI { get; set; }
        public int? AddressId { get; set; }
        public int? AddressTypeID { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Zipcode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public int OrgUserTypeId { get; set; }
        public string PhoneNumber { get; set; }
        public string FullAddress { get { return Address1 + Address2 + City + State + Zipcode; } }
    }

    public class PharmacyDetailsById
    {
        public int OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public int OrganizationTypeID { get; set; }
        public int? UserTypeID { get; set; }
        public string PhoneNumber { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public int? CityStateZipCodeID { get; set; }
        public int? AddressId { get; set; }
        public int? AddressTypeID { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string CountryName { get; set; }
        public string ZipCode { get; set; }
    }

    public class PrescriptionEmail
    {
        public string FirstName { get; set; }
        public string Subject { get; set; }
        public string PatientEmail { get; set; }
        public string PharmacyName { get; set; }
        public string TransferFromPharmacyName { get; set; }
        public string RxNumber { get; set; }
        public string Address { get; set; }
        public string DeliveryDate { get; set; }
        public PrescriptionType PrescriptionType { get; set; }

    }
}
