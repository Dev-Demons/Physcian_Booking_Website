using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Binke.Model
{
    [Table("SocialMedia")]
    public class SocialMedia
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SocialMediaId { get; set; }

        [Column(TypeName = "varchar"), MaxLength(215)]
        public string Facebook { get; set; }

        [Column(TypeName = "varchar"), MaxLength(215)]
        public string Twitter { get; set; }

        [Column(TypeName = "varchar"), MaxLength(215)]
        public string LinkedIn { get; set; }

        [Column(TypeName = "varchar"), MaxLength(215)]
        public string Instagram { get; set; }

        public int? DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctors { get; set; }

        public int? SeniorCareId { get; set; }
        [ForeignKey("SeniorCareId")]
        public virtual SeniorCare SeniorCare { get; set; }
    }
}
