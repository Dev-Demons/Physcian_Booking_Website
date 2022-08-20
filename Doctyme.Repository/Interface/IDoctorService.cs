using Doctyme.Model;
using Doctyme.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace Doctyme.Repository.Interface
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
        IList<DoctorSearchList> GetDoctorSearchListPagination(string spName, List<SqlParameter> paraObjects);
        //IList<DoctorSearchList> GetDoctorSearchListForMap(string spName, List<SqlParameter> paraObjects, out int totalRecord);
        IList<DoctorDetails> GetDoctorDetails(string spName, List<SqlParameter> paraObjects);
        IList<DoctorWithDistance> GetDoctorDistanceFromUser(string spName, List<SqlParameter> paraObjects);

        IList<FeaturedDoctors> GetFeaturedDoctorIdsBySearchText(string commandText, List<SqlParameter> parameters);
        IList<FeaturedFacilities> GetFeaturedFacilityIdsBySearchText(string commandText, List<SqlParameter> parameters);
        IList<PharmacySearchList> GetSearchPharmacyListPagination(string spName, List<SqlParameter> paraObjects, out int totalRecord);
        IList<SeniorCareSearchList> GetSearchSeniorCarePagination(string spName, List<SqlParameter> paraObjects, out int totalRecord);
        IList<FacilitySearchList> GetSearchFacilityPagination(string spName, List<SqlParameter> paraObjects, out int totalRecord);
        
        void ExecuteSqlCommandForUpdate(string tableNameToUpdate, string primaryKeyName, int primaryKeyValue, List<SqlParameter> parametersToUpdate);
        List<int> GetAgeGroupReferenceId(List<int> ageGroups);
        List<int> GetAffiliationDoctorId(List<int> affiliations);
        List<int> GetInsuranceDoctorId(List<int> insurances);
        List<int> GetDoctorIdBySpecialityName(string search);
 
        List<DoctorWithDistance> GetDoctorIdByNameOrAddress(string search, bool isNtPcp, bool isANP, bool primaryCare, string distanceSearch, decimal lat2, decimal long2, string searchLocation="");
  
        List<DoctorWithDistance> GetDistance(List<int> doctorIds, decimal lat2, decimal long2,string searchLocation="");
        #region DoctorBoardCetification
        string AddDoctorBoardCertification(DoctorBoardCertificationModel objmodel);
        List<DoctorBoardCertificationModel> GetDoctorBoardCertifications(Pagination pager, int DoctorId);
        DoctorBoardCertificationModel GetDoctorBoardCertificationsById(int Id);
        List<DoctorQualificationViewModel> GetDoctorQualifications(Pagination pager, int DoctorId);
        List<DoctorLanguageViewModel> GetDoctorLanguages(Model.Pagination pager, int DoctorId);
        List<DoctorExperienceViewModel> GetDoctorExperiences(Pagination pager, int DoctorId);
        List<DoctorAffiliationViewModel> GetDoctorAffiliations(Pagination pager, int DoctorId);
        List<DoctorInsurancesViewModel> GetDoctorInsurancess(Pagination pager, int DoctorId);
        List<DoctorViewModel> GetDoctors(Pagination pager);
        List<DoctorListViewModel> GetDoctorsList(Pagination pager);
        DoctorViewModel GetDoctorbyUserId(int Id);
        #endregion

        #region 
        List<BlogItem> GetBlogsSearchResults(string commandText, List<SqlParameter> parameters, out int RecordCount);

        #endregion
        List<int> GetOrganisationIdsFromTypeId(int organisationTypeId, decimal lat2, decimal long2);//Added by ReenaList<OrganisationsWithDistance> 
        List<ProviderAdvertisements> GetAdvertisementsFromSearchText(List<int> organisationId, int userTypeId); //Added by Reena
        List<ProviderAdvertisements> GetDistanceMilebyOrgIds(List<ProviderAdvertisements> data, decimal lat2, decimal long2);//Added by Reena
        Doctor GetDoctorDetailsById(int id);
        double GetDistanceInMile(decimal lat1, decimal long1, decimal lat2, decimal long2);

        List<T> GetDataList<T>(string commandText, List<SqlParameter> parameters);

        DataSet GetDataSetList(string commandText, List<SqlParameter> parameters);

        bool AddOrUpdateExecuteProcedure(string commandText, List<SqlParameter> parameters);
    }
}
