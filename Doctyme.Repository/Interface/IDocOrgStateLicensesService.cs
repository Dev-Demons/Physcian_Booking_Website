using Doctyme.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Doctyme.Repository.Interface
{
    public interface IDocOrgStateLicensesService
    {
        IEnumerable<DocOrgStateLicense> GetAll();
        IEnumerable<DocOrgStateLicense> GetAll(Expression<Func<DocOrgStateLicense, bool>> predicate, params Expression<Func<DocOrgStateLicense, object>>[] includeProperties);
        DocOrgStateLicense GetById(object id);
        int GetCount(Expression<Func<DocOrgStateLicense, bool>> predicate, params Expression<Func<DocOrgStateLicense, object>>[] includeProperties);
        DocOrgStateLicense GetSingle(Expression<Func<DocOrgStateLicense, bool>> predicate, params Expression<Func<DocOrgStateLicense, object>>[] includeProperties);
        void InsertData(DocOrgStateLicense model);
        void UpdateData(DocOrgStateLicense model);
        void DeleteData(DocOrgStateLicense model);
        void SaveData();
    }
}
