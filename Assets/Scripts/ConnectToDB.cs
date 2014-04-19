using System;
using Npgsql;
using UnityEngine;
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

    private int Port = 5432; // server port number

    public string
        DBname,
        Host,
        User,
        Password;

    public bool StorePassword;

    private bool IsConnected = false;
    private string btnStatus = "Connect";
    private  NpgsqlConnection dbcon; // Create connection object
    private  NpgsqlCommand dbcmd; // Create objected used for issueing db comands
           
 

    void OnGUI()
    {
       
        
        // Make a background box for config menu
        GUI.Box(new Rect(10, 50, 256, 128), "Connection Settings");
        
        
        // Connection Settings
        // Pixels from left side of screen, Pixels from top left of screen, button width, button height
        GUI.Label(new Rect(35, 75, 256, 30), string.Format("Server Address: {0}", Host));
        GUI.Label(new Rect(35, 95, 256, 30), string.Format("Server Port: {0}", Port));
        GUI.Label(new Rect(35, 115, 256, 30), string.Format("Connecting As: {0}", User));


        if (GUI.Button(new Rect(64, 140, 128, 30), btnStatus))
        {
            // Using try to wrap the db connection open and close process.
            try
            {
                if (!IsConnected)
                {
                    Debug.Log("Connecting.....");

                    dbcon =
                        new NpgsqlConnection(string.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};",
                                                           Host, Port, User, Password, DBname));

                    dbcon.Open();

                    Debug.Log("Connection should be open");

                    dbcmd = dbcon.CreateCommand(); // Initialize object command interface
                    btnStatus = "Disconnect";
                    IsConnected = true;
                }
                else
                {
                    dbcmd.Dispose();
                    dbcon.Close();                
                    dbcon = null;
                    Debug.Log("Connection should be closed");
                    dbcmd = null;
                    btnStatus = "Connect";
                    IsConnected = false;
                }

            }
            catch (Exception msg)
            {
                // something went wrong, and you wanna know why
                Debug.LogError(msg.ToString());
                throw;
            }
        }


        // This should be a Select Statment method
        //dbcmd.CommandText = "SELECT username, balance, fuel_reserve FROM my_player"; // Test connection by running this query

        //var reader = dbcmd.ExecuteReader();

        //while (reader.Read())
        //{

        //    string username = reader["username"].ToString();

        //    string balance = reader["balance"].ToString();

        //    string fuel_reserve = reader["fuel_reserve"].ToString();

        //    Debug.Log(string.Format("Username: {0}, Balance: {1}, Fuel Reserve: {2}",username,balance,fuel_reserve));

        //}

        // clean up

        // reader.Close();
        // reader = null;

    }
}
