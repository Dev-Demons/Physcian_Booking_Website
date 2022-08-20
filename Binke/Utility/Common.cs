using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Data.SqlClient;
using Doctyme.Model;
using Microsoft.Ajax.Utilities;
using Binke.Models;
using System.Globalization;
using Doctyme.Model.ViewModels;
using System.Configuration;
using Doctyme.Repository.Interface;
using Newtonsoft.Json.Linq;
using Doctyme.Repository.Enumerable;


namespace Binke.Utility
{
    public class Common
    {
        private  readonly IDoctorService _doctor;

        public Common(IDoctorService doctor)
        {
            _doctor = doctor;
        }

        #region Toastr Message
        public enum JsonType
        {
            Error = 0,
            Success = 1,
            Info = 2,
            Warning = 3,
            Question = 4
        }

        public class JsonModel
        {
            [ScriptIgnore]
            public JsonType JsonType { get; set; }
            public string Message { get; set; }
            public string Title { get; set; }
            public object Result { get; set; }
            public string AlertType => JsonType.ToString();

            public JsonModel(JsonType jsonType, string message, string title = null, object result = null)
            {
                JsonType = jsonType;
                Message = message;
                Title = title;
                Result = result;
            }
        }


        public static int GetResetPasswordDateDaysDifference(DateTime lastReset)
        {
            return (60 - DateTime.UtcNow.Date.Subtract(lastReset.Date).Days);
        }
        #endregion

        #region Date, Time, Week
        private static int CalculateAge(DateTime? dateOfBirth)
        {
            int age = 0;
            if (dateOfBirth.HasValue) return age;
            age = DateTime.Now.Year - dateOfBirth.Value.Year;
            if (DateTime.Now.DayOfYear < dateOfBirth.Value.DayOfYear)
                age = age - 1;
            return age;
        }

