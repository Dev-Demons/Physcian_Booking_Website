using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Doctyme.Model.ViewModels
{
    public class OrganizationAmenityOptionViewModel: BaseViewModel
    {
        public int OrganizationAmenityOptionID { get; set; }
        public int OrganizationID { get; set; }

        [Required(ErrorMessage = "Please enter Amenity Name.")]
        public string Name { get; set; }

        public string Description { get; set; }
        public string ImagePath { get; set; }
        public bool IsOption { get; set; }

    }
    public class BaseViewModel
    {
        public int Id { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsEnable { get; set; }
    }


    public class OrganizationAmenityOptionDataListModel
    {
        public List<OrganizationAmenityOptionViewModel> OrganizationAmenityOptionList { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecord { get; set; }
    }


    public class SiteImageViewModel : BaseViewModel
    {
        public int SiteImageId { get; set; }
        public int? ReferenceId { get; set; }
        public int? UserTypeID { get; set; }
        public string ImagePath { get; set; }        
        public bool IsProfile { get; set; }
        public string Name { get; set; }
    }

    public class SiteImageDataListModel
    {
        public List<SiteImageViewModel> SiteImageList { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecord { get; set; }
    }

}
