namespace Doctyme.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DoctorSpeciality")]
    public partial class DoctorSpeciality
    {
        public int DoctorSpecialityId { get; set; }

        public short SpecialityId { get; set; }

        public int DoctorId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public bool? IsPrimary { get; set; }
        public bool? IsSecondary { get; set; }

        public virtual Doctor Doctor { get; set; }

        public virtual Speciality Speciality { get; set; }
    }
}
