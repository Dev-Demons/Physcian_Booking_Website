

namespace Doctyme.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ContactUs")]
    public partial class ContactUs
    {
         
        public int ContactUsID { get; set; }

        [Column(TypeName = "varchar"), MaxLength(100)]
        public string Name { get; set; }

        [Column(TypeName = "varchar"), MaxLength(100)]
        public string Subject { get; set; }

        [Column(TypeName = "varchar"), MaxLength(100)]
        public string Department { get; set; }

        [Column(TypeName = "varchar"), MaxLength(100)]
        public string Email { get; set; }

        [StringLength(1000)]
        public string Message { get; set; }

        [Column(TypeName = "date")]
        public DateTime DateSubmit { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }
    }
}
