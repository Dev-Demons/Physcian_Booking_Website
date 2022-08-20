using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Doctyme.Model;

namespace Binke.ViewModels
{
    public class DrugTypeViewModel : BaseViewModel
    {
        public string Name { get; set; }

        public string Description { get; set; }
        public bool IsViewMode { get; set; }



    }
    public class DrugSymptomsViewModel : BaseViewModel
    {
        public string Name { get; set; }

        public string Description { get; set; }
        public bool IsViewMode { get; set; }



    }
    public class DrugStrengthViewModel : BaseViewModel
    {
        public string Name { get; set; }

        public string Description { get; set; }
        public bool IsViewMode { get; set; }



    }
    public class AddressTypeViewModel : BaseViewModel
    {
        public string Name { get; set; }
       
        public string Description { get; set; }
        public bool IsViewMode { get; set; }
    }
    public class AmenityOptionViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsViewMode { get; set; }
        public bool IsOption { get; set; }
    }
    public class InsuranceTypeViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsViewMode { get; set; }
    }
    public class GenderTypeViewModel : BaseViewModel
    {
        
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsViewMode { get; set; }
    }
    public class DoctorGenderViewModel 
    {
        public int DoctorGenderTypeId { get; set; }
        public int GenderTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsViewMode { get; set; }
        public string DoctorName { get; set; }
        public int DoctorId { get; set; }
        public List<GenderType> GendersList { get; set; }
        public bool IsActive { get; set; }
    }
    public class PaymentTypeViewModel : BaseViewModel
    {
        public string Name { get; set; }

        public string Description { get; set; }
        public bool IsViewMode { get; set; }
    }
    public class AdvertisementLocationViewModel : BaseViewModel
    {
        public string Name { get; set; }

        public string Description { get; set; }
        public bool IsViewMode { get; set; }
    }
    public class DrugManufacturerViewModel : BaseViewModel
    {
        public string Name { get; set; }

        public string Description { get; set; }
        public bool IsViewMode { get; set; }
    }
    public class DrugStatusViewModel : BaseViewModel
    {
        public string Name { get; set; }

        public string Description { get; set; }
        public bool IsViewMode { get; set; }
    }
    public class DrugTabsViewModel : BaseViewModel
    {
        public string Name { get; set; }

        public string Description { get; set; }
        public bool IsViewMode { get; set; }
    }
    public class OrganisationTypeViewModel : BaseViewModel
    {
        public string Name { get; set; }

        public string Description { get; set; }
        public bool IsViewMode { get; set; }
    }
    public class SpecialitiesViewModel : BaseViewModel
    {
        public string Name { get; set; }

        public string Description { get; set; }
        public bool IsViewMode { get; set; }
        public string TaxonomyID { get; set; }
    }
    public class DoctorFaciityAffiliataionViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsViewMode { get; set; }
        public string DoctorName { get; set; }
        public int DoctorId { get; set; }
        public List<Facility> GendersList { get; set; }
        public bool IsActive { get; set; }
      
    }
 
    public class DoctorBoardCertificationViewModel : BaseViewModel
    {
        public int DoctorBoardCertificationId { get; set; }
        public int BoardCertificationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsViewMode { get; set; }
        public string DoctorName { get; set; }
        public int DoctorId { get; set; }
        public List<DropDownModel> BoardCertificationsList { get; set; }
        public bool IsActive { get; set; }
        public int TotalRows { get; set; }

    }
    public class OrgTaxonomyViewModel : BaseViewModel
    {
        public int DocOrgTaxonomyID { get; set; }
        public int ReferenceID { get; set; }
        public int TaxonomyID { get; set; }
        public int? UserTypeId { get; set; }
        public Taxonomy Taxonomy { get; set; }
        public virtual UserType UserType { get; set; }
        public bool IsViewMode { get; set; }
        public string DoctorName { get; set; }
        public int DoctorId { get; set; }
        public List<Taxonomy> TaxonomyList { get; set; }
        public bool IsActive { get; set; }

    }

}
