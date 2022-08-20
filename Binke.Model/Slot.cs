namespace Binke.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Slot")]
    public partial class Slot
    {
        public int SlotId { get; set; }

        public DateTime SlotDate { get; set; }

        public DateTime SlotTime { get; set; }

        public int? DoctorId { get; set; }

        public int? SeniorCareId { get; set; }

        public int? FacilityId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public bool IsBooked { get; set; }

        public int? PatientUserId { get; set; }

    }
}
