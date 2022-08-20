using Binke.Model.DBContext;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Binke.Model
{
    [Table("TempSlotBooking ")]
    public class TempSlotBooking : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SlotBookingId { get; set; }

        public int LocationId { get; set; }

        public int SlotId { get; set; }
        [ForeignKey("SlotId")]
        public virtual Slot Slot{ get; set; }

        public int PatientId { get; set; }
        [ForeignKey("PatientId")]
        public virtual ApplicationUser PatientUser { get; set; }
    }
}
