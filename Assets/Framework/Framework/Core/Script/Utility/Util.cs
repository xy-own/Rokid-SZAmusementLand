using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using D.Define;
using D.Common;
namespace D.Utility
{
    public static class Util
    {
        #region Log
        public static void Log(string msg, LogColor color = LogColor.None)
        {
            if (AppConst.LogMode)
            {
                if (color == LogColor.None)
                {
                    Debug.Log(msg);
                }
                else
                {
                    string msgColor = string.Empty;
                    switch (color)
                    {
                        case LogColor.Red:
                            msgColor = "red";
                            break;
                        case LogColor.Yellow:
                            msgColor = "yellow";
                            break;
                        case LogColor.Green:
                            msgColor = "green";
                            break;
                        case LogColor.Blue:
                            msgColor = "blue";
                            break;
                        default:
                            break;
                    }
                    Debug.Log($"<color={msgColor}> XY ---> {msg}</color>");
                }
            }
        }
        #endregion

        #region  Game Setting
        public static void SetDebugState(bool state)
        {
            Debug.unityLogger.logEnabled = state;
        }
        public static GameSettings LoadGameSettings()
        {
            return Resources.Load<GameSettings>(AppConst.GameSettingName);
        }
        #endregion

        #region Local Path
        /// <summary>
        /// 应用程序内容路径
        /// </summary>
        public static string AppContentPath()
        {
            string path = string.Empty;
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    path = Application.streamingAssetsPath;
                    break;
                case RuntimePlatform.IPhonePlayer:
                    path = Application.dataPath + "/Raw/";
                    break;
                default:
                    path = Application.streamingAssetsPath;
                    break;
            }
            return path;
        }
        /// <summary>
        /// 应用程序内容路径
        /// </summary>
        public static string AppWorksPath()
        {
            string url = Application.persistentDataPath + "/";
            if (!Directory.Exists(url))
            {
                Directory.CreateDirectory(url);
            }
            return url;
        }
        #endregion

        #region 随机数 && uuid
        public static int Random(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        public static float Random(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        public static string GetGuid()
        {
            return System.Guid.NewGuid().ToString();
        }
        #endregion

        #region Time
        public static long GetTime()
        {
            TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
            return (long)ts.TotalMilliseconds;
        }

        public static string RandomTime()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }
        public static string GetDate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }
        #endregion

