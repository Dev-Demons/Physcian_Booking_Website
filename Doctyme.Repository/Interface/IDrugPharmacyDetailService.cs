using Doctyme.Model;
using System.Collections.Generic;

namespace Doctyme.Repository.Interface
{
    public interface IDrugPharmacyDetailService
    {
        void InsertData(DrugPharmacyDetail obj);
        IEnumerable<DrugPharmacyDetail> GetAll();
        void UpdateData(DrugPharmacyDetail obj);
        DrugPharmacyDetail GetById(object id);
        void SaveData();
        void DeleteData(IEnumerable<DrugPharmacyDetail> obj);
    }
}
