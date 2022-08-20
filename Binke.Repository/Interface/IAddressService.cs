using Binke.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Binke.Repository.Interface
{
    public interface IAddressService
    {
        IEnumerable<Address> GetAll();
        IEnumerable<Address> GetAll(Expression<Func<Address, bool>> predicate, params Expression<Func<Address, object>>[] includeProperties);
        Address GetById(object id);
        Address GetSingle(Expression<Func<Address, bool>> predicate, params Expression<Func<Address, object>>[] includeProperties);
        int GetCount(Expression<Func<Address, bool>> predicate, params Expression<Func<Address, object>>[] includeProperties);
        void InsertData(Address model);
        void UpdateData(Address model);
        void DeleteData(Address model);
        void SaveData();
    }
}
