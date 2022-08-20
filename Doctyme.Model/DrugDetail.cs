namespace Doctyme.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DrugDetail")]
    public partial class DrugDetail
    {
        public int DrugDetailId { get; set; }

        public int DrugId { get; set; }

        public int DrugTypeId { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public virtual Drug Drug { get; set; }

        public virtual DrugType DrugType { get; set; }
    }
}
