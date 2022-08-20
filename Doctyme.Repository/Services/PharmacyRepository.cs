using Doctyme.Model;
using Doctyme.Model.ViewModels;
using Doctyme.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
namespace Doctyme.Repository.Services
{
    public class PharmacyRepository : GenericRepository<Organisation>, IPharmacyService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public PharmacyRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
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
    }

}
