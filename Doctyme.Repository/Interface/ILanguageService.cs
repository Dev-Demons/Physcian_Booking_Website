using Doctyme.Model;
using Doctyme.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Doctyme.Repository.Interface
{
    public interface ILanguageService
    {
        IEnumerable<Language> GetAll();
        IEnumerable<Language> GetAll(Expression<Func<Language, bool>> predicate, params Expression<Func<Language, object>>[] includeProperties);
        Language GetById(object id);
        int GetCount(Expression<Func<Language, bool>> predicate, params Expression<Func<Language, object>>[] includeProperties);
        Language GetSingle(Expression<Func<Language, bool>> predicate, params Expression<Func<Language, object>>[] includeProperties);

        IList<DrpLanguageModel> GeLanguageDropDownList(string spName, object[] paraObjects);

        void InsertData(Language model);
        void UpdateData(Language model);
        void DeleteData(Language model);
        void SaveData();
    }
}
