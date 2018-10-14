using System;
using MySql.Data.MySqlClient;

namespace LightGameServer.Database
{
    class DbConnector
    {
        private string _connectionString;

        public DbConnector()
        {
            Initialize();
        }

        private void Initialize()
        {
            MySqlConnectionStringBuilder connectionStringBuilder =
                new MySqlConnectionStringBuilder
                {
                    Server = "localhost",
                    Database = "castforce",
                    UserID = "root",
                    Password = "Teletabi123!",
                    MinimumPoolSize = 20,
                    MaximumPoolSize = 1000,
                    SslMode = MySqlSslMode.None
                };
            _connectionString = connectionStringBuilder.ToString();
        }

        public MySqlConnection OpenConnection()
        {
            try
            {
                MySqlConnection mySqlConnection = new MySqlConnection(_connectionString);
                mySqlConnection.Open();
                return mySqlConnection;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        Console.WriteLine("Cannot connect to MySQL server");
                        break;

                    case 1045:
                        Console.WriteLine("MySQL: Invalid username/password, please try again");
                        break;
                }
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Connection string is not specifed");
            }
            return null;
        }

        public bool CloseConnection(MySqlConnection connection)
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }
    }
}
