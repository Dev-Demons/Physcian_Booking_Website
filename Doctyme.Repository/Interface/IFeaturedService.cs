using Doctyme.Model;
using Doctyme.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace Doctyme.Repository.Interface
{
    public interface IFeaturedService
    {
        IEnumerable<Featured> GetAll();
        IEnumerable<Featured> GetAll(Expression<Func<Featured, bool>> predicate, params Expression<Func<Featured, object>>[] includeProperties);
        Featured GetById(object id);
        int GetCount(Expression<Func<Featured, bool>> predicate, params Expression<Func<Featured, object>>[] includeProperties);
        Featured GetSingle(Expression<Func<Featured, bool>> predicate, params Expression<Func<Featured, object>>[] includeProperties);

        IList<FeaturedDoctorListModel> GetHomePageFeaturedDoctorList(string spName, object[] paraObjects);
        IList<FeaturedFacilityListModel> GetHomePageFeaturedFacilityList(string spName, object[] paraObjects);

        IList<OrganizationProviderModel> GetOrganisationListByTypeId(string spName, List<SqlParameter> paraObjects, out int totalRecord);
        IList<OrganizationProviderModel> GetOrganisationListByTypeId_HomePageSearch(string spName, List<SqlParameter> paraObjects);
        List<OrganisationsWithDistance> GetOrganisationIdsFromSearchText(string search, int organisationTypeId, decimal lat2, decimal long2, int distance, int skip, int take, out int totalRecords);
        List<OrganisationsWithDistance> GetOrganisationIdsFromSearchTextWithZipcode(string search, int organisationTypeId, decimal lat2, decimal long2, int distance, int skip, int take, string locationSearch, out int totalRecords);
        void InsertData(Featured model);
        void UpdateData(Featured model);
        void DeleteData(Featured model);
        void SaveData();
        List<Advertisements> GetAdvertisementsFromSearchText(List<int> organisationTypeId, int usertypeID);//Added by Reena

        IList<FeaturedDoctorListModel> GetHomePageFeaturedDoctorList(string spName, List<SqlParameter> paraObjects);
        IList<FeaturedFacilityListModel> GetHomePageFeaturedFacilityList(string spName, List<SqlParameter> paraObjects);
    }
}
