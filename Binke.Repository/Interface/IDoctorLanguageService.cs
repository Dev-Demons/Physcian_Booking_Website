using Binke.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Binke.Repository.Interface
{
    public interface IDoctorLanguageService
    {
        IEnumerable<DoctorLanguage> GetAll();
        IEnumerable<DoctorLanguage> GetAll(Expression<Func<DoctorLanguage, bool>> predicate, params Expression<Func<DoctorLanguage, object>>[] includeProperties);
        DoctorLanguage GetById(object id);
        int GetCount(Expression<Func<DoctorLanguage, bool>> predicate, params Expression<Func<DoctorLanguage, object>>[] includeProperties);
        DoctorLanguage GetSingle(Expression<Func<DoctorLanguage, bool>> predicate, params Expression<Func<DoctorLanguage, object>>[] includeProperties);
        void InsertData(DoctorLanguage model);
        void UpdateData(DoctorLanguage model);
        void DeleteData(DoctorLanguage model);
        void SaveData();
    }
}
