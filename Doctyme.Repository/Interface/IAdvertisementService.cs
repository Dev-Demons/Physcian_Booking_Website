using Doctyme.Model;
using Doctyme.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace Doctyme.Repository.Interface
{
    public interface IAdvertisementService
    {
        IEnumerable<Advertisement> GetAll();
        IEnumerable<Advertisement> GetAll(Expression<Func<Advertisement, bool>> predicate, params Expression<Func<Advertisement, object>>[] includeProperties);
        Advertisement GetById(object id);
        int GetCount(Expression<Func<Advertisement, bool>> predicate, params Expression<Func<Advertisement, object>>[] includeProperties);
        Advertisement GetSingle(Expression<Func<Advertisement, bool>> predicate, params Expression<Func<Advertisement, object>>[] includeProperties);

        IList<DrpAdvertisementLocationModel> GetDrpAdvertisementLocationList(string spName, object[] paraObjects);

        void InsertData(Advertisement model);
        void UpdateData(Advertisement model);
        void DeleteData(Advertisement model);
        void SaveData();
        IList<Advertisements> GetAdvertisementList(decimal lat2, decimal long2);//Added by Reena
        IEnumerable<T> ExecWithStoreProcedure<T>(string query, params object[] parameters);
        double GetDistanceInMile(decimal lat1, decimal long1, decimal lat2, decimal long2);//Added by Reena
    }
}
