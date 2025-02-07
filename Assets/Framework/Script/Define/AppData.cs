using System.Collections.Generic;
/// <summary>
/// 资源配置
/// </summary>
public class AssetConfig
{
    public List<string> name = new List<string>();
}
public enum AppState
{
    None,
    IsPlaying,
    Exiting,
}
public enum PlatformType
{
    Android,
    Ios
}
/// <summary>
/// 日志颜色
/// </summary>
public enum LogColor
{
    /// <summary>
    /// 默认
    /// </summary>
    None,
    /// <summary>
    /// 红色
    /// </summary>
    Red,
    /// <summary>
    /// 黄色
    /// </summary>
    Yellow,
    /// <summary>
    /// 绿色
    /// </summary>
    Green,
    /// <summary>
    /// 蓝色
    /// </summary>
    Blue,
}