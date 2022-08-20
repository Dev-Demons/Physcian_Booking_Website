using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Doctyme.Model;
using System.Data.Entity.Validation;
using System.Data;
using System.Threading.Tasks;
using System.Configuration;
using Binke.Api.Models;
using System.Data.SqlClient;
using Doctyme.Repository.Interface;

namespace Binke.Api.Utility
{
    public class Common
    {
        private readonly IRepository _repo;
        public Common(IRepository repo)
        {
            this._repo = repo;
        }
        public static void DeleteFile(string path, bool isFullPath = false)
        {
            var fullPath = isFullPath ? path : HttpContext.Current.Server.MapPath(path);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
        public static void CheckServerPath(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        #region DataTable Or List Mehtod

        /// <summary>
        /// Convert Datatable To Generic List
        /// </summary>s
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ConvertDataTable<T>(DataTable dt)
        {
            var data = new List<T>();
            for (var index = 0; index < dt.Rows.Count; index++)
            {
                var row = dt.Rows[index];
                var item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        /// <summary>
        /// Get DataRow Items
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        private static T GetItem<T>(DataRow dr)
        {
            var temp = typeof(T);
            var obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (var pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                    {
                        if (pro.PropertyType == typeof(DateTime?) && dr[column.ColumnName] == DBNull.Value)
                        {
                            continue;
                        }
                        pro.SetValue(obj, (dr[column.ColumnName] == DBNull.Value) ? string.Empty : dr[column.ColumnName], null);
                    }
                    else
                        continue;
                }
            }
            return obj;
        }

        #endregion

        public string GetCityStateInfoById(int id, string type)
        {
            string result = "";

            var info = _repo.SQLQuery<CityStateInfoByZipCodeModel>("spGetCityStateZipInfoByID @ID", new SqlParameter("ID", System.Data.SqlDbType.Int) { Value = id }).ToList();

            if (type == "zip")
                result = info[0].ZipCode;
            if (type == "city")
                result = info[0].City;
            if (type == "state")
                result = info[0].State;
            if (type == "country")
                result = info[0].Country;

            return result;
        }

    }
}