namespace Doctyme.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AddressType")]
    public partial class AddressType
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AddressTypeId { get; set; }

      
        [Column(Order = 1)]
        [StringLength(50)]
        public string Name { get; set; }
    
        [Column(Order = 2)]
        [StringLength(100)]
        public string  Description { get; set; }
       
        [Column(Order = 3)]
        public DateTime CreatedDate { get; set; }



        public DateTime? UpdatedDate { get; set; }


        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

      
       
        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }
    }
}
