using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Binke.Model
{
    [Table("State")]
    public class State : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StateId { get; set; }

        [Column(TypeName = "varchar"), MaxLength(5)]
        public string StateCode { get; set; }

        [Column(TypeName = "varchar"), MaxLength(50)]
        public string StateName { get; set; }

        public virtual ICollection<Address> Address { get; set; }
        public virtual ICollection<City> Cities { get; set; }
        public virtual ICollection<Experience> Experiences { get; set; }

    }
}
