using Binke.Model.DBContext;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Binke.Model
{
    [Table("Doctor")]
    public class Doctor : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DoctorId { get; set; }

        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser DoctorUser { get; set; }

        public string Npi { get; set; }

        public string Education { get; set; }

        [Column(TypeName = "varchar"), MaxLength(300)]
        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public bool IsAllowNewPatient { get; set; }

        public bool IsNtPcp { get; set; }

        public bool IsPrimaryCare { get; set; }

        public virtual ICollection<Address> Address { get; set; }
        public virtual ICollection<DoctorBoardCertification> DoctorBoardCertifications { get; set; }
        public virtual ICollection<DoctorFacilityAffiliation> DoctorFacilityAffiliations { get; set; }
        public virtual ICollection<DoctorImage> DoctorImages { get; set; }
        public virtual ICollection<DoctorLanguage> DoctorLanguages { get; set; }
        public virtual ICollection<DoctorSpeciality> DoctorSpecialitys { get; set; }
        public virtual ICollection<DoctorInsurance> DoctorInsurances { get; set; }
        public virtual ICollection<DoctorAgeGroup> DoctorAgeGroup { get; set; }
        public virtual ICollection<Experience> Experiences { get; set; }
        public virtual ICollection<FeaturedDoctor> FeaturedDoctors { get; set; }
        public virtual ICollection<OpeningHour> OpeningHours { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Slot> Slots { get; set; }
        public virtual ICollection<SocialMedia> SocialMediaLinks { get; set; }
        public virtual ICollection<Qualification> Qualifications { get; set; }
    }
}