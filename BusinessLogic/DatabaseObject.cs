using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public static class DatabaseObject
    {
        // FetchFromSP2
        public static DataTableCollection FetchFromSP(string SPName, SqlParameter[] sqlprms, ref ErrorResponse strError)
        {
            using (SqlConnection SqlCon = new SqlConnection(ConnectionString))
            {
                if (SqlCon.State == ConnectionState.Open)
                {
                    SqlCon.Close();
                }

                SqlCon.Open();
                using (SqlTransaction SqlTrans = SqlCon.BeginTransaction())
                {
                    try
                    {
                        DataSet ds = new DataSet("DataSet1");
                        using (SqlCommand cmd = new SqlCommand(SPName, SqlCon))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Transaction = SqlTrans;
                            if (sqlprms != null)
                            {
                                foreach (SqlParameter sqlp in sqlprms)
                                    cmd.Parameters.Add(sqlp);
                            }

                            using (SqlDataAdapter sdapt = new SqlDataAdapter(cmd))
                            {
                                sdapt.Fill(ds);
                            }
                        }

                        SqlTrans.Commit();
                        return ds.Tables;
                    }
                    catch (SqlException ae)
                    {
                        SqlTrans.Rollback();
                        strError.Error = true;
                        strError.ErrorList.Add(new ErrorList() { ErrorCode = ae.ErrorCode, Message = ae.Message, ErrorType = "SQL", WarningLevel = Level.Error });

                        Logs.WriteError("DBLayer", "FetchFromSP2(" + SPName + ")", "SQL", ae.Message);

                        return null;
                    }
                    catch (Exception ae)
                    {
                        SqlTrans.Rollback();
                        strError.Error = true;
                        strError.ErrorList.Add(new ErrorList() { ErrorCode = 999, Message = ae.Message, ErrorType = "General", WarningLevel = Level.Error });

                        Logs.WriteError("DBLayer", "FetchFromSP2(" + SPName + ")", "General", ae.Message);
                        return null;
                    }
                }
            }
        }

        public static DataTable FetchTableFromSP(string SPName, SqlParameter[] sqlprms, ref ErrorResponse strError)
        {
            DataTableCollection dtbls = FetchFromSP(SPName, sqlprms, ref strError);
            if (dtbls != null)
            {
                if (dtbls.Count > 0)
                {
                    return dtbls[0];
                }
            }

            return null;
        }

        public static string DLookupDB(string strQuery)
        {
            ErrorResponse response = new ErrorResponse();
            return DLookupDB(strQuery, ref response);
        }

        public static string GetScalarValue(string strQuery, params SqlParameter[] sqlprms)
        {
            using (SqlConnection SqlCon = new SqlConnection(ConnectionString))
            {
                SqlCon.Open();
                try
                {
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = SqlCon.CreateCommand())
                    {
                        cmd.CommandText = strQuery;
                        if (sqlprms != null)
                        {
                            foreach (SqlParameter param in sqlprms)
                                cmd.Parameters.Add(param);
                        }

                        return cmd.ExecuteScalar().ToString();
                    }
                }
                catch (SqlException ae)
                {
                    return "";
                }
                catch (Exception ae)
                {
                    return "";
                }
            }
        }

        public static string DLookupDB(string strQuery, ref ErrorResponse strError)
        {
            string Val = string.Empty;
            DataTable dt = new DataTable("Table1");
            dt = FetchTable(strQuery, ref strError);
            if (dt != null)
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    Val = row[0].ToString();
                }
            return Val;
        }

        public static string DLookupDB(string StoredProcedure, SqlParameter[] sqlprms)
        {
            ErrorResponse strError = new ErrorResponse();
            return DLookupDB(StoredProcedure, sqlprms, ref strError);
        }

        public static string DLookupDB(string StoredProcedure, SqlParameter[] sqlprms, ref ErrorResponse strError)
        {
            string Val = string.Empty;

            using (SqlConnection SqlCon = new SqlConnection(ConnectionString))
            {
                SqlCon.Open();
                try
                {
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = SqlCon.CreateCommand())
                    {
                        cmd.CommandText = StoredProcedure;
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (sqlprms != null)
                        {
                            foreach (SqlParameter param in sqlprms)
                                cmd.Parameters.Add(param);
                        }


                        SqlDataAdapter sdapt = new SqlDataAdapter(cmd);
                        sdapt.Fill(dt);
                    }

                    if (DataHelper.HasRows(dt))
                    {
                        Val = dt.Rows[0][0].ToString();
                    }
                }
                catch (SqlException ae)
                {
                    strError.Error = true;
                    strError.ErrorList.Add(new ErrorList() { ErrorCode = ae.ErrorCode, Message = ae.Message, ErrorType = "SQL", WarningLevel = Level.Error });

                    Logs.WriteError("DBLayer", "DLookupDB(" + StoredProcedure + ")", "SQL", ae.Message);

                    return null;
                }
                catch (Exception ae)
                {
                    strError.Error = true;
                    strError.ErrorList.Add(new ErrorList() { ErrorCode = 999, Message = ae.Message, ErrorType = "General", WarningLevel = Level.Error });

                    Logs.WriteError("DBLayer", "DLookupDB(" + StoredProcedure + ")", "General", ae.Message);

                    return null;
                }
            }

            return Val;
        }

        public static DataTable FetchTable(string strQuery, ref ErrorResponse strError)
        {
            if (strQuery == "") return null;

            using (SqlConnection SqlCon = new SqlConnection(ConnectionString))
            {
                if (SqlCon.State == ConnectionState.Open)
                {
                    SqlCon.Close();
                }
                SqlCon.Open();
                try
                {
                    using (SqlDataAdapter sdapt = new SqlDataAdapter(strQuery, SqlCon))
                    {
                        DataTable dt = new DataTable("Table1");
                        sdapt.Fill(dt);

                        return dt;
                    }
                }
                catch (SqlException ae)
                {
                    strError.Error = true;
                    strError.ErrorList.Add(new ErrorList() { ErrorCode = ae.ErrorCode, Message = ae.Message, ErrorType = "SQL", WarningLevel = Level.Error });

                    Logs.WriteError("DBLayer", "FetchTable1(" + strQuery + ")", "SQL", ae.Message);

                    return null;
                }
                catch (Exception ae)
                {
                    strError.Error = true;
                    strError.ErrorList.Add(new ErrorList() { ErrorCode = 999, Message = ae.Message, ErrorType = "General", WarningLevel = Level.Error });

                    Logs.WriteError("DBLayer", "FetchTable1(" + strQuery + ")", "General", ae.Message);

                    return null;
                }
            }
        }

        static string ConnectionString
        {
            get
            {
                if (ConfigurationManager.ConnectionStrings["ConnectionString"] != null)
                    return ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                else if (ConfigurationManager.AppSettings["ConnectionString"] != null)
                    return ConfigurationManager.AppSettings["ConnectionString"];
                else
                    return "";
            }
        }
        

        // ExecuteSP
        public static void ExecuteSP(string SPName, SqlParameter[] sqlprms, ref ErrorResponse strError)
        {
            using (SqlConnection SqlCon = new SqlConnection(ConnectionString))
            {
                SqlCon.Open();
                using (SqlTransaction SqlTrans = SqlCon.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(SPName, SqlCon))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Transaction = SqlTrans;
                            if (sqlprms != null)
                            {
                                foreach (SqlParameter sqlp in sqlprms)
                                    cmd.Parameters.Add(sqlp);
                            }


                            //  cmd.Parameters.Add(sqlprms);
                            cmd.Transaction = SqlTrans;
                            cmd.ExecuteNonQuery();
                        }

                        SqlTrans.Commit();
                    }
                    catch (SqlException sqle)
                    {
                        SqlTrans.Rollback();
                        strError.Error = true;
                        strError.ErrorList.Add(new ErrorList() { ErrorCode = sqle.ErrorCode, Message = sqle.Message, ErrorType = "SQL", WarningLevel = Level.Error });

                        Logs.WriteError("DBLayer", "ExecuteSP(" + SPName + "," + getspstring(sqlprms) + ")", "SQL", sqle.Message);
                    }
                    catch (Exception ae)
                    {
                        SqlTrans.Rollback();
                        strError.Error = true;
                        strError.ErrorList.Add(new ErrorList() { ErrorCode = 999, Message = ae.Message, ErrorType = "General", WarningLevel = Level.Error });

                        Logs.WriteError("DBLayer", "ExecuteSP(" + SPName + "," + getspstring(sqlprms) + ")", "General", ae.Message);
                    }
                }
            }


        }

        // ExecuteSP1
        public static void ExecuteSP(string SPName, ref ErrorResponse strError)
        {
            using (SqlConnection SqlCon = new SqlConnection(ConnectionString))
            {
                SqlCon.Open();
                using (SqlTransaction SqlTrans = SqlCon.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(SPName, SqlCon))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Transaction = SqlTrans;
                            cmd.ExecuteNonQuery();
                        }

                        SqlTrans.Commit();
                    }


                    catch (SqlException sqle)
                    {
                        SqlTrans.Rollback();
                        strError.Error = true;
                        strError.ErrorList.Add(new ErrorList() { ErrorCode = sqle.ErrorCode, Message = sqle.Message, ErrorType = "SQL", WarningLevel = Level.Error });

                        Logs.WriteError("DBLayer", "ExecuteSP1(" + SPName + ")", "SQL", sqle.Message);
                    }
                    catch (Exception ae)
                    {
                        SqlTrans.Rollback();
                        strError.Error = true;
                        strError.ErrorList.Add(new ErrorList() { ErrorCode = 999, Message = ae.Message, ErrorType = "General", WarningLevel = Level.Error });


                        Logs.WriteError("DBLayer", "FetchFromSP2(" + SPName + ")", "General", ae.Message);
                        Logs.WriteError("DBLayer", "ExecuteSP1(" + SPName + ")", "General", ae.Message);
                    }
                }
            }
        }

        // ExecuteSP2
        public static bool ExecuteSP_(string SPName, SqlParameter[] sqlprms, ref ErrorResponse strError)
        {
            using (SqlConnection SqlCon = new SqlConnection(ConnectionString))
            {



                SqlCon.Open();
                using (SqlTransaction SqlTrans = SqlCon.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(SPName, SqlCon))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            if (sqlprms != null)
                            {
                                foreach (SqlParameter param in sqlprms)
                                    cmd.Parameters.Add(param);
                            }

                            cmd.Transaction = SqlTrans;
                            cmd.ExecuteNonQuery();
                        }

                        SqlTrans.Commit();

                        return true;
                    }
                    catch (SqlException sqle)
                    {
                        SqlTrans.Rollback();
                        strError.Error = true;
                        strError.ErrorList.Add(new ErrorList() { ErrorCode = sqle.ErrorCode, Message = sqle.Message, ErrorType = "SQL", WarningLevel = Level.Error });

                        Logs.WriteError("DBLayer", "ExecuteSPNonQuery1(" + SPName + ")", "General", sqle.Message);
                        Logs.WriteError("DBLayer", "ExecuteSP(" + SPName + "," + getspsstring(sqlprms) + ")", "SQL", sqle.Message);

                        return false;
                    }
                    catch (Exception ae)
                    {
                        SqlTrans.Rollback();
                        strError.Error = true;
                        strError.ErrorList.Add(new ErrorList() { ErrorCode = 999, Message = ae.Message, ErrorType = "General", WarningLevel = Level.Error });
                        Logs.WriteError("DBLayer", "ExecuteSPNonQuery1(" + SPName + ")", "General", ae.Message);
                        Logs.WriteError("DBLayer", "ExecuteSP(" + SPName + "," + getspsstring(sqlprms) + ")", "General", ae.Message);

                        return false;
                    }
                }
            }
        }

        private static string getspsstring(SqlParameter[] sqlprms)

        {
            string str = string.Empty;
            if (sqlprms != null)
            {

                // str += "ParameterName: " + sqlprms.ParameterName + ", ParameterValue: " + sqlprms.Value;

                foreach (SqlParameter item in sqlprms)
                {
                    str += "ParameterName: " + item.ParameterName + ", ParameterValue: " + item.Value + ";";
                }

            }

            return str;
        }

        private static string getspstring(SqlParameter[] sqlprms)

        {
            string str = string.Empty;
            if (sqlprms != null)
            {

                foreach (SqlParameter item in sqlprms)
                {
                    str += "ParameterName: " + item.ParameterName + ", ParameterValue: " + item.Value + ";";
                }



            }

            return str;
        }

    }
}
