
using Doctyme.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Doctyme.Repository.Interface
{
    public interface IUserTypeService
    {
        IEnumerable<UserType> GetAll();
        IEnumerable<UserType> GetAll(Expression<Func<UserType, bool>> predicate, params Expression<Func<UserType, object>>[] includeProperties);
        UserType GetById(object id);
        int GetCount(Expression<Func<UserType, bool>> predicate, params Expression<Func<UserType, object>>[] includeProperties);
        UserType GetSingle(Expression<Func<UserType, bool>> predicate, params Expression<Func<UserType, object>>[] includeProperties);
        void InsertData(UserType model);
        void UpdateData(UserType model);
        void DeleteData(UserType model);
        void SaveData();

        bool ExecuteSQLQueryWithOutParam(string query, params object[] parameters);

        //string GetQuerySingleResult(string query);
    }

}
