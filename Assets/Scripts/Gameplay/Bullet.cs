using UnityEngine;
using GunFireHeroes.Utils;

namespace GunFireHeroes.Gameplay
{
    /// <summary>
    /// 子弹类，负责子弹的移动和碰撞检测
    /// </summary>
    public class Bullet : MonoBehaviour
    {
        [Header("子弹属性")]
        public float damage = 10f;
        public float speed = 20f;
        public float lifetime = 5f;
        
        [Header("特效")]
        public GameObject hitEffect;
        public TrailRenderer trailRenderer;
        
        private Rigidbody2D rb;
        private bool isInitialized = false;
        private Vector2 direction;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0f;
            }
        }
        
        private void OnEnable()
        {
            // 设置生命周期
            Invoke(nameof(DestroyBullet), lifetime);
        }
        
        private void OnDisable()
        {
            // 取消延迟调用
            CancelInvoke();
            
            // 重置轨迹
            if (trailRenderer != null)
            {
                trailRenderer.Clear();
            }
        }
        
        /// <summary>
        /// 初始化子弹
        /// </summary>
        public void Initialize(float bulletDamage, float bulletSpeed, bool facingRight)
        {
            damage = bulletDamage;
            speed = bulletSpeed;
            
            // 设置移动方向
            direction = facingRight ? Vector2.right : Vector2.left;
            
            // 设置速度
            if (rb != null)
            {
                rb.velocity = direction * speed;
            }
            
            isInitialized = true;
        }
        
        private void FixedUpdate()
        {
            if (!isInitialized)
                return;
                
            // 确保子弹保持速度
            if (rb != null && rb.velocity.magnitude < speed * 0.9f)
            {
                rb.velocity = direction * speed;
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            // 忽略玩家和其他子弹
            if (other.CompareTag("Player") || other.CompareTag("Bullet"))
                return;
                
            // 击中敌人
            if (other.CompareTag("Enemy"))
            {
                var enemy = other.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
                
                CreateHitEffect();
                DestroyBullet();
            }
            // 击中墙壁或障碍物
            else if (other.CompareTag("Wall") || other.CompareTag("Ground"))
            {
                CreateHitEffect();
                DestroyBullet();
            }
        }
        
        private void CreateHitEffect()
        {
            if (hitEffect != null)
            {
                // 使用对象池创建特效
                var objectPool = ObjectPool.Instance;
                if (objectPool != null)
                {
                    GameObject effect = objectPool.GetObject("HitEffect");
                    if (effect != null)
                    {
                        effect.transform.position = transform.position;
                        effect.transform.rotation = Quaternion.identity;
                        
                        // 自动回收特效
                        var pooledObject = effect.GetComponent<PooledObject>();
                        if (pooledObject != null)
                        {
                            pooledObject.ReturnToPool(2f); // 2秒后回收
                        }
                    }
                }
                else
                {
                    // 直接实例化
                    GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
                    Destroy(effect, 2f);
                }
            }
            
            // 播放击中音效
            var audioManager = AudioManager.Instance;
            if (audioManager != null)
            {
                audioManager.PlaySFX("BulletHit", 0.7f);
            }
        }
        
        private void DestroyBullet()
        {
            // 使用对象池回收
            var pooledObject = GetComponent<PooledObject>();
            if (pooledObject != null)
            {
                pooledObject.ReturnToPool();
            }
            else
            {
                // 直接销毁
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// 设置子弹颜色（用于不同武器的子弹区分）
        /// </summary>
        public void SetBulletColor(Color color)
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
            }
            
            if (trailRenderer != null)
            {
                trailRenderer.startColor = color;
                trailRenderer.endColor = new Color(color.r, color.g, color.b, 0f);
            }
        }
        
        /// <summary>
        /// 设置子弹大小
        /// </summary>
        public void SetBulletSize(float scale)
        {
            transform.localScale = Vector3.one * scale;
        }
    }
}