using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Model
{
    [Table("DrugStrength")]
    public class DrugStrength
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DrugStrengthID { get; set; }
        public int? DrugID { get; set; }
        public int? DrugStrengthLookUpID { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public DateTime? CreatedDate { get; set; }
    
        public DateTime? UpdateDate { get; set; }
        public int? CreateBy { get; set; }
        public int? ModifiedBy { get; set; }

        //[ForeignKey("DrugStrengthLookUpID")]
        //public virtual DrugStrengthLookUp DrugStrengthLookUp { get; set; }
    }
}