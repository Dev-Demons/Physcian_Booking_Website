using Doctyme.Model;
using Doctyme.Repository.Interface;
using Doctyme.Repository.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;

namespace Doctyme.Repository.Services
{
    public class PatientRepository : GenericRepository<Patient>, IPatientService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public PatientRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<T> ExecWithStoreProcedure<T>(string query, params object[] parameters)
        {
            return _dbContext.Database.SqlQuery<T>(query, parameters);
        }

        public DbRawSqlQuery<T> SQLQuery<T>(string sql, params object[] parameters)
        {
            return _dbContext.Database.SqlQuery<T>(sql, parameters);
        }

        public List<T> GetDataList<T>(string commandText, List<SqlParameter> parameters)
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
                }
            }

            var data = _dbContext.Database.SqlQuery<T>(commandText, parameters.ToArray<object>()).ToList();
            return data;
        }
        public int ExecuteSQLQuery(string query, params object[] parameters)
        {
            return _dbContext.Database.ExecuteSqlCommand("EXEC " + query, parameters);
        }
        //Added by venkat

    }

}
