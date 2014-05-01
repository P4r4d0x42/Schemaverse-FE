using Assets.Scripts.GUI;
using Assets.Scripts;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasicGUI : MonoBehaviour
{
	private List< DataObject > m_Data = new List< DataObject > ();


	private Rect dropTargetRect = new Rect (Screen.width-50.0f, Screen.height-50.0f, 40.0f, 40.0f);

	// Creates an instance of the ConnectToDb class for use in this class. This works because the ConnectToDb
	// does not inherit from the MonoBehavior class. Unity no liky that shit. 
	ConnectToDb _connectToDb = new ConnectToDb();

	void Awake ()
	{
		m_Data.Add(new DataObject("Connection Settings", 1, new Vector2(10f, 10f),new Vector2(256f,128f)));
       // TODO: Need to sort out how I'm going to call different menus. This may not be the best choice
       // TODO: I wounder if I could use XML to pass in the need data or perhaps pass in a class/object with the correct GUI elements... 
		
	}

	public void OnGUI ()
	{
		DataObject toFront, dropDead;
		Color color;

		GUI.Box(dropTargetRect, "X");

		toFront = dropDead = null;
		foreach (DataObject data in m_Data)
		{
			color = GUI.color;

			if (data.Dragging)
			{
				GUI.color = dropTargetRect.Contains (Event.current.mousePosition) ? Color.red : color;
			}

			data.OnGUI ();

			GUI.color = color;

			if (data.Dragging)
			{
				if (m_Data.IndexOf (data) != m_Data.Count - 1)
				{
					toFront = data;
				}
			}
		}

		if (toFront != null)
		// Move an object to front if needed
		{
			m_Data.Remove (toFront);
			m_Data.Add (toFront);
		}
	}
}