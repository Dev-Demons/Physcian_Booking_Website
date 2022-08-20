using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Doctyme.Model;
using Doctyme.Model.ViewModels;
using Doctyme.Repository.Interface;

namespace Doctyme.Repository.Services
{
    public class DashboardRepository : GenericRepository<Patient>, IDashboardService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public DashboardRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        #region Public Method
        /// <summary>
        /// Get Doctor Dashboard Graph Data
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="id"></param>
        /// <param name="fromDate"></param>
        /// <param name="endDate"></param>
        /// <param name="userFlag"></param>
        /// <returns></returns>
        public List<GraphViewModel> GetDoctorDashboardGraphData(string spName,int id,string fromDate, string endDate,string userFlag)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
            parameters.Add(new SqlParameter("@FromDate", SqlDbType.DateTime) { Value = Convert.ToDateTime(fromDate) });
            parameters.Add(new SqlParameter("@ToDate", SqlDbType.DateTime) { Value = Convert.ToDateTime(endDate) });
            parameters.Add(new SqlParameter("@UserFlag", SqlDbType.VarChar) { Value = userFlag });
            List<GraphViewModel> result = new List<GraphViewModel>();//  _dbContext.Database.SqlQuery<GraphViewModel>(spName, parameters.ToArray<object>()).ToList();
            return result;
        }
        public List<GraphViewModel> GetDoctorDashboardPatientGraphData(string spName, int id, string fromDate, string endDate, string userFlag)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@ReferenceId", SqlDbType.Int) { Value = id });
            parameters.Add(new SqlParameter("@FromDate", SqlDbType.DateTime)
            {
                Value =
                DateTime.ParseExact(fromDate, new string[] { "MM.dd.yyyy", "MM-dd-yyyy", "MM/dd/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None)
            });
            //Convert.ToDateTime(fromDate) });
            parameters.Add(new SqlParameter("@ToDate", SqlDbType.DateTime)
            {
                Value =
                DateTime.ParseExact(endDate, new string[] { "MM.dd.yyyy", "MM-dd-yyyy", "MM/dd/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None)
            });
                //Convert.ToDateTime(endDate) });
            parameters.Add(new SqlParameter("@UserTypeId", SqlDbType.VarChar) { Value = userFlag });
            List<GraphViewModel> result =_dbContext.Database.SqlQuery<GraphViewModel>(spName, parameters.ToArray<object>()).ToList();
            return result;
        }
        public List<GraphViewModel> GetDoctorDashboardGraphDataByUser(string spName, int id, string fromDate, string endDate, string userFlag)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@ReferenceId", SqlDbType.Int) { Value = id });
            parameters.Add(new SqlParameter("@FromDate", SqlDbType.DateTime)
            {
                Value =
                DateTime.ParseExact(fromDate, new string[] { "MM.dd.yyyy", "MM-dd-yyyy", "MM/dd/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None)
            });
            //Convert.ToDateTime(fromDate) });
            parameters.Add(new SqlParameter("@ToDate", SqlDbType.DateTime)
            {
                Value =
                DateTime.ParseExact(endDate, new string[] { "MM.dd.yyyy", "MM-dd-yyyy", "MM/dd/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None)
            });
            //Convert.ToDateTime(endDate) });
            parameters.Add(new SqlParameter("@UserTypeId", SqlDbType.VarChar) { Value = userFlag });
            List<GraphViewModel> result =  _dbContext.Database.SqlQuery<GraphViewModel>(spName, parameters.ToArray<object>()).ToList();
            return result;
        }
        /// <summary>
        /// Get Recently Added New Patient List
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<PatientViewModel> GetRecentlyAddedNewPatientList(string spName, int id, string userFlag)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
            parameters.Add(new SqlParameter("@UserFlag", SqlDbType.VarChar) { Value = userFlag });
            List<PatientViewModel> result = _dbContext.Database.SqlQuery<PatientViewModel>(spName, parameters.ToArray<object>()).ToList();
            return result;
        }
        /// <summary>
        /// Get New Patient List
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="id"></param>
        /// <param name="userFlag"></param>
        /// <returns></returns>
        public List<PatientViewModel> GetNewPatientList(string spName, int id,string userFlag)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
            parameters.Add(new SqlParameter("@UserFlag", SqlDbType.VarChar) { Value = userFlag });
            List<PatientViewModel> result = _dbContext.Database.SqlQuery<PatientViewModel>(spName, parameters.ToArray<object>()).ToList();
            return result;
        }
        /// <summary>
        /// Get Recently Completed Appointment List
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="id"></param>
        /// <param name="userFlag"></param>
        /// <returns></returns>
        public List<AppointmentViewModel> GetRecentlyCompletedAppointmentList(string spName, int id, string userFlag)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
            parameters.Add(new SqlParameter("@UserFlag", SqlDbType.VarChar) { Value = userFlag });
            List<AppointmentViewModel> result = _dbContext.Database.SqlQuery<AppointmentViewModel>(spName, parameters.ToArray<object>()).ToList();
            return result;
        }
        /// <summary>
        /// Get Todays Appointment List
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="id"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<AppointmentViewModel> GetTodaysAppointmentList(string spName, int id, string user)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
            parameters.Add(new SqlParameter("@UserFlag", SqlDbType.VarChar) { Value = user });
            List<AppointmentViewModel> result = _dbContext.Database.SqlQuery<AppointmentViewModel>(spName, parameters.ToArray<object>()).ToList();
            return result;
        }

        #endregion
    }

}
