using Binke.Model.DBContext;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Binke.Repository.Interface
{
    public interface IUserRoleService
    {
        IEnumerable<UserRole> GetAll();
        IEnumerable<UserRole> GetAll(Expression<Func<UserRole, bool>> predicate, params Expression<Func<UserRole, object>>[] includeProperties);
        UserRole GetById(object id);
        int GetCount(Expression<Func<UserRole, bool>> predicate, params Expression<Func<UserRole, object>>[] includeProperties);
        UserRole GetSingle(Expression<Func<UserRole, bool>> predicate, params Expression<Func<UserRole, object>>[] includeProperties);
        void InsertData(UserRole model);
        void UpdateData(UserRole model);
        void DeleteData(UserRole model);
        void SaveData();
    }

}
