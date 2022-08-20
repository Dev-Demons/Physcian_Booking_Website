using Doctyme.Model;
using System.Linq;
using Doctyme.Repository.Services;
using Doctyme.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Doctyme.Model.ViewModels;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity.Infrastructure;

namespace Doctyme.Repository.Services
{
    public class FacilityRepository : GenericRepository<Organisation>, IFacilityService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public FacilityRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IList<OrganizationProviderModel> GetOrganisationListByTypeId(string commandText, List<SqlParameter> parameters, out int totalRecord)
        {
            totalRecord = 0;
            var pTotalRecords = new SqlParameter("@TotalRecords", SqlDbType.Int) { Direction = ParameterDirection.Output };
            parameters.Add(pTotalRecords);
            if (parameters != null && parameters.Any())
            {
                for (int i = 0; i <= parameters.Count - 1; i++)
                {
                    var p = parameters[i] as SqlParameter;
                    if (p == null)
                        throw new Exception("Not support parameter type");

                    commandText += i == 0 ? " " : ", ";

                    commandText += p.ParameterName;
                    if (p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Output)
                    {
                        //output parameter
                        commandText += " output";
                    }
                }
            }

            var data = _dbContext.Database.SqlQuery<OrganizationProviderModel>(commandText, parameters.ToArray<object>()).ToList();
            totalRecord = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
            return data;
        }

        //void IFacilityService.DeleteData(Organisation model)
        //{
        //    throw new NotImplementedException();
        //}

        //IEnumerable<Organisation> IFacilityService.GetAll()
        //{
        //    throw new NotImplementedException();
        //}

        //IEnumerable<Organisation> IFacilityService.GetAll(Expression<Func<Organisation, bool>> predicate, params Expression<Func<Organisation, object>>[] includeProperties)
        //{
        //    throw new NotImplementedException();
        //}

        //Organisation IFacilityService.GetById(object id)
        //{
        //    throw new NotImplementedException();
        //}

        public IList<DrpFacilityTypeModel> GetDrpFacilityTypeList(string spName, object[] paraObjects)
        {
            return _dbContext.Database.SqlQuery<DrpFacilityTypeModel>(spName, paraObjects).ToList();
        }

        public IList<FacilityProviderModel> GetFacilityListByTypeId(string commandText, List<SqlParameter> parameters, out int totalRecord)
        {
            totalRecord = 0;
            //var pTotalRecords = new SqlParameter("@TotalRecords", SqlDbType.Int) { Direction = ParameterDirection.Output };
            //parameters.Add(pTotalRecords);
            //if (parameters != null && parameters.Any())
            //{
            //    for (int i = 0; i <= parameters.Count - 1; i++)
            //    {
            //        var p = parameters[i] as SqlParameter;
            //        if (p == null)
            //            throw new Exception("Not support parameter type");

            //        commandText += i == 0 ? " " : ", ";

            //        commandText += p.ParameterName;
            //        if (p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Output)
            //        {
            //            //output parameter
            //            commandText += " output";
            //        }
            //    }
            //}

            //var data = _dbContext.Database.SqlQuery<FacilityProviderModel>(commandText, parameters.ToArray<object>()).ToList();
            //totalRecord = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;

            using (var cmd = _dbContext.Database.Connection.CreateCommand())
            {
                cmd.CommandText = commandText;
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (var item in parameters)
                {
                    cmd.Parameters.Add(item);
                }
                _dbContext.Database.Connection.Open();
                var reader = cmd.ExecuteReader();
                var objectContext = ((IObjectContextAdapter)_dbContext).ObjectContext;
                var data = objectContext.Translate<FacilityProviderModel>(reader).ToList();
                totalRecord = reader.RecordsAffected / 2;
                _dbContext.Database.Connection.Close();
                return data.ToList();
            }

            //return data;
        }

        public IList<OpeningHoursProviderModel> GetOpeningHoursByOrgId(string commandText, List<SqlParameter> parameters, out int RecordCount)
        {
            // Create a SQL command and add parameter
            var cmd = _dbContext.Database.Connection.CreateCommand();
            cmd.CommandText = commandText;
            cmd.CommandType = CommandType.StoredProcedure;
            foreach (var item in parameters)
            {
                cmd.Parameters.Add(item);
            }
            _dbContext.Database.Connection.Open();
            var reader = cmd.ExecuteReader();
            var objectContext = ((IObjectContextAdapter)_dbContext).ObjectContext;
            var data = objectContext.Translate<OpeningHoursProviderModel>(reader).ToList();
            reader.NextResult();
            var d2 = objectContext.Translate<TotalRecords>(reader).ToList();
            RecordCount = d2 != null && d2.Count > 0 ? d2.First().TotalRecordCount : 0;
            return data.ToList();
        }

