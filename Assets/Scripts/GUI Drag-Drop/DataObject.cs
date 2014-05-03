using Assets.Scripts;
using UnityEngine;
using System.Collections;

public class DataObject : GUIDraggableObject
// This class just has the capability of being dragged in GUI - it could be any type of generic data class
// This would be your dragable object
{
	private string m_Name;
	private int m_Value;

	// Holds cass to call
	private ConnectionSettings _connectionSettings;


	//ConnectToDb _connectToDb = new ConnectToDb(); // TODO: Remove this. This is here for test purposes here. 

	// This is here to keep current compatablity with the MyEditor version of the calling class
	public DataObject(string name, int value, Vector2 position, Vector2 size)
		: base(position, size)
	{
		m_Name = name;
		m_Value = value;
	}

	// Override constructor for use with the correct class
	public DataObject(string name, int value, ConnectionSettings connectionSettings, Vector2 position, Vector2 size)
		: base(position, size)
	{
		m_Name = name;
		m_Value = value;

		_connectionSettings = connectionSettings;
	}

  


	public void OnGUI ()
	{
		Rect drawRect = new Rect (Position.x, Position.y, Size.x, Size.y), dragRect;

		GUILayout.BeginArea (drawRect, GUI.skin.GetStyle ("Box"));
		GUILayout.Label (m_Name, GUI.skin.GetStyle ("Box"), GUILayout.ExpandWidth (true));

		dragRect = GUILayoutUtility.GetLastRect ();
		dragRect = new Rect (dragRect.x + Position.x, dragRect.y + Position.y, dragRect.width, dragRect.height);

		// TODO: Call the fed in class here
		_connectionSettings.MenuElement();


		if (Dragging)
		{
			GUILayout.Label (string.Format("Position X: {0} | Y: {1}",Position.x,Position.y));
			// TODO: Save these values to a log when the finished draging the menu. It should then load those values when the software starts

		}
		//else if (GUILayout.Button ("Yes!"))
		//{
		//    Debug.Log ("Yes. It is " + m_Value + "!");
		//}
		GUILayout.EndArea ();

		Drag (dragRect);
	}
}