using Assets.Scripts;
using UnityEngine;
using System.Collections;

public class DataObject : GUIDraggableObject
// This class just has the capability of being dragged in GUI - it could be any type of generic data class
// This would be your dragable object
{
	private string m_Name;
	private int m_Value;

	ConnectToDb _connectToDb = new ConnectToDb(); // TODO: Remove this. This is here for test purposes here. 

	public DataObject(string name, int value, Vector2 position, Vector2 size)
		: base(position, size)
	{
		m_Name = name;
		m_Value = value;
	}

	public void OnGUI ()
	{
		Rect drawRect = new Rect (Position.x, Position.y, Size.x, Size.y), dragRect;

		GUILayout.BeginArea (drawRect, GUI.skin.GetStyle ("Box"));
		GUILayout.Label (m_Name, GUI.skin.GetStyle ("Box"), GUILayout.ExpandWidth (true));

		dragRect = GUILayoutUtility.GetLastRect ();
		dragRect = new Rect (dragRect.x + Position.x, dragRect.y + Position.y, dragRect.width, dragRect.height);

		#region Special Code // This needs to go into it's own class or some such. Also draging won't work if the above two lines are not were they are.

		GUILayout.Label(string.Format("server address: {0}", _connectToDb.Host), GUI.skin.GetStyle("Box"),
						GUILayout.ExpandWidth(true));
		GUILayout.Label(string.Format("server port: {0}", _connectToDb.Port), GUI.skin.GetStyle("Box"),
						GUILayout.ExpandWidth(true));
		GUILayout.Label(string.Format("connecting as: {0}", _connectToDb.User), GUI.skin.GetStyle("Box"),
						GUILayout.ExpandWidth(true));
		GUILayout.Label(string.Format("Connection Status: {0}", _connectToDb.ConnectionStatus), GUI.skin.GetStyle("Box"),
						GUILayout.ExpandWidth(true));

		if (GUILayout.Button(_connectToDb.BtnStatus))
		{
			_connectToDb.ConnectionToDb();
		}

		#endregion



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