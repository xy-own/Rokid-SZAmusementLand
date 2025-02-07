using System;
using System.Collections.Generic;
using UnityEngine;

#region Socket
public class CMD_Head
{
    public short api;
    public int dataLenth;
    public byte[] data;
}
#endregion

#region Http
public class RequestResult<T>
{
    public bool success;
    public string msg;  // 请求状态消息
    public T data;          // 数据
    public string time;
}
#endregion