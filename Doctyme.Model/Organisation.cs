namespace Doctyme.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Organisation")]
    public partial class Organisation: BaseModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Organisation()
        {
            DoctorAffiliations = new HashSet<DoctorAffiliation>();
        }

        public int OrganisationId { get; set; }

        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser OrganisationUser { get; set; }


        public int OrganizationTypeID { get; set; }
        [ForeignKey("OrganizationTypeID")]
        public OrganisationType OrganisationType { get; set; }

        
        [StringLength(200)]
        public string OrganisationName { get; set; }

        [MinLength(10, ErrorMessage = "NPI should be 10 digit")]
        [System.Web.Mvc.Remote("ValidateNPI", "Pharmacy", AdditionalFields = "OrganisationId", ErrorMessage = "NPI already exists")]
        [StringLength(10)]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Accept only number")]
        public string NPI { get; set; }

        [StringLength(10)]
        public string OrganisationSubpart { get; set; }

        [StringLength(200)]
        public string AliasBusinessName { get; set; }

        [StringLength(200)]
        public string LogoFilePath { get; set; }

        [StringLength(15)]
        public string OrganizatonEIN { get; set; }
        [Column(TypeName = "date")]
        public DateTime? EnumerationDate { get; set; }

        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public string Status { get; set; }

        [StringLength(10)]
        public string AuthorisedOfficialCredential { get; set; }

        [StringLength(50)]
        public string AuthorizedOfficialFirstName { get; set; }

        [StringLength(50)]
        public string AuthorizedOfficialLastName { get; set; }

        [StringLength(10)]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Accept only number")]
        public string AuthorizedOfficialTelephoneNumber { get; set; }

        [StringLength(50)]
        public string AuthorizedOfficialTitleOrPosition { get; set; }

        [StringLength(10)]
        public string AuthorizedOfficialNamePrefix { get; set; }

        //public DateTime CreatedDate { get; set; }

        //public DateTime? UpdatedDate { get; set; }

        //public bool IsDeleted { get; set; }

        //public int CreatedBy { get; set; }

        //public int? ModifiedBy { get; set; }
        public bool? EnabledBooking { get; set; }

        public int? UserTypeID { get; set; }
        public int? ApplicationUser_Id { get; set; }

        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DoctorAffiliation> DoctorAffiliations { get; set; }
    }
}
