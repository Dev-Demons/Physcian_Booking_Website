using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Model
{
    [Table("DrugTabs")]
    public class DrugTabs
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DrugTabsID { get; set; }
        public int? DrugID { get; set; }
        public int? DrugTabs_LookUpID { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
       
        public DateTime? CreatedDate { get; set; }
        public int? CreateBy { get; set; }
      
        public DateTime? UpdatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public string Description { get; set; }
    }
}