using Doctyme.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Doctyme.Repository.Interface
{
    public interface IFacilityTypeService
    {
        IEnumerable<OrganisationType> GetAll();
        IEnumerable<OrganisationType> GetAll(Expression<Func<OrganisationType, bool>> predicate, params Expression<Func<OrganisationType, object>>[] includeProperties);
        OrganisationType GetById(object id);
        int GetCount(Expression<Func<OrganisationType, bool>> predicate, params Expression<Func<OrganisationType, object>>[] includeProperties);
        OrganisationType GetSingle(Expression<Func<OrganisationType, bool>> predicate, params Expression<Func<OrganisationType, object>>[] includeProperties);
        void InsertData(OrganisationType model);
        void UpdateData(OrganisationType model);
        void DeleteData(OrganisationType model);
        void SaveData();
    }
}
