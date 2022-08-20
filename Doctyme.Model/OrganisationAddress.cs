namespace Doctyme.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Address")]
    public partial class OrganisationAddress
    {
        public int AddressId { get; set; }

        public int ReferenceId { get; set; }
        public int AddressTypeID { get; set; }


        [StringLength(100)]
        public string Address1 { get; set; }

        [StringLength(100)]
        public string Address2 { get; set; }


        [StringLength(10)]
        public string ZipCode { get; set; }

        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }
        public bool IsViewMode { get; set; }
        public int CreatedBy { get; set; }

        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }

        public int? ModifiedBy { get; set; }
        public bool IsActive { get; set; }


        public decimal? Lat { get; set; }

        public decimal? Lon { get; set; }


        public int CityStateZipCodeID { get; set; }

        public int? UserTypeID { get; set; }


        //[ForeignKey("ReferenceId")]
      //  public virtual Organisation Organisation { get; set; }


    }

    public class OrganisationAddressModel
    {
        [Required(ErrorMessage = "Enter correct Organization name!")]
        [Range(1, int.MaxValue, ErrorMessage = "Enter correct Organization name!")]
        public int OrganisationId { get; set; }
        public int? OrganizationTypeID { get; set; }
        public int? UserTypeID { get; set; }
        public string OrganisationName { get; set; }
        public string OrganizationType { get; set; }
        public int AddressId { get; set; }
        [Required(ErrorMessage = "Select Address Type!")]
        public int AddressTypeId { get; set; }
        [StringLength(100)]
        public string Address1 { get; set; }
        [StringLength(100)]
        public string Address2 { get; set; }
        [Required(ErrorMessage = "Select zip code, city, state and country!")]
        [Range(1, int.MaxValue, ErrorMessage = "Enter correct zip code and city!")]
        public int CityStateZipCodeID { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lon { get; set; }
        [StringLength(20)]
        public string Phone { get; set; }
        [StringLength(20)]
        public string Fax { get; set; }
        [StringLength(200)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [StringLength(200)]
        public string WebSite { get; set; }
        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public string ReferenceName { get; set; }
        public int ReferenceId { get; set; }

        public int TotalRecordCount { get; set; }
    }

    public class DoctorAddressListModel : BaseModel
    {

        public int? UserTypeID { get; set; }
        public string DoctorName { get; set; }
        public int DoctorId { get; set; }
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
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public int ReferenceId { get; set; }
        public int CityStateZipCodeID { get; set; }
        public string IsActiveString { get; set; }

        public int TotalRecordCount { get; set; }
    }

    public class DoctorAddressModel
    {

        public int DoctorId { get; set; }

        public int? UserTypeID { get; set; }
        public string OrganisationName { get; set; }
        public string OrganizationType { get; set; }
        public int AddressId { get; set; }
        [Required(ErrorMessage = "Select Address Type!")]
        public int AddressTypeId { get; set; }
        [StringLength(100)]
        public string Address1 { get; set; }
        [StringLength(100)]
        public string Address2 { get; set; }
        [Required(ErrorMessage = "Select zip code, city, state and country!")]
        [Range(1, int.MaxValue, ErrorMessage = "Enter correct zip code and city!")]
        public int CityStateZipCodeID { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lon { get; set; }
        [StringLength(20)]
        public string Phone { get; set; }
        [StringLength(20)]
        public string Fax { get; set; }
        [StringLength(200)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [StringLength(200)]
        public string WebSite { get; set; }
        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public string ReferenceName { get; set; }
        public int ReferenceId { get; set; }

        public int TotalRecordCount { get; set; }
    }
}
