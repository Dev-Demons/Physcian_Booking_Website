using Binke.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Binke.Repository.Interface
{
    public interface IDrugManufacturerService
    {
        IEnumerable<DrugManufacturer> GetAll();
        IEnumerable<DrugManufacturer> GetAll(Expression<Func<DrugManufacturer, bool>> predicate, params Expression<Func<DrugManufacturer, object>>[] includeProperties);
        void InsertData(DrugManufacturer obj);
        void UpdateData(DrugManufacturer obj);
        DrugManufacturer GetById(object id);
        void SaveData();
    }
}
