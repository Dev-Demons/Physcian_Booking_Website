using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Model
{
    [Table("DrugManufacturer_LookUp")]
    public class DrugManufacturer_LookUp : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DrugManufacturer_LookUpId { get; set; }
        public string CompanyName { get; set; }
        public int? CityStateZipCodeId { get; set; }
      
        public DateTime? CreateDate { get; set; }
       
        public DateTime? UpdateDate { get; set; }
 
    }
}