using System.Data;
using UnityEngine;
using System.Collections;

public class BasicGUI : MonoBehaviour {

    private void Update()
    {
        // Update status Might should have this check if the status has changed before make this update....
        if (conn != null) ConnectionStatus = conn.State.ToString();

    }


    private void OnGUI()
    {


        // Make a background box for config menu
        GUI.Box(new Rect(10, 48, 256, 256), "Connection Settings");


        // Connection Settings
        // Pixels from left side of screen, Pixels from top left of screen, button width, button height
        GUI.Label(new Rect(35, 75, 256, 30), string.Format("Server Address: {0}", Host));
        GUI.Label(new Rect(35, 95, 256, 30), string.Format("Server Port: {0}", Port));
        GUI.Label(new Rect(35, 115, 256, 30), string.Format("Connecting As: {0}", User));
        GUI.Label(new Rect(35, 135, 256, 30), string.Format("Connection Status: {0}", ConnectionStatus));

        if (GUI.Button(new Rect(64, 168, 128, 30), btnStatus))
        {
            ConnectionToDb();
        }

        // Not working quite right ATM
        //if (GUI.Button(new Rect(64, 208, 128, 30), "Quit Application")) { QuitApplication(); }


        // Only Display these buttons if there is an active connection
        if (conn.State == ConnectionState.Open)
        {
            // Make a background box for config menu
            GUI.Box(new Rect(Screen.width - 266, 48, 256, 512), "Commands");
            if (GUI.Button(new Rect(Screen.width - 266, 248, 128, 30), "Stats"))
            {
                GetSelectData();
            }
        }



    }
}
