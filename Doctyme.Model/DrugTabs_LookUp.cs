using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Model
{
    [Table("DrugTabs_LookUp")]
    public class DrugTabs_LookUp
    {

        public int DrugTabs_LookUpID { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
       
        public DateTime? CreatedDate { get; set; }
        public int? CreateBy { get; set; }
       
        public DateTime? UpdatedDate { get; set; }
        public int? ModifiedBy { get; set; }
    }
}