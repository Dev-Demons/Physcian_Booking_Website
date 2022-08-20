using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Binke.Model
{
    [Table("BoardCertification")]
    public class BoardCertification : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short BoardCertificationId { get; set; }

        [Column(TypeName = "varchar"), MaxLength(100)]
        public string CertificationName { get; set; }

        public short SpecialityId { get; set; }
        [ForeignKey("SpecialityId")]
        public virtual Speciality Speciality { get; set; }

        public virtual ICollection<DoctorBoardCertification> DoctorBoardCertifications { get; set; }
    }

    [Table("DoctorBoardCertification")]
    public class DoctorBoardCertification : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DoctorBoardCertificationId { get; set; }

        public short BoardCertificationId { get; set; }
        [ForeignKey("BoardCertificationId")]
        public virtual BoardCertification BoardCertification { get; set; }

        public int DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }
    }
}
