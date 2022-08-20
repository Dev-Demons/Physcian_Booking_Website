using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Binke.Api.DAL.Interfaces;
using System.Threading.Tasks;
using Binke.Api.Models;
using Doctyme.Model;

namespace Binke.Api.DAL.Repository
{
    public abstract class GenericMasterRepo<T> : IGenericMaster<T> where T : class
    {
      
        public List<T> GetAll(string spname, string Activity, Pagination pager )
        {
           

            SqlConnection con = new SqlConnection(Connection.sqlConStr);
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            else
            {
                con.Open();
            }

            SqlDataAdapter adp = new SqlDataAdapter(spname, con);
            adp.SelectCommand.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(Activity))
                adp.SelectCommand.Parameters.AddWithValue("@Activity", Activity);
            adp.SelectCommand.Parameters.AddWithValue("@StartIndex", pager.StartIndex);
            adp.SelectCommand.Parameters.AddWithValue("@PageSize", pager.PageSize);
            if(!string.IsNullOrEmpty(pager.Search))
                adp.SelectCommand.Parameters.AddWithValue("@Search", pager.Search);
            
            adp.SelectCommand.Parameters.AddWithValue("@SortColumnName", pager.SortColumnName);
            adp.SelectCommand.Parameters.AddWithValue("@SortDirection", pager.SortDirection);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            adp.Dispose();
            List<T> objlist = Encrypt.ConvertDataTableToList<T>(dt);
            con.Close();
            return objlist;
        }

        public T GetRecordsById(string spname, string Activity, int id)
        {

            SqlConnection con = new SqlConnection(Connection.sqlConStr);
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            else
            {
                con.Open();
            }
            SqlDataAdapter adp = new SqlDataAdapter(spname, con);
            adp.SelectCommand.CommandType = CommandType.StoredProcedure;
            adp.SelectCommand.Parameters.AddWithValue("@id", id);
            if (!string.IsNullOrEmpty(Activity))
                adp.SelectCommand.Parameters.AddWithValue("@Activity", Activity);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            adp.Dispose();
            List<T> objlist = Encrypt.ConvertDataTableToList<T>(dt);

            con.Close();

            return objlist.FirstOrDefault();
        }

        public DataTable GetTableById(string spname, string Activity, int? id = 0)
        {
            SqlConnection con = new SqlConnection(Connection.sqlConStr);
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            else
            {
                con.Open();
            }

            SqlDataAdapter adp = new SqlDataAdapter(spname, con);
            adp.SelectCommand.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(Activity))
                adp.SelectCommand.Parameters.AddWithValue("@Activity", Activity);
            if (id != 0)
                adp.SelectCommand.Parameters.AddWithValue("@id", id);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            adp.Dispose();
            con.Close();
            return dt;
        }
        public async Task<DataTable> GetTableByIdAsync(string spname, string Activity, int? id = 0)
        {
            SqlConnection con = new SqlConnection(Connection.sqlConStr);
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            else
            {
                con.Open();
            }

            SqlCommand cmd = new SqlCommand
            {
                CommandTimeout = 2000,
                Connection = con,
                CommandType = CommandType.StoredProcedure,
                CommandText = spname
            };
            if (id != 0)
                cmd.Parameters.AddWithValue("@id", id);
            if (!string.IsNullOrEmpty(Activity))
                cmd.Parameters.AddWithValue("@Activity", Activity);
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            var task = await Task.Run(() => adp.Fill(dt));
            con.Close();
            return dt;
        }

        public List<T> GetAllById(string spname, string Activity, int? id)
        {

            SqlConnection con = new SqlConnection(Connection.sqlConStr);
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            else
            {
                con.Open();
            }

            SqlCommand cmd = new SqlCommand
            {
                CommandTimeout = 200000,
                Connection = con,
                CommandType = CommandType.StoredProcedure,
                CommandText = spname
            };
            if (id != 0)
                cmd.Parameters.AddWithValue("@id", id);
            if (!string.IsNullOrEmpty(Activity))
                cmd.Parameters.AddWithValue("@Activity", Activity);
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            List<T> objlist = Encrypt.ConvertDataTableToList<T>(dt);
            con.Close();

            return objlist;
        }
        public List<T> GetAllByUserId(string spname, string Activity, string id)
        {

            SqlConnection con = new SqlConnection(Connection.sqlConStr);
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            else
            {
                con.Open();
            }

            SqlCommand cmd = new SqlCommand
            {
                CommandTimeout = 200000,
                Connection = con,
                CommandType = CommandType.StoredProcedure,
                CommandText = spname
            };
            cmd.Parameters.AddWithValue("@id", id);
            if (!string.IsNullOrEmpty(Activity))
                cmd.Parameters.AddWithValue("@Activity", Activity);
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            List<T> objlist = Encrypt.ConvertDataTableToList<T>(dt);
            con.Close();

            return objlist;
        }

    }
}