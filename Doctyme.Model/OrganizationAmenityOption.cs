using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Model
{
    [Table("OrganizationAmenityOption")]
    public class OrganizationAmenityOption: BaseModel
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrganizationAmenityOptionID { get; set; }
        public int OrganizationID { get; set; }

        [Column(TypeName = "varchar"), MaxLength(100)]
        public string Name { get; set; }

        [Column(TypeName = "varchar"), MaxLength(200)]
        public string Description { get; set; }

        [Column(TypeName = "varchar"), MaxLength(200)]
        public string ImagePath { get; set; }

        [Column(TypeName = "bit")]
        public bool IsOption { get; set; }

        [ForeignKey("OrganizationID")]
        public virtual Organisation Organisation { get; set; }
        
        
    }
}