        public IList<ReviewProviderModel> GetReviewListByTypeId(string commandText, List<SqlParameter> parameters, out int totalRecord)
        {
            using (var cmd = _dbContext.Database.Connection.CreateCommand())
            {
                cmd.CommandText = commandText;
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (var item in parameters)
                {
                    cmd.Parameters.Add(item);
                }
                _dbContext.Database.Connection.Open();
                var reader = cmd.ExecuteReader();
                var objectContext = ((IObjectContextAdapter)_dbContext).ObjectContext;
                var data = objectContext.Translate<ReviewProviderModel>(reader).ToList();
                reader.NextResult();
                var d2 = objectContext.Translate<TotalRecords>(reader).ToList();
                totalRecord = d2 != null && d2.Count > 0 ? d2.First().TotalRecordCount : 0;
                _dbContext.Database.Connection.Close();
                return data.ToList();
            }
        }

        public IList<SpecialityProviderModel> GetSpecialityByOrgId(string spName, List<SqlParameter> paraObjects, out int totalRecord)
        {
            using (var cmd = _dbContext.Database.Connection.CreateCommand())
            {
                cmd.CommandText = spName;
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (var item in paraObjects)
                {
                    cmd.Parameters.Add(item);
                }
                _dbContext.Database.Connection.Open();
                var reader = cmd.ExecuteReader();
                var objectContext = ((IObjectContextAdapter)_dbContext).ObjectContext;
                var data = objectContext.Translate<SpecialityProviderModel>(reader).ToList();
                reader.NextResult();
                var d2 = objectContext.Translate<TotalRecords>(reader).ToList();
                totalRecord = d2 != null && d2.Count > 0 ? d2.First().TotalRecordCount : 0;
                _dbContext.Database.Connection.Close();
                return data.ToList();
            }
        }

        public IList<StateLicenseProviderModel> GetStateLicenseByOrgId(string commandText, List<SqlParameter> parameters, out int totalRecord)
        {
            using (var cmd = _dbContext.Database.Connection.CreateCommand())
            {
                cmd.CommandText = commandText;
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (var item in parameters)
                {
                    cmd.Parameters.Add(item);
                }
                _dbContext.Database.Connection.Open();
                var reader = cmd.ExecuteReader();
                var objectContext = ((IObjectContextAdapter)_dbContext).ObjectContext;
                var data = objectContext.Translate<StateLicenseProviderModel>(reader).ToList();
                reader.NextResult();
                var d2 = objectContext.Translate<TotalRecords>(reader).ToList();
                totalRecord = d2 != null && d2.Count > 0 ? d2.First().TotalRecordCount : 0;
                _dbContext.Database.Connection.Close();
                return data.ToList();
            }
        }

        public IList<InsurancePlanProviderModel> GetInsurancePlanByOrgId(string commandText, List<SqlParameter> parameters, out int totalRecord)
        {
            using (var cmd = _dbContext.Database.Connection.CreateCommand())
            {
                cmd.CommandText = commandText;
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (var item in parameters)
                {
                    cmd.Parameters.Add(item);
                }
                _dbContext.Database.Connection.Open();
                var reader = cmd.ExecuteReader();
                var objectContext = ((IObjectContextAdapter)_dbContext).ObjectContext;
                var data = objectContext.Translate<InsurancePlanProviderModel>(reader).ToList();
                reader.NextResult();
                var d2 = objectContext.Translate<TotalRecords>(reader).ToList();
                totalRecord = d2 != null && d2.Count > 0 ? d2.First().TotalRecordCount : 0;
                _dbContext.Database.Connection.Close();
                return data.ToList();
            }
        }

