using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Binke.Model
{
    [Table("Address")]
    public class Address : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AddressId { get; set; }

        [Column(TypeName = "varchar"), MaxLength(100)]
        public string Address1 { get; set; }

        [Column(TypeName = "varchar"), MaxLength(100)]
        public string Address2 { get; set; }

        public int CityId { get; set; }
        [ForeignKey("CityId")]
        public virtual City City { get; set; }

        public int StateId { get; set; }
        [ForeignKey("StateId")]
        public virtual State State { get; set; }

        [Column(TypeName = "varchar"), MaxLength(20)]
        public string Country { get; set; }

        [Column(TypeName = "varchar"), MaxLength(10)]
        public string ZipCode { get; set; }

        [NotMapped]
        public string FullAddress => $@"{Address1},{City.CityName},{State.StateName}-{ZipCode}";

        [Column(TypeName = "bit")]
        public bool IsDefault { get; set; }

        public int? DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }

        public int? FacilityId { get; set; }
        [ForeignKey("FacilityId")]
        public virtual Facility Facility { get; set; }

        public int? PatientId { get; set; }
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }

        public int? PharmacyId { get; set; }
        [ForeignKey("PharmacyId")]
        public virtual Pharmacy Pharmacy { get; set; }

        public int? SeniorCareId { get; set; }
        [ForeignKey("SeniorCareId")]
        public virtual SeniorCare SeniorCare { get; set; }

    }
}
