namespace Doctyme.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SocialMedia")]
    public partial class SocialMedia
    {
        public int SocialMediaId { get; set; }

        public int? ReferenceId { get; set; }

        [StringLength(215)]
        public string Facebook { get; set; }

        [StringLength(215)]
        public string Twitter { get; set; }

        [StringLength(215)]
        public string LinkedIn { get; set; }

        [StringLength(215)]
        public string Instagram { get; set; }

        [StringLength(215)]
        public string Youtube { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }
        public int? UserTypeId { get; set; }
    }

    public class SocialMediaModel : BaseModel
    {
        public int SocialMediaId { get; set; }

        public string Facebook { get; set; }

        public string Twitter { get; set; }

        public string LinkedIn { get; set; }

        public string Instagram { get; set; }

        public int? DoctorId { get; set; }

        public int? SeniorCareId { get; set; }
    }

    public class DocSocialMediaUpdateModel
    {
        public int SocialMediaId { get; set; }

        public int? ReferenceId { get; set; }

        public string Facebook { get; set; }

        public string Twitter { get; set; }

        public string LinkedIn { get; set; }

        public string Instagram { get; set; }

        public string Youtube { get; set; }

        public string Pinterest { get; set; }

        public string Tumblr { get; set; }

        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public int CreatedBy { get; set; }

        public int? UserTypeID { get; set; }


        public int DoctorId { get; set; }

        public string DoctorName { get; set; }

        public int TotalRecordCount { get; set; }
    }
}
