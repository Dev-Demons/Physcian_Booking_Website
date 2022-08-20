using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Binke.Model
{
    [Table("FeaturedSpeciality")]
    public class FeaturedSpeciality : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FeaturedSpecialityId { get; set; }

        public short SpecialityId { get; set; }
        [ForeignKey("SpecialityId")]
        public virtual Speciality Speciality { get; set; }

        [Column(TypeName = "varchar"), MaxLength(150)]
        public string Description { get; set; }

        [Column(TypeName = "varchar"), MaxLength(50)]
        public string ProfilePicture { get; set; }
    }
}
