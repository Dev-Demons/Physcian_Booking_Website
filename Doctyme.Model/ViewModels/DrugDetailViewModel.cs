using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Model.ViewModels
{
    [NotMapped]
    public class AddDrugDetailParams : DrugDetail
    {
        public List<int> PharmacyList { get; set; }
        public List<int> DrugTypeList { get; set; }
        public List<int> DrugManufacturerList { get; set; }
        public List<int> DrugQuantityList { get; set; }
        public List<int> TabletPowerList { get; set; }
    }

    //public class AddPharmacyDetail : BaseModel
    //{
    //    public int DrugPharmacyDetailId { get; set; }
    //    public int DrugDetailId { get; set; }
    //    public int PharmacyId { get; set; }
    //    public int DrugTypeId { get; set; }
    //    public int DrugManufacturerId { get; set; }
    //    public int DrugQuantityId { get; set; }
    //    public int TabletPowerId { get; set; }
    //    public int Price { get; set; }
    //}
}
