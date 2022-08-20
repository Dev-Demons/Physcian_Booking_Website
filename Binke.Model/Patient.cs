using Binke.Model.DBContext;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Binke.Model
{
    [Table("Patient")]
    public class Patient : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatientId { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser PatientUser { get; set; }

        [Column(TypeName = "varchar"), MaxLength(100)]
        public string PrimaryInsurance { get; set; }

        [Column(TypeName = "varchar"), MaxLength(100)]
        public string SecondaryInsurance { get; set; }

        public virtual ICollection<Address> Address { get; set; }
    }
}
