using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Model
{
    public static class DataReaderExtensions
    {


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static List<T> MapToList<T>(this DbDataReader dr) where T : new()
        {
            List<T> RetVal = null;
            var Entity = typeof(T);
            var PropDict = new Dictionary<string, PropertyInfo>();
            string Name = string.Empty;
            try
            {

                if (dr != null && dr.HasRows)
                {
                    RetVal = new List<T>();
                    var Props = Entity.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    PropDict = Props.ToDictionary(p => p.Name.ToUpper(), p => p);

                    while (dr.Read())
                    {
                        T newObject = new T();
                        for (int Index = 0; Index < dr.FieldCount; Index++)
                        {
                            Name = dr.GetName(Index).ToUpper();
                            if (PropDict.ContainsKey(dr.GetName(Index).ToUpper()))
                            {
                                var Info = PropDict[dr.GetName(Index).ToUpper()];
                                if ((Info != null) && Info.CanWrite)
                                {
                                    var Val = dr.GetValue(Index);
                                    Info.SetValue(newObject, (Val == DBNull.Value) ? null : Val, null);
                                }
                            }
                        }
                        RetVal.Add(newObject);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return RetVal;
        }

        public static List<T> MapToListForCodeValues<T>(this DbDataReader dr) where T : new()
        {
            List<T> RetVal = null;
            var Entity = typeof(T);
            var PropDict = new Dictionary<string, PropertyInfo>();
            string Name = string.Empty;
            int RecordCount = 0;
            DataTable dataRecord = new DataTable();

            try
            {

                if (dr != null && dr.HasRows)
                {

                    RetVal = new List<T>();
                    var Props = Entity.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    PropDict = Props.ToDictionary(p => p.Name.ToUpper(), p => p);
                    dataRecord.Load(dr);
                    var Info = PropDict["NAME"];
                    RecordCount = dataRecord.Rows.Count;
                    var Val = new Object();
                    var Val1 = new Object();

                    for (int Index = 0; Index < RecordCount; Index++)
                    {
                        T newObject = new T();
                        if (dataRecord.Columns.Count > 1)
                        {
                            if ((Info != null) && Info.CanWrite)
                            {
                                Info = PropDict["NAME"];
                                Val = dataRecord.Rows[Index]["Name"];

                                Info.SetValue(newObject, (Val == DBNull.Value) ? null : Val, null);
                                Info = PropDict["VALUE"];
                                Val1 = dataRecord.Rows[Index]["Value"];

                                Info.SetValue(newObject, (Val1 == DBNull.Value) ? null : Val1.ToString(), null);
                                RetVal.Add(newObject);
                            }
                        }
                        else
                        {
                            Val1 = dataRecord.Rows[Index][1];
                            Info.SetValue(newObject, (Val1 == DBNull.Value) ? null : Val1, null);
                            RetVal.Add(newObject);
                        }
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }

            return RetVal;
        }

        //public static List<T> MapToPrimitive<T>(this DbDataReader dr)
        //{
        //    List<T> RetVal = null;
        //    try
        //    {
        //        if (dr != null && dr.HasRows)
        //        {
        //            RetVal = new List<T>();

        //            while (dr.Read())
        //            {
        //                RetVal.Add(TConverter.ChangeType<T>(Convert.ToString(dr.GetValue(0))));
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    return RetVal;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T MapToSingle<T>(this DbDataReader dr) where T : new()
        {
            T RetVal = new T();
            var Entity = typeof(T);
            var PropDict = new Dictionary<string, PropertyInfo>();

            try
            {
                if (dr != null && dr.HasRows)
                {
                    var Props = Entity.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    PropDict = Props.ToDictionary(p => p.Name.ToUpper(), p => p);

                    dr.Read();
                    for (int Index = 0; Index < dr.FieldCount; Index++)
                    {
                        if (PropDict.ContainsKey(dr.GetName(Index).ToUpper()))
                        {
                            var Info = PropDict[dr.GetName(Index).ToUpper()];
                            if ((Info != null) && Info.CanWrite)
                            {
                                var Val = dr.GetValue(Index);
                                Info.SetValue(RetVal, (Val == DBNull.Value) ? null : Val, null);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return RetVal;
        }
        public static IEnumerable<DataRow> AsEnumerable(this DataTable table)
        {
            for (int i = 0; i < table.Rows.Count; i++)
            {
                yield return table.Rows[i];
            }
        }
        public static List<T> DataTableToList<T>(this DataTable table) where T : new()
        {
            try
            {
                List<T> list = new List<T>();

                foreach (var row in table.AsEnumerable())
                {
                    T obj = new T();

                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            Type columnType = propertyInfo.PropertyType;

                            // We need to check whether the property is NULLABLE
                            if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                // If it is NULLABLE, then get the underlying type. eg if "Nullable<int>" then this will return just "int"
                                columnType = propertyInfo.PropertyType.GetGenericArguments()[0];
                            }
                            propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], columnType), null);
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }
        private static List<T> ConvertDataTableToList<T>(this DataTable dt)
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
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }

    }
}
