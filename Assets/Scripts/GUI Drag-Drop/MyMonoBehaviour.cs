using Assets.Scripts.GUI;
using Assets.Scripts;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyMonoBehaviour : MonoBehaviour
{
	private List< DataObject > m_Data = new List< DataObject > ();
	private List< Setting > m_Settings = new List<Setting>();

	private Rect dropTargetRect = new Rect (10.0f, 10.0f, 30.0f, 30.0f);

	// Creates an instance of the ConnectToDb class for use in this class. This works because the ConnectToDb
	// does not inherit from the MonoBehavior class. Unity no liky that shit. 
	ConnectToDb _connectToDb = new ConnectToDb();

	void Awake ()
	{
		m_Data.Add(new DataObject("Connection Settings", 1, new Vector2(10.0f, 10.0f)));
		m_Data.Add (new DataObject ("Two", 2, new Vector2 (20.0f * Random.Range (1.0f, 10.0f), 20.0f * Random.Range (1.0f, 10.0f))));
		m_Data.Add (new DataObject ("Three", 3, new Vector2 (20.0f * Random.Range (1.0f, 10.0f), 20.0f * Random.Range (1.0f, 10.0f))));
		m_Data.Add (new DataObject ("Four", 4, new Vector2 (20.0f * Random.Range (1.0f, 10.0f), 20.0f * Random.Range (1.0f, 10.0f))));
		m_Data.Add(new DataObject("Five", 5, new Vector2(20.0f * Random.Range(1.0f, 10.0f), 20.0f * Random.Range(1.0f, 10.0f))));

		m_Settings.Add(new Setting(string.Format("Server Address: {0}", _connectToDb.Host),6,new Vector2 (20.0f,20.0f)));
	}

	public void OnGUI ()
	{
		DataObject toFront, dropDead;
		Color color;

		GUI.Box(dropTargetRect, "Die");

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