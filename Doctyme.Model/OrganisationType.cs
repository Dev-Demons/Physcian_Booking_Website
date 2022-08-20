using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Model
{
    [Table("OrganisationType")]
    public class OrganisationType : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrganizationTypeID { get; set; }
        public string Org_Type_Name { get; set; }
        public string Org_Type_Description { get; set; }
        [StringLength(int.MaxValue)]
        public string Org_Type_Taxonomy { get; set; }
    }
}
