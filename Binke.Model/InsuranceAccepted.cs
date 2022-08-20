using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Binke.Model
{
    [Table("InsuranceAccepted")]
    public class DoctorInsuranceAccepted : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InsuranceAcceptedId { get; set; }

       
        public string Name { get; set; }

        public bool IsEnable { get; set; }

        public virtual ICollection<DoctorInsurance> DoctorInsurance { get; set; }
    }

    [Table("DoctorInsurance")]
    public class DoctorInsurance : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DoctorInsuranceId { get; set; }
                     
        public int InsuranceAcceptedId { get; set; }
        [ForeignKey("InsuranceAcceptedId")]
        public virtual DoctorInsuranceAccepted Insurance { get; set; }

        public int DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }
    }
}
