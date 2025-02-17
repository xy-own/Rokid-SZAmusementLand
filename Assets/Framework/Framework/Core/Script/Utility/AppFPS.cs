using System;
using UnityEngine;
namespace D.Utility
{
    public class AppFPS : MonoBehaviour
    {
        private float fps = 0;
        private float showTime = 1f;
        private int count = 0;
        private float deltaTime = 0f;

        void Update()
        {
            count++;
            deltaTime += Time.deltaTime;
            if (deltaTime >= showTime)
            {
                fps = count / deltaTime;
                count = 0;
                deltaTime = 0f;
            }
        }

        private void OnGUI()
        {
            GUIStyle style = GUIStyle.none;
            style.fontSize = 50;
            style.alignment = TextAnchor.UpperLeft;
            style.normal.textColor = Color.red;
            GUI.Label(new Rect(80, 60, 500, 500), $" FPS : {fps} ", style);
        }
    }
}