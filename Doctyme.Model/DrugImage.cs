namespace Doctyme.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DrugImage")]
    public partial class DrugImage
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DrugImageID { get; set; }
        public int? DrugID { get; set; }
        public int? DrugStrength_LookUpID { get; set; }
        [StringLength(100)]
        public string ImageFile { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
       
        public DateTime? CreatedDate { get; set; }
        public int? CreateBy { get; set; }
       
        public DateTime? UpdatedDate { get; set; }
        public int? ModifiedBy { get; set; }
    }
}