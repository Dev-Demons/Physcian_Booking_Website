using Doctyme.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Doctyme.Repository.Interface
{
    public interface IGenderService
    {
        IEnumerable<GenderType> GetAll();
        IEnumerable<GenderType> GetAll(Expression<Func<GenderType, bool>> predicate, params Expression<Func<GenderType, object>>[] includeProperties);
        GenderType GetById(object id);
        GenderType GetSingle(Expression<Func<GenderType, bool>> predicate, params Expression<Func<GenderType, object>>[] includeProperties);
        void InsertData(GenderType model);
        void UpdateData(GenderType model);
        void DeleteData(GenderType model);
        void SaveData();
    }
}
