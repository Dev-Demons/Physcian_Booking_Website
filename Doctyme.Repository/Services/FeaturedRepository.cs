using Doctyme.Model;
using Doctyme.Model.ViewModels;
using Doctyme.Repository.Interface;
using Doctyme.Repository.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Ajax.Utilities;

namespace Doctyme.Repository
{
    public class FeaturedRepository : GenericRepository<Featured>, IFeaturedService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public FeaturedRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IList<FeaturedDoctorListModel> GetHomePageFeaturedDoctorList(string spName, object[] paraObjects)
        {
            return _dbContext.Database.SqlQuery<FeaturedDoctorListModel>(spName, paraObjects).ToList();
        }

        public IList<FeaturedFacilityListModel> GetHomePageFeaturedFacilityList(string spName, object[] paraObjects)
        {
            return _dbContext.Database.SqlQuery<FeaturedFacilityListModel>(spName, paraObjects).ToList();
        }


        public IList<FeaturedDoctorListModel> GetHomePageFeaturedDoctorList(string spName, List<SqlParameter> parameters)
        {
           // _dbContext.Database.SqlQuery<FeaturedDoctorListModel>(spName, paraObjects).ToList();

          
           
            if (parameters != null && parameters.Any())
            {
                for (int i = 0; i <= parameters.Count - 1; i++)
                {
                    var p = parameters[i] as SqlParameter;
                    if (p == null)
                        throw new Exception("Not support parameter type");

                    spName += i == 0 ? " " : ", ";

                    spName += p.ParameterName;                   
                }
            }
           
             return _dbContext.Database.SqlQuery<FeaturedDoctorListModel>(spName, parameters.ToArray<object>()).ToList();
            
           

        }

        public IList<FeaturedFacilityListModel> GetHomePageFeaturedFacilityList(string spName, List<SqlParameter> parameters)
        {
           // return _dbContext.Database.SqlQuery<FeaturedFacilityListModel>(spName, paraObjects).ToList();

            if (parameters != null && parameters.Any())
            {
                for (int i = 0; i <= parameters.Count - 1; i++)
                {
                    var p = parameters[i] as SqlParameter;
                    if (p == null)
                        throw new Exception("Not support parameter type");

                    spName += i == 0 ? " " : ", ";

                    spName += p.ParameterName;
                }
            }
           
            return _dbContext.Database.SqlQuery<FeaturedFacilityListModel>(spName, parameters.ToArray<object>()).ToList();
        }


        public IList<OrganizationProviderModel> GetOrganisationListByTypeId(string commandText, List<SqlParameter> parameters, out int totalRecord)
        {
            totalRecord = 0;
            var pTotalRecords = new SqlParameter("@TotalRecords", SqlDbType.Int) { Direction = ParameterDirection.Output };
            parameters.Add(pTotalRecords);
            if (parameters != null && parameters.Any())
            {
                for (int i = 0; i <= parameters.Count - 1; i++)
                {
                    var p = parameters[i] as SqlParameter;
                    if (p == null)
                        throw new Exception("Not support parameter type");

                    commandText += i == 0 ? " " : ", ";

                    commandText += p.ParameterName;
                    if (p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Output)
                    {
                        //output parameter
                        commandText += " output";
                    }
                }
            }

            var data = _dbContext.Database.SqlQuery<OrganizationProviderModel>(commandText, parameters.ToArray<object>()).ToList();
            totalRecord = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
            return data;
        }

        public IList<OrganizationProviderModel> GetOrganisationListByTypeId_HomePageSearch(string commandText, List<SqlParameter> parameters)
        {
            if (parameters != null && parameters.Any())
            {
                for (int i = 0; i <= parameters.Count - 1; i++)
                {
                    var p = parameters[i] as SqlParameter;
                    if (p == null)
                        throw new Exception("Not support parameter type");

                    commandText += i == 0 ? " " : ", ";

                    commandText += p.ParameterName;
                    if (p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Output)
                    {
                        //output parameter
                        commandText += " output";
                    }
                }
            }

            var data = _dbContext.Database.SqlQuery<OrganizationProviderModel>(commandText, parameters.ToArray<object>()).ToList();
            return data;
        }

