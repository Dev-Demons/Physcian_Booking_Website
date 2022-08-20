using Binke.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Binke.Repository.Interface
{
    public interface IDrugService
    {
        IEnumerable<Drug> GetAll();
        IEnumerable<Drug> GetAll(Expression<Func<Drug, bool>> predicate, params Expression<Func<Drug, object>>[] includeProperties);
        Drug GetById(object id);
        int GetCount(Expression<Func<Drug, bool>> predicate, params Expression<Func<Drug, object>>[] includeProperties);
        Drug GetSingle(Expression<Func<Drug, bool>> predicate, params Expression<Func<Drug, object>>[] includeProperties);
        void InsertData(Drug model);
        void UpdateData(Drug model);
        void DeleteData(Drug model);
        void SaveData();
    }

}
