using UnityEngine;
using TMPro;

namespace SU10007
{
    /// <summary>
    /// 伤害数字效果 - 处理伤害数字的显示和动画
    /// </summary>
    public class DamageNumberEffect : MonoBehaviour
    {
        [Header("显示设置")]
        [Tooltip("上升速度")]
        public float floatSpeed = 1.0f;              // 上升速度
        [Tooltip("停留时间(秒)")]
        public float duration = 1.0f;                // 显示时间
        [Tooltip("初始缩放动画")]
        public bool useScaleAnimation = true;        // 是否使用缩放动画
        [Tooltip("缩放动画持续时间")]
        public float scaleAnimDuration = 0.2f;       // 缩放动画时间
        
        [Header("组件引用")]
        public TextMeshPro textMeshPro;              // 文本网格组件
        
        private float elapsedTime = 0f;              // 已经过时间
        private Vector3 startPos;                    // 初始位置
        private Vector3 targetScale;                 // 目标缩放
        
        /// <summary>
        /// 初始化
        /// </summary>
        private void Start()
        {
            if (textMeshPro == null)
            {
                textMeshPro = GetComponentInChildren<TextMeshPro>();
            }
            
            startPos = transform.position;
            
            // 初始缩放设置为0，然后逐渐缩放到目标大小
            if (useScaleAnimation)
            {
                targetScale = transform.localScale;
                transform.localScale = Vector3.zero;
            }
            
            // 确保面向相机
            LookAtCamera();
        }
        
        /// <summary>
        /// 更新
        /// </summary>
        private void Update()
        {
            elapsedTime += Time.deltaTime;
            
            // 处理缩放动画
            if (useScaleAnimation && elapsedTime <= scaleAnimDuration)
            {
                float t = elapsedTime / scaleAnimDuration;
                transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
            }
            
            // 上升并淡出
            float lifePercentage = elapsedTime / duration;
            
            // 上升移动
            transform.position = startPos + new Vector3(0, floatSpeed * lifePercentage, 0);
            
            // 淡出
            if (textMeshPro != null)
            {
                Color color = textMeshPro.color;
                color.a = 1.0f - lifePercentage;
                textMeshPro.color = color;
            }
            
            // 确保始终面向相机
            LookAtCamera();
            
            // 超时销毁
            if (elapsedTime >= duration)
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// 设置伤害数值
        /// </summary>
        /// <param name="damage">伤害值</param>
        /// <param name="isCritical">是否暴击</param>
        public void SetDamageValue(float damage, bool isCritical = false)
        {
            if (textMeshPro == null)
            {
                textMeshPro = GetComponentInChildren<TextMeshPro>();
                if (textMeshPro == null) return;
            }
            
            // 设置文本
            textMeshPro.text = damage.ToString("0");
            
            // 设置颜色和大小（暴击效果）
            if (isCritical)
            {
                textMeshPro.color = Color.red;
                textMeshPro.fontSize *= 1.5f;
                
                // 暴击使用震动效果
                transform.localEulerAngles = new Vector3(0, 0, Random.Range(-15f, 15f));
            }
            else if (damage >= 30f)
            {
                textMeshPro.color = Color.red;
            }
            else if (damage >= 15f)
            {
                textMeshPro.color = Color.yellow;
            }
            else
            {
                textMeshPro.color = Color.white;
            }
        }
        
        /// <summary>
        /// 设置伤害文本
        /// </summary>
        /// <param name="text">显示文本</param>
        /// <param name="color">文本颜色</param>
        public void SetText(string text, Color color)
        {
            if (textMeshPro == null)
            {
                textMeshPro = GetComponentInChildren<TextMeshPro>();
                if (textMeshPro == null) return;
            }
            
            textMeshPro.text = text;
            textMeshPro.color = color;
        }
        
        /// <summary>
        /// 使伤害数字始终面向相机
        /// </summary>
        private void LookAtCamera()
        {
            if (Camera.main != null)
            {
                // 使文字永远面向相机
                transform.rotation = Camera.main.transform.rotation;
            }
        }
    }
}
