using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using Binke.Api.Models;
using Doctyme.Model;
using Doctyme.Model.ViewModels;
using Doctyme.Repository.Interface;
using Microsoft.Ajax.Utilities;

namespace Doctyme.Repository.Services
{
    public class DoctorRepository : GenericRepository<Doctor>, IDoctorService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public DoctorRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        #region Extra
        public Doctor GetByUserId(int userId)
        {
            return _dbContext.Doctors.Where(d => d.UserId == userId).FirstOrDefault();
        }

        public DataSet GetQueryResult(string query)
        {
            return _dbContext.GetQueryAsDatatable(query);
        }

        public IList<DoctorSearchList> GetDoctorSearchList(string spName, object[] paraObjects)
        {
            return _dbContext.Database.SqlQuery<DoctorSearchList>(spName, paraObjects).ToList();
        }

        public IList<DoctorDetails> GetDoctorDetails(string commandText, List<SqlParameter> parameters)
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
                }
            }


            var data = _dbContext.Database.SqlQuery<DoctorDetails>(commandText, parameters.ToArray<object>()).ToList();
            return data;
        }
        public  List<T> GetDataList<T>(string commandText, List<SqlParameter> parameters)
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
                }
            }


            var data =  _dbContext.Database.SqlQuery<T>(commandText, parameters.ToArray<object>()).ToList();
            return data;
        }
        public DataSet GetDataSetList(string commandText, List<SqlParameter> parameters)
        {
            return _dbContext.GetQueryAsDataSet(commandText, parameters);
        }
        public bool AddOrUpdateExecuteProcedure(string commandText, List<SqlParameter> parameters)
        {
            return _dbContext.AddOrUpdateExecuteQuery(commandText, parameters);
        }
        public IList<DoctorWithDistance> GetDoctorDistanceFromUser(string commandText, List<SqlParameter> parameters)
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


            var data = _dbContext.Database.SqlQuery<DoctorWithDistance>(commandText, parameters.ToArray<object>()).ToList();
            return data;
        }

        public IList<DoctorSearchList> GetDoctorSearchListPagination(string commandText, List<SqlParameter> parameters)
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


            var data = _dbContext.Database.SqlQuery<DoctorSearchList>(commandText, parameters.ToArray<object>()).ToList();
            return data;
        }

        //public IList<DoctorSearchList> GetDoctorSearchListForMap(string commandText, List<SqlParameter> parameters, out int totalRecord)
        //{
        //    totalRecord = 0;
        //    var pTotalRecords = new SqlParameter("@TotalRecords", SqlDbType.Int) { Direction = ParameterDirection.Output };
        //    parameters.Add(pTotalRecords);
        //    if (parameters != null && parameters.Any())
        //    {
        //        for (int i = 0; i <= parameters.Count - 1; i++)
        //        {
        //            var p = parameters[i] as SqlParameter;
        //            if (p == null)
        //                throw new Exception("Not support parameter type");

        //            commandText += i == 0 ? " " : ", ";

        //            commandText += p.ParameterName;
        //            if (p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Output)
        //            {
        //                //output parameter
        //                commandText += " output";
        //            }
        //        }
        //    }


        //    var data = _dbContext.Database.SqlQuery<DoctorSearchList>(commandText, parameters.ToArray<object>()).ToList();
        //    totalRecord = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
        //    return data;
        //}

        public IList<FeaturedDoctors> GetFeaturedDoctorIdsBySearchText(string commandText, List<SqlParameter> parameters)
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


            var data = _dbContext.Database.SqlQuery<FeaturedDoctors>(commandText, parameters.ToArray<object>()).ToList();
            return data;
        }

        public IList<FeaturedFacilities> GetFeaturedFacilityIdsBySearchText(string commandText, List<SqlParameter> parameters)
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


            var data = _dbContext.Database.SqlQuery<FeaturedFacilities>(commandText, parameters.ToArray<object>()).ToList();
            return data;
        }

        public IList<PharmacySearchList> GetSearchPharmacyListPagination(string commandText, List<SqlParameter> parameters, out int totalRecord)
        {
            totalRecord = 0;
            var pTotalRecords = new SqlParameter("@TotalRecords", SqlDbType.Int) { Direction = ParameterDirection.Output };
            parameters.Add(pTotalRecords);
            commandText = "SearchPharmacyListPagination";
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


            var data = _dbContext.Database.SqlQuery<PharmacySearchList>(commandText, parameters.ToArray<object>()).ToList();
            totalRecord = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
            return data;
        }


        public IList<SeniorCareSearchList> GetSearchSeniorCarePagination(string commandText, List<SqlParameter> parameters, out int totalRecord)
        {
            totalRecord = 0;
            var pTotalRecords = new SqlParameter("@TotalRecords", SqlDbType.Int) { Direction = ParameterDirection.Output };
            parameters.Add(pTotalRecords);
            commandText = "SearchSeniorCarePagination";
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


            var data = _dbContext.Database.SqlQuery<SeniorCareSearchList>(commandText, parameters.ToArray<object>()).ToList();
            totalRecord = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
            return data;
        }


        public IList<FacilitySearchList> GetSearchFacilityPagination(string commandText, List<SqlParameter> parameters, out int totalRecord)
        {
            totalRecord = 0;
            var pTotalRecords = new SqlParameter("@TotalRecords", SqlDbType.Int) { Direction = ParameterDirection.Output };
            parameters.Add(pTotalRecords);
            commandText = "SearchFacilityPagination";
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


            var data = _dbContext.Database.SqlQuery<FacilitySearchList>(commandText, parameters.ToArray<object>()).ToList();
            totalRecord = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
            return data;
        }

        public IList<PharmacySearchList> GetPharmacySearchList(string spName, object[] paraObjects)
        {
            return _dbContext.Database.SqlQuery<PharmacySearchList>(spName, paraObjects).ToList();
        }

        public IList<SeniorCareSearchList> GetSeniorCareSearchList(string spName, object[] paraObjects)
        {
            return _dbContext.Database.SqlQuery<SeniorCareSearchList>(spName, paraObjects).ToList();
        }

        public IList<FacilitySearchList> GetFacilitySearchList(string spName, object[] paraObjects)
        {
            return _dbContext.Database.SqlQuery<FacilitySearchList>(spName, paraObjects).ToList();
        }

        public IList<DoctorSearchList> GetPharmacySearchListPagination(string spName, List<SqlParameter> paraObjects, out int totalRecord)
        {
            throw new NotImplementedException();
        }

        public IList<DoctorSearchList> GetSeniorCareSearchListPagination(string spName, List<SqlParameter> paraObjects, out int totalRecord)
        {
            throw new NotImplementedException();
        }

        public IList<DoctorSearchList> GetFacilitySearchListPagination(string spName, List<SqlParameter> paraObjects, out int totalRecord)
        {
            throw new NotImplementedException();
        }
        public void ExecuteSqlCommandForUpdate(string tableNameToUpdate, string primaryKeyName, int primaryKeyValue, List<SqlParameter> parametersToUpdate)
        {
            try
            {
                var commandText = "Update " + tableNameToUpdate + " Set ";
                if (parametersToUpdate != null && parametersToUpdate.Any())
                {
                    var count = 0;
                    foreach (var para in parametersToUpdate)
                    {
                        if (count != 0)
                            commandText += ",";
                        if (para.SqlDbType == SqlDbType.VarChar || para.SqlDbType == SqlDbType.NVarChar || para.SqlDbType == SqlDbType.DateTime)
                            commandText += para.ParameterName + "=" + "'" + para.Value + "'" + " ";
                        else
                            commandText += para.ParameterName + "=" + para.Value + " ";
                        count++;
                    }
                }

                commandText += "Where " + primaryKeyName + "=" + primaryKeyValue;
                _dbContext.Database.ExecuteSqlCommand(commandText);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion

        public List<int> GetAgeGroupReferenceId(List<int> ageGroups)
        {
            return _dbContext.DoctorAgeGroups.Where(x => ageGroups.Contains(x.AgeGroupId))
                .Select(x => x.DoctorId).ToList();
        }
        public List<int> GetAgeGroupReferenceIdWithSearchLocation(List<int> ageGroups)
        {
            return _dbContext.DoctorAgeGroups.Where(x => ageGroups.Contains(x.AgeGroupId)).Select(x => x.DoctorId).ToList();
        }
        public List<int> GetAffiliationDoctorId(List<int> affiliations)
        {
            return _dbContext.DoctorAffiliations.Where(x => affiliations.Contains(x.OrganisationId)).Select(x => x.DoctorId).ToList();
        }

        public List<int> GetInsuranceDoctorId(List<int> insurances)
        {
            var docIds = (from doi in _dbContext.DocOrgInsurance
                          join ip in _dbContext.InsurancePlans on doi.InsurancePlanId equals ip.InsurancePlanId
                          join it in _dbContext.InsuranceTypes on ip.InsuranceTypeId equals it.InsuranceTypeId
                          where ip.IsActive && !ip.IsDeleted && it.IsActive && !it.IsDeleted && insurances.Contains(it.InsuranceTypeId)
                          select doi.ReferenceId).Distinct().ToList();

            return docIds;
        }
        public List<int> GetDoctorIdBySpecialityName(string search)
        {
            var docIds = (from dosl in _dbContext.DocOrgStateLicense
                          join tx in _dbContext.Taxonomies on dosl.HealthCareProviderTaxonomyCode.Trim() equals tx.Taxonomy_Code.Trim()
                          where (!tx.IsActive.HasValue || tx.IsActive.Value) && (!tx.IsDeleted.HasValue || tx.IsDeleted.Value) &&
                          (!dosl.IsActive.HasValue || dosl.IsActive.Value) && (!dosl.IsDeleted.HasValue || dosl.IsDeleted.Value) &&
                          (tx.Specialization.Contains(search) || tx.Taxonomy_Code.Contains(search))
                          select dosl.ReferenceId).ToList();
            return docIds;

        }




        public List<DoctorWithDistance> GetDoctorIdByNameOrAddress(string search, bool isNtPcp, bool isANP, bool primaryCare, string distanceSearch, decimal lat2, decimal long2, string searchLocation = "")
        {
            var searchTerms = new List<string>();
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                searchTerms.AddRange(search.Split(' '));
            }
            dynamic result;
            if (string.IsNullOrEmpty(searchLocation))
            {
                result = (from d in _dbContext.Doctors
                          join ad in _dbContext.Addresses on d.DoctorId equals ad.ReferenceId
                          join csz in _dbContext.CityStateZips on ad.CityStateZipCodeID equals csz.CityStateZipCodeID
                          where ad.UserTypeID.Value == 2 && ad.AddressTypeID == 11 && ad.IsActive && !ad.IsDeleted && d.IsActive && !d.IsDeleted &&
                          (string.IsNullOrEmpty(search) || searchTerms.All(m => (d.NamePrefix + d.FirstName + d.MiddleName + d.LastName + ad.Address1 + ad.Address2 + csz.City + csz.State + csz.Country + csz.ZipCode).Contains(m)))
                          && (!isNtPcp || d.IsNtPcp == isNtPcp) && (!isANP || d.IsAllowNewPatient == isANP) &&
                          (!primaryCare || d.IsPrimaryCare == primaryCare) && (string.IsNullOrEmpty(distanceSearch) || csz.City.Contains(distanceSearch) ||
                          csz.State.Contains(distanceSearch) || csz.Country.Contains(distanceSearch) || csz.ZipCode.Contains(distanceSearch))
                          select new DoctorWithDistance()
                          {
                              DoctorId = d.DoctorId,
                              Lat = csz.Lat ?? 0,
                              Long = csz.Long ?? 0
                          }).ToList();
                foreach (var item in result)
                {
                    item.DistanceCount = this.GetDistanceInMile(item.Lat, item.Long, lat2, long2);
                }
            }
            else
            {
                string city = searchLocation.Split('|')[0];
                string state = searchLocation.Split('|')[1];
                string Zipcode = searchLocation.Split('|')[2];
                result = (from d in _dbContext.Doctors
                          join ad in _dbContext.Addresses on d.DoctorId equals ad.ReferenceId
                          join csz in _dbContext.CityStateZips on ad.CityStateZipCodeID equals csz.CityStateZipCodeID
                          where ad.UserTypeID.Value == 2 && ad.AddressTypeID == 11 
                          && ad.IsActive && !ad.IsDeleted && d.IsActive && !d.IsDeleted &&
                          ((string.IsNullOrEmpty(search)  
                           ||searchTerms.All(m => (d.NamePrefix + d.FirstName + d.MiddleName + d.LastName).Contains(m)))
                          && (!isNtPcp || d.IsNtPcp == isNtPcp) && (!isANP || d.IsAllowNewPatient == isANP) &&
                          (!primaryCare || d.IsPrimaryCare == primaryCare))                            
                          && (csz.ZipCode.Trim()==Zipcode.Trim() && (csz.City.ToLower().Trim() == city.ToLower().Trim()) && (csz.State.ToLower().Trim() == state.ToLower().Trim()))
                          select new DoctorWithDistance()
                          {
                              DoctorId = d.DoctorId,
                              Lat = csz.Lat ?? 0,
                              Long = csz.Long ?? 0
                          }).ToList();
            }
            
            return result;

        }

        public List<DoctorWithDistance> GetDistance(List<int> doctorIds, decimal lat2, decimal long2, string Searchlocation = "")
        {
            dynamic result;
            if (!string.IsNullOrEmpty(Searchlocation))
            {

                string city = Searchlocation.Split('|')[0];
                string state = Searchlocation.Split('|')[1];
                string Zipcode = Searchlocation.Split('|')[2];
               
                result = (from ad in _dbContext.Addresses
                          join csz in _dbContext.CityStateZips on ad.CityStateZipCodeID equals csz.CityStateZipCodeID
                          where doctorIds.Contains(ad.ReferenceId) && ad.IsActive && !ad.IsDeleted && ad.UserTypeID == 2
                          && csz.City == city && csz.State == state && csz.ZipCode == Zipcode 
                          select new DoctorWithDistance()
                          {
                              DoctorId = ad.ReferenceId,
                              Lat = csz.Lat ?? 0,
                              Long = csz.Long ?? 0

                          }).ToList();
            }
            else
            {
                result = (from ad in _dbContext.Addresses
                          join csz in _dbContext.CityStateZips on ad.CityStateZipCodeID equals csz.CityStateZipCodeID
                          where doctorIds.Contains(ad.ReferenceId) && ad.IsActive && !ad.IsDeleted && ad.UserTypeID == 2
                          select new DoctorWithDistance()
                          {
                              DoctorId = ad.ReferenceId,
                              Lat = csz.Lat ?? 0,
                              Long = csz.Long ?? 0

                          }).ToList().OrderBy(m => m.DistanceCount).ToList();
            }
            foreach (var item in result)
            {
                item.DistanceCount = this.GetDistanceInMile(item.Lat, item.Long, lat2, long2);
            }

            return result;
            //if (da.Count > 1)
            //    return 0;

            //var lat1 = da.First().Lat == null ? 0 : Convert.ToDouble(da.First().Lat);
            //var long1 = da.First().Long == null ? 0 : Convert.ToDouble(da.First().Long);

            //if (lat1 == 0 || long1 == 0 || lat2 == 0 || long2 == 0)
            //    return 0;

            //var degToRoad = 57.29577951;
            //var ans = Math.Sin(lat1 / degToRoad) * Math.Sin(@lat2 / degToRoad) + Math.Cos(@lat1 / degToRoad) * Math.Cos(@lat2 / degToRoad) * Math.Cos(Math.Abs(long2 - long1) / degToRoad);
            //var miles = 3959 * Math.Atan(Math.Sqrt(1 - Math.Pow(ans, 2)) / ans);
            //miles = Math.Ceiling(Math.Abs(miles));
            //return miles;
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

        #region DoctorBoardCertification
        public string AddDoctorBoardCertification(DoctorBoardCertificationModel objmodel)
        {

            SqlConnection con = new SqlConnection();
            con = new Connection().GetConnection();
            con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandText = "spDoctorBoardCertification";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Activity", "Insert");
                cmd.Parameters.AddWithValue("@DoctorId", objmodel.DoctorId);
                cmd.Parameters.AddWithValue("@IsActive", objmodel.IsActive);
                cmd.Parameters.AddWithValue("@BoardCertificationId", objmodel.BoardCertificationId);
                cmd.Parameters.AddWithValue("@Id", objmodel.DoctorBoardCertificationId);
                return Connection.ExecuteNonQuery(cmd);
            }
        }
        public List<DoctorBoardCertificationModel> GetDoctorBoardCertifications(Model.Pagination pager, int DoctorId)
        {
            return GetTable("spDoctorBoardCertification", "Filter", pager, DoctorId).DataTableToList<DoctorBoardCertificationModel>();
        }
        public List<DoctorQualificationViewModel> GetDoctorQualifications(Model.Pagination pager, int DoctorId)
        {
            return GetTable("spQualification", "Filter", pager, DoctorId).DataTableToList<DoctorQualificationViewModel>();
        }
        public List<DoctorExperienceViewModel> GetDoctorExperiences(Model.Pagination pager, int DoctorId)
        {
            return GetTable("spExperience", "Filter", pager, DoctorId).DataTableToList<DoctorExperienceViewModel>();
        }
        public List<DoctorAffiliationViewModel> GetDoctorAffiliations(Model.Pagination pager, int DoctorId)
        {
            return GetTable("spDoctorAffiliation", "Filter", pager, DoctorId).DataTableToList<DoctorAffiliationViewModel>();
        }
        public List<DoctorInsurancesViewModel> GetDoctorInsurancess(Model.Pagination pager, int DoctorId)
        {
            return GetTable("spDocOrgInsurances", "Filter", pager, DoctorId).DataTableToList<DoctorInsurancesViewModel>();
        }
        public List<DoctorLanguageViewModel> GetDoctorLanguages(Model.Pagination pager, int DoctorId)
        {
            return GetTable("spDoctorLanguage", "Filter", pager, DoctorId).DataTableToList<DoctorLanguageViewModel>();
        }
        public List<DoctorViewModel> GetDoctors(Pagination pager)
        {
            return GetTable("spDoctor", "Filter", pager, 0).DataTableToList<DoctorViewModel>();
        }
        public List<DoctorListViewModel> GetDoctorsList(Pagination pager)
        {
            return GetTable("spDoctor_Backup", "Filter", pager, 0).DataTableToList<DoctorListViewModel>();
        }
        public DoctorBoardCertificationModel GetDoctorBoardCertificationsById(int Id)
        {
            var model = GetTableById("spDoctorBoardCertification", "Find", Id).DataTableToList<DoctorBoardCertificationModel>().FirstOrDefault();
            return model;
        }
        public DoctorViewModel GetDoctorbyUserId(int Id)
        {
            return GetTableById("sprGetDoctorByUserId", "", Id).DataTableToList<DoctorViewModel>().FirstOrDefault();
        }
        #endregion

        #region Blog
        public List<BlogItem> GetBlogsSearchResults(string commandText, List<SqlParameter> parameters, out int RecordCount)
        {
            // Create a SQL command and add parameter
            var cmd = _dbContext.Database.Connection.CreateCommand();
            cmd.CommandText = commandText;
            cmd.CommandType = CommandType.StoredProcedure;
            foreach (var item in parameters)
            {
                cmd.Parameters.Add(item);
            }
            _dbContext.Database.Connection.Open();
            var reader = cmd.ExecuteReader();
            var objectContext = ((IObjectContextAdapter)_dbContext).ObjectContext;
            var data = objectContext.Translate<BlogItem>(reader).ToList();
            reader.NextResult();
            var d2 = objectContext.Translate<TotalRecords>(reader).ToList();
            RecordCount = d2 != null && d2.Count > 0 ? d2.First().TotalRecordCount : 0;
            return data.ToList();
        }
        #endregion

        public List<int> GetOrganisationIdsFromTypeId(int organisationTypeId, decimal lat2, decimal long2)//Added by Reena
        {
            var data = (from o in _dbContext.Organisations
                        join ad in _dbContext.Addresses on o.OrganisationId equals ad.ReferenceId
                        join csz in _dbContext.CityStateZips on ad.CityStateZipCodeID equals csz.CityStateZipCodeID
                        where o.OrganizationTypeID == organisationTypeId && ad.UserTypeID == o.UserTypeID && ad.IsActive && !ad.IsDeleted && o.IsActive && !o.IsDeleted
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

            return data.Select(m => m.OrganisationId).ToList();
        }

        public List<ProviderAdvertisements> GetAdvertisementsFromSearchText(List<int> organisationId, int userTypeId) //Added by Reena
        {
            var data = (from A in _dbContext.Advertisements
                        join PT in _dbContext.PaymentTypes on A.PaymentTypeId equals PT.PaymentTypeId
                        join O in _dbContext.Organisations on A.ReferenceId equals O.OrganisationId
                        where organisationId.Contains(O.OrganisationId) && A.IsActive == true && A.IsDeleted == false && PT.IsActive
                        && !PT.IsDeleted && A.AdvertisementTypeId == 1 && O.OrganizationTypeID == A.UserTypeId && O.OrganizationTypeID == userTypeId
                        select new ProviderAdvertisements()
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

        public List<ProviderAdvertisements> GetDistanceMilebyOrgIds(List<ProviderAdvertisements> data, decimal lat2, decimal long2)//Added by Reena
        {
            data = data.DistinctBy(m => m.OrganisationId).ToList();

            foreach (var item in data)
            {
                item.DistanceCount = this.GetDistanceInMile(item.Lat, item.Long, lat2, long2);
            }

            return data.OrderBy(m => m.DistanceCount).ToList();
        }

        public Doctor GetDoctorDetailsById(int id)
        {
            return _dbContext.Doctors.Where(d => d.DoctorId == id).FirstOrDefault();
        }
    }

}
