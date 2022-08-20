using Doctyme.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Repository.Interface
{
    public interface ITabletService
    {
        IEnumerable<Tablet> GetAll();
        IEnumerable<Tablet> GetAll(Expression<Func<Tablet, bool>> predicate, params Expression<Func<Tablet, object>>[] includeProperties);
        void InsertData(Tablet obj);
        void UpdateData(Tablet obj);
        void SaveData();
        Tablet GetById(object id);
    }
}
