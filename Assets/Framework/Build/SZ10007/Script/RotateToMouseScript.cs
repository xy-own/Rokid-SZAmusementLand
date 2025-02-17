using UnityEngine;
using System.Collections;

/// <summary>
/// 物体朝向鼠标旋转控制脚本
/// 支持2D和3D模式的物体旋转
/// </summary>
public class RotateToMouseScript : MonoBehaviour
{
	[Header("射线检测设置")]
	[Tooltip("射线最大检测距离")]
	public float maximumLength = 100f;

	[Header("内部参数")]
	[SerializeField]
	private bool use2D = false;  // 是否使用2D模式
	private Ray rayMouse;        // 鼠标射线
	private Vector3 direction;    // 旋转方向
	private Quaternion rotation;  // 旋转四元数
	private Camera cam;          // 主相机引用

	// 更新频率控制
	private readonly WaitForSeconds updateTime = new WaitForSeconds(0.01f);

	/// <summary>
	/// 开始射线更新协程
	/// </summary>
	public void StartUpdateRay()
	{
		StartCoroutine(UpdateRay());
	}

	/// <summary>
	/// 更新射线检测
	/// </summary>
	private IEnumerator UpdateRay()
	{
		if (cam == null)
		{
			Debug.LogWarning("未设置摄像机引用");
			yield break;
		}

		while (true)
		{
			if (use2D)
			{
				Update2DRotation();
			}
			else
			{
				Update3DRotation();
			}

			yield return updateTime;
		}
	}

	/// <summary>
	/// 更新2D模式下的旋转
	/// </summary>
	private void Update2DRotation()
	{
		Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

		if (angle > 180)
		{
			angle -= 360;
		}

		rotation.eulerAngles = new Vector3(-angle, 90, 0);
		transform.rotation = rotation;
	}

	/// <summary>
	/// 更新3D模式下的旋转
	/// </summary>
	private void Update3DRotation()
	{
		rayMouse = cam.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(rayMouse.origin, rayMouse.direction, out RaycastHit hit, maximumLength))
		{
			RotateToMouse(gameObject, hit.point);
		}
		else
		{
			Vector3 pos = rayMouse.GetPoint(maximumLength);
			RotateToMouse(gameObject, pos);
		}
	}

	/// <summary>
	/// 使物体旋转朝向目标点
	/// </summary>
	/// <param name="obj">要旋转的物体</param>
	/// <param name="destination">目标位置</param>
	public void RotateToMouse(GameObject obj, Vector3 destination)
	{
		direction = destination - obj.transform.position;
		rotation = Quaternion.LookRotation(direction);
		obj.transform.localRotation = Quaternion.Lerp(obj.transform.rotation, rotation, 1);
	}

	/// <summary>
	/// 设置2D/3D模式
	/// </summary>
	/// <param name="state">true为2D模式，false为3D模式</param>
	public void Set2D(bool state)
	{
		use2D = state;
	}

	/// <summary>
	/// 设置摄像机引用
	/// </summary>
	/// <param name="camera">摄像机对象</param>
	public void SetCamera(Camera camera)
	{
		cam = camera;
	}

	/// <summary>
	/// 获取当前旋转方向
	/// </summary>
	public Vector3 GetDirection()
	{
		return direction;
	}

	/// <summary>
	/// 获取当前旋转四元数
	/// </summary>
	public Quaternion GetRotation()
	{
		return rotation;
	}
}
