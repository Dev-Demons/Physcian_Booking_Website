namespace Doctyme.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Review")]
    public partial class Review : BaseModel
    {
        public long ReviewId { get; set; }

        public int? ReferenceId { get; set; }
        public int UserTypeID { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        public int Rating { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }
    }
}
