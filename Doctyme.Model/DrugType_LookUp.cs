namespace Doctyme.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DrugType_LookUp")]
    public partial class DrugType_LookUp
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int DrugType_LookUpID { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(100)]
        public string Description { get; set; }
     
        public DateTime? CreatedDate { get; set; }
  
        public DateTime? UpdateDate { get; set; }
        public int? CreateBy { get; set; }
        public int? ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}