using Binke.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Binke.Repository.Interface
{
    public interface IFacilityOptionService
    {
        IEnumerable<FacilityOption> GetAll();
        IEnumerable<FacilityOption> GetAll(Expression<Func<FacilityOption, bool>> predicate, params Expression<Func<FacilityOption, object>>[] includeProperties);
        FacilityOption GetById(object id);
        int GetCount(Expression<Func<FacilityOption, bool>> predicate, params Expression<Func<FacilityOption, object>>[] includeProperties);
        FacilityOption GetSingle(Expression<Func<FacilityOption, bool>> predicate, params Expression<Func<FacilityOption, object>>[] includeProperties);
        void InsertData(FacilityOption model);
        void UpdateData(FacilityOption model);
        void DeleteData(FacilityOption model);
        void SaveData();
    }
}
