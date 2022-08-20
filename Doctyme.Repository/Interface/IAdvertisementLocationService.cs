using Doctyme.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Doctyme.Repository.Interface
{
    public interface IAdvertisementLocationService
    {
        IEnumerable<AdvertisementLocation> GetAll();
        IEnumerable<AdvertisementLocation> GetAll(Expression<Func<AdvertisementLocation, bool>> predicate, params Expression<Func<AdvertisementLocation, object>>[] includeProperties);
        AdvertisementLocation GetById(object id);
        int GetCount(Expression<Func<AdvertisementLocation, bool>> predicate, params Expression<Func<AdvertisementLocation, object>>[] includeProperties);
        AdvertisementLocation GetSingle(Expression<Func<AdvertisementLocation, bool>> predicate, params Expression<Func<AdvertisementLocation, object>>[] includeProperties);
        void InsertData(AdvertisementLocation model);
        void UpdateData(AdvertisementLocation model);
        void DeleteData(AdvertisementLocation model);
        void SaveData();
    }
}
