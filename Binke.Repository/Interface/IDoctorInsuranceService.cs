using Binke.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Binke.Repository.Interface
{
    public interface IDoctorInsuranceService
    {
        IEnumerable<DoctorInsurance> GetAll();
        IEnumerable<DoctorInsurance> GetAll(Expression<Func<DoctorInsurance, bool>> predicate, params Expression<Func<DoctorInsurance, object>>[] includeProperties);
        DoctorInsurance GetById(object id);
        int GetCount(Expression<Func<DoctorInsurance, bool>> predicate, params Expression<Func<DoctorInsurance, object>>[] includeProperties);
        DoctorInsurance GetSingle(Expression<Func<DoctorInsurance, bool>> predicate, params Expression<Func<DoctorInsurance, object>>[] includeProperties);
        void InsertData(DoctorInsurance model);
        void UpdateData(DoctorInsurance model);
        void DeleteData(DoctorInsurance model);
        void SaveData();
    }
}
