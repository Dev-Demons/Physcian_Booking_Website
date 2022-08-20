using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Binke.Model
{
    [Table("OpeningHour")]
    public class OpeningHour : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OpeningHourId { get; set; }

        public int WeekDay { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int? DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }

        public int? SeniorCareId { get; set; }
        [ForeignKey("SeniorCareId")]
        public virtual SeniorCare SeniorCare { get; set; }
    }
}
