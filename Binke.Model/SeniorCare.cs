using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Binke.Model.DBContext;

namespace Binke.Model
{
    [Table("SeniorCare")]
    public class SeniorCare : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SeniorCareId { get; set; }

        [Column(TypeName = "varchar"), MaxLength(50)]
        public string SeniorCareName { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }

        public string Amenities { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser SeniorCareUser { get; set; }

        public virtual ICollection<Address> Address { get; set; }
        public virtual ICollection<OpeningHour> OpeningHours { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<SeniorCareImage> SeniorCareImages { get; set; }
        public virtual ICollection<SocialMedia> SocialMediaLinks { get; set; }
    }

    [Table("SeniorCareImage")]
    public class SeniorCareImage : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SeniorCareImageId { get; set; }

        public int SeniorCareId { get; set; }
        [ForeignKey("SeniorCareId")]
        public virtual SeniorCare SeniorCare { get; set; }

        [Column(TypeName = "varchar"), MaxLength(100)]
        public string ImagePath { get; set; }

    }
}
