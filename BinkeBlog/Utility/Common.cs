using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BinkeBlog.Utility
{
    public class Common
    {
        /// <summary>
        /// Convert Datatable To Generic List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ConvertDataTable<T>(System.Data.DataTable dt)
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

    }
}