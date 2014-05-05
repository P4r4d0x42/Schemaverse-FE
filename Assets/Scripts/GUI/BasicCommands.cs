using System;
using System.Data;
using Assets.Scripts;
using UnityEngine;

/// <summary>
/// Dragable Menu
/// </summary>
public class BasicCommands : GUIDraggableObject
{
    private string m_Name;
    private int m_Value; // May have this for tracking menu numbers or some such

    // Holds cass to call
    private ConnectToDb _connectToDb;


    // Override constructor for use with the correct class
    public BasicCommands(string name, int value, ConnectToDb connectToDb, Vector2 position, Vector2 size)
        : base(position, size)
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
            GUILayout.Label(string.Format("Position X: {0} | Y: {1}", Position.x, Position.y));
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

        // Only Display these buttons if there is an active connection
        // TODO: Uncomment this when ready for use
        // if (_connectToDb.conn.State != ConnectionState.Open) return;
        // May want to set this up with GUI.enable 

        // Right Side Menu
        GUI.backgroundColor = Color.yellow; // Setting this for safe buttons
        GUILayout.BeginArea(new Rect(5, 40, Size.x / 2 -5, Size.y-10));

        // Make a background box for config menu
        //GUI.Button(new Rect(10, 10, 128, 10), "Stats"); // This works with in the area but you got to line stuff up
        if (GUILayout.Button("Stats"))
        {
            if (_connectToDb != null) _connectToDb.GetSelectData();
        }

        GUILayout.EndArea();
        

    }
}

