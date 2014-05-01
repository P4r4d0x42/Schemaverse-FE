using System.Data;
using UnityEngine;

namespace Assets.Scripts.GUI
{

    public class BasicGUI : MonoBehaviour
    {

        // Creates an instance of the ConnectToDb class for use in this class. This works because the ConnectToDb
        // does not inherit from the MonoBehavior class. Unity no liky that shit. 
        ConnectToDb _connectToDb = new ConnectToDb();





        private void OnGUI()
        {
            

            //// Make a background box for config menu
            //GUI.Box(new Rect(10, 48, 256, 256), "Connection Settings");


            //// Connection Settings
            //// Pixels from left side of screen, Pixels from top left of screen, button width, button height
            //GUI.Label(new Rect(35, 75, 256, 30), string.Format("Server Address: {0}", _connectToDb.Host));
            //GUI.Label(new Rect(35, 95, 256, 30), string.Format("Server Port: {0}", _connectToDb.Port));
            //GUI.Label(new Rect(35, 115, 256, 30), string.Format("Connecting As: {0}", _connectToDb.User));
            //GUI.Label(new Rect(35, 135, 256, 30), string.Format("Connection Status: {0}", _connectToDb.ConnectionStatus));

            //if (GUI.Button(new Rect(64, 168, 128, 30), _connectToDb.BtnStatus))
            //{
            //    _connectToDb.ConnectionToDb();
            //}

            //// Not working quite right ATM
            ////if (GUI.Button(new Rect(64, 208, 128, 30), "Quit Application")) { QuitApplication(); }


            //// Only Display these buttons if there is an active connection
            //if (_connectToDb.conn.State != ConnectionState.Open) return;
            //// Make a background box for config menu
            //GUI.Box(new Rect(Screen.width - 266, 48, 256, 512), "Commands");
            //if (GUI.Button(new Rect(Screen.width - 266, 248, 128, 30), "Stats"))
            //{
            //    _connectToDb.GetSelectData();
            //}
        }
    }
}