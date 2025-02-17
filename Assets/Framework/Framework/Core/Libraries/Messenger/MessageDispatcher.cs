using System;
using System.Collections.Generic;
using D.Define;
using D.Utility;
public class MessageDispatcher
{
    private static Dictionary<object, Delegate> MessageEvents = new Dictionary<object, Delegate>();

    #region 消息广播
    /// <summary>
    /// 公用无参广播监听
    /// </summary>
    /// <param name="key"></param>
    public static void SendMessageData(object key)
    {
        Delegate d;
        if (MessageEvents.TryGetValue(key, out d))
        {
            MessageCallBack callBack = d as MessageCallBack;
            if (callBack != null)
            {
                callBack();
            }
            else
            {
                Log($"广播事件错误：事件{key}对应的委托有不同类型", 2);
            }
        }
    }

    /// <summary>
    /// 公共一个参数广播监听
    /// </summary>
    /// <param name="eventType"></param>
    public static void SendMessageData<T>(object key, T arg)
    {
        Delegate d;
        if (MessageEvents.TryGetValue(key, out d))
        {
            MessageCallBack<T> callBack = d as MessageCallBack<T>;
            if (callBack != null)
            {
                callBack(arg);
            }
            else
            {
                Log($"广播事件错误：事件{key}对应的委托有不同类型", 2);
            }
        }
    }
    #endregion

    #region 消息监听
    /// <summary>
    /// 公用无参添加监听
    /// </summary>
    /// <param name="key"></param>
    /// <param name="callback"></param>
    public static void AddListener(object key, MessageCallBack callback)
    {
        if (!MessageEvents.ContainsKey(key))
        {
            MessageEvents.Add(key, null);
        }
        Delegate d = MessageEvents[key];
        if (d != null && d.GetType() != callback.GetType())
        {
            Log($"尝试为时间{key}添加不同类型的委托，当前事件所对应的委托是{d.GetType()}，要添加的委托类型为{callback.GetType()}", 2);
        }
        MessageEvents[key] = (MessageCallBack)MessageEvents[key] + callback;
    }

    /// <summary>
    /// 公共一个参数添加监听
    /// </summary>
    /// <param name="key"></param>
    /// <param name="callBack"></param>
    public static void AddListener<T>(object key, MessageCallBack<T> callBack)
    {
        if (!MessageEvents.ContainsKey(key))
        {
            MessageEvents.Add(key, null);
        }
        Delegate d = MessageEvents[key];
        if (d != null && d.GetType() != callBack.GetType())
        {
            Log($"尝试为事件{key}添加不同类型的委托，当前事件所对应的委托是{d.GetType()}，要添加的委托类型为{callBack.GetType()}", 2);
        }
        MessageEvents[key] = (MessageCallBack<T>)MessageEvents[key] + callBack;
    }
    #endregion

    #region 消息监听移除
    /// <summary>
    /// 公用无参移除监听
    /// </summary>
    /// <param name="key"></param>
    /// <param name="callback"></param>
    public static void RemoveListener(object key, MessageCallBack callBack)
    {
        if (MessageEvents.ContainsKey(key))
        {
            Delegate d = MessageEvents[key];
            if (d == null)
            {
                Log($"移除监听错误：事件{key}没有对应的委托", 2);
            }
            else if (d.GetType() != callBack.GetType())
            {
                Log($"移除监听错误：尝试为事件{key}移除不同类型的委托，当前委托类型为{d.GetType()},要移除的委托类型为{callBack.GetType()}", 2);
            }

            MessageEvents[key] = (MessageCallBack)MessageEvents[key] - callBack;
            if (MessageEvents[key] == null)
            {
                MessageEvents.Remove(key);
            }
        }
        else
        {
            Log($"移除监听错误：没有事件码{key}", 2);
        }
    }
    /// <summary>
    /// 公共一个参数移除监听
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void RemoveListener<T>(object key, MessageCallBack<T> callBack)
    {
        if (MessageEvents.ContainsKey(key))
        {
            Delegate d = MessageEvents[key];
            if (d == null)
            {
                Log($"移除监听错误：事件{key}没有对应的委托", 2);
            }
            else if (d.GetType() != callBack.GetType())
            {
                Log($"移除监听错误：尝试为事件{key}移除不同类型的委托，当前委托类型为{d.GetType()},要移除的委托类型为{callBack.GetType()}", 2);
            }
        }
        else
        {
            Log($"移除监听错误：没有事件码{key}", 2);
        }
        MessageEvents[key] = (MessageCallBack<T>)MessageEvents[key] - callBack;
        if (MessageEvents[key] == null)
        {
            MessageEvents.Remove(key);
        }
    }
    #endregion

    #region 消息监听移除
    /// <summary>
    /// 日志输出
    /// </summary>
    /// <param name="text">日志信息</param>
    /// <param name="logLevel">日志等级 0 = default 1 = warning 2 = error </param>
    static void Log(string text, int logLevel = 0)
    {
        LogColor logColor = LogColor.None;
        switch (logLevel)
        {
            case 1:
                logColor = LogColor.Yellow;
                break;
            case 2:
                logColor = LogColor.Red;
                break;
        }
        Util.Log(text, logColor);
    }
    #endregion
}

//通用委托
public delegate void MessageCallBack();
public delegate void MessageCallBack<T>(T arg);