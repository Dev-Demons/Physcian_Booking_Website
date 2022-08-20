using Doctyme.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Doctyme.Repository.Interface
{
    public interface IFeaturedSpecialityService
    {
        IEnumerable<FeaturedSpeciality> GetAll();
        IEnumerable<FeaturedSpeciality> GetAll(Expression<Func<FeaturedSpeciality, bool>> predicate, params Expression<Func<FeaturedSpeciality, object>>[] includeProperties);
        FeaturedSpeciality GetById(object id);
        int GetCount(Expression<Func<FeaturedSpeciality, bool>> predicate, params Expression<Func<FeaturedSpeciality, object>>[] includeProperties);
        FeaturedSpeciality GetSingle(Expression<Func<FeaturedSpeciality, bool>> predicate, params Expression<Func<FeaturedSpeciality, object>>[] includeProperties);
        void InsertData(FeaturedSpeciality model);
        void UpdateData(FeaturedSpeciality model);
        void DeleteData(FeaturedSpeciality model);
        void SaveData();
    }
}
