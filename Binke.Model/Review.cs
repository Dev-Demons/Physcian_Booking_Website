using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Binke.Model.DBContext;

namespace Binke.Model
{
    [Table("Review")]
    public class Review : BaseModel
    {
        public Review()
        {
            IsActive = true;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ReviewId { get; set; }

        [Column(TypeName = "varchar"), MaxLength(50)]
        public string Title { get; set; }

        public string ReviewText { get; set; }

        public int Rating { get; set; }

        public int? DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }

        public int? FacilityId { get; set; }
        [ForeignKey("FacilityId")]
        public virtual Facility Facility { get; set; }

        public int? PharmacyId { get; set; }
        [ForeignKey("PharmacyId")]
        public virtual Pharmacy Pharmacy { get; set; }

        public int? SeniorCareId { get; set; }
        [ForeignKey("SeniorCareId")]
        public virtual SeniorCare SeniorCare { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual ApplicationUser ReviewBy { get; set; }
    }
}
