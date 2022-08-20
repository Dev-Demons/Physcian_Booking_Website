using Binke.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Binke.Repository.Interface
{
   public interface IDrugQuantityService
    {
        IEnumerable<DrugQuantity> GetAll();
        IEnumerable<DrugQuantity> GetAll(Expression<Func<DrugQuantity, bool>> predicate, params Expression<Func<DrugQuantity, object>>[] includeProperties);
        void InsertData(DrugQuantity obj);
        void UpdateData(DrugQuantity obj);
        void SaveData();
        DrugQuantity GetById(object id);
    }
}
