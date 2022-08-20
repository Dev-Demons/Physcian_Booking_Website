using Doctyme.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Doctyme.Repository.Interface
{
    public interface IInsuranceTypeRepository
    {
        IEnumerable<InsuranceType> GetAll();
        IEnumerable<InsuranceType> GetAll(Expression<Func<InsuranceType, bool>> predicate, params Expression<Func<InsuranceType, object>>[] includeProperties);
        InsuranceType GetById(object id);
        int GetCount(Expression<Func<InsuranceType, bool>> predicate, params Expression<Func<InsuranceType, object>>[] includeProperties);
        InsuranceType GetSingle(Expression<Func<InsuranceType, bool>> predicate, params Expression<Func<InsuranceType, object>>[] includeProperties);
        void InsertData(InsuranceType model);
        void InsertData(IEnumerable<InsuranceType> model);
        void UpdateData(InsuranceType model);
        void DeleteData(InsuranceType model);
        void SaveData();
    }
}
