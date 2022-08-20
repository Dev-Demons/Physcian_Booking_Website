﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doctyme.Model
{
    public partial class BoardCertifications
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short BoardCertificationId { get; set; }
        [StringLength(100)]
        public string CertificationName { get; set; }
        [Column(TypeName = "nvarchar")]
        public string Description { get; set; }
        public int ParentID { get; set; }
        public short SpecialityId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public virtual ICollection<DoctorBoardCertification> DoctorBoardCertifications { get; set; }
    }
    public class DropDownModel
    {
        public int Key { get; set; }
        public string Value { get; set; }
    }
}