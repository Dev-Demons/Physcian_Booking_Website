using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binke.Model
{
    [Table("DrugDetail")]
    public class DrugDetail : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DrugDetailId { get; set; }
        public string MedicineName { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string SideEffects { get; set; }
        public string Dosage { get; set; }
        public string Professional { get; set; }
        public string Tips { get; set; }
        public string Interaction { get; set; }
    }
}
