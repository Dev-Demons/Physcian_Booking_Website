using Doctyme.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Doctyme.Repository.Interface
{
    public interface IExperienceService
    {
        IEnumerable<Experience> GetAll();
        IEnumerable<Experience> GetAll(Expression<Func<Experience, bool>> predicate, params Expression<Func<Experience, object>>[] includeProperties);
        Experience GetById(object id);
        int GetCount(Expression<Func<Experience, bool>> predicate, params Expression<Func<Experience, object>>[] includeProperties);
        Experience GetSingle(Expression<Func<Experience, bool>> predicate, params Expression<Func<Experience, object>>[] includeProperties);
        void InsertData(Experience model);
        void UpdateData(Experience model);
        void DeleteData(Experience model);
        void SaveData();
    }
}
