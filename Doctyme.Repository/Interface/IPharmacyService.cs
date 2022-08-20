using Doctyme.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace Doctyme.Repository.Interface
{
    public interface IPharmacyService
    {
        IEnumerable<Organisation> GetAll();
        IEnumerable<Organisation> GetAll(Expression<Func<Organisation, bool>> predicate, params Expression<Func<Organisation, object>>[] includeProperties);
        Organisation GetById(object id);
        int GetCount(Expression<Func<Organisation, bool>> predicate, params Expression<Func<Organisation, object>>[] includeProperties);
        Organisation GetSingle(Expression<Func<Organisation, bool>> predicate, params Expression<Func<Organisation, object>>[] includeProperties);
        void InsertData(Organisation model);
        void UpdateData(Organisation model);
        void DeleteData(Organisation model);
        void SaveData();
        void ExecuteSqlCommandForUpdate(string tableNameToUpdate, string primaryKeyName, int primaryKeyValue, List<SqlParameter> parametersToUpdate);
    }

}