        public List<OrganisationsWithDistance> GetOrganisationIdsFromSearchText(string search, int organisationTypeId, decimal lat2, decimal long2, int distance, int skip, int take, out int totalRecords)
        {
            var data = (from o in _dbContext.Organisations
                        join ad in _dbContext.Addresses on o.OrganisationId equals ad.ReferenceId
                        join csz in _dbContext.CityStateZips on ad.CityStateZipCodeID equals csz.CityStateZipCodeID
                        where o.OrganizationTypeID == organisationTypeId && ad.UserTypeID == o.UserTypeID && ad.IsActive && !ad.IsDeleted && o.IsActive && !o.IsDeleted &&
                        (string.IsNullOrEmpty(search) || o.OrganisationName.Contains(search) ||
                        ad.Address1.Contains(search) || ad.Address2.Contains(search) || csz.City.Contains(search) || csz.State.Contains(search) ||
                        csz.Country.Contains(search) || csz.ZipCode.Contains(search))
                        select new OrganisationsWithDistance()
                        {
                            OrganisationId = o.OrganisationId,
                            Lat = csz.Lat ?? 0,
                            Long = csz.Long ?? 0
                        }).ToList();

            data = data.DistinctBy(m => m.OrganisationId).ToList();

            foreach (var item in data)
            {
                item.DistanceCount = this.GetDistanceInMile(item.Lat, item.Long, lat2, long2);
            }
            if (distance > 0)
            {
                data = data.Where(m => m.DistanceCount <= distance).ToList();
            }

            totalRecords = data.Count;
            return data.OrderBy(m => m.DistanceCount).Skip(skip).Take(take).ToList();
        }

        public List<OrganisationsWithDistance> GetOrganisationIdsFromSearchTextWithZipcode(string search, int organisationTypeId, decimal lat2, decimal long2, int distance, int skip, int take,string locationSearch ,out int totalRecords)
        {
            string city = "";
            string state = "";
            string zip = "";
            if (!string.IsNullOrEmpty(locationSearch))
            {
                var location = locationSearch.Split('|');
                city = location[0];
                state = location[1];
                zip = location[2];
            }
            var data = (from o in _dbContext.Organisations
                        join ad in _dbContext.Addresses on o.OrganisationId equals ad.ReferenceId
                        join csz in _dbContext.CityStateZips on ad.CityStateZipCodeID equals csz.CityStateZipCodeID
                        where o.OrganizationTypeID == organisationTypeId && ad.UserTypeID == o.UserTypeID && ad.IsActive && !ad.IsDeleted && o.IsActive && !o.IsDeleted &&
                         (string.IsNullOrEmpty(search) || o.OrganisationName.StartsWith(search) ||
                        ad.Address1.StartsWith(search) || ad.Address2.StartsWith(search))
                        && csz.City==city  && csz.State==state  && csz.ZipCode== zip 
                        select new OrganisationsWithDistance()
                        {
                            OrganisationId = o.OrganisationId,
                            Lat = csz.Lat ?? 0,
                            Long = csz.Long ?? 0
                        }).ToList();

            data = data.DistinctBy(m => m.OrganisationId).ToList();

            foreach (var item in data)
            {
                item.DistanceCount = this.GetDistanceInMile(item.Lat, item.Long, lat2, long2);
            }
            if (distance > 0)
            {
                data = data.Where(m => m.DistanceCount <= distance).ToList();
            }

            totalRecords = data.Count;
            return data.OrderBy(m => m.DistanceCount).Skip(skip).Take(take).ToList();
        }

        public double GetDistanceInMile(decimal lat1, decimal long1, decimal lat2, decimal long2)
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

        public List<Advertisements> GetAdvertisementsFromSearchText(List<int> organisationId, int usertypeID) //Added by Reena
        {            
            var data = (from A in _dbContext.Advertisements
                        join PT in _dbContext.PaymentTypes on A.PaymentTypeId equals PT.PaymentTypeId
                        join O in _dbContext.Organisations on A.ReferenceId equals O.OrganisationId
                        where organisationId.Contains(O.OrganisationId) && 
                        A.IsActive == true && A.IsDeleted == false && PT.IsActive && !PT.IsDeleted && A.AdvertisementTypeId == 1 
                        && O.OrganizationTypeID == A.UserTypeId && O.OrganizationTypeID == usertypeID
                        select new Advertisements()
                        {
                            AdvertisementId = A.AdvertisementId,
                            ReferenceId = A.ReferenceId ?? 0,
                            PaymentTypeId = A.PaymentTypeId ?? 0,
                            Title = A.Title,
                            ImagePath = A.ImagePath,
                            PaymentTypeName = PT.Name,
                            PaymentTypeDescription = PT.Description
                        }).ToList();            
            return data;
        }        
    }
}
