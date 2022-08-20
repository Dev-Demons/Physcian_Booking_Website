namespace Doctyme.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Taxonomy")]
    public partial class Taxonomy
    {
        public int TaxonomyID { get; set; }

        [StringLength(200)]
        public string Taxonomy_Code { get; set; }

        [StringLength(200)]
        public string Taxonomy_Type { get; set; }

        [StringLength(200)]
        public string Taxonomy_Level { get; set; }

        public string Specialization { get; set; }

        public string Description { get; set; }

        public int? ParentID { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CreateDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? UpdateDate { get; set; }

        public int? UpdateBy { get; set; }

        public int? CreateBy { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsDeleted { get; set; }
        [Column(TypeName = "varchar(max)")]
        //[StringLength(int.MaxValue)]
        public string SpecialtyText { get; set; }
        public bool? IsSpecialty { get; set; }
       // public ICollection<FeaturedSpeciality> FeaturedSpeciality { get; set; }
    }

    public class DoctorTaxonomyModel
    {
        public int DocOrgTaxonomyID { get; set; }
        public int ReferenceID { get; set; }
        public int ParentID { get; set; }
        public int TaxonomyID { get; set; }
        public int? UserTypeId { get; set; }

        public int DoctorId { get; set; }

        public string Taxonomy_Code { get; set; }
        public string Specialization { get; set; }



        public DateTime? UpdatedDate { get; set; }

        public bool IsActive { get; set; }
        public string IsActiveString { get; set; }
        public bool IsDeleted { get; set; }

        public int CreatedBy { get; set; }
        public string Description { get; set; }
        public int TotalRecordCount { get; set; }
    }

    public class DoctorTaxonomyListModel
    {
        public int DocOrgTaxonomyID { get; set; }
        public int ReferenceID { get; set; }
        public int TaxonomyID { get; set; }
        public int? UserTypeId { get; set; }
        public int? ParentID { get; set; }
        public string Taxonomy_Code { get; set; }
        public string Specialization { get; set; }

        public int DoctorId { get; set; }



        public string UpdatedDate { get; set; }

        public bool IsActive { get; set; }
        public string IsActiveString { get; set; }
        public bool IsDeleted { get; set; }

        public int CreatedBy { get; set; }
        public string Description { get; set; }
        public int TotalRecordCount { get; set; }
    }

    public class DoctorTaxonomyUpdateModel : BaseModel
    {
        public int DocOrgTaxonomyID { get; set; }

        [Required(ErrorMessage = "Taxonomy code is required! Should be select valid Taxonomy Code!")]
        [Range(1, int.MaxValue, ErrorMessage = "Taxonomy code is required! Should be select valid Taxonomy Code!")]
        public int TaxonomyID { get; set; }

        public int UserTypeID { get; set; }

        public string Taxonomy_Code { get; set; }
        public string Specialization { get; set; }


        public int DoctorId { get; set; }


        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

    }
}
