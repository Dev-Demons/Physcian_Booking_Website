using Doctyme.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace Doctyme.Repository.Interface
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

        List<T> GetAllList(string spname, string Activity, Pagination pager);
        T GetRecordsById(string spname, string Activity, int id);
         DataTable GetTableById(string spname, string Activity, int? id = 0);
        DataTable GetTable(string spname, string Activity, Model.Pagination pager,int? Id);

        List<T> GetAllById(string spname, string Activity, int? id);
    }
}