        public static List<SelectListItem> GetWeekdayList()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem { Text = @"Sunday", Value = "1"},
                new SelectListItem { Text = @"Monday", Value = "2" },
                new SelectListItem { Text = @"Tuesday", Value = "3" },
                new SelectListItem { Text = @"Wednesday", Value = "4"},
                new SelectListItem { Text = @"Thursday", Value = "5"},
                new SelectListItem { Text = @"Friday", Value = "6"},
                new SelectListItem { Text = @"Saturday", Value = "7"},
            };
        }

        public static string GetDayofWeek(int day, bool isShort = false)
        {
            string dayOfWeek = GetWeekdayList().FirstOrDefault(x => x.Value.Equals(Convert.ToString(day)))?.Text ?? "";
            return !isShort ? dayOfWeek : dayOfWeek.Substring(0, 3);
        }

        public static List<SelectListItem> GetTimeHoursList()
        {
            DateTime today = DateTime.Today;
            var clockQuery = from offset in Enumerable.Range(0, 48)
                             select TimeSpan.FromMinutes(30 * offset);
            var result = clockQuery.Select(x =>
                new SelectListItem
                {
                    Text = today.Add(x).ToString("hh:mm tt"),
                    Value = Convert.ToString(x.TotalMinutes)
                }
            ).ToList();
            return result;
        }
        #endregion

        #region Static SelectionList
        public static List<SelectListItem> GetUserTypes()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem { Value="2", Text= @"Doctor" },
                new SelectListItem { Value="4", Text= @"Facility" },
                new SelectListItem { Value="6", Text= @"Patient" },
                new SelectListItem { Value="3", Text= @"Pharmacy" },
                new SelectListItem { Value="5", Text= @"SeniorCare" }
            };
        }
        #endregion

        #region Enum Methods
        public static string GetEnumTypeName(Enum enumType)
        {
            return enumType.GetType().GetMember(enumType.ToString()).First().GetCustomAttribute<DisplayAttribute>().Name;
        }

        public class NameValue
        {
            public string Name { get; set; }
            public object Value { get; set; }
        }

        public static List<NameValue> EnumToList<T>()
        {
            var array = (T[])(Enum.GetValues(typeof(T)).Cast<T>());
            var array2 = Enum.GetNames(typeof(T)).ToArray<string>();
            List<NameValue> lst = null;
            for (var i = 0; i < array.Length; i++)
            {
                if (lst == null)
                    lst = new List<NameValue>();
                var name = array2[i];
                var value = array[i];
                lst.Add(new NameValue { Name = name, Value = value });
            }
            return lst;
        }

        #endregion

        #region ReadEmailTemplate

        public static string ReadEmailTemplate(string templateName)
        {
            try
            {
                string body;
                using (var sr = new StreamReader(HttpContext.Current.Server.MapPath($@"\\App_Data\\EmailTemplate\\{templateName}")))
                {
                    body = sr.ReadToEnd();
                }
                body = body.Replace("{url}", RequestHelpers.GetConfigValue("physicalLocalURL"));
                return body;
            }
            catch (Exception ex)
            {
                LogError(ex, "ReadEmailTemplate");
                return "";
            }
        }

        public static string ReadReportTemplate(string templateName)
        {
            try
            {
                string body;
                using (var sr = new StreamReader(HttpContext.Current.Server.MapPath($@"\\App_Data\\ReportTemplate\\{templateName}")))
                {
                    body = sr.ReadToEnd();
                }
                return body;
            }
            catch (Exception ex)
            {
                LogError(ex, "ReadReportTemplate");
                return "";
            }
        }
        #endregion

        #region Get File Size In Text
        private static string ThreeNonZeroDigits(double value)
        {
            if (value >= 100)
            {
                // No digits after the decimal.
                return value.ToString("0,0");
            }
            else if (value >= 10)
            {
                // One digit after the decimal.
                return value.ToString("0.0");
            }
            else
            {
                // Two digits after the decimal.
                return value.ToString("0.00");
            }
        }

        public static string ToFileSize(double value)
        {
            string[] suffixes = { "bytes", "KB", "MB", "GB",
                "TB", "PB", "EB", "ZB", "YB"};
            for (var i = 0; i < suffixes.Length; i++)
            {
                if (value <= (Math.Pow(1024, i + 1)))
                {
                    return ThreeNonZeroDigits(value /
                                              Math.Pow(1024, i)) +
                           " " + suffixes[i];
                }
            }

            return ThreeNonZeroDigits(value /
                                      Math.Pow(1024, suffixes.Length - 1)) +
                   " " + suffixes[suffixes.Length - 1];
        }
        #endregion

        #region Url
        public static bool IsUrlValid(string url)
        {
            string pattern = @"^(http|https|ftp|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$";
            Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return !string.IsNullOrEmpty(url) && reg.IsMatch(url);
        }
        #endregion

        #region Check Directory Path

        public static void CheckServerPath(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public static void DeleteFile(string path, bool isFullPath = false)
        {
            var fullPath = isFullPath ? path : HttpContext.Current.Server.MapPath(path);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
        #endregion

        #region ErrorLog
        public static void LogError(Exception ex, string appType = "")
        {
            try
            {
                var errorlogService = UnityFactory.CreateContext();
                var entityValidationError = "";
                if (ex is DbEntityValidationException validationException)
                {
                    entityValidationError = Environment.NewLine + "Encountered entity validation exception. Details follow:";
                    entityValidationError = validationException.EntityValidationErrors.SelectMany(ve => ve.ValidationErrors).Aggregate(entityValidationError, (current, eve) => current + (Environment.NewLine + $"Field: {eve.PropertyName}, Error: {eve.ErrorMessage}"));
                }

                // You could use any logging approach here
                var log = new ErrorLog
                {
                    Source = ex.Source,
                    AppType = appType,
                    TargetSite = ex.TargetSite.Name,
                    Type = ex.GetType().Name,
                    Message = ex.Message + entityValidationError,
                    Stack = ex.StackTrace,
                    InnerExceptionMessage = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.Message + "Inner Exception:" + ex.InnerException.InnerException.Message : ex.InnerException.Message : string.Empty,
                    InnerStackTrace = ex.InnerException?.StackTrace,
                    LogDate = DateTime.UtcNow
                };
                errorlogService.InsertData(log);
                errorlogService.SaveData();
            }
            catch (Exception ex1)
            {
                var entityValidationError = "";
                if (ex1 is DbEntityValidationException validationException)
                {
                    entityValidationError = Environment.NewLine + "Encountered entity validation exception. Details follow:";
                    entityValidationError = validationException.EntityValidationErrors.SelectMany(ve => ve.ValidationErrors).Aggregate(entityValidationError, (current, eve) => current + (Environment.NewLine + $"Field: {eve.PropertyName}, Error: {eve.ErrorMessage}"));
                }
                var log = new ErrorLog
                {
                    Source = ex1.Source,
                    AppType = appType,
                    TargetSite = ex1.TargetSite.Name,
                    Type = ex1.GetType().Name,
                    Message = ex1.Message + entityValidationError,
                    Stack = ex1.StackTrace,
                    InnerExceptionMessage = ex1.InnerException != null ? ex1.InnerException.InnerException != null ? ex1.InnerException.Message + "Inner Exception:" + ex1.InnerException.InnerException.Message : ex1.InnerException.Message : string.Empty,
                    InnerStackTrace = ex1.InnerException?.StackTrace,
                    LogDate = DateTime.UtcNow
                };
                WriteFile(log);
            }
        }
        public static void LogsInformation(ErrorLog logs, string appType = "")
        {
            var errorlogService = UnityFactory.CreateContext();
            var log = new ErrorLog
            {
                Source = logs.Source,
                AppType = appType,
                TargetSite = logs.TargetSite,
                Type = logs.GetType().Name,
                Message = logs.Message,
                Stack = logs.Stack,
                InnerExceptionMessage = logs.InnerExceptionMessage != null ? logs.InnerExceptionMessage : string.Empty,
                InnerStackTrace =  "",
                LogDate = DateTime.UtcNow
            };
            errorlogService.InsertData(log);
            errorlogService.SaveData();
        }
            private static void WriteFile(ErrorLog logs)
        {
//            var fileName = "Error_Log_" + DateTime.Today.ToString("dd-MM-yyyy") + ".txt";
//            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/") + fileName;
//            var logFileInfo = new FileInfo(path);
//            var fileStream = !logFileInfo.Exists ? logFileInfo.Create() : new FileStream(path, FileMode.Append);
//            var log = new StreamWriter(fileStream);
//            var errorString = $@"AppType :- {Environment.NewLine}
//{logs.AppType}
//{Environment.NewLine}
//Source :-
//{Environment.NewLine}
//{logs.Source}
//{Environment.NewLine}
//TargetSite :-
//{Environment.NewLine}
//{logs.TargetSite}
//{Environment.NewLine}
//Type :-
//{Environment.NewLine}
//{logs.Type}
//{Environment.NewLine}
//Message :-
//{Environment.NewLine}
//{logs.Message}
//{Environment.NewLine}
//Stack :-
//{logs.Stack}
//{Environment.NewLine}
//InnerExceptionMessage
//{Environment.NewLine}
//{logs.InnerExceptionMessage}
//{Environment.NewLine}
//InnerStackTrace
//{Environment.NewLine}
//{logs.InnerStackTrace}";
//            log.WriteLine(errorString);
//            log.Close();
//        }

//        public static void WriteLog()
//        {
//            // get the base directory
//            var baseDir = AppDomain.CurrentDomain.BaseDirectory;

//            // search the file below the current directory
//            var oldFile = $"ErrorTest_{DateTime.Now.AddDays(-1).Day}-{DateTime.Now.Month}-{DateTime.Now.Year}.txt";
//            if (System.IO.File.Exists(baseDir + oldFile))
//            {
//                System.IO.File.Delete(baseDir + oldFile);
//            }
//            var fileName = $"ErrorTest_{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}.txt";
//            var retFilePath = baseDir + fileName;

//            // Create a writer and open the file:

//            var log = !File.Exists(retFilePath) ? new StreamWriter(retFilePath) : File.AppendText(retFilePath);

//            var strDate = "Date  : " + DateTime.Now;
//            var strdescription = "Error :";
//            const string eol = "^^---------------------------------------------------------^^";

//            // Write log entries to the file:
//            log.WriteLine(strDate);
//            log.WriteLine(strdescription);
//            log.WriteLine(eol);

//            // Close the stream:
//            log.Close();
        }

        public static void WriteLog(string message)
        {
            // get the base directory
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;

            // search the file below the current directory
            var fileName = $"ErrorTest_{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}-Log.txt";
            var retFilePath = baseDir + fileName;

            // Create a writer and open the file:

            var log = !File.Exists(retFilePath) ? new StreamWriter(retFilePath) : File.AppendText(retFilePath);
            var strDate = "Date  : " + DateTime.Now;
            log.WriteLine(strDate);
            var strdescription = "Log :";
            log.WriteLine(strdescription);
            const string eol = "^^---------------------------------------------------------^^";
            log.WriteLine(eol);
            log.WriteLine(message);
            // Write log entries to the file:

            // Close the stream:
            log.Close();
        }

        #endregion

        #region DataTable Or List Mehtod

        //public static List<T> ToListof<T>(DataTable dt)
        //{
        //    const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
        //    var columnNames = dt.Columns.Cast<DataColumn>()
        //        .Select(c => c.ColumnName)
        //        .ToList();
        //    var objectProperties = typeof(T).GetProperties(flags);
        //    var targetList = dt.AsEnumerable().Select(dataRow =>
        //    {
        //        var instanceOfT = Activator.CreateInstance<T>();

        //        foreach (var properties in objectProperties.Where(properties => columnNames.Contains(properties.Name) && dataRow[properties.Name] != DBNull.Value))
        //        {
        //            properties.SetValue(instanceOfT, dataRow[properties.Name], null);
        //        }
        //        return instanceOfT;
        //    }).ToList();

        //    return targetList;
        //}

        /// <summary>
        /// Gets a Inverted DataTable
        /// </summary>
        /// <param name="table">DataTable do invert</param>
        /// <param name="columnX">X Axis Column</param>
        /// <param name="columnsToIgnore">Columns that should be ignored in the pivot 
        /// process (X Axis column is ignored by default)</param>
        /// <returns>C# Pivot Table Method  - Felipe Sabino</returns>
        public static DataTable GetInversedDataTable(DataTable table, string columnX, params string[] columnsToIgnore)
        {
            //Create a DataTable to Return
            var returnTable = new DataTable();

            if (columnX == "")
                columnX = table.Columns[0].ColumnName;

            //Add a Column at the beginning of the table

            returnTable.Columns.Add(columnX);

            //Read all DISTINCT values from columnX Column in the provided DataTale
            var columnXValues = new List<string>();

            //Creates list of columns to ignore
            var listColumnsToIgnore = new List<string>();
            if (columnsToIgnore.Length > 0)
                listColumnsToIgnore.AddRange(columnsToIgnore);

            if (!listColumnsToIgnore.Contains(columnX))
                listColumnsToIgnore.Add(columnX);

            foreach (DataRow dr in table.Rows)
            {
                var columnXTemp = dr[columnX].ToString();
                //Verify if the value was already listed
                if (!columnXValues.Contains(columnXTemp))
                {
                    //if the value id different from others provided, add to the list of 
                    //values and creates a new Column with its value.
                    columnXValues.Add(columnXTemp);
                    returnTable.Columns.Add(columnXTemp);
                }
                else
                {
                    //Throw exception for a repeated value
                    throw new Exception("The inversion used must have " +
                                        "unique values for column " + columnX);
                }
            }

            //Add a line for each column of the DataTable

            foreach (DataColumn dc in table.Columns)
            {
                if (listColumnsToIgnore.Contains(dc.ColumnName)) continue;
                var dr = returnTable.NewRow();
                dr[0] = dc.ColumnName;
                returnTable.Rows.Add(dr);
            }

            //Complete the datatable with the values
            for (var i = 0; i < returnTable.Rows.Count; i++)
            {
                for (var j = 1; j < returnTable.Columns.Count; j++)
                {
                    returnTable.Rows[i][j] =
                        table.Rows[j - 1][returnTable.Rows[i][0].ToString()].ToString();
                }
            }

            return returnTable;
        }

        /// <summary>
        /// Convert Datatable To Generic List
        /// </summary>
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
        /// Convert Generic List To Datatable 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataTable ConvertListToDatatable<T>(List<T> data)
        {
            var props = TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();
            for (var i = 0; i < props.Count; i++)
            {
                var prop = props[i];
                if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    table.Columns.Add(prop.Name, prop.PropertyType.GetGenericArguments()[0]);
                else
                    table.Columns.Add(prop.Name, prop.PropertyType);
            }
            var values = new object[props.Count];
            foreach (var item in data)
            {
                for (var i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
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
                        if((pro.PropertyType == typeof(int?) || pro.PropertyType == typeof(int) || pro.PropertyType == typeof(DateTime?)) && dr[column.ColumnName] == DBNull.Value )
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

        /// <summary>
        /// Read Data From Excel File
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static DataTable GetExcelfiledata(string filePath)
        {
            var strcon = string.Empty;
            var strFileType = Path.GetExtension(filePath);
            if (strFileType == ".xlsp")
            {
                //strcon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source="
                //                + filePath +
                //                ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1;TypeGuessRows=0;ImportMixedTypes=Text\"";
            }
            else
            {
                strcon = @"Provider=Microsoft.ACE.OLEDB.12.0;";
                strcon += @"Data Source=" + filePath + ";";
                strcon += @"Extended Properties=""Excel 12.0 xml;HDR=YES;Imex=1;""";
                strcon = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source="
                         + filePath +
                         ";Extended Properties=\"Excel 12.0 xml;HDR=YES;IMEX=1;TypeGuessRows=0;ImportMixedTypes=Text\"";

                strcon = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath +
                         ";Extended Properties=\"Excel 12.0 xml;HDR=YES;IMEX=1;TypeGuessRows=0;ImportMixedTypes=Text\"";
            }
            DataTable dtexcel = new DataTable();

            using (OleDbConnection excelCon = new OleDbConnection(strcon))
            {
                try
                {
                    excelCon.Open();

                    //Start::Get Sheet Name Dynamically
                    DataTable dtSheetNames = excelCon.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dtSheetNames == null)
                    {
                        return null;
                    }
                    string strselect = "Select * from [" + dtSheetNames.Rows[0]["TABLE_NAME"] + "]";

                    using (OleDbDataAdapter exDA = new OleDbDataAdapter(strselect, excelCon))
                    {
                        exDA.Fill(dtexcel);
                    }
                }
                catch (OleDbException oledb)
                {
                    throw new Exception(oledb.Message.ToString());
                }
                finally
                {
                    excelCon.Close();
                }
            }

            return dtexcel;
        }

        /// <summary>
        /// Convert Data from datatable to json string
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string DataTableToJsonWithJsonNet(DataTable table)
        {
            var jsonString = JsonConvert.SerializeObject(table);
            return jsonString;
        }

        /// <summary>
        /// Convert Datatable to dynamic IEnumerable
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        //public static IEnumerable<dynamic> AsDynamicEnumerable(DataTable table)
        //{
        //    // Validate argument here..
        //    return table.AsEnumerable().Select(row => new DynamicRow(row));
        //}
        #endregion

        #region XML Methods
        /// <summary>
        /// Convert Generic List To XML Document
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static XmlDocument ConvertToXml(object list)
        {
            var xmlDoc = new XmlDocument();
            var xmlSerializer = new XmlSerializer(list.GetType());
            using (var xmlStream = new MemoryStream())
            {
                xmlSerializer.Serialize(xmlStream, list);
                xmlStream.Position = 0;
                xmlDoc.Load(xmlStream);
                return xmlDoc;
            }
        }

        #endregion

        #region OTP Methods
        /// <summary>
        /// Send OTP CODE
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool SendOtpCode(string mobile, string message)
        {
            var request = (HttpWebRequest)WebRequest.Create("http://login.redsms.in/API/SendMessage.ashx?user=dhavalpokiya&password=pokiya155&phone=" + mobile.Trim() + "&text=" + message + "&type=t&senderid=DBCACC");
            var response = (HttpWebResponse)request.GetResponse();
            var stream = response.GetResponseStream();
            if (stream != null)
            {
                var reader = new StreamReader(stream);
                var result = reader.ReadToEnd();
                reader.Close();
                return result.Contains("sent");
            }
            else
            {
                return false;
            }
            //return !result.Contains("<error-code>");
        }
        /// <summary>
        /// Create OTP
        /// </summary>
        /// <returns></returns>
        public static int RendomNumber()
        {
            var rnd = new Random();
            return rnd.Next(100000, 999999);
        }
        #endregion

        #region EncryptDecrypt

        /// <summary>
        /// Encrypt Password for security purpose
        /// </summary>
        /// <param name="clearText"></param>
        /// <returns></returns>
        public static string EncryptPassword(string clearText)
        {
            var encryptionKey = RequestHelpers.GetConfigValue("EncryptDecryptKey");
            var clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (var encryptor = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                if (encryptor == null) return clearText;
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        /// <summary>
        /// Decrypt Password from encrypted password
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public static string DecryptPassword(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return cipherText;
            var encryptionKey = RequestHelpers.GetConfigValue("EncryptDecryptKey");
            var cipherBytes = Convert.FromBase64String(cipherText);
            using (var encryptor = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                if (encryptor == null) return cipherText;
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public static string EncodeMd5(string pass) //Encrypt using MD5    
        {
            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)    
            MD5 md5 = new MD5CryptoServiceProvider();
            var originalBytes = Encoding.Default.GetBytes(pass);
            var encodedBytes = md5.ComputeHash(originalBytes);
            //Convert encoded bytes back to a 'readable' string    
            return BitConverter.ToString(encodedBytes);
        }

        #endregion

        #region DataTable To List Convert All Dynamic Start
        private sealed class DynamicRow : DynamicObject
        {
            private readonly DataRow _row;

            internal DynamicRow(DataRow row) { _row = row; }

            // Interprets a member-access as an indexer-access on the 
            // contained DataRow.
            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                var retVal = _row.Table.Columns.Contains(binder.Name);
                result = retVal ? _row[binder.Name] : null;
                return retVal;
            }
        }
        //public static List<dynamic> ToDynamicList(DataTable dt, string className)
        //{
        //    return ToDynamicList(ToDictionary(dt), GetNewObject(dt.Columns, className));
        //}
        //private static List<Dictionary<string, object>> ToDictionary(DataTable dt)
        //{
        //    var columns = dt.Columns.Cast<DataColumn>();
        //    var Temp = dt.AsEnumerable().Select(dataRow => columns.Select(column =>
        //            new { Column = column.ColumnName, Value = dataRow[column] })
        //        .ToDictionary(data => data.Column, data => data.Value)).ToList();
        //    return Temp.ToList();
        //}
        private static List<dynamic> ToDynamicList(IEnumerable<Dictionary<string, object>> list, Type TypeObj)
        {
            dynamic temp = new List<dynamic>();
            foreach (var step in list)
            {
                var obj = Activator.CreateInstance(TypeObj);

                var properties = obj.GetType().GetProperties();

                var dictList = (Dictionary<string, object>)step;

                foreach (var keyValuePair in dictList)
                {
                    foreach (var property in properties)
                    {
                        if (property.Name != keyValuePair.Key) continue;
                        if (keyValuePair.Value != null && keyValuePair.Value.GetType() != typeof(System.DBNull))
                        {
                            if (keyValuePair.Value is Guid)
                            {
                                property.SetValue(obj, keyValuePair.Value, null);
                            }
                            else
                            {
                                property.SetValue(obj, keyValuePair.Value, null);
                            }
                        }
                        break;
                    }
                }
                temp.Add(obj);
            }
            return temp;
        }
        private static Type GetNewObject(IEnumerable columns, string className)
        {
            var assemblyName = new AssemblyName { Name = "YourAssembly" };
            var assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var module = assemblyBuilder.DefineDynamicModule("YourDynamicModule");
            var typeBuilder = module.DefineType(className, TypeAttributes.Public);

            foreach (DataColumn column in columns)
            {
                var propertyName = column.ColumnName;
                var field = typeBuilder.DefineField(propertyName, column.DataType, FieldAttributes.Public);
                var property = typeBuilder.DefineProperty(propertyName, System.Reflection.PropertyAttributes.None, column.DataType, new Type[] { column.DataType });
                var getSetAttr = MethodAttributes.Public | MethodAttributes.HideBySig;
                var currGetPropMthdBldr = typeBuilder.DefineMethod("get_value", getSetAttr, column.DataType, new Type[] { column.DataType }); // Type.EmptyTypes);
                var currGetIl = currGetPropMthdBldr.GetILGenerator();
                currGetIl.Emit(OpCodes.Ldarg_0);
                currGetIl.Emit(OpCodes.Ldfld, field);
                currGetIl.Emit(OpCodes.Ret);
                var currSetPropMthdBldr = typeBuilder.DefineMethod("set_value", getSetAttr, null, new Type[] { column.DataType });
                var currSetIl = currSetPropMthdBldr.GetILGenerator();
                currSetIl.Emit(OpCodes.Ldarg_0);
                currSetIl.Emit(OpCodes.Ldarg_1);
                currSetIl.Emit(OpCodes.Stfld, field);
                currSetIl.Emit(OpCodes.Ret);
                property.SetGetMethod(currGetPropMthdBldr);
                property.SetSetMethod(currSetPropMthdBldr);
            }
            var obj = typeBuilder.CreateType();
            return obj;
        }
        #endregion DataTable To List Convert All Dynamic End

        #region Get Client Information
        public static void GetClientInformation()
        {
            var browserName = HttpContext.Current.Session["BrowserName"].ToString();
            var browserVersion = HttpContext.Current.Session["Bvrsn"].ToString();
            var ipAddress = HttpContext.Current.Session["IpAddress"].ToString();
            var compName = Environment.MachineName;
            var compUserName = Environment.UserName;
            var domainName = Environment.UserDomainName;


            var nics = NetworkInterface.GetAllNetworkInterfaces();
            var mac = "";
            for (var j = 0; j <= 1; j++)
            {
                var address = nics[j].GetPhysicalAddress();
                var bytes = address.GetAddressBytes();
                for (var i = 0; i < bytes.Length; i++)
                {
                    mac = mac + bytes[i].ToString("X2");
                    if (i != bytes.Length - 1)
                    {
                        mac = mac + ("-");
                    }
                }
            }

        }
        #endregion

        #region Image functionality
        //public static string SaveBarcodeImages(string barcodeText, string folder, int Width, int Height)
        //{
        //    using (BarcodeLib.Barcode b = new BarcodeLib.Barcode())
        //    {
        //        if (!Directory.Exists(HttpContext.Current.Server.MapPath(folder)))
        //        {
        //            Directory.CreateDirectory(HttpContext.Current.Server.MapPath(folder));
        //        }
        //        BarcodeLib.TYPE type = BarcodeLib.TYPE.CODE39;
        //        MemoryStream ms = new MemoryStream();
        //        if (type != BarcodeLib.TYPE.UNSPECIFIED)
        //        {
        //            b.IncludeLabel = true;
        //            if (Width == 0 || Height == 0)
        //            {
        //                (b.Encode(type, barcodeText.ToUpper().Trim())).Save(HttpContext.Current.Server.MapPath(folder + barcodeText + ".jpg"), System.Drawing.Imaging.ImageFormat.Jpeg);
        //                //(b.Encode(type, barcodeText.ToUpper().Trim(), Color.Black, Color.White, Width, Height)).Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
        //            }
        //            else
        //            {
        //                (b.Encode(type, barcodeText.ToUpper().Trim(), Color.Black, Color.White, Width, Height)).Save(HttpContext.Current.Server.MapPath(folder + barcodeText + ".jpg"), System.Drawing.Imaging.ImageFormat.Jpeg);
        //                //(b.Encode(type, barcodeText.ToUpper().Trim(), Color.Black, Color.White, Width, Height)).Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
        //            }
        //        }
        //        //return ms.GetBuffer();
        //        return folder + barcodeText + ".jpg";
        //    }
        //}

        public static void SaveCompressImage(System.IO.Stream stream, string path, int quality)
        {
            Image myImage = null;
            myImage = Image.FromStream(stream);
            var qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            var jpegCodec = GetEncoderInfo("image/jpeg");
            var encoderParams = new EncoderParameters(1) { Param = { [0] = qualityParam } };
            myImage.Save(path, jpegCodec, encoderParams);
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            var codecs = ImageCodecInfo.GetImageEncoders();
            return codecs.FirstOrDefault(t => t.MimeType == mimeType);
        }

        public static void ResizeImage(Stream stream, int width, int height, int quality, string filePath)
        {
            Image image = null;
            image = System.Drawing.Image.FromStream(stream);

            var newImage = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            using (var graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.DrawImage(image, 0, 0, width, height);
                //graphics.CompositingMode = CompositingMode.SourceCopy;

            }

            var imageCodecInfo = GetEncoderInfo("image/jpeg");
            var encoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            var encoderParameters = new EncoderParameters(1) { Param = { [0] = encoderParameter } };
            newImage.Save(filePath, imageCodecInfo, encoderParameters);
        }
        public static void AddWaterMarkImage(string fromfilePath, string filePath)
        {
            using (var image = Image.FromFile(fromfilePath))
            using (var watermarkImage = Image.FromFile(HttpContext.Current.Server.MapPath("~/Images/watermark.png")))
            using (var imageGraphics = Graphics.FromImage(image))
            using (var watermarkBrush = new TextureBrush(watermarkImage))
            {
                var x = (image.Width - watermarkImage.Width) - 10;
                var y = (image.Height - watermarkImage.Height) - 10;
                watermarkBrush.TranslateTransform(x, y);
                imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(x, y), new Size(watermarkImage.Width + 1, watermarkImage.Height)));
                image.Save(filePath);
            }
        }

        public static void GetThumbnailImage(Stream stream, int width, int height, string filePath)
        {
            var thWidth = width;
            var thHeight = height;
            var i = System.Drawing.Image.FromStream(stream);
            var w = i.Width;
            var h = i.Height;
            int th;
            int tw;
            if (h > w)
            {
                var ratio = (double)w / (double)h;
                th = thHeight < h ? thHeight : h;
                tw = thWidth < w ? (int)(ratio * thWidth) : w;
            }
            else
            {
                var ratio = (double)h / (double)w;
                th = thHeight < h ? (int)(ratio * thHeight) : h;
                tw = thWidth < w ? thWidth : w;
                //th = thHeight;
                //tw = thWidth;
            }
            var target = new Bitmap(tw, th);
            var g = Graphics.FromImage(target);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            var rect = new Rectangle(0, 0, tw, th);
            g.DrawImage(i, rect, 0, 0, w, h, GraphicsUnit.Pixel);
            var img = (Image)target;
            img.Save(filePath);

            // return (Image)target;
        }

        public static void ResizePicture(string originalpath, string newpath, int width, int height)
        {
            using (Bitmap newbmp = new Bitmap(width, height), oldbmp = Image.FromFile(originalpath) as Bitmap)
            {
                using (var newgraphics = Graphics.FromImage(newbmp))
                {
                    newgraphics.Clear(Color.FromArgb(-1));
                    if ((float)oldbmp.Width / (float)width == (float)oldbmp.Height / (float)height) //Target size has a 1:1 aspect ratio
                    {
                        newgraphics.DrawImage(oldbmp, 0, 0, width, height);
                    }


                    else if ((float)oldbmp.Width / (float)width > (float)oldbmp.Height / (float)height) //There will be white space on the top and bottom
                    {
                        newgraphics.DrawImage(oldbmp, 0f, (float)newbmp.Height / 2f - (oldbmp.Height * ((float)newbmp.Width / (float)oldbmp.Width)) / 2f, (float)newbmp.Width, oldbmp.Height * ((float)newbmp.Width / (float)oldbmp.Width));
                    }


                    else if ((float)oldbmp.Width / (float)width < (float)oldbmp.Height / (float)height) //There will be white space on the sides
                    {
                        newgraphics.DrawImage(oldbmp, (float)newbmp.Width / 2f - (oldbmp.Width * ((float)newbmp.Height / (float)oldbmp.Height)) / 2f, 0f, oldbmp.Width * ((float)newbmp.Height / (float)oldbmp.Height), (float)newbmp.Height);
                    }


                    newgraphics.Save();
                    newbmp.Save(newpath, ImageFormat.Jpeg);
                }
            }
        }

        public static bool UploadFile(HttpPostedFileBase file, string mapPath)
        {
            if (file.ContentLength <= 0) return false;
            var filePath = mapPath;
            if (File.Exists(filePath)) return false;
            file.SaveAs(filePath);
            return true;
        }

        #endregion

        #region File Encryption & Decryption
        public static void FileEncryption(HttpPostedFileBase file, string filePath, string fileName)
        {
            byte[] keyBytes = Encoding.ASCII.GetBytes(RequestHelpers.GetConfigValue("FileEncryptDecryptKey"));
            DESCryptoServiceProvider cryptic = new DESCryptoServiceProvider
            {
                Key = keyBytes,
                IV = keyBytes
            };
            fileName = fileName + ".txt";
            string outPutFilePath = Path.Combine(filePath, fileName);
            using (FileStream fsOutput = new FileStream(outPutFilePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (CryptoStream cs = new CryptoStream(fsOutput, cryptic.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    byte[] fileLength = new byte[file.ContentLength];
                    foreach (var data in fileLength)
                    {
                        cs.WriteByte(data);
                    }
                }
            }
        }

        public static void FileDecryption(string filePath, string fileName, string originalName)
        {
            byte[] keyBytes = Encoding.ASCII.GetBytes(RequestHelpers.GetConfigValue("FileEncryptDecryptKey"));
            DESCryptoServiceProvider cryptic = new DESCryptoServiceProvider
            {
                Key = keyBytes,
                IV = keyBytes
            };
            string inputFilePath = Path.Combine(filePath, fileName + ".txt");
            string outPutFilePath = Path.Combine(filePath, originalName);
            using (var fsInput = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            {
                using (var cs = new CryptoStream(fsInput, cryptic.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (var fsOutput = new FileStream(outPutFilePath, FileMode.Create))
                    {
                        int data;
                        while ((data = cs.ReadByte()) != -1)
                        {
                            fsOutput.WriteByte((byte)data);
                        }
                    }
                }
            }
        }
        #endregion

        public static DateTime UtcToUserTime()
        {
            #region For Global TimeZone
            //var currentTimeZone = TimeZone.CurrentTimeZone;
            //TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(currentTimeZone.StandardName);
            //var date = TimeZoneInfo.ConvertTimeFromUtc(value, timeZoneInfo); 
            #endregion

            #region Only For Eastern Standard Time
            string standardTimeZoneName = RequestHelpers.GetConfigValue("DefaultTimeZone");
            var timezone = TimeZoneInfo.FindSystemTimeZoneById(standardTimeZoneName);
            var date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timezone);
            #endregion

            return date;
        }

        public static bool IsFileExists(string imagePath = "")
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                if (File.Exists(HttpContext.Current.Server.MapPath(imagePath)))
                {
                    return true;
                }
            }
            return false;
        }

        public static string GetFeaturedUrl(int userType)
        {
            switch (userType)
            {
                case 1:
                    return "/Profile/Pharmacy";
                case 2:
                    return "/Profile/Facility";
                case 3:
                    return "/Profile/SeniorCare";
                default:
                    return "/Profile/Pharmacy";
            }
        }

        public static string GetValidProfileImagePath(string imageName)
        {
            if (!string.IsNullOrEmpty(imageName))
            {
                if (File.Exists(HttpContext.Current.Server.MapPath("/Uploads/DoctorSiteImages/" + imageName)))
                {
                    return "/Uploads/DoctorSiteImages/" + imageName;
                }
                else if (File.Exists(HttpContext.Current.Server.MapPath("/Uploads/FacilitySiteImages/" + imageName)))
                {
                    return "/Uploads/FacilitySiteImages/" + imageName;
                }
                else if (File.Exists(HttpContext.Current.Server.MapPath("/Uploads/PharmacySiteImages/" + imageName)))
                {
                    return "/Uploads/PharmacySiteImages/" + imageName;
                }
                else if (File.Exists(HttpContext.Current.Server.MapPath("/Uploads/SeniorCareProfile/" + imageName)))
                {
                    return "/Uploads/SeniorCareProfile/" + imageName;
                }
            }
            return "/Uploads/ProfilePic/no_picture.png";
        }

        public static string GetOpenUntilTimeString(string time)
        {
            string returnText = string.Empty;
            if (!string.IsNullOrEmpty(time))
            {
                returnText = "Open today until {0}";
                try
                {
                    var timeFromInput = DateTime.ParseExact(time, "hh:mm tt", null, DateTimeStyles.None);
                    string minute = timeFromInput.ToString("mm", CultureInfo.InvariantCulture);
                    if (!string.IsNullOrEmpty(minute) && minute != "00")
                        return string.Format(returnText, timeFromInput.ToString("h:m tt", CultureInfo.InvariantCulture));

                    return string.Format(returnText, timeFromInput.ToString("h tt", CultureInfo.InvariantCulture));
                }
                catch(Exception ex)
                {
                    return string.Empty;
                }
            }
            return returnText;
        }
        public static string FormatLocation(string location, IpInfo info)
        {
            string slocation = "";
            if (location != null)
            {
                slocation = location.Replace(", ", "|");
            }
            else
            {
                slocation = info.City + "|" + info.region_code + "|" + info.zip;
            }
            return slocation;
        }

        public string GetIPString()
        {

            string VisitorsIPAddr = string.Empty;
            if (System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] != null)
            {
                VisitorsIPAddr = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                string log = "My client IP: " + VisitorsIPAddr;
                AddLogs(log);
                //VisitorsIPAddr = "106.197.46.85";
            }
            if (VisitorsIPAddr.Split('.').Length == 4)
            {
                try
                {
                   
                    var GetIPApiUri = ConfigurationManager.AppSettings["GetIpAddressLocation"].Replace("ipaddress", VisitorsIPAddr);
                    //var log1 = "GetIPApiUri : " + GetIPApiUri;
                    //AddLogs(log1);
                    string info = new WebClient().DownloadString(GetIPApiUri);
                    return info;
                }
                catch (Exception ex)
                {
                    string log2 = " exception : " + ex.Message;
                    AddLogs(log2);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public  void AddLogs(string message)
        {
            var log = new ErrorLog() { Type = "IP_IpAddress", Message = message, AppType = "SEARCH_CONT_LOG" };
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@Message", SqlDbType.VarChar) { Value = log.Message == null ? "" : log.Message });
            parameters.Add(new SqlParameter("@type", SqlDbType.VarChar) { Value = log.Type });
            parameters.Add(new SqlParameter("@apptype", SqlDbType.VarChar) { Value = log.AppType });

            try
            {
                _doctor.AddOrUpdateExecuteProcedure("AddErrorLog", parameters);
            }
            catch (Exception ex)
            {

            }
        }

        public  IpInfo GetRemoteLocation()
        {
            
            string ipString = GetIPString();
            //var str = "ipString :" + ipString;
            //AddLogs(str);
            //ipString = "106.197.46.85";
            IpInfo ipInfo = null;
            try
            {
                ipInfo = new IpInfo();
                if (!string.IsNullOrEmpty(ipString))
                    ipInfo = JsonConvert.DeserializeObject<IpInfo>(ipString);
                return ipInfo;
            }
            catch
            {
                return ipInfo;
            }
        }

        public static List<Dictionary<string, object>> ConvertToList(DataTable table)
        {
            var list = new List<Dictionary<string, object>>();
            foreach (DataRow row in table.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    dict[col.ColumnName] = row[col];
                }
                list.Add(dict);
            }
            return list;
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //return serializer.Serialize(list);
        }


        public static string GetPublicIPOfServer()
        {
            JObject jObj = new JObject();
            jObj.Add("ip", "");
            WebRequest request = WebRequest.Create("https://api.ipify.org/?format=json");
            using (WebResponse response = request.GetResponse())
            using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            {
                jObj = JObject.Parse(stream.ReadToEnd().ToString());
            }

            return jObj["ip"].ToString();
        }

       
        public static string GetIPString(string VisitorsIPAddr)//Added by Reena
        {
            JObject jobj = new JObject();
           
            try
            {
                if (VisitorsIPAddr.Split('.').Length == 4)
                {
                    var ApiUrl = ConfigurationManager.AppSettings["GetIpAddressLocation"].Replace("ipaddress", VisitorsIPAddr);
                    string info = new System.Net.WebClient().DownloadString(ApiUrl);
                    if (!string.IsNullOrEmpty(info))
                    {
                        return info;
                    }
                    else
                    {
                        return JsonConvert.SerializeObject(jobj);

                    }

                }

            }
            catch (Exception ex)
            {

            }
            return JsonConvert.SerializeObject(jobj);
        }

      
    }


}
