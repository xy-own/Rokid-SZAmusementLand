using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SU10007;

/// <summary>
/// 投射物移动控制脚本
/// 用于控制投射物的移动、旋转、碰撞等行为
/// </summary>
public class ProjectileMoveScript : MonoBehaviour
{
    [Header("是否启用旋转")]
    public bool rotate = false;
    [Header("旋转速度")]
    public float rotateAmount = 45f;
    [Header("是否启用弹跳")]
    public bool bounce = false;
    [Header("弹跳力度")]
    public float bounceForce = 10f;
    [Header("移动速度")]
    public float speed = 10f;
    [Header("精确度(0-100%)")]
    [Range(0, 100)]
    public float accuracy = 100f;

    [Header("枪口特效预制体")]
    public GameObject muzzlePrefab;
    [Header("击中特效预制体")]
    public GameObject hitPrefab;
    [Header("拖尾特效列表")]
    public List<GameObject> trails = new List<GameObject>();

    [Header("子弹设置")]
    [Tooltip("子弹所属(Player/NPC)")]
    public string bulletOwner = "Player";           // 子弹所属者
    [Tooltip("子弹伤害值")]
    public float damage = 10f;                      // 子弹伤害值
    [Tooltip("子弹是否可以穿透")]
    public bool canPenetrate = false;               // 是否可穿透
    [Tooltip("子弹穿透次数")]
    public int penetrateCount = 0;                  // 穿透次数

    // 私有变量
    private Vector3 startPos;
    private Vector3 offset;
    private bool collided;
    private Rigidbody rb;
    private GameObject target;
    private int hitCount = 0;                       // 命中次数计数

    private void Start()
    {
        InitializeProjectile();
        CreateMuzzleEffect();
    }

    private void InitializeProjectile()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody>();
        CalculateAccuracyOffset();
    }

    /// <summary>
    /// 计算精确度偏移
    /// </summary>
    private void CalculateAccuracyOffset()
    {
        if (accuracy == 100f) return;

        accuracy = 1f - (accuracy / 100f);
        float[] randomOffsets = new float[2];

        for (int i = 0; i < 2; i++)
        {
            float val = Random.Range(-accuracy, accuracy);
            bool isPositive = Random.value > 0.5f;
            offset = i == 0
                ? new Vector3(0, isPositive ? val : -val, 0)
                : new Vector3(0, offset.y, isPositive ? val : -val);
        }
    }

    /// <summary>
    /// 创建枪口特效
    /// </summary>
    private void CreateMuzzleEffect()
    {
        if (muzzlePrefab == null) return;

        GameObject muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
        muzzleVFX.transform.forward = transform.forward + offset;

        ParticleSystem ps = muzzleVFX.GetComponent<ParticleSystem>();
        float duration = ps != null
            ? ps.main.duration
            : muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>().main.duration;

        Destroy(muzzleVFX, duration);
    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        if (rotate)
        {
            transform.Rotate(0, 0, rotateAmount, Space.Self);
        }

        if (speed != 0 && rb != null)
        {
            rb.position += (transform.forward + offset) * (speed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 如果子弹来自NPC且碰到的是NPC，或子弹来自玩家且碰到的是玩家，则忽略该碰撞
        if ((gameObject.CompareTag("NPCBullet") && collision.gameObject.CompareTag("NPC")) ||
            (gameObject.CompareTag("PlayerBullet") && collision.gameObject.CompareTag("Player")))
        {
            return;
        }

        // 如果碰到了其他子弹，两者都销毁
        if (collision.gameObject.GetComponent<ProjectileMoveScript>() != null)
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
            return;
        }

        // 处理穿透逻辑
        hitCount++;
        if (canPenetrate && hitCount <= penetrateCount)
        {
            // 创建击中效果但不销毁子弹
            CreateHitEffect(collision.contacts[0]);
            return;
        }

        if (!bounce)
        {
            HandleNormalCollision(collision);
        }
        else
        {
            HandleBounceCollision(collision);
        }
    }

    /// <summary>
    /// 处理普通碰撞
    /// </summary>
    private void HandleNormalCollision(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet") || collided) return;

        collided = true;
        DetachTrails();
        DisableMovement();
        CreateHitEffect(collision.contacts[0]);

        // 如果碰撞对象有生命值组件，则造成伤害
        HealthComponent healthComp = collision.gameObject.GetComponent<HealthComponent>();
        if (healthComp != null)
        {
            healthComp.TakeDamage(damage, bulletOwner);
        }

        StartCoroutine(DestroyParticle(0f));
    }

    /// <summary>
    /// 处理弹跳碰撞
    /// </summary>
    private void HandleBounceCollision(Collision collision)
    {
        rb.useGravity = true;
        rb.drag = 0.5f;
        ContactPoint contact = collision.contacts[0];
        Vector3 reflectDir = Vector3.Reflect((contact.point - startPos).normalized, contact.normal);
        rb.AddForce(reflectDir * bounceForce, ForceMode.Impulse);
        Destroy(this);
    }

    /// <summary>
    /// 分离拖尾特效
    /// </summary>
    private void DetachTrails()
    {
        foreach (GameObject trail in trails)
        {
            trail.transform.parent = null;
            var ps = trail.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Stop();
                Destroy(trail, ps.main.duration + ps.main.startLifetime.constantMax);
            }
        }
    }

    private void DisableMovement()
    {
        speed = 0;
        rb.isKinematic = true;
    }

    /// <summary>
    /// 创建击中特效
    /// </summary>
    private void CreateHitEffect(ContactPoint contact)
    {
        if (hitPrefab == null) return;

        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
        GameObject hitVFX = Instantiate(hitPrefab, contact.point, rotation);

        ParticleSystem ps = hitVFX.GetComponent<ParticleSystem>();
        float duration = ps != null
            ? ps.main.duration
            : hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>().main.duration;

        Destroy(hitVFX, duration);
    }

    /// <summary>
    /// 销毁粒子效果
    /// </summary>
    public IEnumerator DestroyParticle(float waitTime)
    {
        if (transform.childCount > 0 && waitTime != 0)
        {
            Transform firstChild = transform.GetChild(0);
            List<Transform> childTransforms = new List<Transform>();

            foreach (Transform child in firstChild)
            {
                childTransforms.Add(child);
            }

            while (firstChild.localScale.x > 0)
            {
                yield return new WaitForSeconds(0.01f);
                Vector3 scaleReduction = new Vector3(0.1f, 0.1f, 0.1f);
                firstChild.localScale -= scaleReduction;

                foreach (Transform child in childTransforms)
                {
                    child.localScale -= scaleReduction;
                }
            }
        }

        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }

    /// <summary>
    /// 设置目标对象
    /// </summary>
    public void SetTarget(GameObject trg)
    {
        target = trg;
    }

    /// <summary>
    /// 设置子弹所有者
    /// </summary>
    public void SetBulletOwner(string owner)
    {
        bulletOwner = owner;
    }
}
