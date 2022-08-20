using Binke.Model;
using Binke.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Binke.Repository.Interface
{
    public interface IDrugDetailService
    {
        IEnumerable<DrugDetail> GetAll();
        IEnumerable<DrugDetail> GetAll(Expression<Func<DrugDetail, bool>> predicate, params Expression<Func<DrugDetail, object>>[] includeProperties);
        IList<SpSearchDrugViewModel> SearchDrug(string spName, int drugDetailId);
        DrugDetail GetSingle(Expression<Func<DrugDetail, bool>> predicate, params Expression<Func<DrugDetail, object>>[] includeProperties);
        void InsertData(DrugDetail obj);
        void UpdateData(DrugDetail obj);
        void SaveData();
        DrugDetail GetById(object id);
    }
}
