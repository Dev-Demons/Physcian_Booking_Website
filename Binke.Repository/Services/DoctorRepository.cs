using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Binke.Model;
using Binke.Model.DBContext;
using Binke.Model.Utility;
using Binke.Model.ViewModels;
using Binke.Repository.Interface;

namespace Binke.Repository.Services
{
    public class DoctorRepository : GenericRepository<Doctor>, IDoctorService
    {
        private readonly BinkeDbContext _dbContext;

        public DoctorRepository(BinkeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        #region Extra
        public Doctor GetByUserId(int userId)
        {
            return _dbContext.Doctors.Where(d => d.UserId == userId).FirstOrDefault();
        }

        public DataSet GetQueryResult(string query)
        {
            return _dbContext.GetQueryAsDatatable(query);
        }

        public IList<DoctorSearchList> GetDoctorSearchList(string spName, object[] paraObjects)
        {
            return _dbContext.Database.SqlQuery<DoctorSearchList>(spName, paraObjects).ToList();
        }

        public IList<DoctorSearchList> GetDoctorSearchListPagination(string commandText, List<SqlParameter> parameters, out int totalRecord)
        {
            totalRecord = 0;
            var pTotalRecords = new SqlParameter("@TotalRecords", SqlDbType.Int) { Direction = ParameterDirection.Output };
            parameters.Add(pTotalRecords);
            commandText = "SearchDoctorListPagination";
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


            var data = _dbContext.Database.SqlQuery<DoctorSearchList>(commandText, parameters.ToArray<object>()).ToList();
            totalRecord = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
            return data;
        }


        public IList<PharmacySearchList> GetSearchPharmacyListPagination(string commandText, List<SqlParameter> parameters, out int totalRecord)
        {
            totalRecord = 0;
            var pTotalRecords = new SqlParameter("@TotalRecords", SqlDbType.Int) { Direction = ParameterDirection.Output };
            parameters.Add(pTotalRecords);
            commandText = "SearchPharmacyListPagination";
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


            var data = _dbContext.Database.SqlQuery<PharmacySearchList>(commandText, parameters.ToArray<object>()).ToList();
            totalRecord = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
            return data;
        }


        public IList<SeniorCareSearchList> GetSearchSeniorCarePagination(string commandText, List<SqlParameter> parameters, out int totalRecord)
        {
            totalRecord = 0;
            var pTotalRecords = new SqlParameter("@TotalRecords", SqlDbType.Int) { Direction = ParameterDirection.Output };
            parameters.Add(pTotalRecords);
            commandText = "SearchSeniorCarePagination";
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


            var data = _dbContext.Database.SqlQuery<SeniorCareSearchList>(commandText, parameters.ToArray<object>()).ToList();
            totalRecord = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
            return data;
        }


        public IList<FacilitySearchList> GetSearchFacilityPagination(string commandText, List<SqlParameter> parameters, out int totalRecord)
        {
            totalRecord = 0;
            var pTotalRecords = new SqlParameter("@TotalRecords", SqlDbType.Int) { Direction = ParameterDirection.Output };
            parameters.Add(pTotalRecords);
            commandText = "SearchFacilityPagination";
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


            var data = _dbContext.Database.SqlQuery<FacilitySearchList>(commandText, parameters.ToArray<object>()).ToList();
            totalRecord = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
            return data;
        }


        public IList<PharmacySearchList> GetPharmacySearchList(string spName, object[] paraObjects)
        {
            return _dbContext.Database.SqlQuery<PharmacySearchList>(spName, paraObjects).ToList();
        }

        public IList<SeniorCareSearchList> GetSeniorCareSearchList(string spName, object[] paraObjects)
        {
            return _dbContext.Database.SqlQuery<SeniorCareSearchList>(spName, paraObjects).ToList();
        }

        public IList<FacilitySearchList> GetFacilitySearchList(string spName, object[] paraObjects)
        {
            return _dbContext.Database.SqlQuery<FacilitySearchList>(spName, paraObjects).ToList();
        }

        public IList<DoctorSearchList> GetPharmacySearchListPagination(string spName, List<SqlParameter> paraObjects, out int totalRecord)
        {
            throw new NotImplementedException();
        }

        public IList<DoctorSearchList> GetSeniorCareSearchListPagination(string spName, List<SqlParameter> paraObjects, out int totalRecord)
        {
            throw new NotImplementedException();
        }

        public IList<DoctorSearchList> GetFacilitySearchListPagination(string spName, List<SqlParameter> paraObjects, out int totalRecord)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

}
