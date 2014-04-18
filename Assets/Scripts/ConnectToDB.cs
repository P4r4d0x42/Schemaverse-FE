using UnityEngine;
using System.Collections;
// For reference http://www.codeproject.com/Articles/30989/Using-PostgreSQL-in-your-C-NET-application-An-intr

public class ConnectToDB : MonoBehaviour
{

    private int Port = 5432; // server port number

    private string
        ServerName = "The Schemaverse",
        ServerIP = "db.schemaverse.com",
        Username = "mrfreeman";

    public bool StorePassword;

    private string Password = "M*EdCXpDm*9mQJ9FJ";
    
        




    void OnGUI()
    {
        // Make a background box for config menu
        GUI.Box(new Rect(10, 50, 256, 128), "Connection Settings");
        
        
        // Connection Settings
        // Pixels from left side of screen, Pixels from top left of screen, button width, button height
        GUI.Label(new Rect(35, 75, 256, 30), string.Format("Server Address: {0}", ServerIP));
        GUI.Label(new Rect(35, 95, 256, 30), string.Format("Server Port: {0}", Port));
        GUI.Label(new Rect(35, 115, 256, 30), string.Format("Connecting As: {0}", Username));
        if (GUI.Button(new Rect(64, 140, 128, 30), "Connect")) { Debug.Log("Connecting....."); }
        

    }
}
