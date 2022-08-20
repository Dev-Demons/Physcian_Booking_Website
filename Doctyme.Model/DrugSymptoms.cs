using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Model
{
    [Table("DrugSymptoms")]
    public class DrugSymptoms
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DrugSymptomsID { get; set; }
        public int? DrugID { get; set; }
        public int? DrugSymptomsLookUpID { get; set; }
        public bool? IsMoreCommon { get; set; }
        public bool? IsLessCommon { get; set; }
        public bool? IsNotKnown { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
     
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
       
        public DateTime? UpdateDate { get; set; }
        public int? ModifiedBy { get; set; }

        //[ForeignKey("DrugSymptomsLookUpID")]
        //public virtual DrugSymptomsLookUp DrugSymptomsLookUp { get; set; }
    }
}