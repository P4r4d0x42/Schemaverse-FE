using UnityEngine;


namespace Assets.Scripts
{
    public class ConnectionSettings
    {
        private ConnectToDb _connectToDb;


        public ConnectionSettings()
        {
            _connectToDb = new ConnectToDb(); // Create a new connection
        }



        /// <summary>
        /// This is what will be called to draw up the menu.
        /// </summary>
        public void MenuElement()
        {


            GUILayout.Label(string.Format("server address: {0}", _connectToDb.Host), GUI.skin.GetStyle("Box"),
                            GUILayout.ExpandWidth(true));
            GUILayout.Label(string.Format("server port: {0}", _connectToDb.Port), GUI.skin.GetStyle("Box"),
                            GUILayout.ExpandWidth(true));
            GUILayout.Label(string.Format("connecting as: {0}", _connectToDb.User), GUI.skin.GetStyle("Box"),
                            GUILayout.ExpandWidth(true));
            GUILayout.Label(string.Format("Connection Status: {0}", _connectToDb.ConnectionStatus),
                            GUI.skin.GetStyle("Box"),
                            GUILayout.ExpandWidth(true));

            if (GUILayout.Button(_connectToDb.BtnStatus))
            {
                _connectToDb.ConnectionToDb();// TODO: May want this to be static class
            }
        }

    }
}

