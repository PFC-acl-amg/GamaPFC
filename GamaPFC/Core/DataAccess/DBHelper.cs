using Core.Util;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DataAccess
{
    public static class DBHelper
    {
        public static void Backup(string connectionString, string fileName)
        {
            // Important Additional Connection Options
            connectionString += "charset=utf8;convertzerodatetime=true;";

            using (MySqlConnection mysqlConnection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand sqlCommand = new MySqlCommand())
                {
                    using (MySqlBackup mySqlBackup = new MySqlBackup(sqlCommand))
                    {
                        sqlCommand.Connection = mysqlConnection;
                        mysqlConnection.Open();
                        UIServices.SetBusyState();
                        mySqlBackup.ExportToFile(fileName);
                        mysqlConnection.Close();
                    }
                }
            }
        }

        public static void Restore(string connectionString, string fileName)
        {
            // Important Additional Connection Options
            connectionString += "charset=utf8;convertzerodatetime=true;";

            using (MySqlConnection mysqlConnection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand sqlCommand = new MySqlCommand())
                {
                    sqlCommand.Connection = mysqlConnection;
                    sqlCommand.CommandText = "SET GLOBAL max_allowed_packet = 1677721656;";
                    mysqlConnection.Open();
                    sqlCommand.ExecuteNonQuery();

                    using (MySqlBackup mySqlBackup = new MySqlBackup(sqlCommand))
                    {
                        //mySqlBackup.ExportInfo.MaxSqlLength = 1677721656;
                        sqlCommand.Connection = mysqlConnection;
                        UIServices.SetBusyState();
                        mySqlBackup.ImportFromFile(fileName);
                        mysqlConnection.Close();
                    }
                }
            }
        }
    }
}
