using Doctyme.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace Doctyme.Repository.Interface
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
        IEnumerable<T> ExecWithStoreProcedure<T>(string query, params object[] parameters);
        void ExecuteSqlCommandForUpdate(string tableNameToUpdate, string primaryKeyName, int primaryKeyValue, List<SqlParameter> parametersToUpdate);
        DbRawSqlQuery<T> SQLQuery<T>(string sql, params object[] parameters);
        List<T> GetDataList<T>(string commandText, List<SqlParameter> parameters);
        int ExecuteSQLQuery(string query, params object[] parameters);
    }

}
