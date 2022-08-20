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
    public class DashboardRepository : GenericRepository<Patient>, IDashboardService
    {
        private readonly BinkeDbContext _dbContext;

        public DashboardRepository(BinkeDbContext dbContext) : base(dbContext)
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
            List<GraphViewModel> result = _dbContext.Database.SqlQuery<GraphViewModel>(spName, parameters.ToArray<object>()).ToList();
            return result;
        }
        /// <summary>
        /// Get Recently Added New Patient List
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<PatientViewModel> GetRecentlyAddedNewPatientList(string spName, int id)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
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
        public List<AppointmentViewModel> GetTodaysAppointmentList(string spName, int id, string date)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
            parameters.Add(new SqlParameter("@UserFlag", SqlDbType.DateTime) { Value = date });
            List<AppointmentViewModel> result = _dbContext.Database.SqlQuery<AppointmentViewModel>(spName, parameters.ToArray<object>()).ToList();
            return result;
        }

        #endregion
    }

}
