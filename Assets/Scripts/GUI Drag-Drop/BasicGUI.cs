using Assets.Scripts;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasicGUI : MonoBehaviour
{
	private List<GUIDraggableObject> m_Data = new List<GUIDraggableObject>();

	// Changed this from DataObject to GUIDraggableObject parent to keep a list of different child objects
	// I wont be able to access the properties of anything in this list with out major effort
	// It would be better to create an interface if I need to access properties and make changes. 
	// http://stackoverflow.com/questions/11354620/creating-a-generic-list-of-objects-in-c-sharp
	private Rect dropTargetRect = new Rect (Screen.width-50.0f, Screen.height-50.0f, 40.0f, 40.0f);

	// Creates an instance of the ConnectToDb class for use in this class. This works because the ConnectToDb
	// does not inherit from the MonoBehavior class. Unity no liky that shit. 
	ConnectToDb _connectToDb = new ConnectToDb();
	//ConnectionSettings _connectionSettings = new ConnectionSettings();

	void Awake ()
	{
		// TODO: Pass in the ConnectionSettings class to the DataObject in a constructor override
		m_Data.Add(new ConnectionSettings("Connection Settings", 1, _connectToDb, new Vector2(10f, 10f), new Vector2(256f, 166f)));
	   
	
	}

	public void OnGUI ()
	{
		// DataObject toFront, dropDead;
		Color color;

		GUI.Box(dropTargetRect, "X");

		// toFront = dropDead = null;


		foreach (ConnectionSettings data in m_Data)
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
					//toFront = data;
				}
			}
		}

		//if (toFront != null)
		//// Move an object to front if needed
		//{
		//    m_Data.Remove (toFront);
		//    m_Data.Add (toFront);
		//}
	}
}