        #region 编码 && MD5
        /// <summary>
        /// Base64编码
        /// </summary>
        public static string Encode(string message)
        {
            byte[] bytes = Encoding.GetEncoding("utf-8").GetBytes(message);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Base64解码
        /// </summary>
        public static string Decode(string message)
        {
            byte[] bytes = Convert.FromBase64String(message);
            return Encoding.GetEncoding("utf-8").GetString(bytes);
        }

        /// <summary>
        /// HashToMD5Hex
        /// </summary>
        public static string HashToMD5Hex(string sourceStr)
        {
            byte[] Bytes = Encoding.UTF8.GetBytes(sourceStr);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] result = md5.ComputeHash(Bytes);
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < result.Length; i++)
                    builder.Append(result[i].ToString("x2"));
                return builder.ToString();
            }
        }
        /// <summary>
        /// 计算字符串的MD5值
        /// </summary>
        public static string MD5(string source)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(source);
            byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
            md5.Clear();

            string destString = "";
            for (int i = 0; i < md5Data.Length; i++)
            {
                destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
            }
            destString = destString.PadLeft(32, '0');
            return destString;
        }

        /// <summary>
        /// 计算文件的MD5值
        /// </summary>
        public static string MD5File(string file)
        {
            try
            {
                FileStream fs = new FileStream(file, FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(fs);
                fs.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("md5file() fail, error:" + ex.Message);
            }
        }
        #endregion

        #region 检测网络
        /// <summary>
        /// 网络可用
        /// </summary>
        public static bool NetAvailable
        {
            get
            {
                return Application.internetReachability != NetworkReachability.NotReachable;
            }
        }

        /// <summary>
        /// 是否是无线
        /// </summary>
        public static bool IsWifi
        {
            get
            {
                return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
            }
        }
        #endregion

        #region 资源卸载
        public static void UnloadAsset(GameObject gameObj)
        {
            if (gameObj != null)
            {
                UnloadAssetType<Text>(gameObj);
                UnloadAssetType<Image>(gameObj);
            }
        }

        static void UnloadAssetType<T>(GameObject gameObj)
        {
            var components = gameObj.GetComponentsInChildren<T>();
            if (components.Length > 0)
            {
                Type compType = typeof(T);
                var assets = new List<UnityEngine.Object>();
                for (int i = 0; i < components.Length; i++)
                {
                    var c = components[i];
                    if (compType == typeof(Image))
                    {
                        var image = c as Image;
                        if (image != null && image.sprite != null && !assets.Contains(image.sprite.texture))
                        {
                            assets.Add(image.sprite.texture);
                        }
                    }
                    else if (compType == typeof(Text))
                    {
                        var text = c as Text;
                        if (text != null && !assets.Contains(text.font))
                        {
                            assets.Add(text.font);
                        }
                    }
                }
                for (int i = 0; i < assets.Count; i++)
                {
                    if (assets[i] != null)
                    {
                        Resources.UnloadAsset(assets[i]);
                    }
                }
                assets = null;
            }
        }
        #endregion

        #region 数据持久化
        /// <summary>
        /// 生成一个Key名
        /// </summary>
        public static string GetKey(string key)
        {
            return AppConst.AppPrefix + "_" + key;
        }
        /// <summary>
        /// 取得数据
        /// </summary>
        public static string GetString(string key)
        {
            string name = GetKey(key);
            return PlayerPrefs.GetString(name);
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        public static void SetString(string key, string value)
        {
            string name = GetKey(key);
            PlayerPrefs.DeleteKey(name);
            PlayerPrefs.SetString(name, value);
            PlayerPrefs.Save();
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        public static void DeleteString(string key)
        {
            string name = GetKey(key);
            PlayerPrefs.DeleteKey(name);
        }
        /// <summary>
        /// 删除所有本地数据
        /// </summary>
        public static void DeleteAllString()
        {
            PlayerPrefs.DeleteAll();
        }
        #endregion

        #region IO
        /// <summary>
        /// 文件IO写入
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="data">文件信息</param>
        public static void WriteFile(string path, byte[] data)
        {
            FileStream fs = File.Open(path, FileMode.OpenOrCreate);
            byte[] info = data;
            fs.Write(info, 0, info.Length);
            fs.Close();
            fs.Dispose();

            Log("文件写入本地完成 -- > " + path);
        }
        /**
        * 读取文本文件
        * path：读取文件的路径
        */
        public static string ReadFileString(string path)
        {
            if (!File.Exists(path))
            {
                Log("路径未找到文件");
                return null;
            }
            byte[] info = File.ReadAllBytes(path);

            string line = Encoding.UTF8.GetString(info);
            return line;
        }
        /**
        * 读取文本文件
        * path：读取文件的路径
        */
        public static byte[] ReadFileBytes(string path)
        {
            if (!File.Exists(path))
            {
                Log("路径未找到文件");
                return null;
            }
            byte[] info = File.ReadAllBytes(path);
            return info;
        }
        #endregion

        #region 其他
        /// <summary>
        /// 判断数字
        /// </summary>
        public static bool IsNumeric(string str)
        {
            if (str == null || str.Length == 0) return false;
            for (int i = 0; i < str.Length; i++)
            {
                if (!Char.IsNumber(str[i])) { return false; }
            }
            return true;
        }

        /// <summary>
        /// A点绕B点旋转
        /// </summary>
        /// <param name="point">要旋转的点</param>
        /// <param name="pivot">中心点</param>
        /// <param name="euler">旋转的角度</param>
        /// <returns></returns>
        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 euler)
        {
            Vector3 direction = point - pivot;
            Vector3 rotatedDirection = Quaternion.Euler(euler) * direction;
            Vector3 rotatedPoint = rotatedDirection + pivot;
            return rotatedPoint;
        }
        #endregion

        #region Memory
        /// <summary>
        /// 清理内存
        /// </summary>
        public static void ClearMemory()
        {
            GC.Collect(); Resources.UnloadUnusedAssets();
        }
        #endregion

        #region 拍照

        /// <summary>
        /// 截屏
        /// </summary>
        /// <param name="action">截取图片的回调</param>
        /// <returns></returns>
        public static IEnumerator OnScreenShot(UnityAction<Texture> action = null)
        {
            Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            yield return new WaitForEndOfFrame();
            texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            texture.Apply();
            yield return null;
            action?.Invoke(texture);
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="texture">要保存的图片</param>
        public static void SaveSprite(Texture2D texture)
        {
            byte[] bytes = texture.EncodeToPNG();
            string currentTime = string.Format("{0:yyyyMMddHHmmssffff}", DateTime.Now);
            string savePath = AppWorksPath() + "ScreenCapture/";
            //若没路径 创建
            if (!Directory.Exists((savePath)))
            {
                Directory.CreateDirectory(savePath);
            }

            //保存路径
            string path_save = savePath + "IMG_" + currentTime + ".png";
            WriteFile(path_save, bytes);
        }

        #endregion
        /// <summary>
        /// 修改Item层级
        /// </summary>
        /// <param name="trans">对象父级</param>
        /// <param name="targetLayer">要修改的layer名</param>
        public static void ChangeLayer(Transform trans, string targetLayer)
        {
            if (LayerMask.NameToLayer(targetLayer) == -1)
            {
                Debug.Log("Layer中不存在,请手动添加LayerName");
                return;
            }
            //遍历更改所有子物体layer
            trans.gameObject.layer = LayerMask.NameToLayer(targetLayer);
            foreach (Transform child in trans)
            {
                ChangeLayer(child, targetLayer);
            }
        }
        #region AudioClip
        public static byte[] GetAudioByteArray(AudioClip clip)
        {
            float[] data = new float[clip.samples];
            clip.GetData(data, 0);
            int rescaleFactor = 32767;
            byte[] outData = new byte[data.Length * 2];
            for (int i = 0; i < data.Length; i++)
            {
                short temshort = (short)(data[i] * rescaleFactor);
                byte[] temdata = BitConverter.GetBytes(temshort);
                outData[i * 2] = temdata[0];
                outData[i * 2 + 1] = temdata[1];
            }
            return outData;
        }
        #endregion

        /// <summary>
        /// 获取角度
        /// </summary>
        /// <param name="p1">点A</param>
        /// <param name="p2">点B</param>
        /// <returns></returns>
        public static Vector3 GetAngle(Vector3 pointA, Vector3 pointB)
        {
            Vector3 direction = pointA - pointB;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Vector3 vec = Vector3.zero;
            vec.z = angle;
            return vec;
        }
    }
}