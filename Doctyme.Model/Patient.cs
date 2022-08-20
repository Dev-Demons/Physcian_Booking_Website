using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doctyme.Model
{
    [Table("Patient")]
    public class Patient : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatientId { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser PatientUser { get; set; }

        public int? InsurancePlanID { get; set; }
        [ForeignKey("InsurancePlanID")]
        public InsurancePlan InsurancePlan { get; set; }

        public bool? IsPrimary { get; set; }
        public long? ReferenceId { get; set; }
        public bool? IsTeleMedCovered { get; set; }
        [NotMapped]
        public virtual ICollection<Address> Address { get; set; }
    }
}
