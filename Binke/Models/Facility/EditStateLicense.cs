using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Binke.Models.Facility
{
    public class EditStateLicense
    {
        public int DocOrgStateLicense { get; set; }
        public string HealthCareProviderTaxonomyCode { get; set; }
        public string ProviderLicenseNumber { get; set; }
        public string ProviderLicenseNumberStateCode { get; set; }
        public bool HealthcareProviderPrimaryTaxonomySwitch { get; set; }
        public bool IsActive { get; set; }
    }
}
