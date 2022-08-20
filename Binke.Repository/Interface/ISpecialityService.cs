using Binke.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Binke.Repository.Interface
{
    public interface ISpecialityService
    {
        IEnumerable<Speciality> GetAll();
        IEnumerable<Speciality> GetAll(Expression<Func<Speciality, bool>> predicate, params Expression<Func<Speciality, object>>[] includeProperties);
        Speciality GetById(object id);
        int GetCount(Expression<Func<Speciality, bool>> predicate, params Expression<Func<Speciality, object>>[] includeProperties);
        Speciality GetSingle(Expression<Func<Speciality, bool>> predicate, params Expression<Func<Speciality, object>>[] includeProperties);
        void InsertData(Speciality model);
        void UpdateData(Speciality model);
        void DeleteData(Speciality model);
        void SaveData();
    }
}
