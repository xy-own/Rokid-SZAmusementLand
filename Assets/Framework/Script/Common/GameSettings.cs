using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Framework/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("打印日志")]
    public bool logMode = false;
    [Header("显示帧频")]
    public bool showFps = false;
    [Header("运行帧频"),Range(25,120)]
    public int GameFrameRate = 30;
    [Header("平台")]
    public PlatformType PlatformType = PlatformType.Android;
}