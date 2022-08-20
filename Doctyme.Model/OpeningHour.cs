using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Model
{
    [Table("OpeningHour")]
    public class OpeningHour
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OpeningHourID { get; set; }
        public int DoctorID { get; set; }
        public int OrganizationID { get; set; }
        public int? WeekDay { get; set; }
        [StringLength(10)]
        public string StartDateTime { get; set; }
        public DateTime? CalendarDate { get; set; }
        public int? SlotDuration { get; set; }

        public bool? IsHoliday { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
       [StringLength(10)]
        public string EndDateTime { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public int? ReferenceID { get; set; }
        [MaxLength(100)]
        public string Comments { get; set; }
    }

    public class DocOpeningHoursModel
    {
        public int OpeningHourID { get; set; }
        public int? DoctorID { get; set; }
        public string DoctorName { get; set; }
        public int? WeekDay { get; set; }
        public string WeekDayName { get; set; }
        public DateTime? CalendarDate { get; set; }
        public int? SlotDuration { get; set; }

        public string StartDateTime { get; set; }
        public string Comments { get; set; }
        public bool? IsHoliday { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public string EndDateTime { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int UserTypeID { get; set; }

        public int TotalRecordCount { get; set; }
    }

    public class DocOpeningHoursListModel
    {
        public int OpeningHourID { get; set; }
        public int? DoctorID { get; set; }
        public string DoctorName { get; set; }
        public int? WeekDay { get; set; }
        public string WeekDayName { get; set; }
        public DateTime? CalendarDate { get; set; }
        public int? SlotDuration { get; set; }

        public string StartDateTime { get; set; }
        public string Comments { get; set; }
        public bool? IsHoliday { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public string EndDateTime { get; set; }

        public string UpdatedDate { get; set; }

        public int UserTypeID { get; set; }

        public int TotalRecordCount { get; set; }
    }

    public class DoctorOpeningHoursUpdateModel :BaseModel
    {
        public int OpeningHourID { get; set; }
        public int? DoctorID { get; set; }

        public int? WeekDay { get; set; }

        public DateTime? CalendarDate { get; set; }

        public string StartDateTime { get; set; }

        public string EndDateTime { get; set; }

        public int UserTypeID { get; set; }

        public int[] DayNo { get; set; }

        public string[] StartTime { get; set; }

        public string[] EndTime { get; set; }

        public int[] SlotDuration { get; set; }

        public string[] Comments { get; set; }

        public bool[] IsHoliday { get; set; }

        public bool[] IsActive { get; set; }

        public bool[] IsDeleted { get; set; }

    }
}
