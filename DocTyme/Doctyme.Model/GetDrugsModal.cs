using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Doctyme.Model
{
  public  class GetDrugsModal
    {
        [Key]
        public Int64 Id { get; set; }
        public int DrugId { get; set; }
        public string Drugname { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string Dosage { get; set; }
        public string Company { get; set; }

    }
}
