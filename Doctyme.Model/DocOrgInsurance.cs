using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doctyme.Model
{
    public  class DocOrgInsurances : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocOrgInsuranceId { get; set; }
        public int ReferenceId { get; set; }
        public int InsurancePlanId { get; set; }
        public InsurancePlan InsurancePlan { get; set; }
        [Column(TypeName = "varchar"), MaxLength(20)]
        public string InsuranceIdentifierId { get; set; }
        public string StateId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public int? UserTypeId { get; set; }
        //[ForeignKey("UserTypeId")]
        //public virtual UserType UserType { get; set; }
    }
}
