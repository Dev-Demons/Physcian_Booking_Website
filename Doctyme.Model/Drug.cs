namespace Doctyme.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Drug")]
    public partial class Drug
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Drug()
        {
            DrugDetails = new HashSet<DrugDetail>();
        }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DrugId { get; set; }
        public int DrugType_LookUpID { get; set; }
        public int? DrugStatus_LookUpID { get; set; }
        [Required]
        [StringLength(200)]
        public string DrugName { get; set; }
        public int? GenericID { get; set; }
        public bool? IsGeneric { get; set; }
        [StringLength(300)]
        public string ShortDescription { get; set; }
        public string Description { get; set; }
       
        public DateTime CreatedDate { get; set; }
        
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DrugDetail> DrugDetails { get; set; }
        public virtual ICollection<DrugTabs> DrugTabs { get; set; }
    }
}
