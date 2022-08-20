using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Model
{
    [Table("UserType")]
    public class UserType: BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserTypeId { get; set; }
        public string UserTypeName { get; set; }

        [Column(TypeName = "varchar"), MaxLength(100)]
        public string Description { get; set; }
        //public ICollection<DocOrgTaxonomy> DocOrgTaxonomy { get; set; }
        //public ICollection<DocOrgInsurances> DocOrgInsurance { get; set; }
       // public ICollection<DocOrgStateLicense> DocOrgStateLicense { get; set; }
        //public ICollection<Address> Address { get; set; }
    }
}
