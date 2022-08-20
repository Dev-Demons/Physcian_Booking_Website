using Binke.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Binke.Repository.Interface
{
    public interface ILanguageService
    {
        IEnumerable<Language> GetAll();
        IEnumerable<Language> GetAll(Expression<Func<Language, bool>> predicate, params Expression<Func<Language, object>>[] includeProperties);
        Language GetById(object id);
        int GetCount(Expression<Func<Language, bool>> predicate, params Expression<Func<Language, object>>[] includeProperties);
        Language GetSingle(Expression<Func<Language, bool>> predicate, params Expression<Func<Language, object>>[] includeProperties);
        void InsertData(Language model);
        void UpdateData(Language model);
        void DeleteData(Language model);
        void SaveData();
    }
}
