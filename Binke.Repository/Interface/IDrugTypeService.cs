using Binke.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Binke.Repository.Interface
{
    public interface IDrugTypeService
    {
        IEnumerable<DrugType> GetAll();
        IEnumerable<DrugType> GetAll(Expression<Func<DrugType, bool>> predicate, params Expression<Func<DrugType, object>>[] includeProperties);
        void InsertData(DrugType obj);
        void UpdateData(DrugType obj);
        void SaveData();
        DrugType GetById(object id);
    }

}
