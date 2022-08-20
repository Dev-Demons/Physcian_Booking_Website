namespace Doctyme.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AmenityOption")]
    public partial class AmenityOption: BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AmenityId { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }
        public bool IsOption { get; set; }

        public int? OrganisationId { get; set; }

        [ForeignKey("OrganisationId")]
        public virtual Organisation Organisation { get; set; }

        //public DateTime CreatedDate { get; set; }

        //public DateTime? UpdatedDate { get; set; }

        //public bool IsDeleted { get; set; }

        //public int CreatedBy { get; set; }

        //public int? ModifiedBy { get; set; }
    }
}
