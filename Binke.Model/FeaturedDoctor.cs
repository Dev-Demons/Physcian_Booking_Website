using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Binke.Model
{
    [Table("FeaturedDoctor")]
    public class FeaturedDoctor : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FeaturedDoctorId { get; set; }

        public int DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }

        [Column(TypeName = "varchar"), MaxLength(50)]
        public string ProfilePicture { get; set; }
    }
}
