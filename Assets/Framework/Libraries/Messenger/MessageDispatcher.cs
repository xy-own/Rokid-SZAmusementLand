using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                throw new Exception(string.Format("广播事件错误：事件{0}对应的委托有不同类型", key));
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
                throw new Exception(string.Format("广播事件错误：事件{0}对应的委托有不同类型", key));
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
            throw new Exception(string.Format("尝试为时间{0}添加不同类型的委托，当前事件所对应的委托是{1}，要添加的委托类型为{2}", key, d.GetType(), callback.GetType()));
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
            throw new Exception(string.Format("尝试为事件{0}添加不同类型的委托，当前事件所对应的委托是{1}，要添加的委托类型为{2}", key, d.GetType(), callBack.GetType()));
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
    public static void RemoveListener(object key, MessageCallBack callback)
    {
        if (MessageEvents.ContainsKey(key))
        {
            Delegate d = MessageEvents[key];
            if (d == null)
            {
                throw new Exception(string.Format("移除监听错误：事件{0}没有对应的委托", key));
            }
            else if (d.GetType() != callback.GetType())
            {
                throw new Exception(string.Format("移除监听错误：尝试为事件{0}移除不同类型的委托，当前委托类型为{1},要移除的委托类型为{2}", key, d.GetType(), callback.GetType()));
            }

            MessageEvents[key] = (MessageCallBack)MessageEvents[key] - callback;
            if (MessageEvents[key] == null)
            {
                MessageEvents.Remove(key);
            }
        }
        else
        {
            Util.Log($"移除监听错误：没有事件码{key}", LogColor.Red);
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
                throw new Exception(string.Format("移除监听错误：事件{0}没有对应的委托", key));
            }
            else if (d.GetType() != callBack.GetType())
            {
                throw new Exception(string.Format("移除监听错误：尝试为事件{0}移除不同类型的委托，当前委托类型为{1},要移除的委托类型为{2}", key, d.GetType(), callBack.GetType()));
            }
        }
        else
        {
            throw new Exception(string.Format("移除监听错误：没有事件码{0}", key));
        }
        MessageEvents[key] = (MessageCallBack<T>)MessageEvents[key] - callBack;
        if (MessageEvents[key] == null)
        {
            MessageEvents.Remove(key);
        }
    }
    #endregion
}

//通用委托
public delegate void MessageCallBack();
public delegate void MessageCallBack<T>(T arg);