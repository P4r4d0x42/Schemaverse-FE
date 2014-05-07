using System;
using Npgsql;
using UnityEngine;
using System.Data;



// For reference 
// http://www.codeproject.com/Articles/30989/Using-PostgreSQL-in-your-C-NET-application-An-intr
// http://forum.unity3d.com/threads/9051-Test-Connect-Script-to-PostgreSQL-Server
// http://forum.unity3d.com/threads/7373-using-Npgsql-dll-to-access-PostgreSQL-server
// http://forum.unity3d.com/threads/139887-Data-does-not-exist-in-the-namespace-System
// Information about connections and transactions. Please review it in detail.
// http://stackoverflow.com/questions/11517342/when-to-open-close-connection-to-database

namespace Assets.Scripts
{

    public class ConnectToDb
    {

        #region Fields

        public bool StorePassword;


        public  NpgsqlConnection conn; // Create connection object
        
        private  NpgsqlCommand dbcmd; // Create objected used for issueing db comands
        
        #endregion



        #region Properties

        // Auto Properties
        public string Host { get; private set; }
        public string DBname { get; private set; }
        public string User { get; private set; }
        public string Password { get; private set; }
        public string BtnStatus { get; private set; }
        public string ConnectionString { get; private set; }

        public string ConnectionStatus
        {
            get { return conn.State.ToString(); }
        }

        public  int Port { get; private set; }

        /// <summary>
        /// Create a link to the static property in Terminal Window for use in this class.
        /// </summary>
        internal string Terminal
        {
            get { return TerminalWindow.Terminal; }
            set { TerminalWindow.Terminal = value; } 
        }

        #endregion

        // TODO: Put this shit in a config file
        public ConnectToDb()
        {
            Host = "db.schemaverse.com";
            Port = 5432;
            DBname = "schemaverse";
            User = "mrfreeman";
            Password = "M*EdCXpDm*9mQJ9FJ";
            BtnStatus = "Connect";

            // Setup connection string
            ConnectionString =
                string.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};",
                              Host, Port, User, Password, DBname);
            
            // Initialize a connection (socket)? 
            conn = new NpgsqlConnection(ConnectionString);
        }


        public  void GetSelectData(string [] fields, string table)
        {

            #region Create Select String from params
            // Grab paramiters and create a SELECT statement to 
            string cmdText = "SELECT";
            
            int arryLen = fields.Length;// caching the array length for optimization
            for (int i = 0; i < arryLen-1; i++)
            {
                cmdText += " " + fields[i] + ",";
            }
            // Grabs the last item in the array and then specify table to pull data FROM
            cmdText += fields[arryLen-1] + " " + "FROM" + " "  + table;
            
            Terminal = cmdText; // Output what we have so there is feed back from the button
            #endregion

            dbcmd.CommandText = cmdText;//"SELECT username, balance, fuel_reserve FROM my_player";





            var reader = dbcmd.ExecuteReader();
            
            
            string outputTest = "|  ";
            
            while (reader.Read())
            {
                //Terminal = string.Format("Username: {0}, Balance: {1}, Fuel Reserve: {2}", reader["username"], reader["balance"], reader["fuel_reserve"]);

                for (int i = 0; i < arryLen; i++)
                {
                    string temp = fields[i];
                    outputTest +=   Utilities.TitleCase(temp) + " : " + reader[fields[i]] + "  |  ";
                }
                Terminal = outputTest;
            }

            // clean up

            reader.Close();
            reader = null;
        }

        // SELECT id, name, location, mine_limit, conqueror_id  FROM planets WHERE conqueror_id=GET_PLAYER_ID(SESSION_USER);



        public void ConnectionToDb()
        {
            // Using try to wrap the db connection open and close process.
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    Terminal = "Connecting.....";

                    conn.Open();

                    Terminal = "Success open postgreSQL connection.";
                    BtnStatus = "Disconnect";

                    // Initialize object command interface
                    dbcmd = conn.CreateCommand();
                }
                else
                {
                    dbcmd.Dispose();
                    conn.Close();
                    //conn = null;
                    Terminal = "Connection should be closed";
                    dbcmd = null;
                    BtnStatus = "Connect";
                }
            }
            catch (Exception msg)
            {
                // something went wrong, and you wanna know why
                Debug.LogError(msg.ToString());
                Terminal = msg.ToString();
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