namespace Doctyme.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Attachment")]
    public partial class Attachment
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AttachmentId { get; set; }

     
        [Column(Order = 1)]
        [StringLength(200)]
        public string FilePath { get; set; }

       
        [Column(Order = 2)]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

       
        [Column(Order = 3)]
        public DateTime CreatedDate { get; set; }

        
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }

       
        [Column(Order = 5)]
        public bool IsActive { get; set; }

      
        [Column(Order = 6)]
        public bool IsDeleted { get; set; }
    }
}
