using Binke.Model.DBContext;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Binke.Model
{
    [Table("Facility")]
    public class Facility : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FacilityId { get; set; }

        [Column(TypeName = "varchar"), MaxLength(50)]
        public string FacilityName { get; set; }

        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser FacilityUser { get; set; }

        public byte? FacilityTypeId { get; set; }
        [ForeignKey("FacilityTypeId")]
        public virtual FacilityType FacilityType { get; set; }

        public virtual ICollection<Address> Address { get; set; }
        public virtual ICollection<DoctorFacilityAffiliation> DoctorFacilityAffiliations { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }

    [Table("FacilityType")]
    public class FacilityType : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte FacilityTypeId { get; set; }

        [Column(TypeName = "varchar"), MaxLength(50)]
        public string FacilityTypeName { get; set; }

        public byte? FacilityOptionId { get; set; }
        [ForeignKey("FacilityOptionId")]
        public virtual FacilityOption FacilityOptions { get; set; }

        public virtual ICollection<Facility> Facilitys { get; set; }
    }
}
