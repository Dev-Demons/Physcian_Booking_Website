using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Model
{
    [Table("DoctorInsuranceAccepted")]
    public class DoctorInsuranceAccepted : BaseModel
    { 
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InsuranceAcceptedId { get; set; }


        public string Name { get; set; }

        public bool IsEnable { get; set; }

        public virtual ICollection<InsurancePlan> InsurancePlan { get; set; }
    }

    public class InsuranceAcceptedModel : BaseModel
    {
        public int Id { get; set; }
        public int InsuranceAcceptedId { get; set; }
        [Required(ErrorMessage = "Please enter Insurance Name.")]
        public string Name { get; set; }

        public bool IsEnable { get; set; }
    }
}
