/*
 * 作者：您的姓名
 * 命名空间：您的命名空间
 * 版权：© 2024 杭州西雨动画有限公司。保留所有权利。
 * 描述：修改脚本模板
 */

using System;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace YZJ
{
    /// <summary>
    /// 添加文件头说明
    /// </summary>
    public class AddFileHead
    {
        #region 更新格式
        [MenuItem("依旧/ScriptTemplates/2.更新文件头格式")]
        public static void UpdateFileHead()
        {
            string NewBehaviourScriptPath = EditorApplication.applicationContentsPath + "/Resources/ScriptTemplates/81-C# Script-NewBehaviourScript.cs.txt";
            Debug.Log(NewBehaviourScriptPath);
            string head =
@"/*
* 文件名：#SCRIPTFULLNAME#
* 作者：#AUTHOR#
* Unity版本：#UNITYVERSION#
* 创建日期：#DATE#
* 版权：© #YEAR# #COMPANY#
* All rights reserved.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace #NAMESPACE#
{
    /// <summary>
    /// 类：#SCRIPTNAME#
    /// 描述：此类的功能和用途...
    /// </summary>
    public class #SCRIPTNAME# : MonoBehaviour
    {
        //Awake
        void Awake()
        {

        }
        //Start 回收内存
        void Start()
        {

        }
        //待办 初始化
        void Initialize()
        {

        }
        //OnDestroy 回收内存
        void OnDestroy()
        {

        }
    }
}";


            byte[] curTexts = System.Text.Encoding.UTF8.GetBytes(head);
            using (FileStream fs = new FileStream(NewBehaviourScriptPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                if (fs != null)
                {
                    fs.SetLength(0);    //清空文件
                    fs.Write(curTexts, 0, curTexts.Length);

                    fs.Flush();
                    fs.Dispose();
                    Debug.Log("更新模板脚本");
                    //Log.I("Update File: 81-C# Script-NewBehaviourScript.cs.txt, Success");
                }
            }
        }
        #endregion

        #region 打开文件
        public const string NotePadJJ_APP_NAME = "notepad++.exe";
        public const string NotePad_APP_NAME = "notepad.exe";

        /// <summary>
        /// 用notepad++打开文件
        /// </summary>
        [MenuItem("依旧/ScriptTemplates/1.编辑文件头格式 NotePad++")]
        static public void OpenForNotePadJJ()
        {
            string dir_path = Application.dataPath + "/UIFramework/Editor/ScriptTemplate/AddFileHead.cs";
            InvokeCmd(NotePadJJ_APP_NAME, dir_path);
        }

        /// <summary>
        /// 用记事本打开文件
        /// </summary>
        [MenuItem("依旧/ScriptTemplates/1.编辑文件头格式 记事本")]
        static public void OpenForNotePad()
        {
            string dir_path = Application.dataPath + "/UIFramework/Editor/ScriptTemplate/AddFileHead.cs";
            InvokeCmd(NotePad_APP_NAME, dir_path);
        }

        /// <summary>
        /// 调用CMD 命令
        /// </summary>
        public static void InvokeCmd(string cmd, string dir_path)
        {
            UnityEngine.Debug.Log(cmd);
            AssetDatabase.Refresh();
            new Thread(new ThreadStart(() =>
            {
                try
                {
                    System.Diagnostics.Process p = new System.Diagnostics.Process();
                    p.StartInfo.FileName = cmd;
                    p.StartInfo.Arguments = dir_path;
                    p.Start();
                    p.WaitForExit();
                    p.Close();
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
            })).Start();
        }

        #endregion
    }

    public class KeywordReplace : AssetModificationProcessor
    {
        /// <summary>  
        /// 此函数在asset被创建完，文件已经生成到磁盘上，但是没有生成.meta文件和import之前被调用  
        /// </summary>  
        /// <param name="newFileMeta">newfilemeta 是由创建文件的path加上.meta组成的</param>  
        public static void OnWillCreateAsset(string newFileMeta)
        {
            string newFilePath = newFileMeta.Replace(".meta", "");
            string fileExt = Path.GetExtension(newFilePath);
            if (fileExt != ".cs")
            {
                return;
            }
            //注意，Application.datapath会根据使用平台不同而不同  
            string realPath = Application.dataPath.Replace("Assets", "") + newFilePath;
            string scriptContent = File.ReadAllText(realPath);

            //这里现自定义的一些规则  
            scriptContent = scriptContent.Replace("#SCRIPTFULLNAME#", Path.GetFileName(newFilePath));
            scriptContent = scriptContent.Replace("#COMPANY#", PlayerSettings.companyName);
            scriptContent = scriptContent.Replace("#AUTHOR#", "依旧");
            scriptContent = scriptContent.Replace("#NAMESPACE#", "HQG");
            scriptContent = scriptContent.Replace("#UNITYVERSION#", Application.unityVersion);
            scriptContent = scriptContent.Replace("#DATE#", System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            scriptContent = scriptContent.Replace("#YEAR#", System.DateTime.Now.ToString("yyyy"));

            File.WriteAllText(realPath, scriptContent);
        }
    }
}
