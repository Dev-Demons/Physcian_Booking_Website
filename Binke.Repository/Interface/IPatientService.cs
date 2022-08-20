using Binke.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Binke.Repository.Interface
{
    public interface IPatientService
    {
        IEnumerable<Patient> GetAll();
        IEnumerable<Patient> GetAll(Expression<Func<Patient, bool>> predicate, params Expression<Func<Patient, object>>[] includeProperties);
        Patient GetById(object id);
        int GetCount(Expression<Func<Patient, bool>> predicate, params Expression<Func<Patient, object>>[] includeProperties);
        Patient GetSingle(Expression<Func<Patient, bool>> predicate, params Expression<Func<Patient, object>>[] includeProperties);
        void InsertData(Patient model);
        void UpdateData(Patient model);
        void DeleteData(Patient model);
        void SaveData();
    }

}
