namespace Doctyme.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Featured")]
    public partial class Featured
    {
        public int FeaturedId { get; set; }

        public int ReferenceId { get; set; }
        public int UserTypeID { get; set; }

        [Column(TypeName = "varchar"), MaxLength(200)]
        public string ProfileImage { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        public int? TotalImpressions { get; set; }
        public int? PaymentTypeID { get; set; }
        public int? AdvertisementLocationID { get; set; }

        [StringLength (200)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string GeoLocation { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? EndDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public virtual Doctor Doctor { get; set; }
    }
}
