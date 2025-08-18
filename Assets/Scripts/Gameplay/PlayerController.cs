using UnityEngine;
using GunFireHeroes.Character;
using GunFireHeroes.Weapon;

namespace GunFireHeroes.Gameplay
{
    /// <summary>
    /// 玩家控制器，负责玩家角色的移动、攻击等操作
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("移动设置")]
        public float moveSpeed = 5f;
        public float jumpForce = 10f;
        
        [Header("攻击设置")]
        public Transform firePoint;
        public float fireRate = 0.5f;
        
        [Header("组件引用")]
        public Rigidbody2D rb;
        public Animator animator;
        public SpriteRenderer spriteRenderer;
        
        private float nextFireTime = 0f;
        private bool isGrounded = true;
        private bool facingRight = true;
        
        // 角色数据
        private CharacterData currentCharacter;
        private CharacterStats characterStats;
        private WeaponData currentWeapon;
        private WeaponStats weaponStats;
        
        // 输入
        private float horizontalInput;
        private bool jumpInput;
        private bool fireInput;
        
        private void Start()
        {
            InitializePlayer();
        }
        
        private void Update()
        {
            HandleInput();
            HandleMovement();
            HandleAttack();
            UpdateAnimations();
        }
        
        private void InitializePlayer()
        {
            // 获取当前装备的角色和武器
            var characterManager = FindObjectOfType<CharacterManager>();
            var weaponManager = FindObjectOfType<WeaponManager>();
            
            if (characterManager != null)
            {
                var ownedCharacters = characterManager.GetOwnedCharacters();
                if (ownedCharacters.Count > 0)
                {
                    currentCharacter = ownedCharacters[0]; // 使用第一个角色
                    characterStats = characterManager.CalculateCharacterStats(currentCharacter);
                    
                    // 应用角色属性
                    moveSpeed = characterStats.moveSpeed;
                }
            }
            
            if (weaponManager != null)
            {
                currentWeapon = weaponManager.GetEquippedWeapon();
                if (currentWeapon != null)
                {
                    weaponStats = weaponManager.CalculateWeaponStats(currentWeapon);
                    fireRate = weaponStats.attackSpeed;
                }
            }
            
            // 初始化组件
            if (rb == null)
                rb = GetComponent<Rigidbody2D>();
            if (animator == null)
                animator = GetComponent<Animator>();
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        private void HandleInput()
        {
            // 获取输入
            horizontalInput = Input.GetAxis("Horizontal");
            jumpInput = Input.GetButtonDown("Jump");
            fireInput = Input.GetButton("Fire1") || Input.GetMouseButton(0);
            
            // 移动端触摸输入处理
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                
                // 简单的触摸移动逻辑
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    float touchX = touchPos.x;
                    float playerX = transform.position.x;
                    
                    if (Mathf.Abs(touchX - playerX) > 0.5f)
                    {
                        horizontalInput = touchX > playerX ? 1f : -1f;
                    }
                    else
                    {
                        horizontalInput = 0f;
                    }
                }
                
                // 触摸攻击
                if (touch.phase == TouchPhase.Began)
                {
                    fireInput = true;
                }
            }
        }
        
        private void HandleMovement()
        {
            // 水平移动
            Vector2 velocity = rb.velocity;
            velocity.x = horizontalInput * moveSpeed;
            rb.velocity = velocity;
            
            // 跳跃
            if (jumpInput && isGrounded)
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                isGrounded = false;
            }
            
            // 翻转角色
            if (horizontalInput > 0 && !facingRight)
            {
                Flip();
            }
            else if (horizontalInput < 0 && facingRight)
            {
                Flip();
            }
        }
        
        private void HandleAttack()
        {
            if (fireInput && Time.time >= nextFireTime)
            {
                Fire();
                nextFireTime = Time.time + (1f / fireRate);
            }
        }
        
        private void Fire()
        {
            if (currentWeapon == null || firePoint == null)
                return;
                
            // 创建子弹
            var weaponManager = FindObjectOfType<WeaponManager>();
            if (weaponManager != null)
            {
                var weaponConfig = weaponManager.GetWeaponConfig(currentWeapon.weaponId);
                if (weaponConfig != null && weaponConfig.bulletPrefab != null)
                {
                    // 使用对象池创建子弹
                    var objectPool = Utils.ObjectPool.Instance;
                    if (objectPool != null)
                    {
                        GameObject bullet = objectPool.GetObject("Bullet");
                        if (bullet != null)
                        {
                            bullet.transform.position = firePoint.position;
                            bullet.transform.rotation = firePoint.rotation;
                            
                            // 设置子弹属性
                            var bulletScript = bullet.GetComponent<Bullet>();
                            if (bulletScript != null)
                            {
                                bulletScript.Initialize(weaponStats.damage, weaponStats.bulletSpeed, facingRight);
                            }
                        }
                    }
                    else
                    {
                        // 直接实例化
                        GameObject bullet = Instantiate(weaponConfig.bulletPrefab, firePoint.position, firePoint.rotation);
                        var bulletScript = bullet.GetComponent<Bullet>();
                        if (bulletScript != null)
                        {
                            bulletScript.Initialize(weaponStats.damage, weaponStats.bulletSpeed, facingRight);
                        }
                    }
                }
            }
            
            // 播放射击音效
            var audioManager = Utils.AudioManager.Instance;
            if (audioManager != null)
            {
                audioManager.PlaySFX("Shoot");
            }
        }
        
        private void UpdateAnimations()
        {
            if (animator == null)
                return;
                
            // 设置动画参数
            animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetBool("IsShooting", fireInput);
        }
        
        private void Flip()
        {
            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                isGrounded = true;
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            // 处理道具拾取
            if (other.CompareTag("Pickup"))
            {
                HandlePickup(other.gameObject);
            }
            
            // 处理敌人碰撞
            if (other.CompareTag("Enemy"))
            {
                TakeDamage(10); // 简单的伤害处理
            }
        }
        
        private void HandlePickup(GameObject pickup)
        {
            // 处理不同类型的道具
            var pickupScript = pickup.GetComponent<Pickup>();
            if (pickupScript != null)
            {
                pickupScript.Collect();
            }
            
            Destroy(pickup);
        }
        
        public void TakeDamage(float damage)
        {
            // 计算实际伤害（考虑防御力）
            float actualDamage = Mathf.Max(1, damage - characterStats.defense);
            
            // 扣除生命值
            // 这里需要实现生命值系统
            
            // 播放受伤音效
            var audioManager = Utils.AudioManager.Instance;
            if (audioManager != null)
            {
                audioManager.PlaySFX("Hit");
            }
            
            // 更新UI
            var uiManager = FindObjectOfType<UI.UIManager>();
            if (uiManager != null)
            {
                // uiManager.UpdateHealthBar(currentHP, maxHP);
            }
        }
        
        public void Heal(float amount)
        {
            // 恢复生命值
            // 这里需要实现生命值系统
            
            // 播放治疗音效
            var audioManager = Utils.AudioManager.Instance;
            if (audioManager != null)
            {
                audioManager.PlaySFX("Heal");
            }
        }
    }
}