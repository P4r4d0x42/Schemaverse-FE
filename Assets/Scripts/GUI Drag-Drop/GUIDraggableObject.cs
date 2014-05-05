using UnityEngine;
using System.Collections;
/// <summary>
/// This class apears to be handling the draging and droping portion
/// Polymorphism, Up-casting and Down-casting
/// http://www.c-sharpcorner.com/UploadFile/pcurnow/polymorphcasting06222007131659PM/polymorphcasting.aspx
/// </summary>
public class GUIDraggableObject
{
	private Vector2 m_DragStart;

	// Constructor 
	public GUIDraggableObject (Vector2 position, Vector2 size)
	{
		Position = position;
		Size = size;
	}


	#region Autoproperties 

	public bool Dragging { get; private set; }

	public Vector2 Position { get; set; }

	public Vector2 Size { get; set; }

	#endregion


	public void Drag (Rect draggingRect)
	{
		if (Event.current.type == EventType.MouseUp)
		{
			Dragging = false;
		}
		else if (Event.current.type == EventType.MouseDown && draggingRect.Contains (Event.current.mousePosition))
		{
			Dragging = true;
			m_DragStart = Event.current.mousePosition - Position;
			Event.current.Use();
		}

		if (Dragging)
		{
			Position = Event.current.mousePosition - m_DragStart;
		}
	}


    public virtual void DrawMenuObject()
    {
        Debug.Log("OnGUI is being called from the GUIDraggableObjects Class");
        Debug.Log("This should be overriden in the child class");
    }
}