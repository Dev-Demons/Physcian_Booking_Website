using Doctyme.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Doctyme.Repository.Interface
{
    public interface IQualificationService
    {
        IEnumerable<Qualification> GetAll();
        IEnumerable<Qualification> GetAll(Expression<Func<Qualification, bool>> predicate, params Expression<Func<Qualification, object>>[] includeProperties);
        Qualification GetById(object id);
        int GetCount(Expression<Func<Qualification, bool>> predicate, params Expression<Func<Qualification, object>>[] includeProperties);
        Qualification GetSingle(Expression<Func<Qualification, bool>> predicate, params Expression<Func<Qualification, object>>[] includeProperties);
        void InsertData(Qualification model);
        void UpdateData(Qualification model);
        void DeleteData(Qualification model);
        void SaveData();
    }

}
