using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System.IO;

public class ADBDeviceConnector : EditorWindow
{
    private string adbPath = "";
    private string deviceIP = "";
    private bool Init = false;

    [MenuItem("工具/ADB设备连接器")]
    public static void ShowWindow()
    {
        GetWindow<ADBDeviceConnector>("ADB设备连接器");
    }

    private void OnGUI()
    {
        if (string.IsNullOrEmpty(adbPath) && !Init)
        {
            Init = true;
            adbPath = GetAdbPath();
        }
        EditorGUILayout.LabelField("ADB路径：", EditorStyles.boldLabel);
        adbPath = EditorGUILayout.TextField(adbPath);

        if (GUILayout.Button("检查设备IP"))
        {
            CheckDeviceIP();
        }

        EditorGUILayout.LabelField("设备IP地址：", EditorStyles.boldLabel);
        deviceIP = EditorGUILayout.TextField(deviceIP);

        if (GUILayout.Button("连接设备"))
        {
            ConnectDevice();
        }
    }
    public string GetAdbPath()
    {
        string sdkPath = EditorPrefs.GetString("AndroidSdkRoot");
        if (string.IsNullOrEmpty(sdkPath))
        {
            UnityEngine.Debug.LogError("Android SDK 路径未设置。请在 Unity 编辑器中设置 Android SDK 路径 (Edit > Preferences > External Tools)。");
            return "";
        }

        string adbPath = System.IO.Path.Combine(sdkPath, "platform-tools", "adb.exe");
        if (System.IO.File.Exists(adbPath))
        {
            UnityEngine.Debug.Log("ADB 路径: " + adbPath);
        }
        else
        {
            UnityEngine.Debug.LogError("未找到 ADB 可执行文件，请确认 Android SDK 安装是否正确。");
        }
        return adbPath;
    }
    private void CheckDeviceIP()
    {
        if (string.IsNullOrEmpty(adbPath))
        {
            EditorUtility.DisplayDialog("错误", "请填写ADB路径。", "确定");
            return;
        }

        ResetADBServer();

        string ipOutput = RunADBCommand("shell ip -f inet addr show wlan0");
        if (!string.IsNullOrEmpty(ipOutput))
        {
            ParseIP(ipOutput);
        }
        else
        {
            EditorUtility.DisplayDialog("错误", "无法获取设备IP地址。", "确定");
        }
    }

    private void ParseIP(string output)
    {
        string[] lines = output.Split('\n');
        foreach (string line in lines)
        {
            if (line.Trim().StartsWith("inet "))
            {
                deviceIP = line.Trim().Split(' ')[1].Split('/')[0];
                UnityEngine.Debug.Log("设备IP地址: " + deviceIP);
                break;
            }
        }
    }

    private void ConnectDevice()
    {
        if (string.IsNullOrEmpty(adbPath) || string.IsNullOrEmpty(deviceIP))
        {
            EditorUtility.DisplayDialog("错误", "请填写ADB路径和设备IP地址。", "确定");
            return;
        }

        PrepareDeviceForConnection();
        System.Threading.Thread.Sleep(1000);
        DisplayConnectionStatus();
    }

    private void PrepareDeviceForConnection()
    {
        ResetADBServer();
        RunADBCommand("disconnect");
        RunADBCommand("tcpip 5555");
        RunADBCommand($"connect {deviceIP}:5555");
    }

    private void DisplayConnectionStatus()
    {
        string devicesOutput = RunADBCommand("devices");
        UnityEngine.Debug.Log("设备列表: " + devicesOutput);
        EditorUtility.DisplayDialog("ADB设备连接器", "设备连接成功！", "确定");
    }

    private void ResetADBServer()
    {
        RunADBCommand("kill-server");
        RunADBCommand("start-server");
    }

    private string RunADBCommand(string arguments)
    {
        Process adbProcess = new Process();
        adbProcess.StartInfo.FileName = adbPath;
        adbProcess.StartInfo.Arguments = arguments;
        adbProcess.StartInfo.UseShellExecute = false;
        adbProcess.StartInfo.RedirectStandardOutput = true;
        adbProcess.StartInfo.RedirectStandardError = true;
        adbProcess.StartInfo.CreateNoWindow = true;

        adbProcess.Start();
        string output = adbProcess.StandardOutput.ReadToEnd();
        adbProcess.WaitForExit();

        if (adbProcess.ExitCode != 0)
        {
            string errorOutput = adbProcess.StandardError.ReadToEnd();
            UnityEngine.Debug.LogError("ADB命令错误: " + errorOutput);
            return null;
        }

        return output;
    }
}
