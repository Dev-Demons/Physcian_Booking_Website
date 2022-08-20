using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;

namespace Binke.Model.Utility
{
    public static class DbContextExtensions
    {
        #region Stored Procedure MultipleResults
        #region Public Methods
        public static MultipleResultSetWrapper MultipleResults(this DbContext db, string query, IEnumerable<SqlParameter> parameters = null) => new MultipleResultSetWrapper(db: db, query: query, parameters: parameters);
        #endregion Public Methods

        #region Public Classes
        public class MultipleResultSetWrapper
        {

            #region Public Fields
            public List<Func<DbDataReader, IEnumerable>> ResultSets;
            #endregion Public Fields

            #region Private Fields
            private readonly IObjectContextAdapter _adapter;
            private readonly string _commandText;
            private readonly DbContext _db;
            private readonly IEnumerable<SqlParameter> _parameters;
            #endregion Private Fields

            #region Public Constructors
            public MultipleResultSetWrapper(DbContext db, string query, IEnumerable<SqlParameter> parameters = null)
            {
                _db = db;
                _adapter = db;
                _commandText = query;
                _parameters = parameters;
                ResultSets = new List<Func<DbDataReader, IEnumerable>>();
            }
            #endregion Public Constructors

            #region Public Methods
            public MultipleResultSetWrapper AddResult<TResult>()
            {
                ResultSets.Add(OneResult<TResult>);
                return this;
            }

            public List<IEnumerable> Execute()
            {
                var results = new List<IEnumerable>();

                using (var connection = new SqlConnection(_db.Database.Connection.ConnectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = _commandText;
                    if (_parameters?.Any() ?? false) { command.Parameters.AddRange(_parameters.ToArray()); }
                    using (var reader = command.ExecuteReader())
                    {
                        foreach (var resultSet in ResultSets)
                        {
                            results.Add(resultSet(reader));
                        }
                    }
                    return results;
                }
            }
            #endregion Public Methods

            #region Private Methods
            private IEnumerable OneResult<TResult>(DbDataReader reader)
            {
                var result = _adapter
                    .ObjectContext
                    .Translate<TResult>(reader)
                    .ToArray();
                reader.NextResult();
                return result;
            }
            #endregion Private Methods

        }
        #endregion Public Classes 
        #endregion

        #region Get Query Result as Datatable or DataSet
        public static DataSet GetQueryAsDatatable(this DbContext context, string sqlQuery)
        {
            DbProviderFactory dbFactory = DbProviderFactories.GetFactory(context.Database.Connection);

            using (var cmd = dbFactory.CreateCommand())
            {
                if (cmd == null) return new DataSet();
                cmd.Connection = context.Database.Connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlQuery;
                using (DbDataAdapter adapter = dbFactory.CreateDataAdapter())
                {
                    if (adapter == null) return new DataSet();
                    adapter.SelectCommand = cmd;
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    return ds;
                }
            }
        }
        #endregion

        public static int GetQueryCounts(this DbContext context, string sqlQuery)
        {
            DbProviderFactory dbFactory = DbProviderFactories.GetFactory(context.Database.Connection);
            using (var cmd = dbFactory.CreateCommand())
            {
                if (cmd == null) return 0;
                cmd.Connection = context.Database.Connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlQuery;
                context.Database.Connection.Open();
                var result = (int)cmd.ExecuteScalar();
                context.Database.Connection.Close();
                return result;
            }
        }

        public static string GetQuerySingleResult(this DbContext context, string sqlQuery)
        {
            DbProviderFactory dbFactory = DbProviderFactories.GetFactory(context.Database.Connection);
            using (var cmd = dbFactory.CreateCommand())
            {
                if (cmd == null) return "";
                cmd.Connection = context.Database.Connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlQuery;
                context.Database.Connection.Open();
                var result = (string)cmd.ExecuteScalar();
                context.Database.Connection.Close();
                return result;
            }
        }
    }
}

