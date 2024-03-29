﻿using System;
using Assets.Scripts;
using UnityEngine;

/// <summary>
/// Dragable Menu
/// </summary>
public class TerminalWindow : GUIDraggableObject
{
    private Vector2 scrollPosition;
    internal static string _terminal = "";
    
    private string m_Name;
    private int m_Value; // May have this for tracking menu numbers or some such

    
    // Holds reference to class
    private ConnectToDb _connectToDb;

    public string stringToEdit = "Terminal Is Currently Read Only";

    #region Properties

    /// <summary>
    /// Provides formatted access to the internal _terminal var
    /// This will sort of work like a terminal now.
    /// </summary>
    public static String Terminal
    {
        get { return _terminal; }
        set { _terminal = string.Format("{0}\n{1}", value, Terminal); }
    }

    #endregion

    // Override constructor for use with the correct class
    public TerminalWindow(string name, int value, ConnectToDb connectToDb, Vector2 position, Vector2 size)
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
        // This is for scrolling
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(Size.x-10), GUILayout.Height(110));
        GUILayout.Label(_terminal);
        GUILayout.EndScrollView();
        
        if (GUILayout.Button("Clear"))
            _terminal = "";

        stringToEdit = GUILayout.TextField(stringToEdit, (int)Size.x);

    }
}

