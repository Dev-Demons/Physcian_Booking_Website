namespace Doctyme.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PatientHealthCondition")]
    public partial class PatientHealthCondition
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatientHealthConditionID { get; set; }
        public int? PatientID { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? IsAllergy { get; set; }
        public bool? IsHealthCondition { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        [Column(TypeName = "date")]
        public DateTime? CreateDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? UpdatedDate { get; set; }
        public int? CreateBy { get; set; }
        public int? ModifiedBy { get; set; }
    }
}