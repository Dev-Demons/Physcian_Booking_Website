using Binke.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Binke.Repository.Interface
{
    public interface IPharmacyService
    {
        IEnumerable<Pharmacy> GetAll();
        IEnumerable<Pharmacy> GetAll(Expression<Func<Pharmacy, bool>> predicate, params Expression<Func<Pharmacy, object>>[] includeProperties);
        Pharmacy GetById(object id);
        int GetCount(Expression<Func<Pharmacy, bool>> predicate, params Expression<Func<Pharmacy, object>>[] includeProperties);
        Pharmacy GetSingle(Expression<Func<Pharmacy, bool>> predicate, params Expression<Func<Pharmacy, object>>[] includeProperties);
        void InsertData(Pharmacy model);
        void UpdateData(Pharmacy model);
        void DeleteData(Pharmacy model);
        void SaveData();
    }

}
