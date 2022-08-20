namespace Doctyme.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DoctorAffiliation")]
    public partial class DoctorAffiliation
    {
        public int DoctorAffiliationId { get; set; }

        public int DoctorId { get; set; }

        public int OrganisationId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }
        //public int FacilityId { get; set; }
        public virtual Doctor Doctor { get; set; }

        public virtual Organisation Organisation { get; set; }

    }
}
