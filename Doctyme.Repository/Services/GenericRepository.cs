
using Doctyme.Model.ViewModels;
using Binke.Api.Models;
using Doctyme.Model;
using Doctyme.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace Doctyme.Repository.Services
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly Doctyme.Model.DoctymeDbContext _dbContext;
        private readonly DbSet<T> _table;

        public GenericRepository(Doctyme.Model.DoctymeDbContext dbcontext)
        {
            _dbContext = dbcontext;
            _dbContext.Database.Log = Console.Write;
            _table = _dbContext.Set<T>();
        }
        public IEnumerable<T> GetAll()
        {
            return _table.AsEnumerable(); //.AsNoTracking(); //
        }
        public IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            //Just to make it work faster during development. will remove take20 in production.
            return query.Where(predicate).AsEnumerable();
        }
        public T GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return query.SingleOrDefault(predicate);
        }

        public IList<SP_GetUserAddressZipCodeModel> GetUserAddressCityStateZip(string spName, List<SqlParameter> parameters)
        {
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
            //return _dbContext.Database.SqlQuery<SP_GetUserAddressZipCodeModel>(spName, parameters.ToArray<object>()).ToList();
            return _dbContext.Database.SqlQuery<SP_GetUserAddressZipCodeModel>(spName, parameters.ToArray<object>()).ToList();
        }

        public T GetById(object id)
        {
            return _table.Find(id);
        }
        public int GetCount(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return query.Count(predicate);
        }

        public IList<T> ExecuteSP(string spName, List<SqlParameter> paraObjects)
        {
            return _dbContext.Database.SqlQuery<T>(spName, paraObjects).ToList();
        }

        public int ExecuteSQLQuery(string query, params object[] parameters)
        {
            return _dbContext.Database.ExecuteSqlCommand("EXEC " + query, parameters);
        }

        public bool ExecuteSQLQueryWithOutParam(string query,params object[] parameters)
        {
            // assign the return code to the new output parameter and pass it to the sp
            var data = _dbContext.Database.SqlQuery<bool>(string.Format("exec {0}", query), parameters);

            return data.SingleOrDefault();
        }

        public void InsertData(T obj)
        {
            _table.Add(obj);
        }
        public void InsertData(IEnumerable<T> obj)
        {
            _table.AddRange(obj);
        }
        public void UpdateData(T obj)
        {
            _table.Attach(obj);
            _dbContext.Entry(obj).State = EntityState.Modified;
        }
        public void UpdateData(IEnumerable<T> obj)
        {
            foreach (var item in obj)
            {
                _table.Attach(item);
                _dbContext.Entry(item).State = EntityState.Modified;
            }
        }
        public void DeleteData(T obj)
        {
            _table.Remove(obj);
        }
        public void DeleteData(IEnumerable<T> obj)
        {
            _table.RemoveRange(obj);
        }
        public void SaveData()
        {
            _dbContext.SaveChanges();
        }
        public List<T> GetAllList(string spname, string Activity, Model.Pagination pager)
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
            if (!string.IsNullOrEmpty(pager.Search))
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
        public DataTable GetTable(string spname, string Activity, Model.Pagination pager,int? Id)
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
            if (!string.IsNullOrEmpty(pager.Search))
                adp.SelectCommand.Parameters.AddWithValue("@Search", pager.Search);
            if(Id!=null)
            {
                adp.SelectCommand.Parameters.AddWithValue("@Id", Id);

            }
            adp.SelectCommand.Parameters.AddWithValue("@SortColumnName", pager.SortColumnName);
            adp.SelectCommand.Parameters.AddWithValue("@SortDirection", pager.SortDirection);
            
            DataTable dt = new DataTable();
            adp.Fill(dt);
            adp.Dispose();
            con.Close();
            return dt;
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

        public List<T> GetDataList<T>(string commandText, List<SqlParameter> parameters)
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


            var data = _dbContext.Database.SqlQuery<T>(commandText, parameters.ToArray<object>()).ToList();
            return data;
        }
    }
}
