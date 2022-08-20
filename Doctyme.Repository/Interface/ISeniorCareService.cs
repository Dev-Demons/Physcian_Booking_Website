using Doctyme.Model;
using Doctyme.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace Doctyme.Repository.Interface
{
    public interface ISeniorCareService
    {
        IEnumerable<Organisation> GetAll();
        IEnumerable<Organisation> GetAll(Expression<Func<Organisation, bool>> predicate, params Expression<Func<Organisation, object>>[] includeProperties);
        Organisation GetById(object id);
        int GetCount(Expression<Func<Organisation, bool>> predicate, params Expression<Func<Organisation, object>>[] includeProperties);
        Organisation GetSingle(Expression<Func<Organisation, bool>> predicate, params Expression<Func<Organisation, object>>[] includeProperties);
        void InsertData(Organisation model);
        void InsertData(IEnumerable<Organisation> model);
        void UpdateData(Organisation model);
        void UpdateData(IEnumerable<Organisation> model);
        void DeleteData(Organisation model);
        void SaveData();
        void UpdateSeniorCareProfile(string commandText, Organisation parameters);        
        IList<SeniorCareData> GetSeniorCareListByTypeId(string commandText, List<SqlParameter> parameters, out int totalRecord);
        IList<OrganizationAmenityOptionViewModel> GetOrganizationAmenityOptionByOrganization(string commandText, List<SqlParameter> parameters, out int totalRecord);
        IList<SiteImageViewModel> GetSiteImageByOrganization(string commandText, List<SqlParameter> parameters, out int totalRecord);
    }

    //public interface ISeniorCareImageService
    //{
    //    IEnumerable<SeniorCareImage> GetAll();
    //    IEnumerable<SeniorCareImage> GetAll(Expression<Func<SeniorCareImage, bool>> predicate, params Expression<Func<SeniorCareImage, object>>[] includeProperties);
    //    SeniorCareImage GetById(object id);
    //    int GetCount(Expression<Func<SeniorCareImage, bool>> predicate, params Expression<Func<SeniorCareImage, object>>[] includeProperties);
    //    SeniorCareImage GetSingle(Expression<Func<SeniorCareImage, bool>> predicate, params Expression<Func<SeniorCareImage, object>>[] includeProperties);
    //    void InsertData(SeniorCareImage model);
    //    void InsertData(IEnumerable<SeniorCareImage> model);
    //    void UpdateData(SeniorCareImage model);
    //    void UpdateData(IEnumerable<SeniorCareImage> model);
    //    void DeleteData(SeniorCareImage model);
    //    void SaveData();
    //}

}
