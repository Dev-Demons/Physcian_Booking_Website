using Doctyme.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Doctyme.Repository.Interface
{
    public interface IDoctorImageService
    {
        IEnumerable<SiteImage> GetAll();
        IEnumerable<SiteImage> GetAll(Expression<Func<SiteImage, bool>> predicate, params Expression<Func<SiteImage, object>>[] includeProperties);
        SiteImage GetById(object id);
        int GetCount(Expression<Func<SiteImage, bool>> predicate, params Expression<Func<SiteImage, object>>[] includeProperties);
        SiteImage GetSingle(Expression<Func<SiteImage, bool>> predicate, params Expression<Func<SiteImage, object>>[] includeProperties);
        void InsertData(SiteImage model);
        void InsertData(IEnumerable<SiteImage> model);
        void UpdateData(SiteImage model);
        void DeleteData(SiteImage model);
        void SaveData();
    }
}
