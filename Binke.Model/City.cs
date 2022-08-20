using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Binke.Model
{
    [Table("City")]
    public class City : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CityId { get; set; }

        [Column(TypeName = "varchar"), MaxLength(50)]
        public string CityName { get; set; }

        public int? StateId { get; set; }
        [ForeignKey("StateId")]
        public virtual State States { get; set; }

        [Column(TypeName = "varchar"), MaxLength(50)]
        public string County { get; set; }

        [Column(TypeName = "nvarchar"), MaxLength(20)]
        public string Longitude { get; set; }

        [Column(TypeName = "nvarchar"), MaxLength(20)]
        public string Latitude { get; set; }

        public virtual ICollection<Address> Address { get; set; }
        public virtual ICollection<Experience> Experiences { get; set; }
    }
}
