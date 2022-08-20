using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Repository.Interface
{
    public interface IRepository
    {
        //***************** Used In Stored Procedure - Added By Kapila ======
        IEnumerable<T> ExecWithStoreProcedure<T>(string query, params object[] parameters);
        int ExecuteSQLQuery(string query, params object[] parameters);
        int ExecuteSQLQuery<T>(string query, params object[] parameters);
        DbRawSqlQuery<T> SQLQuery<T>(string sql, params object[] parameters);
        //***************** Used In Stored Procedure - Added By Kapila ======

        // void ExecuteWithStoreProcedure(string query, params object[] parameters);
        /// <summary>
        /// Gets all objects from database
        /// </summary>
        IQueryable<T> All<T>() where T : class;

        /// <summary>
        /// Gets objects from database by filter.
        /// </summary>
        /// <param name="predicate">Specified a filter</param>
        IQueryable<T> Filter<T>(Expression<Func<T, bool>> predicate, params string[] includes) where T : class;

        /// <summary>
        /// Find object by keys.
        /// </summary>
        /// <param name="keys">Specified the search keys.</param>
        T Find<T>(params object[] keys) where T : class;

        /// <summary>
        /// Find object by specified expression.
        /// </summary>
        /// <param name="predicate"></param>
        T Find<T>(Expression<Func<T, bool>> predicate) where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="commit"></param>
        /// <returns></returns>
        T Insert<T>(T t, bool commit) where T : class;

        /// <summary>
        /// Delete the object from database.
        /// </summary>
        /// <param name="t">Specified a existing object to delete.</param>        
        int Delete<T>(T t) where T : class;

        /// <summary>
        /// Delete objects from database by specified filter expression.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="commit"></param>
        int Delete<T>(Expression<Func<T, bool>> predicate, bool commit) where T : class;

        /// <summary>
        /// Update object changes and save to database.
        /// </summary>
        /// <param name="t">Specified the object to save.</param>
        /// <param name="commit"></param>
        int Update<T>(T t, bool commit) where T : class;
    }
}
