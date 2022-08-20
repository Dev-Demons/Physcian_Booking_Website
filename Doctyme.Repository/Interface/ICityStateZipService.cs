using Doctyme.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Doctyme.Repository.Interface
{
    public interface ICityStateZipService
    {
        IEnumerable<CityStateZip> GetAll();
        IEnumerable<CityStateZip> GetAll(Expression<Func<CityStateZip, bool>> predicate, params Expression<Func<CityStateZip, object>>[] includeProperties);
        CityStateZip GetById(object id);
        int GetCount(Expression<Func<CityStateZip, bool>> predicate, params Expression<Func<CityStateZip, object>>[] includeProperties);
        CityStateZip GetSingle(Expression<Func<CityStateZip, bool>> predicate, params Expression<Func<CityStateZip, object>>[] includeProperties);
        void InsertData(CityStateZip model);
        void UpdateData(CityStateZip model);
        void DeleteData(CityStateZip model);
        void SaveData();

       
    }
}
