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

public class ConnectToDB : MonoBehaviour
{


    public string
        DBname,
        Host,
        User,
        Password;

    public int Port;
 
    public bool StorePassword;
    
    
    private string connectionString;
    private string ConnectionStatus;
    private string btnStatus = "Connect";
    
    private  NpgsqlConnection conn; // Create connection object
    private  NpgsqlCommand dbcmd; // Create objected used for issueing db comands
           
     void Start()
     {
         // Setup connection string
         connectionString = 
             string.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};",
                                          Host, Port, User, Password, DBname);

         // Initialize a connection (socket)? 
         conn = new NpgsqlConnection(connectionString);
     }
 


    void Update()
    {
        // Update status Might should have this check if the status has changed before make this update....
        if (conn != null) ConnectionStatus = conn.State.ToString();

    }


    void OnGUI()
    {
       
        
        // Make a background box for config menu
        GUI.Box(new Rect(10, 48, 256, 208), "Connection Settings");
        
        
        // Connection Settings
        // Pixels from left side of screen, Pixels from top left of screen, button width, button height
        GUI.Label(new Rect(35, 75, 256, 30), string.Format("Server Address: {0}", Host));
        GUI.Label(new Rect(35, 95, 256, 30), string.Format("Server Port: {0}", Port));
        GUI.Label(new Rect(35, 115, 256, 30), string.Format("Connecting As: {0}", User));
        GUI.Label(new Rect(35, 135, 256, 30), string.Format("Connection Status: {0}", ConnectionStatus));

        if (GUI.Button(new Rect(64, 168, 128, 30), btnStatus)){ ConnectionToDb(); }
        if (GUI.Button(new Rect(64, 208, 128, 30), "Quit Application")) { QuitApplication(); }

        // This should be a Select Statment method
        dbcmd.CommandText = "SELECT username, balance, fuel_reserve FROM my_player"; // Test connection by running this query

        var reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {

            string username = reader["username"].ToString();

            string balance = reader["balance"].ToString();

            string fuel_reserve = reader["fuel_reserve"].ToString();

            Debug.Log(string.Format("Username: {0}, Balance: {1}, Fuel Reserve: {2}",username,balance,fuel_reserve));

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
                btnStatus = "Disconnect";

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
                btnStatus = "Connect";
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
        
        btnStatus = "Connect";
        
        Application.Quit();
        // This won't work in the editor. Only in build
    }


}
