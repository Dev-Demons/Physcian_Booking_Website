using Doctyme.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Doctyme.Repository.Interface
{
    public interface IAgeGroupService
    {
        IEnumerable<AgeGroup> GetAll();
        IEnumerable<AgeGroup> GetAll(Expression<Func<AgeGroup, bool>> predicate, params Expression<Func<AgeGroup, object>>[] includeProperties);
        AgeGroup GetById(object id);
        AgeGroup GetSingle(Expression<Func<AgeGroup, bool>> predicate, params Expression<Func<AgeGroup, object>>[] includeProperties);
        void InsertData(AgeGroup model);
        void UpdateData(AgeGroup model);
        void DeleteData(AgeGroup model);
        void SaveData();
    }
}
