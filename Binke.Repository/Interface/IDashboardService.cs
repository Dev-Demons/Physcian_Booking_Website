using Binke.Model;
using Binke.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace Binke.Repository.Interface
{
    public interface IDashboardService
    {
        List<GraphViewModel> GetDoctorDashboardGraphData(string spName,int Id, string fromDate, string endDate, string userFlag);
        List<PatientViewModel> GetRecentlyAddedNewPatientList(string spName, int id);
        List<PatientViewModel> GetNewPatientList(string spName, int id, string userFlag);
        List<AppointmentViewModel> GetRecentlyCompletedAppointmentList(string spName, int id, string userFlag);
        List<AppointmentViewModel> GetTodaysAppointmentList(string spName, int id, string date);

    }
}
