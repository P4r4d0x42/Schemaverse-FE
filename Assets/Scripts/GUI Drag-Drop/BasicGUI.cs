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
	//ConnectionSettings _connectionSettings = new ConnectionSettings();

	void Awake ()
	{
		// TODO: Pass in the ConnectionSettings class to the DataObject in a constructor override
		m_Data.Add(new DataObject("Connection Settings", 1, new ConnectionSettings(), new Vector2(10f, 10f), new Vector2(256f, 166f)));
	   
	
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
				
			data.OnGUI (); // Call the OnGUI function in each DataObject in m_Data

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