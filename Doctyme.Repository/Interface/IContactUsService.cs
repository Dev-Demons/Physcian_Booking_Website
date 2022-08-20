using Doctyme.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Doctyme.Repository.Interface
{
    public interface IContactUsService
    {
        IEnumerable<ContactUs> GetAll();
        IEnumerable<ContactUs> GetAll(Expression<Func<ContactUs, bool>> predicate, params Expression<Func<ContactUs, object>>[] includeProperties);
        ContactUs GetById(object id);
        int GetCount(Expression<Func<ContactUs, bool>> predicate, params Expression<Func<ContactUs, object>>[] includeProperties);
        ContactUs GetSingle(Expression<Func<ContactUs, bool>> predicate, params Expression<Func<ContactUs, object>>[] includeProperties);
        void InsertData(ContactUs model);
        void UpdateData(ContactUs model);
        void DeleteData(ContactUs model);
        void SaveData();       
    }
}
