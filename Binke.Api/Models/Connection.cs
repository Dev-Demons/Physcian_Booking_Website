using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Configuration;
namespace Binke.Api.Models
{
    public class Connection
    {
        public static string sqlConStr = ConfigurationManager.ConnectionStrings["Doctyme"].ConnectionString;
        public static string InvalidDate = "Invalid Date. Please enter correct format";
        public static string NoProjectDefine = "No project is available.";
        public static string UNIQUE = "Record Already Exists";
        public static string SaveSuccess = "Saved Successfully.";
        public static string UpdateSuccess = "Updated Successfully.";
        public static string NotMoreThenCurrentDate = "Up to date can not be greater than Current date.";
        public static string NotMoreThenFromDate = "From date cannot be greater than Up to date.";
        public static string NoRecord = "No record found.";
        public static string DeleteSucess = "Deleted Successfully.";
        public static string ReferenceMessage = "This record can not be deleted as it has active reference to other modules.";
        public static string NotSave = "Some problem occured during save or update into the  database.";
        public static string InvalidAmount = "You are entering invalid amount.";
        public static string UserSignedUpSuccess = "User signedup successfully,kindly login with credential";
        public static string StatusSuccess = "Success";
        public static string UserSignedUpError = "There is some issue during sign up,please try after some time";
        public static string EmailAlreadyExists = "Email already exists";

      

        public Connection()
        {

            sqlConStr = ConfigurationManager.ConnectionStrings["Doctyme"].ConnectionString;
        }

        public static string ExecuteNonQuery(SqlCommand cmd)
        {
            string saveSuccess = string.Empty;
            int execute = 0;
            Connection objCon = new Connection();
            SqlConnection con = objCon.GetConnection();

            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            else
            {
                con.Open();
            }

            using (con)
            {
                try
                {

                    cmd.Connection = con;
                    if (cmd != null && cmd.CommandText == "SpManageHelp")
                    {
                        try
                        {
                            saveSuccess = HandleHelpProcess(cmd, SaveSuccess);
                        }
                        catch (SqlException ex)
                        {
                            saveSuccess = ex.Message.ToString();
                        }
                    }
                    else
                    {
                        execute = cmd.ExecuteNonQuery();
                        if (execute > 0)
                        {
                            saveSuccess = "Record Saved";
                        }
                        else
                            saveSuccess = "Operation Failed";


                    }
                }
                catch (Exception excp)
                {
                    if (excp.Message.Contains("Violation of PRIMARY KEY constraint"))
                        saveSuccess = Connection.UNIQUE;
                    else if (excp.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                        saveSuccess = Connection.ReferenceMessage;
                    else if (excp.Message.Contains("Violation of UNIQUE KEY constraint"))
                        saveSuccess = Connection.UNIQUE;
                    else if (excp.Message.Contains("Object reference not set to an instance of an object"))
                        saveSuccess = "Object are not set anywhere";
                    else if (excp.Message.Contains("Error converting data type nvarchar to float."))
                        saveSuccess = "Invalid opening balance.";
                    else
                        saveSuccess = excp.Message.ToString();
                }
            }
            return saveSuccess;
        }
        public static SqlDataReader ExecuteReader(SqlCommand cmd)
        {

            SqlDataReader dr = null;
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(sqlConStr);
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                else
                {
                    con.Open();
                }
                cmd.Connection = con;
                dr = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {

            }

            return dr;
        }
        public static DataSet GetData(SqlCommand cmd)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection())
            {
                SqlDataAdapter adap = new SqlDataAdapter();
                con.Open();

                cmd.Connection = con;
                cmd.CommandTimeout = 0;
                adap.SelectCommand = cmd;
                adap.Fill(ds, "table");

            }
            return ds;

        }
        private static string HandleHelpProcess(SqlCommand cmd, string saveSuccess)
        {
            int execute = 0;
            execute = cmd.ExecuteNonQuery();
            if (execute > 0)
            {
                saveSuccess = "success";
            }
            else
                saveSuccess = "error";
            return saveSuccess;
        }
        public static object ExecuteScaler(SqlCommand cmd)
        {
            object RetVal = null;
            using (SqlConnection con = new SqlConnection(sqlConStr))
            {

                con.Open();
                cmd.Connection = con;
                RetVal = cmd.ExecuteScalar();
            }
            return RetVal;
        }
        public static int Delete(int Id, string sp_name, string ActivityType)
        {
            using (SqlCommand cmd = new SqlCommand())
            {

                cmd.CommandText = sp_name;

                SqlConnection con = new SqlConnection(Connection.sqlConStr);
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                else
                {

                    con.Open();
                }
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Activity", ActivityType);
                cmd.Parameters.AddWithValue("@id", Id);

                int n_record_deleted = cmd.ExecuteNonQuery();
                return n_record_deleted;
            }
        }
        public static string CommandExecuteNonQuery(string Parameters, string Values, bool IsStoredProcedure, string command)
        {
            string[] Parameter = Parameters.Split(',');
            string[] Value = Values.Split(',');
            string s_Message = "";
            if (Parameter.Length < Value.Length)
            {
                s_Message = "Number of Paramters Supplied is less than Number of Values Supplied.";
                return s_Message;
            }
            else if (Parameter.Length > Value.Length)
            {
                s_Message = "Number of Values Supplied is less than Number of Parameters Supplied.";
                return s_Message;
            }
            else
            {
                SqlCommand cmd = new SqlCommand();
                for (var i = 0; i < Parameter.Length; i++)
                {
                    cmd.Parameters.AddWithValue(Parameter[i], Value[i]);

                }
                cmd.CommandText = command;
                if (IsStoredProcedure)
                    cmd.CommandType = CommandType.StoredProcedure;
                string saveSuccess = string.Empty;
                int execute = 0;
                using (SqlConnection con = new SqlConnection(sqlConStr))
                {
                    try
                    {
                        con.Open();
                        cmd.Connection = con;
                        execute = cmd.ExecuteNonQuery();
                        if (execute > 0)
                        {
                            s_Message = "Record Saved";
                        }
                        else
                            s_Message = "Operation Failed";



                    }
                    catch (Exception excp)
                    {
                        s_Message = excp.Message;
                    }

                }
            }
            return s_Message.ToString();
        }
        public SqlConnection GetConnection()
        {
            SqlConnection con = new SqlConnection(sqlConStr);
            return con;
        }
    }
}