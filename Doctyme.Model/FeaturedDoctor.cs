using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace Doctyme.Model
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

    public class FeaturedDoctorModel : BaseModel {
        public int FeaturedDoctorId { get; set; }

        public int DoctorId { get; set; }

        public string DoctorName { get; set; }

        public HttpPostedFile ProfilePic { get; set; }

        public string ProfilePicture { get; set; }

        public string SpecialityName { get; set; }

        public int Reviews { get; set; }

        public SocialMediaModel SocialMedia { get; set; }
    }
}
