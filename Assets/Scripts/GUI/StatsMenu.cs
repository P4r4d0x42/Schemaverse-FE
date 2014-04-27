using UnityEngine;
using System.Collections;

//namespace Assets.Scripts.GUI
//{
    public class StatsMenu : GUIDraggableObject
    {
        private string m_Name;
        private int m_Value;

        public StatsMenu(string name, int value, Vector2 position) : base(position)
        {
            m_Name = name;
            m_Value = value;
        }

        public void OnGUI()
        {
            Rect drawRect = new Rect(m_Position.x, m_Position.y, 100.0f, 100.0f), dragRect;

            GUILayout.BeginArea(drawRect, GUI.skin.GetStyle("Box"));
            GUILayout.Label(m_Name, GUI.skin.GetStyle("Box"), GUILayout.ExpandWidth(true));

            dragRect = GUILayoutUtility.GetLastRect();
            dragRect = new Rect(dragRect.x + m_Position.x, dragRect.y + m_Position.y, dragRect.width, dragRect.height);

            if (Dragging)
            {
                GUILayout.Label("Wooo...");
            }
            else if (GUILayout.Button("Yes!"))
            {
                Debug.Log("Yes. It is " + m_Value + "!");
            }
            GUILayout.EndArea();

            Drag(dragRect);
        }






    }

//}

