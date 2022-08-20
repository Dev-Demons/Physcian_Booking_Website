using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doctyme.Model
{
    [Table("Language")]
    public class Language : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short LanguageId { get; set; }

        [Column(TypeName = "varchar"), MaxLength(100)]
        public string LanguageName { get; set; }
        public string LanguageCode { get; set; }

        public virtual ICollection<DoctorLanguage> DoctorLanguages { get; set; }
    }

    [Table("DoctorLanguage")]
    public class DoctorLanguage : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DoctorLanguageId { get; set; }

        public short LanguageId { get; set; }
        [ForeignKey("LanguageId")]
        public virtual Language Language { get; set; }

        public int DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }
    }

}
