namespace Doctyme.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web;
    using Newtonsoft.Json;
    using Utility;

    [Table("Doctor")]
    public partial class Doctor: BaseModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Doctor()
        {
            DoctorAgeGroups = new HashSet<DoctorAgeGroup>();
            DoctorFacilityAffiliation = new HashSet<DoctorFacilityAffiliation>();
            DoctorSpecialities = new HashSet<DoctorSpeciality>();
            Featureds = new HashSet<Featured>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DoctorId { get; set; }


        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser DoctorUser { get; set; }

        [StringLength(10)]
        public string NamePrefix { get; set; }

        [StringLength(10)]
        public string Credential { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(50)]
        public string MiddleName { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(10)]
        public string Gender { get; set; }

        
        [StringLength(10)]
        public string Status { get; set; }

        [Column(TypeName = "date")]
        public DateTime? EnumerationDate { get; set; }

        [StringLength(10)]
        public string NPI { get; set; }

        [StringLength(50)]
        public string Education { get; set; }

        [StringLength(300)]
        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public bool? SoleProprietor { get; set; }

        public bool IsAllowNewPatient { get; set; }

        public bool IsNtPcp { get; set; }

        [StringLength(1000)]
        public string Language { get; set; }

        public string OtherNames { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }
        public bool IsPrimaryCare { get; set; }
        public string Keywords { get; set; }
        //public string Npi { get; set; }
        public DateTime? PracticeStartDate { get; set; }
        public bool? EnabledBooking { get; set; }
      
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DoctorAgeGroup> DoctorAgeGroups { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DoctorFacilityAffiliation> DoctorFacilityAffiliation { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DoctorSpeciality> DoctorSpecialities { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Featured> Featureds { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<SocialMedia> SocialMediaLinks { get; set; }
    }

    public class DoctorImageModel : BaseModel
    {
        public int Id { get; set; }
        public int DoctorImageId { get; set; }

        [Required(ErrorMessage = "Doctor Name is required")]
        public int DoctorId { get; set; }

        [JsonConverter(typeof(Base64FileJsonConverter))]
        public byte[] Image { get; set; }

        public string ImageName { get; set; }
        public string ImagePath { get; set; }
    }
}
