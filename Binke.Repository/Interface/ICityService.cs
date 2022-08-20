using Binke.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Binke.Repository.Interface
{
    public interface ICityService
    {
        IEnumerable<City> GetAll();
        IEnumerable<City> GetAll(Expression<Func<City, bool>> predicate, params Expression<Func<City, object>>[] includeProperties);
        City GetById(object id);
        int GetCount(Expression<Func<City, bool>> predicate, params Expression<Func<City, object>>[] includeProperties);
        City GetSingle(Expression<Func<City, bool>> predicate, params Expression<Func<City, object>>[] includeProperties);
        void InsertData(City model);
        void UpdateData(City model);
        void DeleteData(City model);
        void SaveData();
    }
}
