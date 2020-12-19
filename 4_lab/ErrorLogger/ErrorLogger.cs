using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ErrorLogger
{//; ;Integrated Security=SSPI;Persist Security Info=True
    public class LoggerProps
    {
        public string ConnectionString;
        public string FileLog;
        
        public LoggerProps()
        {
            ConnectionString = @"Data Source = DESKTOP-U7SE2UE; Initial Catalog = basedb ;Integrated Security=True";
            FileLog = @"D:\csharp\log.txt";
            
        }
    }

    public class Logger : IErLogger
    {
        private LoggerProps Props;
        private int numberErr;
        public Logger()
        {
            Props = new LoggerProps();
            numberErr = 2;
        }

        public void WriteError(Exception ex)
        {
            using (var connection = new SqlConnection(Props.ConnectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                var command = new SqlCommand("SaveErr", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Transaction = transaction;
                var command1 = new SqlCommand("GetMaxErrorNum", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command1.Transaction = transaction;

                try
                {
                    var outputparam = new SqlParameter("@NUM", SqlDbType.Int);
                    outputparam.Direction = ParameterDirection.Output;
                    command1.Parameters.Add(outputparam);
                    command1.ExecuteNonQuery();
                    command.Parameters.Add(new SqlParameter("@ErrorID", Convert.ToInt32(outputparam.Value)+1));
                    command.Parameters.Add(new SqlParameter("@ErrorMess", ex.Message));
                    command.Parameters.Add(new SqlParameter("@ErrorClass", ex.Source));
                    command.Parameters.Add(new SqlParameter("@ErrorNum", ex.HResult));
                    command.Parameters.Add(new SqlParameter("@ErrorMethodName", ex.TargetSite.Name));
                    command.Parameters.Add(new SqlParameter("@ErrorTime", DateTime.Now));
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    numberErr++;
                }
                catch (Exception exe)
                {
                    transaction.Rollback();
                    string Mes = "Logger Faild" + DateTime.Now.ToString(CultureInfo.InvariantCulture) + "\n";

                    using (StreamWriter sw = new StreamWriter(Props.FileLog, true, System.Text.Encoding.Default))
                    {
                        sw.WriteLine(exe.Message);
                    }



                }
            }
        }
    }
}