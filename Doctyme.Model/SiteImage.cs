namespace Doctyme.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SiteImage")]
    public partial class SiteImage
    {
        public int SiteImageId { get; set; }

        public int? ReferenceId { get; set; }
        public int? UserTypeID { get; set; }

        [Required]
        [StringLength(200)]
        public string ImagePath { get; set; }

        public bool IsProfile { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }
    }
}
