using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Binke.Repository.Interface
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        T GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        T GetById(object id);
        int GetCount(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        void InsertData(T obj);
        void UpdateData(T obj);
        void UpdateData(IEnumerable<T> obj);
        void DeleteData(T obj);
        void DeleteData(IEnumerable<T> obj);
       
        void SaveData();
    }
}
