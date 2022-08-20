using Binke.Model.DBContext;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Binke.Model
{
    [Table("Pharmacy")]
    public class Pharmacy : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PharmacyId { get; set; }

        [Column(TypeName = "varchar"), MaxLength(50)]
        public string PharmacyName { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser PharmacyUser { get; set; }

        public virtual ICollection<Address> Address { get; set; }
        public virtual ICollection<Drug> Drugs { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }

}
