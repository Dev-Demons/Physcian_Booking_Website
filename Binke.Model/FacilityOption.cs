using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Binke.Model
{
    [Table("FacilityOption")]
    public class FacilityOption : BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte FacilityOptionId { get; set; }

        [Column(TypeName = "varchar"), MaxLength(50)]
        public string FacilityOptionName { get; set; }

        public virtual ICollection<FacilityType> FacilityTypes { get; set; }

    }
}
