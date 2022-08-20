using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Binke.Model
{
    [Table("Drug")]
    public class Drug : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DrugId { get; set; }

        public string DrugName { get; set; }

        public string Description { get; set; }

        public float UnitoryPrice { get; set; }

        public float SellingPrice { get; set; }

        public string ManufactureName { get; set; }

        public DateTime ExpiryDate { get; set; }

        public int PharmacyId { get; set; }
        [ForeignKey("PharmacyId")]
        public virtual Pharmacy Pharmacy { get; set; }
    }
}
