using Binke.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Binke.Repository.Interface
{
    public interface  IDoctorInsuranceAcceptedService
    {
        IEnumerable<DoctorInsuranceAccepted> GetAll();
        IEnumerable<DoctorInsuranceAccepted> GetAll(Expression<Func<DoctorInsuranceAccepted, bool>> predicate, params Expression<Func<DoctorInsuranceAccepted   , object>>[] includeProperties);
        DoctorInsuranceAccepted GetById(object id);
        DoctorInsuranceAccepted GetSingle(Expression<Func<DoctorInsuranceAccepted, bool>> predicate, params Expression<Func<DoctorInsuranceAccepted, object>>[] includeProperties);
        void InsertData(DoctorInsuranceAccepted model);
        void UpdateData(DoctorInsuranceAccepted model);
        void DeleteData(DoctorInsuranceAccepted model);
        void SaveData();
    }
}
