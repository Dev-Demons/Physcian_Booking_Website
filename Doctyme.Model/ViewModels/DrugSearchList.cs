using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Model.ViewModels
{
    public class FilterDroplist
    {
        //public List<Tablet> Tablet { get; set; }
        public List<DrugManufacturer> DrugManufacturer { get; set; }
        public List<DrugQuantity> DrugQuantity { get; set; }
        public List<DrugType> DrugType { get; set; }
    }

    public class SpSearchDrugViewModel
    {
        public int DrugId { get; set; }
        public int DrugPharmacyDetailId { get; set; }
        public int Price { get; set; }
        public string PharmacyName { get; set; }
        public string PharmacyAddress { get; set; }
        public int DrugTypeId { get; set; }
        public string DrugType { get; set; }
        public int TabletId { get; set; }
        public string Tablet { get; set; }
        public int DrugQuantityId { get; set; }
        public string DrugQuantity { get; set; }
        public int DrugManufacturerId { get; set; }
        public string DrugManufacturer { get; set; }
    }

    public class SearchDrugRecord
    {
        public int DrugDetailId { get; set; }
        public string MedicineName { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string SideEffects { get; set; }
        public string Dosage { get; set; }
        public string Professional { get; set; }
        public string Tips { get; set; }
        public string Interaction { get; set; }

        public string Overview { get; set; }
        public string DosageformsandStrengths { get; set; }
        public string DrugInteractions { get; set; }
        public string Symptoms { get; set; }
        public string IndicationandUsage { get; set; }
        public string DosageandAdministration { get; set; }
        public string Contradictions { get; set; }
        public List<SpSearchDrugViewModel> PharmacyDetail { get; set; }
    }



    public class SearchDrugRecordsParam
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string MedicineName { get; set; }
        public List<int> DrugTypeId { get; set; }
        public List<int> TabletId { get; set; }
        public List<int> DrugQuantityId { get; set; }
        public List<int> DrugManufacturerId { get; set; }
        public string StartWithAlphabetically { get; set; }
    }

}
