
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doctyme.Model
{
    public partial class InsuranceProvider
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InsProviderId { get; set; }
        [Column(TypeName = "varchar"), MaxLength(200)]
        public string InsCompanyName { get; set; }
        [Column(TypeName = "varchar"), MaxLength(200)]
        public string InsAddress { get; set; }
        [Column(TypeName = "varchar"), MaxLength(200)]
        public string InsCity { get; set; }
        [Column(TypeName = "varchar"), MaxLength(10)]
        public string InsState { get; set; }
        [Column(TypeName = "varchar"), MaxLength(10)]
        public string InsZipCode { get; set; }
        [Column(TypeName = "varchar"), MaxLength(12)]
        public string InsPhone { get; set; }
        [Column(TypeName = "varchar"), MaxLength(10)]
        public string InsFax { get; set; }
        [Column(TypeName = "varchar"), MaxLength(200)]
        public string InsEmail { get; set; }
        [Column(TypeName = "varchar"), MaxLength(200)]
        public string InsWebSite { get; set; }
        public int? SocialMediaId { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? UpdateBy { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
    }
}