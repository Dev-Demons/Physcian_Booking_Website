//using Binke.Repository.Interface;
using Doctyme.Model;
using Doctyme.Model.ViewModels;
using Doctyme.Repository.Interface;
using Doctyme.Repository.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binke.Repository.Services
{
   public  class  BlogRepository : IBlogService
    {
        public DataSet GetQueryResult(BlogItem model)
        {
            //string strConnection = ConfigurationManager.AppSettings["Doctyme"];
            string strConnection = System.Configuration.ConfigurationManager.ConnectionStrings["Doctyme"].ConnectionString;
            DataSet ds;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 0;
            SqlConnection con = new SqlConnection(strConnection);
            try
            {
                cmd.Connection = con;
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "prc_Get_BlogDetails";
                addCommandParameter(ref cmd, "@ArticleName", SqlDbType.VarChar, 200, ParameterDirection.Input, model.ArticleName);
                addCommandParameter(ref cmd, "@ArticleId", SqlDbType.Int, 200, ParameterDirection.Input, model.ArticleId);
                addCommandParameter(ref cmd, "@CategoryId", SqlDbType.Int, 200, ParameterDirection.Input, model.CategoryId);
                addCommandParameter(ref cmd, "@CategoryName", SqlDbType.VarChar, 200, ParameterDirection.Input, model.CategoryName);
                addCommandParameter(ref cmd, "@type", SqlDbType.VarChar, 200, ParameterDirection.Input, model.Type);
                addCommandParameter(ref cmd, "@ExcArticleId", SqlDbType.VarChar, 200, ParameterDirection.Input, "");
                addCommandParameter(ref cmd, "@SubCategoryId", SqlDbType.Int, 200, ParameterDirection.Input, model.SubCategoryId);
                addCommandParameter(ref cmd, "@SubCategoryName", SqlDbType.VarChar, 200, ParameterDirection.Input, model.SubCategoryName);              
                ds = returnDataset(ref cmd);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                cmd.Connection.Close();
            }
            return ds;
        }
        public DataSet GetArticleId(BlogItem model)
        {
            //string strConnection = ConfigurationManager.AppSettings["Doctyme"];
            string strConnection = System.Configuration.ConfigurationManager.ConnectionStrings["Doctyme"].ConnectionString;
            DataSet ds;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 0;
            SqlConnection con = new SqlConnection(strConnection);
            try
            {
                cmd.Connection = con;
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "prc_GetArticles_By_Id";
                addCommandParameter(ref cmd, "@ArticleName", SqlDbType.VarChar, 200, ParameterDirection.Input, model.ArticleName);                              
                ds = returnDataset(ref cmd);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                cmd.Connection.Close();
            }
            return ds;
        }
        public DataTable GetCategories()
        {
            string strConnection = System.Configuration.ConfigurationManager.ConnectionStrings["Doctyme"].ConnectionString;
            DataSet ds;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 0;
            SqlConnection con = new SqlConnection(strConnection);
            try
            {
                cmd.Connection = con;
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "prc_Category_SubCategory_Get";
                ds = returnDataset(ref cmd);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                cmd.Connection.Close();
            }
            return ds.Tables[0];
        }
        public DataTable GetMenuCategories()
        {
            string strConnection = System.Configuration.ConfigurationManager.ConnectionStrings["Doctyme"].ConnectionString;
            DataSet ds;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 0;
            SqlConnection con = new SqlConnection(strConnection);
            try
            {
                cmd.Connection = con;
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "Get_MenuCategory_SubCategory";
                ds = returnDataset(ref cmd);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                cmd.Connection.Close();
            }
            return ds.Tables[0];
        }
        public static SqlParameter addCommandParameter(ref SqlCommand sqlCommand,string name,SqlDbType type,int size, ParameterDirection direction,object value)
        {
            try
            {
                SqlParameter sqlParam = new SqlParameter();
                sqlParam.SqlDbType = type;
                sqlParam.ParameterName = name;
                sqlParam.Direction = direction;
                sqlParam.Value = value;
                sqlCommand.Parameters.Add(sqlParam);
                return sqlParam;

            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static DataSet returnDataset(ref SqlCommand sqlcommand)
        {
            string strConnString = ConfigurationManager.AppSettings["Doctyme"];
            SqlConnection con = new SqlConnection(strConnString);
            try
            {
                if (sqlcommand.Connection == null)
                {
                    sqlcommand.Connection = con;
                    con.Open();
                }
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                sqlDataAdapter.SelectCommand = sqlcommand;
                DataSet dataset = new DataSet();
                sqlDataAdapter.Fill(dataset);
                if(sqlcommand.Connection.State.ToString() =="Open")
                {
                    sqlcommand.Connection.Close();
                }
                return dataset;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
