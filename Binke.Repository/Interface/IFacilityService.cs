using Binke.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Binke.Repository.Interface
{
    public interface IFacilityService
    {
        IEnumerable<Facility> GetAll();
        IEnumerable<Facility> GetAll(Expression<Func<Facility, bool>> predicate, params Expression<Func<Facility, object>>[] includeProperties);
        Facility GetById(object id);
        int GetCount(Expression<Func<Facility, bool>> predicate, params Expression<Func<Facility, object>>[] includeProperties);
        Facility GetSingle(Expression<Func<Facility, bool>> predicate, params Expression<Func<Facility, object>>[] includeProperties);
        void InsertData(Facility model);
        void UpdateData(Facility model);
        void DeleteData(Facility model);
        void SaveData();

        Facility GetByUserId(int userId);
    }

}
