using Doctyme.Model;
using Doctyme.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace Doctyme.Repository.Interface
{
    public interface IFacilityService
    {
        IEnumerable<Organisation> GetAll();
        IEnumerable<Organisation> GetAll(Expression<Func<Organisation, bool>> predicate, params Expression<Func<Organisation, object>>[] includeProperties);
        Organisation GetById(object id);
        int GetCount(Expression<Func<Organisation, bool>> predicate, params Expression<Func<Organisation, object>>[] includeProperties);
        Organisation GetSingle(Expression<Func<Organisation, bool>> predicate, params Expression<Func<Organisation, object>>[] includeProperties);
        IList<Organisation> ExecuteSP(string spName, List<SqlParameter> paraObjects);
        int ExecuteSQLQuery(string query, params object[] parameters);
        IList<DrpFacilityTypeModel> GetDrpFacilityTypeList(string spName, object[] paraObjects);
        IList<FacilityProviderModel> GetFacilityListByTypeId(string commandText, List<SqlParameter> parameters, out int totalRecord);
        IList<OpeningHoursProviderModel> GetOpeningHoursByOrgId(string commandText, List<SqlParameter> parameters, out int RecordCount);
        IList<ReviewProviderModel> GetReviewListByTypeId(string spName, List<SqlParameter> paraObjects, out int RecordCount);
        IList<SpecialityProviderModel> GetSpecialityByOrgId(string spName, List<SqlParameter> paraObjects, out int RecordCount);

        IList<StateLicenseProviderModel> GetStateLicenseByOrgId(string commandText, List<SqlParameter> parameters, out int totalRecord);
        IList<InsurancePlanProviderModel> GetInsurancePlanByOrgId(string commandText, List<SqlParameter> parameters, out int totalRecord);
        IList<SummaryProviderModel> GetSummary(string commandText, List<SqlParameter> parameters);
        void ExecuteSqlCommandForUpdate(string tableNameToUpdate, string primaryKeyName, long primaryKeyValue, List<SqlParameter> parametersToUpdate);
        void ExecuteSqlCommandForUpdate(string tableNameToUpdate, string primaryKeyName, int primaryKeyValue, List<SqlParameter> parametersToUpdate);
        void ExecuteSqlCommandForInsert(string tableNameToInsertInto, List<SqlParameter> parametersToInsertInto);
        IEnumerable<T> ExecWithStoreProcedure<T>(string query, params object[] parameters);
        DbRawSqlQuery<T> SQLQuery<T>(string sql, params object[] parameters);
        void InsertData(Organisation model);
        void UpdateData(Organisation model);
        void DeleteData(Organisation model);
        void SaveData();

        Organisation GetByUserId(int userId);
    }

}
