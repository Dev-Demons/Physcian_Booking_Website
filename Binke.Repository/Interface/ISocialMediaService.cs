using Binke.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Binke.Repository.Interface
{
    public interface ISocialMediaService
    {
        IEnumerable<SocialMedia> GetAll();
        IEnumerable<SocialMedia> GetAll(Expression<Func<SocialMedia, bool>> predicate, params Expression<Func<SocialMedia, object>>[] includeProperties);
        SocialMedia GetById(object id);
        int GetCount(Expression<Func<SocialMedia, bool>> predicate, params Expression<Func<SocialMedia, object>>[] includeProperties);
        SocialMedia GetSingle(Expression<Func<SocialMedia, bool>> predicate, params Expression<Func<SocialMedia, object>>[] includeProperties);
        void InsertData(SocialMedia model);
        void UpdateData(SocialMedia model);
        void DeleteData(SocialMedia model);
        void SaveData();
    }

}
