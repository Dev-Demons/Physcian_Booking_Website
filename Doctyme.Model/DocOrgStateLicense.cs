
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doctyme.Model
{
    public partial class DocOrgStateLicense
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("DocOrgStateLicense")]
        public int DocOrgStateLicense1 { get; set; }
        public int ReferenceId { get; set; }
        public string HealthCareProviderTaxonomyCode { get; set; }
        public string ProviderLicenseNumber { get; set; }
        public string ProviderLicenseNumberStateCode { get; set; }
        public bool? HealthcareProviderPrimaryTaxonomySwitch { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public int? UserTypeId { get; set; }
       // [ForeignKey("ReferenceId")]
       // public virtual Organisation Organisation { get; set; }
    }

    public class DoctorStateLicenseModel
    {
        public int DocOrgStateLicense { get; set; }
        public int ReferenceId { get; set; }
        public string HealthCareProviderTaxonomyCode { get; set; }
        public string ProviderLicenseNumber { get; set; }
        public string ProviderLicenseNumberStateCode { get; set; }
        public bool? HealthcareProviderPrimaryTaxonomySwitch { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public int? UserTypeID { get; set; }

        public string ReferenceName { get; set; }
        public int DoctorId { get; set; }
        public int TotalRecordCount { get; set; }
    }

    public class DoctorStateLicenseListModel
    {
        public int DocOrgStateLicenseId { get; set; }
        public int ReferenceId { get; set; }
        public string HealthCareProviderTaxonomyCode { get; set; }
        public string ProviderLicenseNumber { get; set; }
        public string ProviderLicenseNumberStateCode { get; set; }
        public bool? HealthcareProviderPrimaryTaxonomySwitch { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public string UpdatedDate { get; set; }
        public string ReferenceName { get; set; }
        public int TotalRecordCount { get; set; }
    }

    public class DoctorStateLicenseUpdateModel :BaseModel
    {
        public int DocOrgStateLicenseId { get; set; }

        [Required(ErrorMessage = "Taxonomy Code is required")]
        public string HealthCareProviderTaxonomyCode { get; set; }

        [Required(ErrorMessage = "License Number is required")]
        public string ProviderLicenseNumber { get; set; }
        public string ProviderLicenseNumberStateCode { get; set; }
        public bool? HealthcareProviderPrimaryTaxonomySwitch { get; set; }
        public int? UserTypeID { get; set; }
        public int DoctorId { get; set; }
    }
}