using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Doctyme.Model;
using Doctyme.Model.ViewModels;
using Doctyme.Repository.Interface;

namespace Doctyme.Repository.Services
{
    public class AdvertisementRepository : GenericRepository<Advertisement>, IAdvertisementService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public AdvertisementRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IList<DrpAdvertisementLocationModel> GetDrpAdvertisementLocationList(string spName, object[] paraObjects)
        {
            return _dbContext.Database.SqlQuery<DrpAdvertisementLocationModel>(spName, paraObjects).ToList();
        }

        public IList<Advertisements> GetAdvertisementList(decimal lat2, decimal long2)//Added by Reena
        {
            var list = (from A in _dbContext.Advertisements
                        join PT in _dbContext.PaymentTypes on A.PaymentTypeId equals PT.PaymentTypeId
                        join O in _dbContext.Organisations on A.ReferenceId equals O.OrganisationId
                        join ad in _dbContext.Addresses on O.OrganisationId equals ad.ReferenceId
                        join csz in _dbContext.CityStateZips on ad.CityStateZipCodeID equals csz.CityStateZipCodeID
                        where
                         A.IsActive == true && A.IsDeleted == false && PT.IsActive == true && PT.IsDeleted == false
                         && A.AdvertisementTypeId == 1 //featured Ads
                         select new Advertisements()
                         {
                             AdvertisementId = A.AdvertisementId,
                             ReferenceId = A.ReferenceId??0,
                             PaymentTypeId = A.PaymentTypeId??0,
                             Title = A.Title,
                             ImagePath = A.ImagePath,
                             PaymentTypeName = PT.Name,
                             PaymentTypeDescription = PT.Description,
                             Lat = csz.Lat ?? 0,
                             Long = csz.Long ?? 0
                         }).ToList();
            foreach (var item in list)
            {
                item.DistanceCount = this.GetDistanceInMile(item.Lat, item.Long, lat2, long2);
            }            
            return list;
        }

        public double GetDistanceInMile(decimal lat1, decimal long1, decimal lat2, decimal long2)//Added by Reena
        {
            if (lat1 == 0 || long1 == 0 || lat2 == 0 || long2 == 0)
                return 0;
            double degToRoad = 57.29577951;
            var ans = Math.Sin(Decimal.ToDouble(lat1) / degToRoad) * Math.Sin(Decimal.ToDouble(lat2) / degToRoad) +
                Math.Cos(Decimal.ToDouble(lat1) / degToRoad) * Math.Cos(Decimal.ToDouble(lat2) / degToRoad) *
                Math.Cos(Math.Abs(Decimal.ToDouble(long2 - long1) / degToRoad));
            var miles = 3959 * Math.Atan(Math.Sqrt(1 - Math.Pow(ans, 2)) / ans);
            miles = Math.Ceiling(Math.Abs(miles));
            return miles;
        }

        public IEnumerable<T> ExecWithStoreProcedure<T>(string query, params object[] parameters)
        {
            return _dbContext.Database.SqlQuery<T>(query, parameters);
        }
    }
}
