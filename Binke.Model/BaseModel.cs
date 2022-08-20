using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Binke.Model
{
    public class BaseModel
    {
        [Column(TypeName = "datetime")]
        public DateTime CreatedDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? UpdatedDate { get; set; }

        [Column(TypeName = "bit")]
        public bool IsActive { get; set; }

        [Column(TypeName = "bit")]
        public bool IsDeleted { get; set; }

        [Column(TypeName = "int")]
        public int CreatedBy { get; set; }

        [Column(TypeName = "int")]
        public int? ModifiedBy { get; set; }
    }
}
