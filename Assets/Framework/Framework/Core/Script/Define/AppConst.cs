using UnityEngine;
namespace D.Define
{
    public class AppConst
    {
        public static bool LogMode = false;                                 // 日志模式
        public static bool ShowFps = false;                                 // 显示FPS
        public static int GameFrameRate = 30;                               // 帧率
        public static bool ABLoad = false;
        public static string AppPrefix = Application.productName + "_";     // 应用程序前缀


        public const string GameSettingName = "GameSettings";       //游戏设置
    }
}