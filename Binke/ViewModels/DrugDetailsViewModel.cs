using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Binke.ViewModels
{
    public class DrugDetailsViewModel
    {
        public DrugInfoViewModel DrugInfo { get; set; }

        public DrugFilterViewModel DrugFilter { get; set; }
        public List<DrugTypeFilterViewModel> DrugTypeTabletFilter { get; set; }
        public List<DrugManufacturerFilterViewModel> DrugManufacturerFilter { get; set; }
        public List<DrugStrengthFilterViewModel> DrugStrengthFilter { get; set; }
        public List<DrugTabsInfoViewModel> DrugTabsInfo { get; set; }
        public List<DrugQuantityPriceFilterViewModel> DrugQuantityPriceFilter { get; set; }
        public List<DrugInfoViewModel> DrugRelatedInfo { get; set; }

        public int OpenDefaultTabId { get; set; }
    }



    public class DrugInfoViewModel
    {
        public int? DrugId { get; set; }
        public string DrugName { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public bool IsGeneric { get; set; }
        public string FilterPrice { get; set; }
        public string FilterStrength { get; set; }
        public string FilterQuantity { get; set; }
        public string FilterType { get; set; }
        public string FilterManufacturer { get; set; }
    }

    public class DrugFilterViewModel
    {
        public string DrugName { get; set; }
        public string Price { get; set; }
        public string Quantity { get; set; }
        public string Strength { get; set; }
        public string Type { get; set; }
        public string Maunfacturer { get; set; }

        public int DrugStrengthId { get; set; }
        public int DrugTypeId { get; set; }
        public int ManufacturerId { get; set; }
        public int DrugPriceId { get; set; }

    }
    public class DrugTabsInfoViewModel
    {
        public string Name { get; set; }
        public int DrugTabsId { get; set; }
        public int Drugtabs_LookUpID { get; set; }
        public string Description { get; set; }
    }
    public class DrugTypeFilterViewModel
    {
        public int DrugType_LookUpID { get; set; }
        public int DrugTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class DrugManufacturerFilterViewModel
    {
        public int DrugId { get; set; }
        public int DrugManufacturer_LookUpId { get; set; }
        public int DrugManufacturerID { get; set; }
        public string CompanyName { get; set; }
    }
    public class DrugStrengthFilterViewModel
    {
        public int DrugId { get; set; }
        public int DrugStrengthID { get; set; }
        public int DrugStrengthLookUpId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
    }

    public class DrugQuantityPriceFilterViewModel
    {
        public int DrugPriceID { get; set; }
        public int DrugId { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }
        public int DrugStrengthID { get; set; }
        public int OrganisationId { get; set; }
        public int DrugTypeID { get; set; }
        public bool IsChecked { get; set; }
    }
}
