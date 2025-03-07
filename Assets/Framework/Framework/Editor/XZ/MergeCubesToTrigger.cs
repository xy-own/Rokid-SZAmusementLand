using UnityEngine;
using UnityEditor;
using System.IO;

public class MergeCubesToTrigger : EditorWindow
{
    private string saveAssetPath = "Assets/MergedMesh.asset";  // 网格保存路径
    private bool exportAsAsset = true;  // 默认为保存为 asset 格式

    [MenuItem("Tools/Merge Selected Cubes into Trigger")]
    public static void ShowWindow()
    {
        GetWindow<MergeCubesToTrigger>("Merge Cubes to Trigger");
    }

    private void OnGUI()
    {
        GUILayout.Label("合并选中的 Cube 对象", EditorStyles.boldLabel);

        // 文件保存路径
        GUILayout.Label("选择保存路径", EditorStyles.boldLabel);
        saveAssetPath = EditorGUILayout.TextField("保存路径", saveAssetPath);

        // 保存格式选择
        exportAsAsset = GUILayout.Toggle(exportAsAsset, "保存为 .asset 格式");

        if (GUILayout.Button("开始合并并生成触发器"))
        {
            MergeSelectedCubes();
        }

        if (GUILayout.Button("保存合并网格"))
        {
            if (exportAsAsset)
            {
                SaveMeshAsAsset();
            }
        }
        if (GUILayout.Button("重置中心点"))
        {
            ResetPivot();
        }
    }

    private void MergeSelectedCubes()
    {
        // 获取当前选中的对象
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("未选择任何对象，请先选择需要合并的 Cube！");
            return;
        }

        CombineInstance[] combineInstances = new CombineInstance[selectedObjects.Length];
        Vector3 totalPosition = Vector3.zero;
        int combineIndex = 0;

        // 合并所有选中的网格
        foreach (var obj in selectedObjects)
        {
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                Debug.LogWarning($"对象 {obj.name} 缺少 MeshFilter，跳过此对象。");
                continue;
            }

            // 计算合并网格的位置
            totalPosition += obj.transform.position;

            // 将网格添加到合并实例中
            combineInstances[combineIndex].mesh = meshFilter.sharedMesh;
            combineInstances[combineIndex].transform = obj.transform.localToWorldMatrix;
            combineIndex++;
        }

        if (combineIndex == 0)
        {
            Debug.LogWarning("未找到有效的 Mesh 对象用于合并。");
            return;
        }

        // 创建一个新的网格并合并
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combineInstances, true, true);

        // 创建一个新的 GameObject 显示合并后的网格
        GameObject mergedTrigger = new GameObject("MergedTrigger");
        MeshFilter mergedMeshFilter = mergedTrigger.AddComponent<MeshFilter>();
        mergedMeshFilter.mesh = combinedMesh;

        MeshRenderer mergedMeshRenderer = mergedTrigger.AddComponent<MeshRenderer>();
        mergedMeshRenderer.enabled = false;  // 隐藏渲染器

        // 添加 MeshCollider
        MeshCollider meshCollider = mergedTrigger.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = combinedMesh;

        // 检查是否可以设置为凸形触发器
        if (combinedMesh.vertexCount <= 255)  // Unity 对凸形网格的限制
        {
            meshCollider.convex = true;
            meshCollider.isTrigger = true;
            Debug.Log("成功生成凸形触发器！");
        }
        else
        {
            meshCollider.convex = false;
            meshCollider.isTrigger = false;
            Debug.LogWarning("网格过于复杂，未设置为凸形触发器，已恢复为非触发器。");
            BoxCollider boxCollider = mergedTrigger.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            Debug.Log("已使用 BoxCollider 作为触发器");
        }

        Debug.Log("合并网格完成！");
    }

    private void SaveMeshAsAsset()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("未选择任何对象，请先选择需要合并的 Cube！");
            return;
        }

        CombineInstance[] combineInstances = new CombineInstance[selectedObjects.Length];
        int combineIndex = 0;
        foreach (var obj in selectedObjects)
        {
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                Debug.LogWarning($"对象 {obj.name} 缺少 MeshFilter，跳过此对象。");
                continue;
            }

            // 将网格添加到合并数组
            combineInstances[combineIndex].mesh = meshFilter.sharedMesh;
            combineInstances[combineIndex].transform = obj.transform.localToWorldMatrix;
            combineIndex++;
        }

        if (combineIndex == 0)
        {
            Debug.LogWarning("未找到有效的 Mesh 对象用于合并。");
            return;
        }

        // 合并网格
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combineInstances, true, true);

        // 将合并后的网格保存为 .asset 文件
        AssetDatabase.CreateAsset(combinedMesh, saveAssetPath);
        AssetDatabase.SaveAssets();

        Debug.Log($"合并网格已成功保存为 .asset 文件: {saveAssetPath}");
    }
    //重置模型的轴心为中心
    static void ResetPivot()
    {
        //获取选中的物体
        GameObject target = Selection.activeGameObject;
        string dialogTitle = "Tools/MyTool/ResetPivot";

        if (target == null)
        {
            EditorUtility.DisplayDialog(dialogTitle, "没有选中需要重置轴心的物体!!!", "确定");
            return;
        }

        //获取目标物体下所有网格渲染
        MeshRenderer[] meshRenderers = target.GetComponentsInChildren<MeshRenderer>(true);
        if (meshRenderers.Length == 0)
        {
            EditorUtility.DisplayDialog(dialogTitle, "选中的物体不是有效模型物体!!!", "确定");
            return;
        }
        //将所有的网格渲染的边界进行合并
        Bounds centerBounds = meshRenderers[0].bounds;
        for (int i = 1; i < meshRenderers.Length; i++)
        {
            centerBounds.Encapsulate(meshRenderers[i].bounds);
        }
        //创建目标的父物体
        Transform targetParent = new GameObject(target.name + "-Parent").transform;

        //如果目标原来已有父物体,则将创建目标父物体的父物体设为原父物体;
        Transform originalParent = target.transform.parent;
        if (originalParent != null)
        {
            targetParent.SetParent(originalParent);
        }
        //设置目标父物体的位置为合并后的网格渲染边界中心
        targetParent.position = centerBounds.center;
        //设置目标物体的父物体
        target.transform.parent = targetParent;

        Selection.activeGameObject = targetParent.gameObject;
        EditorUtility.DisplayDialog(dialogTitle, "重置模型物体的轴心完成!", "确定");
    }
}