        public IList<SummaryProviderModel> GetSummary(string commandText, List<SqlParameter> parameters)
        {
            try
            {
                if (parameters != null && parameters.Any())
                {
                    for (int i = 0; i <= parameters.Count - 1; i++)
                    {
                        var p = parameters[i] as SqlParameter;
                        if (p == null)
                            throw new Exception("Not support parameter type");

                        commandText += i == 0 ? " " : ", ";

                        commandText += p.ParameterName;
                        if (p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Output)
                        {
                            //output parameter
                            commandText += " output";
                        }
                    }
                }
                var data = _dbContext.Database.SqlQuery<SummaryProviderModel>(commandText, parameters.ToArray<object>()).ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void ExecuteSqlCommandForUpdate(string tableNameToUpdate, string primaryKeyName, long primaryKeyValue, List<SqlParameter> parametersToUpdate)
        {
            try
            {
                var commandText = "Update " + tableNameToUpdate + " Set ";
                if (parametersToUpdate != null && parametersToUpdate.Any())
                {
                    var count = 0;
                    foreach (var para in parametersToUpdate)
                    {
                        if (count != 0)
                            commandText += ",";
                        if (para.SqlDbType == SqlDbType.VarChar || para.SqlDbType == SqlDbType.NVarChar || para.SqlDbType == SqlDbType.DateTime)
                            commandText += para.ParameterName + "=" + "'" + para.Value + "'" + " ";
                        else
                            commandText += para.ParameterName + "=" + para.Value + " ";
                        count++;
                    }
                }

                commandText += "Where " + primaryKeyName + "=" + primaryKeyValue;
                _dbContext.Database.ExecuteSqlCommand(commandText);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void ExecuteSqlCommandForUpdate(string tableNameToUpdate, string primaryKeyName, int primaryKeyValue, List<SqlParameter> parametersToUpdate)
        {
            try
            {
                var commandText = "Update " + tableNameToUpdate + " Set ";
                if (parametersToUpdate != null && parametersToUpdate.Any())
                {
                    var count = 0;
                    foreach (var para in parametersToUpdate)
                    {
                        if (count != 0)
                            commandText += ",";
                        if (para.SqlDbType == SqlDbType.VarChar || para.SqlDbType == SqlDbType.NVarChar || para.SqlDbType == SqlDbType.DateTime)
                            commandText += para.ParameterName + "=" + "'" + para.Value + "'" + " ";
                        else
                            commandText += para.ParameterName + "=" + para.Value + " ";
                        count++;
                    }
                }

                commandText += "Where " + primaryKeyName + "=" + primaryKeyValue;
                _dbContext.Database.ExecuteSqlCommand(commandText);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void ExecuteSqlCommandForInsert(string tableNameToInsertInto, List<SqlParameter> parametersToInsertInto)
        {
            try
            {
                var commandText = "Insert into " + tableNameToInsertInto + " ";
                var fields = "(";
                var values = "(";
                if (parametersToInsertInto != null && parametersToInsertInto.Any())
                {
                    var count = 1;
                    foreach (var para in parametersToInsertInto)
                    {
                        fields += para.ParameterName +  (count != parametersToInsertInto.Count ? ", " : "");
                        if (para.SqlDbType == SqlDbType.VarChar || para.SqlDbType == SqlDbType.NVarChar || para.SqlDbType == SqlDbType.DateTime)
                            values += "'" + para.Value + "'" + (count != parametersToInsertInto.Count ? ", " : "");
                        else
                            values += (para.Value != null ? para.Value : "null") + (count != parametersToInsertInto.Count ? ", " : "");
                        count++;
                    }
                }

                commandText += fields + ") values " + values + ")";
                _dbContext.Database.ExecuteSqlCommand(commandText);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IEnumerable<T> ExecWithStoreProcedure<T>(string query, params object[] parameters)
        {
            return _dbContext.Database.SqlQuery<T>(query, parameters);
        }

        public DbRawSqlQuery<T> SQLQuery<T>(string sql, params object[] parameters)
        {
            return _dbContext.Database.SqlQuery<T>(sql, parameters);
        }

        public Organisation GetByUserId(int userId)
        {
            return _dbContext.Organisations.Where(f => f.UserId == userId).FirstOrDefault();
        }

        //int IFacilityService.GetCount(Expression<Func<Organisation, bool>> predicate, params Expression<Func<Organisation, object>>[] includeProperties)
        //{
        //}

        //Organisation IFacilityService.GetSingle(Expression<Func<Organisation, bool>> predicate, params Expression<Func<Organisation, object>>[] includeProperties)
        //{
        //    throw new NotImplementedException();
        //}

        //void IFacilityService.InsertData(Organisation model)
        //{
        //    throw new NotImplementedException();
        //}

        //void IFacilityService.SaveData()
        //{
        //    throw new NotImplementedException();
        //}

        //void IFacilityService.UpdateData(Organisation model)
        //{
        //    throw new NotImplementedException();
        //}
    }

}
