using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Binke.Models.Facility
{
    public class EditInsurancePlan
    {
        public int InsurancePlanId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
