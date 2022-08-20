using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doctyme.Model
{
    [Table("Experience")]
    public class Experience : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExperienceId { get; set; }

        public int DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }

        [Column(TypeName = "varchar"), MaxLength(100)]
        public string Designation { get; set; }

        [Column(TypeName = "varchar"), MaxLength(100)]
        public string Organization { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Column(TypeName = "nvarchar")]
        public string Description { get; set; }

        //public int? CityId { get; set; }
        //[ForeignKey("CityId")]
        //public virtual City City { get; set; }

        //public int? StateId { get; set; }
        //[ForeignKey("StateId")]
        //public virtual State State { get; set; }
    }
}
