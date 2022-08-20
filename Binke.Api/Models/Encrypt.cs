using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Binke.Api.Models
{
    public static class Encrypt
    {
       

        

        public static List<T> ConvertDataTableToList<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name.ToLower() == column.ColumnName.ToLower())
                    {
                        if (pro.PropertyType.Name == "String")
                        {
                            pro.SetValue(obj, Convert.ToString(dr[column.ColumnName]), null);
                        }
                        else if (pro.PropertyType.Name == "Int32")
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(dr[column.ColumnName])))
                                pro.SetValue(obj, dr[column.ColumnName] ?? 0, null);
                            else
                                pro.SetValue(obj, 0, null);
                        }
                        else if (pro.PropertyType.Name == "DateTime")
                        {
                            if (string.IsNullOrEmpty(Convert.ToString(dr[column.ColumnName])))
                            {
                                pro.SetValue(obj, DateTime.Now, null);
                            }
                            else
                            {
                                pro.SetValue(obj, dr[column.ColumnName], null);
                            }
                        }
                        else if (pro.PropertyType.Name == "Boolean")
                        {
                            if (dr[column.ColumnName] == DBNull.Value)
                            {
                                pro.SetValue(obj, false, null);
                            }
                            else
                                pro.SetValue(obj, dr[column.ColumnName], null);

                        }
                        else
                        {
                            if (!(dr[column.ColumnName] == DBNull.Value))
                            {
                                pro.SetValue(obj, dr[column.ColumnName], null);
                            }
                        }



                    }
                    else
                        continue;
                }
            }
            return obj;
        }
     

    }

    
}