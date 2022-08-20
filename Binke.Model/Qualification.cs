using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Binke.Model
{
    [Table("Qualification")]
    public class Qualification : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QualificationId { get; set; }

        [Column(TypeName = "varchar"), MaxLength(200)]
        public string Institute { get; set; }

        [Column(TypeName = "varchar"), MaxLength(100)]
        public string Degree { get; set; }

        public short PassingYear { get; set; }

        [Column(TypeName = "varchar"), MaxLength(200)]
        public string Notes { get; set; }

        public int DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }
    }
}
