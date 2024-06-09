using Automation_Framework.Configurations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace Automation_Framework.Utilities
{
    public class DatabaseHelper
    {
        static string connStr = TestSettings.DBConnectionString;
        static string fileDir = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent + "\\TestQueryFiles\\";

        public static DataTable readData(string sqlQuery)
        {
            DataTable dataTable;
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {                   
                    SqlCommand command = new SqlCommand(sqlQuery, conn);
                    conn.Open();
                    dataTable = new DataTable();                  
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    dataAdapter.Fill(dataTable);
                    command.Dispose();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return dataTable;
        }

        public static void insertData(string sqlQuery)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    SqlCommand command = new SqlCommand(sqlQuery, conn);
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.InsertCommand = new SqlCommand(sqlQuery, conn);
                    adapter.InsertCommand.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void updateData(string sqlQuery)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    SqlCommand command = new SqlCommand(sqlQuery, conn);
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.UpdateCommand = new SqlCommand(sqlQuery, conn);
                    adapter.UpdateCommand.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void deleteData(string sqlQuery)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    SqlCommand command = new SqlCommand(sqlQuery, conn);
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.DeleteCommand = new SqlCommand(sqlQuery, conn);
                    adapter.DeleteCommand.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //method takes sql script file as an input
        public static void executeDbScriptFile(String sqlFileName)
        {

            try
            {
                string script = File.ReadAllText(fileDir + sqlFileName);

                // split script on GO command
                System.Collections.Generic.IEnumerable<string> commandStrings = Regex.Split(script, @"^\s*GO\s*$",
                                         RegexOptions.Multiline | RegexOptions.IgnoreCase);
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    connection.Open();
                    foreach (string commandString in commandStrings)
                    {
                        if (commandString.Trim() != "")
                        {
                            using (var command = new SqlCommand(commandString, connection))
                            {
                                try
                                {
                                    command.ExecuteNonQuery();
                                }
                                catch (SqlException ex)
                                {
                                    //_ = ex.Message;
                                    Serilog.Log.Information("The exception is : " + ex.Message);
                                }
                            }
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                //_ = ex.Message;
                Serilog.Log.Information("The exception is : " +ex.Message);
            }


        }

    }
}
