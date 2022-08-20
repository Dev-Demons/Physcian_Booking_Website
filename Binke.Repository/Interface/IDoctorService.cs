using Binke.Model;
using Binke.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace Binke.Repository.Interface
{
    public interface IDoctorService
    {
        IEnumerable<Doctor> GetAll();
        IEnumerable<Doctor> GetAll(Expression<Func<Doctor, bool>> predicate, params Expression<Func<Doctor, object>>[] includeProperties);
        Doctor GetById(object id);
        int GetCount(Expression<Func<Doctor, bool>> predicate, params Expression<Func<Doctor, object>>[] includeProperties);
        Doctor GetSingle(Expression<Func<Doctor, bool>> predicate, params Expression<Func<Doctor, object>>[] includeProperties);
        void InsertData(Doctor model);
        void UpdateData(Doctor model);
        void DeleteData(Doctor model);
        void SaveData();

        Doctor GetByUserId(int userId);
        DataSet GetQueryResult(string query);
        IList<DoctorSearchList> GetDoctorSearchList(string spName, object[] paraObjects);
        IList<PharmacySearchList> GetPharmacySearchList(string spName, object[] paraObjects);
        IList<DoctorSearchList> GetPharmacySearchListPagination(string spName, List<SqlParameter> paraObjects, out int totalRecord);
        IList<SeniorCareSearchList> GetSeniorCareSearchList(string spName, object[] paraObjects);
        IList<DoctorSearchList> GetSeniorCareSearchListPagination(string spName, List<SqlParameter> paraObjects, out int totalRecord);
        IList<FacilitySearchList> GetFacilitySearchList(string spName, object[] paraObjects);
        IList<DoctorSearchList> GetFacilitySearchListPagination(string spName, List<SqlParameter> paraObjects, out int totalRecord);
        IList<DoctorSearchList> GetDoctorSearchListPagination(string spName, List<SqlParameter> paraObjects, out int totalRecord);


        IList<PharmacySearchList> GetSearchPharmacyListPagination(string spName, List<SqlParameter> paraObjects, out int totalRecord);
        IList<SeniorCareSearchList> GetSearchSeniorCarePagination(string spName, List<SqlParameter> paraObjects, out int totalRecord);
        IList<FacilitySearchList> GetSearchFacilityPagination(string spName, List<SqlParameter> paraObjects, out int totalRecord);
    }
}
