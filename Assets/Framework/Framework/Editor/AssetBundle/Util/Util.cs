
using UnityEngine;

namespace D.Editor
{
    public static class Util
    {
        public static void Log(string log, LogType logType = LogType.None)
        {
            string msgColor = string.Empty;
            string txt = logType.ToString();
            switch (logType)
            {
                case LogType.Warning:
                    msgColor = "green";
                    break;
                case LogType.Error:
                    msgColor = "red";
                    break;
                default:
                    msgColor = "yellow";
                    break;
            }
            Debug.Log($"<color={msgColor}> {txt} : {log} </color>");

        }
    }
}