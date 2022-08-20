using Doctyme.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Doctyme.Repository.Interface
{
    public interface IFacilityOptionService
    {
        IEnumerable<OrganizationAmenityOption> GetAll();
        IEnumerable<OrganizationAmenityOption> GetAll(Expression<Func<OrganizationAmenityOption, bool>> predicate, params Expression<Func<OrganizationAmenityOption, object>>[] includeProperties);
        OrganizationAmenityOption GetById(object id);
        int GetCount(Expression<Func<OrganizationAmenityOption, bool>> predicate, params Expression<Func<OrganizationAmenityOption, object>>[] includeProperties);
        OrganizationAmenityOption GetSingle(Expression<Func<OrganizationAmenityOption, bool>> predicate, params Expression<Func<OrganizationAmenityOption, object>>[] includeProperties);
        void InsertData(OrganizationAmenityOption model);
        void UpdateData(OrganizationAmenityOption model);
        void DeleteData(OrganizationAmenityOption model);
        void SaveData();
    }
}
