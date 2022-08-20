using Binke.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Binke.Repository.Interface
{
    public interface IFacilityTypeService
    {
        IEnumerable<FacilityType> GetAll();
        IEnumerable<FacilityType> GetAll(Expression<Func<FacilityType, bool>> predicate, params Expression<Func<FacilityType, object>>[] includeProperties);
        FacilityType GetById(object id);
        int GetCount(Expression<Func<FacilityType, bool>> predicate, params Expression<Func<FacilityType, object>>[] includeProperties);
        FacilityType GetSingle(Expression<Func<FacilityType, bool>> predicate, params Expression<Func<FacilityType, object>>[] includeProperties);
        void InsertData(FacilityType model);
        void UpdateData(FacilityType model);
        void DeleteData(FacilityType model);
        void SaveData();
    }
}
