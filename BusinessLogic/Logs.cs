using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public static class Logs
    {
        public static void WriteError(string PageName, string functionName, string ErrorType, string Error)
        {
            try
            {
                string connString = string.Empty;

                if (ConfigurationManager.ConnectionStrings["ConnectionString"] != null)
                    connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                else if (ConfigurationManager.AppSettings["ConnectionString"] != null)
                    connString = ConfigurationManager.AppSettings["ConnectionString"];

                //WriteTextError("Error Log", functionName, ErrorType, connString);

                using (SqlConnection SqlCon = new SqlConnection(connString))
                {
                    SqlCon.Open();

                    using (SqlCommand cmd = new SqlCommand("WB_WriteError", SqlCon))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@PageName", PageName);
                        cmd.Parameters.AddWithValue("@functionName", functionName);
                        cmd.Parameters.AddWithValue("@ErrorType", ErrorType);
                        cmd.Parameters.AddWithValue("@Error", Error);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ae)
            {
                WriteTextError(PageName, functionName, ErrorType, ae.Message);
            }
        }

        public static void WriteError(string ErrorType, string Error)
        {
            try
            {
                var stackTrace = new System.Diagnostics.StackTrace();
                var methodBase = stackTrace.GetFrame(1).GetMethod();
                var Class = methodBase.ReflectedType;
                var Namespace = Class.Namespace;         //Added finding the namespace


                WriteError(Namespace + "." + Class.Name, methodBase.Name, ErrorType, Error);

            }
            catch (Exception)
            {

            }
        }

        private static void WriteTextError(string PageName, string functionName, string ErrorType, string Error)
        {
            try
            {
                string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(Logs)).CodeBase);


                string FileName = path + @"\Logs"; //\logs_" + DateTime.Now.ToString("yyyyMMdd") + ".txt"; //HttpContext.Current.Server.MapPath("~/Logs/logs_" + DateTime.Now.ToString("yyyyMMdd") + ".txt");
                FileName = FileName.Replace("\\", "/").Replace("file:/", "");
                string strText = DateTime.UtcNow.ToString() + "\t" + ErrorType + "\t" + PageName + "\t\t" + functionName + "\t" + Error;

                if (!(Directory.Exists(FileName)))
                {
                    Directory.CreateDirectory(FileName);
                }

                FileName += "/logs_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";

                if (!File.Exists(FileName))
                {
                    // Create New Text File
                    FileStream fStream = new FileStream(FileName, FileMode.Create, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fStream);
                    sw.WriteLine(strText);
                    sw.Close();
                    fStream.Close();

                }
                else
                {
                    FileInfo finfo = new FileInfo(FileName);

                    while (IsFileLocked(finfo))
                    {
                        Thread.Sleep(500);
                    }

                    finfo = null;

                    FileStream aFile = new FileStream(FileName, FileMode.Append, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(aFile);
                    sw.WriteLine(strText);
                    sw.Close();
                    aFile.Close();
                }
            }
            catch (Exception)
            {

            }
        }

        private static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }
    }
}
