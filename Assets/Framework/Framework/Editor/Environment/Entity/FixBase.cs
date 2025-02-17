using UnityEditor;

namespace D.Editor.Environment
{
    public abstract class FixBase
    {
        protected MessageType level;
        public FixBase(MessageType level)
        {
            this.level = level;
        }
        public abstract bool IsValid();
        public abstract void Fix();
        public abstract void DrawGUI();
        protected void DrawContent(string title, string msg)
        {
            EditorGUILayout.HelpBox(title, level);
            EditorGUILayout.LabelField(msg, EditorStyles.textArea);
        }
    }
}