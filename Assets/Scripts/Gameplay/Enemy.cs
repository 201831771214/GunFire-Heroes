using UnityEngine;
using GunFireHeroes.Utils;

namespace GunFireHeroes.Gameplay
{
    /// <summary>
    /// 敌人基类，负责敌人的基本行为
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        [Header("敌人属性")]
        public float maxHealth = 100f;
        public float currentHealth;
        public float damage = 20f;
        public float moveSpeed = 3f;
        public float attackRange = 2f;
        public float detectionRange = 10f;
        
        [Header("AI设置")]
        public EnemyAIType aiType = EnemyAIType.Aggressive;
        public float attackCooldown = 2f;
        
        [Header("组件引用")]
        public Rigidbody2D rb;
        public Animator animator;
        public SpriteRenderer spriteRenderer;
        public Transform attackPoint;
        
        [Header("掉落设置")]
        public DropItem[] dropItems;
        
        protected Transform player;
        protected float lastAttackTime;
        protected bool facingRight = true;
        protected bool isDead = false;
        
        // 状态机
        protected EnemyState currentState = EnemyState.Patrol;
        
        protected virtual void Start()
        {
            InitializeEnemy();
        }
        
        protected virtual void Update()
        {
            if (isDead)
                return;
                
            UpdateAI();
            UpdateAnimations();
        }
        
        protected virtual void InitializeEnemy()
        {
            currentHealth = maxHealth;
            
            // 查找玩家
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            
            // 初始化组件
            if (rb == null)
                rb = GetComponent<Rigidbody2D>();
            if (animator == null)
                animator = GetComponent<Animator>();
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        protected virtual void UpdateAI()
        {
            if (player == null)
                return;
                
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            
            switch (aiType)
            {
                case EnemyAIType.Aggressive:
                    UpdateAggressiveAI(distanceToPlayer);
                    break;
                case EnemyAIType.Defensive:
                    UpdateDefensiveAI(distanceToPlayer);
                    break;
                case EnemyAIType.Patrol:
                    UpdatePatrolAI(distanceToPlayer);
                    break;
                case EnemyAIType.Ranged:
                    UpdateRangedAI(distanceToPlayer);
                    break;
            }
        }
        
        protected virtual void UpdateAggressiveAI(float distanceToPlayer)
        {
            if (distanceToPlayer <= attackRange)
            {
                currentState = EnemyState.Attack;
                Attack();
            }
            else if (distanceToPlayer <= detectionRange)
            {
                currentState = EnemyState.Chase;
                ChasePlayer();
            }
            else
            {
                currentState = EnemyState.Patrol;
                Patrol();
            }
        }
        
        protected virtual void UpdateDefensiveAI(float distanceToPlayer)
        {
            if (distanceToPlayer <= attackRange)
            {
                currentState = EnemyState.Attack;
                Attack();
            }
            else if (distanceToPlayer <= detectionRange * 0.5f)
            {
                currentState = EnemyState.Chase;
                ChasePlayer();
            }
            else
            {
                currentState = EnemyState.Idle;
            }
        }
        
        protected virtual void UpdatePatrolAI(float distanceToPlayer)
        {
            if (distanceToPlayer <= attackRange)
            {
                currentState = EnemyState.Attack;
                Attack();
            }
            else if (distanceToPlayer <= detectionRange * 0.7f)
            {
                currentState = EnemyState.Chase;
                ChasePlayer();
            }
            else
            {
                currentState = EnemyState.Patrol;
                Patrol();
            }
        }
        
        protected virtual void UpdateRangedAI(float distanceToPlayer)
        {
            if (distanceToPlayer <= detectionRange && distanceToPlayer > attackRange * 0.5f)
            {
                currentState = EnemyState.Attack;
                RangedAttack();
            }
            else if (distanceToPlayer < attackRange * 0.5f)
            {
                currentState = EnemyState.Retreat;
                RetreatFromPlayer();
            }
            else
            {
                currentState = EnemyState.Patrol;
                Patrol();
            }
        }
        
        protected virtual void ChasePlayer()
        {
            if (player == null)
                return;
                
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
            
            // 翻转精灵
            if (direction.x > 0 && !facingRight)
                Flip();
            else if (direction.x < 0 && facingRight)
                Flip();
        }
        
        protected virtual void Attack()
        {
            if (Time.time - lastAttackTime < attackCooldown)
                return;
                
            lastAttackTime = Time.time;
            
            // 播放攻击动画
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
            
            // 造成伤害
            if (player != null)
            {
                var playerController = player.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.TakeDamage(damage);
                }
            }
            
            // 播放攻击音效
            var audioManager = AudioManager.Instance;
            if (audioManager != null)
            {
                audioManager.PlaySFX("EnemyAttack");
            }
        }
        
