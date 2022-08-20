namespace Doctyme.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Coupon")]
    public partial class Coupon
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DrugCouponID { get; set; }
        public int? OrganizationID { get; set; }
        [StringLength(50)]
        public string CouponCode { get; set; }
        public int? CouponDiscountType { get; set; }
        [Column(TypeName = "money")]
        public decimal? CouponDiscountAmount { get; set; }
        [Column(TypeName = "date")]
        public DateTime? CouponStartDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? CouponEndDate { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        [Column(TypeName = "date")]
        public DateTime? CreatedDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? UpdateDate { get; set; }
        public int? CreateBy { get; set; }
        public int? ModifiedBy { get; set; }
    }
}