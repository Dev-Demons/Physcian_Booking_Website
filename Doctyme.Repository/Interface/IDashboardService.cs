using Doctyme.Model;
using Doctyme.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace Doctyme.Repository.Interface
{
    public interface IDashboardService
    {
        List<GraphViewModel> GetDoctorDashboardGraphData(string spName,int Id, string fromDate, string endDate, string userFlag);
        List<GraphViewModel> GetDoctorDashboardPatientGraphData(string spName, int Id, string fromDate, string endDate, string userFlag);
        List<GraphViewModel> GetDoctorDashboardGraphDataByUser(string spName, int Id, string fromDate, string endDate, string userFlag);
        List<PatientViewModel> GetRecentlyAddedNewPatientList(string spName, int id, string userFlag);
        List<PatientViewModel> GetNewPatientList(string spName, int id, string userFlag);
        List<AppointmentViewModel> GetRecentlyCompletedAppointmentList(string spName, int id, string userFlag);
        List<AppointmentViewModel> GetTodaysAppointmentList(string spName, int id, string date);

    }
}
