using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Binke.Model
{
    [Table("AgeGroup")]
    public class AgeGroup : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AgeGroupId { get; set; }

        public string Name { get; set; }

        public virtual ICollection<DoctorAgeGroup> Doctoragegroup { get; set; }
    }

    [Table("DoctorAgeGroup")]
    public class DoctorAgeGroup : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DoctorAgeGroupId { get; set; }

        public int AgeGroupId { get; set; }
        [ForeignKey("AgeGroupId")]
        public virtual AgeGroup agegroup { get; set; }

        public int DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }
    }
}
