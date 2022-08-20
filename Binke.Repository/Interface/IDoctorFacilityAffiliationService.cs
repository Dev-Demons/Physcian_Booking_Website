using Binke.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Binke.Repository.Interface
{
    public interface IDoctorFacilityAffiliationService
    {
        IEnumerable<DoctorFacilityAffiliation> GetAll();
        IEnumerable<DoctorFacilityAffiliation> GetAll(Expression<Func<DoctorFacilityAffiliation, bool>> predicate, params Expression<Func<DoctorFacilityAffiliation, object>>[] includeProperties);
        DoctorFacilityAffiliation GetById(object id);
        int GetCount(Expression<Func<DoctorFacilityAffiliation, bool>> predicate, params Expression<Func<DoctorFacilityAffiliation, object>>[] includeProperties);
        DoctorFacilityAffiliation GetSingle(Expression<Func<DoctorFacilityAffiliation, bool>> predicate, params Expression<Func<DoctorFacilityAffiliation, object>>[] includeProperties);
        void InsertData(DoctorFacilityAffiliation model);
        void UpdateData(DoctorFacilityAffiliation model);
        void DeleteData(DoctorFacilityAffiliation model);
        void SaveData();
    }
}
