using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binke.Model
{
    [Table("DrugPharmacyDetail")]
    public class DrugPharmacyDetail: BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DrugPharmacyDetailId { get; set; }

        [ForeignKey("Pharmacy")]
        public int PharmacyId { get; set; }

        [ForeignKey("DrugDetail")]
        public int DrugDetailId { get; set; }

        [ForeignKey("DrugType")]
        public int DrugTypeId { get; set; }

        [ForeignKey("Tablet")]
        public int TabletId { get; set; }

        [ForeignKey("DrugQuantity")]
        public int DrugQuantityId { get; set; }

        [ForeignKey("DrugManufacturer")]
        public int DrugManufacturerId { get; set; }
        public int Price { get; set; }

        public virtual Pharmacy Pharmacy { get; set; }
        public virtual DrugDetail DrugDetail { get; set; }
        public virtual DrugType DrugType { get; set; }
        public virtual Tablet Tablet { get; set; }
        public virtual DrugQuantity DrugQuantity { get; set; }
        public virtual DrugManufacturer DrugManufacturer { get; set; }
    }
}
