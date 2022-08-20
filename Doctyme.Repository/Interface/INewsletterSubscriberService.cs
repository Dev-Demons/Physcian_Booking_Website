using Doctyme.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Doctyme.Repository.Interface
{
    public interface INewsletterSubscriberService
    {
        IEnumerable<NewsletterSubscriber> GetAll();
        IEnumerable<NewsletterSubscriber> GetAll(Expression<Func<NewsletterSubscriber, bool>> predicate, params Expression<Func<NewsletterSubscriber, object>>[] includeProperties);
        NewsletterSubscriber GetById(object id);
        int GetCount(Expression<Func<NewsletterSubscriber, bool>> predicate, params Expression<Func<NewsletterSubscriber, object>>[] includeProperties);
        NewsletterSubscriber GetSingle(Expression<Func<NewsletterSubscriber, bool>> predicate, params Expression<Func<NewsletterSubscriber, object>>[] includeProperties);
        void InsertData(NewsletterSubscriber model);
        void UpdateData(NewsletterSubscriber model);
        void DeleteData(NewsletterSubscriber model);
        void SaveData();       
    }
}
