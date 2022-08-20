using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Model
{
    [Table("DocOrgTaxonomy")]
    public class DocOrgTaxonomy: BaseModel
    {
        public int DocOrgTaxonomyID { get; set; }
        public int ReferenceID { get; set; }
        public int TaxonomyID { get; set; }
        public int? UserTypeId { get; set; }
        [ForeignKey("TaxonomyID")]
        public virtual Taxonomy Taxonomy { get; set; }
        [ForeignKey("UserTypeId")]
        public virtual UserType UserType { get; set; }
        //[ForeignKey("UserTypeId")]
        //public virtual UserType UserType { get; set; }

    }
}
