using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Binke.Model
{
    [Table("SlotBooking ")]
    public class SlotBooking : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SlotBookingId { get; set; }

        public int SlotId { get; set; }
        [ForeignKey("SlotId")]
        public virtual Slot Slot{ get; set; }

        public int PatientId { get; set; }
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }
    }
}
