using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Model
{
    [Table("DrugManufacturer")]
    public class DrugManufacturer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DrugManufacturerID { get; set; }
        public int? DrugID { get; set; }
        public int? DrugManufacturerLookUpID { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? updateDate { get; set; }
        public int? ModifiedBy { get; set; }
    }
}
