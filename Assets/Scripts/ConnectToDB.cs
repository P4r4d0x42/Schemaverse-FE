using System;
using Npgsql;
using UnityEngine;
using System.Data;
using System.Collections;


// For reference 
// http://www.codeproject.com/Articles/30989/Using-PostgreSQL-in-your-C-NET-application-An-intr
// http://forum.unity3d.com/threads/9051-Test-Connect-Script-to-PostgreSQL-Server
// http://forum.unity3d.com/threads/7373-using-Npgsql-dll-to-access-PostgreSQL-server
// http://forum.unity3d.com/threads/139887-Data-does-not-exist-in-the-namespace-System
// Information about connections and transactions. Please review it in detail.
// http://stackoverflow.com/questions/11517342/when-to-open-close-connection-to-database

namespace Assets.Scripts
{

    public class ConnectToDB
    {

        #region Fields

        public bool StorePassword;


        private NpgsqlConnection conn; // Create connection object
        private NpgsqlCommand dbcmd; // Create objected used for issueing db comands

        #endregion



        #region Properties

        // Auto Properties
        public string Host { get; private set; }
        public string DBname { get; private set; }
        public string User { get; private set; }
        public string Password { get; private set; }
        public string BtnStatus { get; private set; }
        public string ConnectionString { get; private set; }
        
        public int Port { get; private set; }

        #endregion




        public ConnectToDB()
        {
            Host = "db.schemaverse.com";
            Port = 5432;
            DBname = "schemaverse";
            User = "mrfreeman";
            Password = "M*EdCXpDm*9mQJ9FJ";
            BtnStatus = "Connected";

            // Setup connection string
            ConnectionString =
                string.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};",
                              Host, Port, User, Password, DBname);
            
            // Initialize a connection (socket)? 
            conn = new NpgsqlConnection(ConnectionString);
        }


        

        private void GetSelectData()
        {
            // This should be a Select Statment method
            dbcmd.CommandText = "SELECT username, balance, fuel_reserve FROM my_player";
            // Test connection by running this query

            var reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                string username = reader["username"].ToString();

                string balance = reader["balance"].ToString();

                string fuel_reserve = reader["fuel_reserve"].ToString();

                Debug.Log(string.Format("Username: {0}, Balance: {1}, Fuel Reserve: {2}", username, balance,
                                        fuel_reserve));
            }

            // clean up

            reader.Close();
            reader = null;
        }


        private void ConnectionToDb()
        {
            // Using try to wrap the db connection open and close process.
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    Debug.Log("Connecting.....");

                    conn.Open();

                    Debug.Log("Success open postgreSQL connection.");
                    BtnStatus = "Disconnect";

                    // Initialize object command interface
                    dbcmd = conn.CreateCommand();
                }
                else
                {
                    dbcmd.Dispose();
                    conn.Close();
                    //conn = null;
                    Debug.Log("Connection should be closed");
                    dbcmd = null;
                    BtnStatus = "Connect";
                }
            }
            catch (Exception msg)
            {
                // something went wrong, and you wanna know why
                Debug.LogError(msg.ToString());
                throw;
            }
        }


        private void QuitApplication()
        {
            if (dbcmd != null)
            {
                dbcmd.Dispose();
                dbcmd = null;
            }

            if (conn != null)
            {
                conn.Close();
                conn = null;
            }

            BtnStatus = "Connect";

            Application.Quit();
            // This won't work in the editor. Only in build
        }


    }


}