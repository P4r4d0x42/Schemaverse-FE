using Assets.Scripts;
using UnityEngine;

/// <summary>
/// Dragable Menu
/// </summary>
public class ConnectionSettings : GUIDraggableObject
{
    private string m_Name;
    private int m_Value; // May have this for tracking menu numbers or some such

    // Holds cass to call
    private ConnectToDb _connectToDb;


    // Override constructor for use with the correct class
    public ConnectionSettings(string name, int value, ConnectToDb connectToDb, Vector2 position, Vector2 size) : base(position, size)
    {
        m_Name = name;
        m_Value = value;

        _connectToDb = connectToDb;

    }

    /// <summary>
    /// Override that gets called from the On_GUI() method.
    /// </summary>
    public override void DrawMenuObject()
    {
        Rect drawRect = new Rect(Position.x, Position.y, Size.x, Size.y), dragRect;

        GUILayout.BeginArea(drawRect, GUI.skin.GetStyle("Box"));
        GUILayout.Label(m_Name, GUI.skin.GetStyle("Box"), GUILayout.ExpandWidth(true));

        dragRect = GUILayoutUtility.GetLastRect();
        dragRect = new Rect(dragRect.x + Position.x, dragRect.y + Position.y, dragRect.width, dragRect.height);

        if (Dragging)
        {
            GUILayout.Label(string.Format("Position X: {0} | Y: {1}", Position.x, Position.y), GUI.skin.GetStyle("Box"), GUILayout.ExpandWidth(true));
            // TODO: Save these values to a log when the finished draging the menu. It should then load those values when the software starts

        }       

        // Custom code for each class goes into the below method
        MenuElement();
        
        GUILayout.EndArea();

        Drag(dragRect);
    }


    /// <summary>
    /// This is what will be called to draw up the custom menu for this class.
    /// </summary>
    public void MenuElement()
    {

         
       

        GUILayout.Label(string.Format("Server Address: {0}", _connectToDb.Host), GUI.skin.GetStyle("Box"),
                        GUILayout.ExpandWidth(true));
        GUILayout.Label(string.Format("Server Port: {0}", _connectToDb.Port), GUI.skin.GetStyle("Box"),
                        GUILayout.ExpandWidth(true));
        GUILayout.Label(string.Format("Connecting as: {0}", _connectToDb.User), GUI.skin.GetStyle("Box"),
                        GUILayout.ExpandWidth(true));
        GUILayout.Label(string.Format("Connection Status: {0}", _connectToDb.ConnectionStatus),
                        GUI.skin.GetStyle("Box"),
                        GUILayout.ExpandWidth(true));

        // It's one of those......Need to remeber what these type of statments are called.
        GUI.backgroundColor = _connectToDb.BtnStatus == "Disconnect" ? Color.red : Color.green;

        if (GUILayout.Button(_connectToDb.BtnStatus))
        {
            _connectToDb.ConnectionToDb();// TODO: May want this to be static class at some point?
        }
        
    }
}