        protected virtual void RangedAttack()
        {
            if (Time.time - lastAttackTime < attackCooldown)
                return;
                
            lastAttackTime = Time.time;
            
            // 发射子弹
            if (attackPoint != null)
            {
                var objectPool = ObjectPool.Instance;
                if (objectPool != null)
                {
                    GameObject bullet = objectPool.GetObject("EnemyBullet");
                    if (bullet != null)
                    {
                        bullet.transform.position = attackPoint.position;
                        
                        Vector2 direction = (player.position - attackPoint.position).normalized;
                        var bulletScript = bullet.GetComponent<EnemyBullet>();
                        if (bulletScript != null)
                        {
                            bulletScript.Initialize(damage, 15f, direction);
                        }
                    }
                }
            }
        }
        
        protected virtual void RetreatFromPlayer()
        {
            if (player == null)
                return;
                
            Vector2 direction = (transform.position - player.position).normalized;
            rb.velocity = new Vector2(direction.x * moveSpeed * 0.7f, rb.velocity.y);
            
            // 翻转精灵
            if (direction.x > 0 && !facingRight)
                Flip();
            else if (direction.x < 0 && facingRight)
                Flip();
        }
        
        protected virtual void Patrol()
        {
            // 简单的巡逻逻辑
            if (Random.Range(0f, 1f) < 0.02f) // 2%概率改变方向
            {
                facingRight = !facingRight;
                Flip();
            }
            
            float direction = facingRight ? 1f : -1f;
            rb.velocity = new Vector2(direction * moveSpeed * 0.5f, rb.velocity.y);
        }
        
        protected virtual void Flip()
        {
            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
        
        protected virtual void UpdateAnimations()
        {
            if (animator == null)
                return;
                
            animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
            animator.SetBool("IsAttacking", currentState == EnemyState.Attack);
            animator.SetBool("IsDead", isDead);
        }
        
        public virtual void TakeDamage(float damageAmount)
        {
            if (isDead)
                return;
                
            currentHealth -= damageAmount;
            
            // 播放受伤音效
            var audioManager = AudioManager.Instance;
            if (audioManager != null)
            {
                audioManager.PlaySFX("EnemyHit");
            }
            
            // 受伤效果
            if (spriteRenderer != null)
            {
                StartCoroutine(FlashRed());
            }
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        protected virtual System.Collections.IEnumerator FlashRed()
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
        }
        
        protected virtual void Die()
        {
            isDead = true;
            currentState = EnemyState.Dead;
            
            // 停止移动
            rb.velocity = Vector2.zero;
            
            // 播放死亡动画
            if (animator != null)
            {
                animator.SetTrigger("Die");
            }
            
            // 播放死亡音效
            var audioManager = AudioManager.Instance;
            if (audioManager != null)
            {
                audioManager.PlaySFX("EnemyDeath");
            }
            
            // 掉落物品
            DropItems();
            
            // 延迟销毁
            Invoke(nameof(DestroyEnemy), 2f);
        }
        
        protected virtual void DropItems()
        {
            foreach (var drop in dropItems)
            {
                if (Random.value < drop.dropRate)
                {
                    // 创建掉落物品
                    // 这里可以实现具体的掉落逻辑
                    Debug.Log($"掉落物品: {drop.itemName}");
                }
            }
        }
        
        protected virtual void DestroyEnemy()
        {
            // 使用对象池回收
            var pooledObject = GetComponent<PooledObject>();
            if (pooledObject != null)
            {
                pooledObject.ReturnToPool();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            // 绘制检测范围
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
            
            // 绘制攻击范围
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
    
    public enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Retreat,
        Dead
    }
}