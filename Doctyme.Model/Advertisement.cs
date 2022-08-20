namespace Doctyme.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Advertisement")]
    public partial class Advertisement
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AdvertisementId { get; set; }

        public int? ReferenceId { get; set; }

       
        [Column(Order = 1, TypeName = "date")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "varchar"), MaxLength(100)]
        public string Title { get; set; }

        [Column(TypeName = "varchar"), MaxLength(200)]
        public string ImagePath { get; set; }

        [Column(TypeName = "date")]
        public DateTime? EndDate { get; set; }

        public int? AdvertisementTypeId { get; set; }

        public int? TotalImpressions { get; set; }

        public int? PaymentTypeId { get; set; }

        public int? CityStateZipCodeId { get; set; }

        public int? UserTypeId { get; set; }

        [Column(Order = 2)]
        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        [Column(Order = 4)]
        public bool? IsActive { get; set; }

        [Column(Order = 5)]
        public bool? IsDeleted { get; set; }

        public virtual AdvertisementType AdvertisementType { get; set; }

        public virtual PaymentType PaymentType { get; set; }
    }
}
