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
    public class SeniorCareRepository : GenericRepository<Organisation>, ISeniorCareService
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;

        public SeniorCareRepository(Doctyme.Model.DoctymeDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void UpdateSeniorCareProfile(string commandText,Organisation parameters)
        {
            SqlConnection con = new SqlConnection(_dbContext.Database.Connection.ConnectionString.ToString());
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = commandText;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@OrganisationId", parameters.OrganisationId);
            cmd.Parameters.AddWithValue("@OrganisationName", parameters.OrganisationName);
            cmd.Parameters.AddWithValue("@AuthorizedOfficialFirstName", parameters.AuthorizedOfficialFirstName);
            cmd.Parameters.AddWithValue("@AuthorizedOfficialLastName", parameters.AuthorizedOfficialLastName);
            cmd.Parameters.AddWithValue("@AuthorizedOfficialTitleOrPosition", parameters.AuthorizedOfficialTitleOrPosition);
            cmd.Parameters.AddWithValue("@AuthorizedOfficialTelephoneNumber", parameters.AuthorizedOfficialTelephoneNumber);
            cmd.Parameters.AddWithValue("@ShortDescription", parameters.ShortDescription);
            cmd.Parameters.AddWithValue("@LongDescription", parameters.LongDescription);
            cmd.ExecuteNonQuery();            
            con.Close();
        }

        public IList<SeniorCareData> GetSeniorCareListByTypeId(string commandText, List<SqlParameter> parameters, out int totalRecord)
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

            var data = _dbContext.Database.SqlQuery<SeniorCareData>(commandText, parameters.ToArray<object>()).ToList();
            totalRecord = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
            return data;
        }
        public IList<OrganizationAmenityOptionViewModel> GetOrganizationAmenityOptionByOrganization(string commandText, List<SqlParameter> parameters, out int totalRecord)
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

            var data = _dbContext.Database.SqlQuery<OrganizationAmenityOptionViewModel>(commandText, parameters.ToArray<object>()).ToList();
            totalRecord = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
            return data;
        }

        public IList<SiteImageViewModel> GetSiteImageByOrganization(string commandText, List<SqlParameter> parameters, out int totalRecord)
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

            var data = _dbContext.Database.SqlQuery<SiteImageViewModel>(commandText, parameters.ToArray<object>()).ToList();
            totalRecord = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
            return data;
        }
    }

    //public class SeniorCareImageRepository : GenericRepository<SeniorCareImage>, ISeniorCareImageService
    //{
    //    private readonly Doctyme.Model.Doctyme _dbContext;

    //    public SeniorCareImageRepository(Doctyme.Model.Doctyme dbContext) : base(dbContext)
    //    {
    //        _dbContext = dbContext;
    //    }
    //}

}
