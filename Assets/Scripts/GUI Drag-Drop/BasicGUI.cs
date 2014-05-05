using Assets.Scripts;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasicGUI : MonoBehaviour
{
	/// <summary>
    /// Changed this from DataObject to GUIDraggableObject parent to keep a list of different child objects/classes
    /// It would be better to create an interface if I need to access properties and make changes. 
    /// http://stackoverflow.com/questions/11354620/creating-a-generic-list-of-objects-in-c-sharp
    /// Polymorphism, Up-casting and Down-casting
    /// http://www.c-sharpcorner.com/UploadFile/pcurnow/polymorphcasting06222007131659PM/polymorphcasting.aspx
	/// </summary>
    private List<GUIDraggableObject> m_Data = new List<GUIDraggableObject>();

	

	private Rect dropTargetRect = new Rect (Screen.width-50.0f, Screen.height-50.0f, 40.0f, 40.0f);

	// Creates an instance of the ConnectToDb class for use in this class.
	ConnectToDb _connectToDb = new ConnectToDb();


	void Awake ()
	{
	    int sw = Screen.width;
	    int sh = Screen.height;


        // Initialize all the menu objects
		m_Data.Add(new ConnectionSettings("Connection Settings", 1, _connectToDb, new Vector2(10f, 10f), new Vector2(256f, 166f)));
        m_Data.Add(new BasicCommands("Basic Commands", 2, _connectToDb, new Vector2(sw - 266f, 10f), new Vector2(256f, 166f)));
        m_Data.Add(new TerminalOutput("Terminal Output", 3, _connectToDb, new Vector2(sw /2 - 256, sh - 176f), new Vector2(512f, 166f)));
	
	}

	public void OnGUI ()
	{
		// DataObject toFront, dropDead;
	    GUIDraggableObject toFront, dropDead;
		Color color;
        

		GUI.Box(dropTargetRect, "X");

		toFront = dropDead = null;

        // Draw each object in the list
		foreach (var data in m_Data)
		{
			color = GUI.color;

			if (data.Dragging)
			{
				GUI.color = dropTargetRect.Contains (Event.current.mousePosition) ? Color.red : color;
			}
			
	        // Call the OnGUI function in each DataObject in m_Data. This is where the magic happens
            data.DrawMenuObject(); 

			GUI.color = color;

			if (data.Dragging)
			{
				if (m_Data.IndexOf (data) != m_Data.Count - 1)
				{
					toFront = data;
				}
			}
		}

        // Move items to front when title clicked???
        if (toFront != null)
        // Move an object to front if needed
        {
            m_Data.Remove(toFront);
            m_Data.Add(toFront);
        }
	}
}