namespace Doctyme.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DoctorAgeGroup")]
    public partial class DoctorAgeGroup: BaseModel
    {
        public int DoctorAgeGroupId { get; set; }

        public int AgeGroupId { get; set; }

        public int DoctorId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public virtual AgeGroup AgeGroup { get; set; }
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }
    }

    public class AgeGroupModel : BaseModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "AgeGroup Name is required.")]
        public int AgeGroupId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsViewMode { get; set; }
        public string DoctorName { get; set; }
        public int DoctorId { get; set; }
        public List<AgeGroup> AgeGroupsList { get; set; }
    }
}
