using Binke.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Binke.Repository.Interface
{
    public interface IFeaturedDoctorService
    {
        IEnumerable<FeaturedDoctor> GetAll();
        IEnumerable<FeaturedDoctor> GetAll(Expression<Func<FeaturedDoctor, bool>> predicate, params Expression<Func<FeaturedDoctor, object>>[] includeProperties);
        FeaturedDoctor GetById(object id);
        int GetCount(Expression<Func<FeaturedDoctor, bool>> predicate, params Expression<Func<FeaturedDoctor, object>>[] includeProperties);
        FeaturedDoctor GetSingle(Expression<Func<FeaturedDoctor, bool>> predicate, params Expression<Func<FeaturedDoctor, object>>[] includeProperties);
        void InsertData(FeaturedDoctor model);
        void UpdateData(FeaturedDoctor model);
        void DeleteData(FeaturedDoctor model);
        void SaveData();
    }
}
