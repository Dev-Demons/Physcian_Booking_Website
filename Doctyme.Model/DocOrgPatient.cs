using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Model
{
    [Table("DocOrgPatient")]
    public class DocOrgPatient
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int DocOrgPatientID { get; set; }
        public int PatientID { get; set; }
        public int ReferenceID { get; set; }
        public bool? IsPrimary { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdateDate { get; set; }
        public int? CreateBy { get; set; }
        public int? ModifiedBy { get; set; }
    }
}
