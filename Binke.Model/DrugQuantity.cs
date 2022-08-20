using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binke.Model
{
    [Table("DrugQuantity")]
    public class DrugQuantity: BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DrugQuantityId { get; set; }
        public string Quantity { get; set; }
    }
}
