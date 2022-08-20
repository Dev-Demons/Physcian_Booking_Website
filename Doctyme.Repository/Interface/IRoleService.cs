
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Binke.Repository.Interface
{
    public interface IRoleService
    {
        IEnumerable<Role> GetAll();
        IEnumerable<Role> GetAll(Expression<Func<Role, bool>> predicate, params Expression<Func<Role, object>>[] includeProperties);
        Role GetById(object id);
        int GetCount(Expression<Func<Role, bool>> predicate, params Expression<Func<Role, object>>[] includeProperties);
        Role GetSingle(Expression<Func<Role, bool>> predicate, params Expression<Func<Role, object>>[] includeProperties);
        void InsertData(Role model);
        void UpdateData(Role model);
        void DeleteData(Role model);
        void SaveData();
    }

}
