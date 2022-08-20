using Binke.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Binke.Repository.Interface
{
    public interface IDoctorAgeGroupService
    {

        IEnumerable<DoctorAgeGroup> GetAll();
        IEnumerable<DoctorAgeGroup> GetAll(Expression<Func<DoctorAgeGroup, bool>> predicate, params Expression<Func<DoctorAgeGroup, object>>[] includeProperties);
        DoctorAgeGroup GetById(object id);
        int GetCount(Expression<Func<DoctorAgeGroup, bool>> predicate, params Expression<Func<DoctorAgeGroup, object>>[] includeProperties);
        DoctorAgeGroup GetSingle(Expression<Func<DoctorAgeGroup, bool>> predicate, params Expression<Func<DoctorAgeGroup, object>>[] includeProperties);
        void InsertData(DoctorAgeGroup model);
        void UpdateData(DoctorAgeGroup model);
        void DeleteData(DoctorAgeGroup model);
        void SaveData();
    }
}
