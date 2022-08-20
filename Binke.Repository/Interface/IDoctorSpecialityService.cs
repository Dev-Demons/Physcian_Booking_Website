using Binke.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Binke.Repository.Interface
{
    public interface IDoctorSpecialityService
    {
        IEnumerable<DoctorSpeciality> GetAll();
        IEnumerable<DoctorSpeciality> GetAll(Expression<Func<DoctorSpeciality, bool>> predicate, params Expression<Func<DoctorSpeciality, object>>[] includeProperties);
        DoctorSpeciality GetById(object id);
        int GetCount(Expression<Func<DoctorSpeciality, bool>> predicate, params Expression<Func<DoctorSpeciality, object>>[] includeProperties);
        DoctorSpeciality GetSingle(Expression<Func<DoctorSpeciality, bool>> predicate, params Expression<Func<DoctorSpeciality, object>>[] includeProperties);
        void InsertData(DoctorSpeciality model);
        void UpdateData(DoctorSpeciality model);
        void DeleteData(DoctorSpeciality model);
        void SaveData();
    }
}
