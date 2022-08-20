namespace Doctyme.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Address")]
    public partial class Address
    {
        public int AddressId { get; set; }
        
        public int ReferenceId { get; set; }
        public int AddressTypeID { get; set; }

        [NotMapped]
        public int? PatientId { get; set; }
        [StringLength(100)]
        public string Address1 { get; set; }

        [StringLength(100)]
        public string Address2 { get; set; }
        [NotMapped]
        public int CityId { get; set; }

        [NotMapped]
        public int StateId { get; set; }

        [NotMapped]
        public int CountryId { get; set; }

        [NotMapped]
        [StringLength(10)]
        public string ZipCode { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public int CreatedBy { get; set; }
        //public int CountryID { get; set; }

        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }

        public int? ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        [NotMapped]
        public string Country { get; set; }

        public decimal? Lat { get; set; }

        public decimal? Lon { get; set; }
        //public virtual City City { get; set; }

        //public virtual Country Country { get; set; }

        //public virtual State State { get; set; }

        public int? CityStateZipCodeID { get; set; }

        //[ForeignKey("CityStateZipCodeID")]
        //public virtual CityStateZip CityStateZip { get; set; }
        public int? UserTypeID { get; set; }
        [ForeignKey("ReferenceId")] 
        public virtual Organisation Organisation { get; set; }
        
       
    }
}
