using Binke.Model;
using System.Collections.Generic;

namespace Binke.Repository.Interface
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
