using System;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace HelpLibrary
{
    public class DataBaseWorker
    {
        readonly string connectionString;
        public DataBaseWorker(string ConnectionString)
        {
            connectionString = ConnectionString;
        }
        public Task ClearInsightsAsync()
        {
            return Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SqlCommand command = new SqlCommand("sp_ClearInsights", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transaction;
                    try
                    {
                        command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        using (StreamWriter sw = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exceptions.txt"), true))
                        {
                            sw.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} Exception: {ex.Message}");
                        }

                        transaction.Rollback();
                    }
                }
            });
        }
        public Task InsertInsightAsync(string message)
        {
            return Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SqlCommand command = new SqlCommand("sp_InsertInsight", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transaction;
                    try
                    {
                        SqlParameter messageParam = new SqlParameter
                        {
                            ParameterName = "@message",
                            Value = message
                        };
                        SqlParameter timeParam = new SqlParameter
                        {
                            ParameterName = "@time",
                            Value = DateTime.Now
                        };

                        command.Parameters.AddRange(new[] { messageParam, timeParam });
                        command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        using (StreamWriter sw = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exceptions.txt"), true))
                        {
                            sw.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} Exception: {ex.Message}");
                        }
                        transaction.Rollback();
                    }
                }
            });
        }
        public Task WriteInsightsToXmlAsync(string outputFolder)
        {
            return Task.Run(async () =>
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SqlCommand command = new SqlCommand("sp_GetInsights", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transaction;
                    try
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataSet dataSet = new DataSet("Insights");
                        DataTable dataTable = new DataTable("Insight");
                        dataSet.Tables.Add(dataTable);
                        adapter.Fill(dataSet.Tables["Insight"]);
                        XmlGenerator xmlGenerator = new XmlGenerator(outputFolder);
                        await xmlGenerator.WriteToXmlAsync(dataSet, "appInsights");
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        using (StreamWriter sw = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exceptions.txt"), true))
                        {
                            await sw.WriteLineAsync($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} Exception: {ex.Message}");
                        }
                        transaction.Rollback();
                    }
                }
            });
        }
        public Task GetCustomersAsync(string outputFolder, DataBaseWorker appInsights, string customersFileName)
        {
            return Task.Run(async () =>
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SqlCommand command = new SqlCommand("sp_GetCustomers", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transaction;
                    try
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataSet dataSet = new DataSet("Customers");
                        DataTable dataTable = new DataTable("Customer");
                        dataSet.Tables.Add(dataTable);
                        adapter.Fill(dataSet.Tables["Customer"]);
                        XmlGenerator xmlGenerator = new XmlGenerator(outputFolder);
                        await xmlGenerator.WriteToXmlAsync(dataSet, customersFileName);
                        await appInsights.InsertInsightAsync("Customers were received successfully");
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        await appInsights.InsertInsightAsync("EXCEPTION: " + ex.Message);
                        transaction.Rollback();
                    }
                }
            });
        }
    }
}
