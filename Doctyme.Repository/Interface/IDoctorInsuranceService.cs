using Doctyme.Model;
using Doctyme.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Doctyme.Repository.Interface
{
    public interface IDoctorInsuranceService
    {
        IEnumerable<DocOrgInsurances> GetAll();
        IEnumerable<DocOrgInsurances> GetAll(Expression<Func<DocOrgInsurances, bool>> predicate, params Expression<Func<DocOrgInsurances, object>>[] includeProperties);
        DocOrgInsurances GetById(object id);
        int GetCount(Expression<Func<DocOrgInsurances, bool>> predicate, params Expression<Func<DocOrgInsurances, object>>[] includeProperties);
        DocOrgInsurances GetSingle(Expression<Func<DocOrgInsurances, bool>> predicate, params Expression<Func<DocOrgInsurances, object>>[] includeProperties);

        IList<DrpKeyValueModel> GetDrpInsuranceList(string spName, object[] paraObjects);

        void InsertData(DocOrgInsurances model);
        void UpdateData(DocOrgInsurances model);
        void DeleteData(DocOrgInsurances model);
        void SaveData();
    }
}
