using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Model
{
    [Table("DrugSymptoms_LookUp")]
    public class DrugSymptoms_LookUp
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int DrugSymptoms_LookUpID { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
       
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
      
        public DateTime? UpdateDate { get; set; }
        public int? ModifiedBy { get; set; }
    }
}