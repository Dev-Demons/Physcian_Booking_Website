using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Binke.Model
{
    [Table("Speciality")]
    public class Speciality : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short SpecialityId { get; set; }

        [Column(TypeName = "varchar"), MaxLength(100)]
        public string SpecialityName { get; set; }

        public virtual ICollection<DoctorSpeciality> DoctorSpecialitys { get; set; }
        public virtual ICollection<BoardCertification> BoardCertifications { get; set; }
        public virtual ICollection<FeaturedSpeciality> FeaturedSpecialities { get; set; }
    }

    [Table("DoctorSpeciality")]
    public class DoctorSpeciality : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DoctorSpecialityId { get; set; }

        public short SpecialityId { get; set; }
        [ForeignKey("SpecialityId")]
        public virtual Speciality Speciality { get; set; }

        public int DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }
    }
}
