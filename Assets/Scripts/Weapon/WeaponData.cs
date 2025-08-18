using UnityEngine;

namespace GunFireHeroes.Weapon
{
    /// <summary>
    /// 武器数据类
    /// </summary>
    [System.Serializable]
    public class WeaponData
    {
        public int weaponId;
        public int level = 1;
        public int enhanceLevel = 0; // 强化等级
        public bool isEquipped = false;
        
        public WeaponData()
        {
        }
        
        public WeaponData(int id)
        {
            weaponId = id;
        }
    }
    
    /// <summary>
    /// 武器配置
    /// </summary>
    [CreateAssetMenu(fileName = "WeaponConfig", menuName = "GunFire Heroes/Weapon Config")]
    public class WeaponConfig : ScriptableObject
    {
        [Header("基础信息")]
        public int weaponId;
        public string weaponName;
        public WeaponType weaponType;
        public WeaponRarity rarity;
        
        [Header("外观")]
        public Sprite weaponIcon;
        public Sprite weaponSprite;
        public GameObject weaponPrefab;
        
        [Header("基础属性")]
        public float baseDamage = 50f;
        public float attackSpeed = 1f;
        public float range = 10f;
        public float accuracy = 0.9f;
        public int magazineSize = 30;
        public float reloadTime = 2f;
        
        [Header("弹道设置")]
        public GameObject bulletPrefab;
        public float bulletSpeed = 20f;
        public int bulletsPerShot = 1;
        public float spreadAngle = 0f;
        
        [Header("特殊效果")]
        public WeaponEffect[] specialEffects;
        
        [Header("强化配置")]
        public int maxEnhanceLevel = 10;
        public WeaponEnhanceConfig[] enhanceConfigs;
        
        [Header("描述")]
        [TextArea(3, 5)]
        public string description;
    }
    
    /// <summary>
    /// 武器类型
    /// </summary>
    public enum WeaponType
    {
        AssaultRifle,   // 突击步枪
        Shotgun,        // 霰弹枪
        SniperRifle,    // 狙击步枪
        Pistol,         // 手枪
        MachineGun,     // 机枪
        RocketLauncher  // 火箭筒
    }
    
    /// <summary>
    /// 武器稀有度
    /// </summary>
    public enum WeaponRarity
    {
        Common = 1,     // 普通
        Rare = 2,       // 稀有
        Epic = 3,       // 史诗
        Legendary = 4   // 传说
    }
    
    /// <summary>
    /// 武器效果
    /// </summary>
    [System.Serializable]
    public class WeaponEffect
    {
        public WeaponEffectType effectType;
        public float value;
        public float duration;
        public float probability;
        
        [TextArea(2, 3)]
        public string description;
    }
    
    /// <summary>
    /// 武器效果类型
    /// </summary>
    public enum WeaponEffectType
    {
        Burn,           // 燃烧
        Freeze,         // 冰冻
        Poison,         // 中毒
        Stun,           // 眩晕
        CriticalHit,    // 暴击
        LifeSteal,      // 生命偷取
        ArmorPierce     // 护甲穿透
    }
    
    /// <summary>
    /// 武器强化配置
    /// </summary>
    [System.Serializable]
    public class WeaponEnhanceConfig
    {
        public int enhanceLevel;
        public float damageBonus;
        public float attackSpeedBonus;
        public float rangeBonus;
        public float accuracyBonus;
        
        [Header("强化材料需求")]
        public WeaponMaterialRequirement[] materialRequirements;
        
        [Header("金币需求")]
        public int goldCost;
    }
    
    /// <summary>
    /// 武器材料需求
    /// </summary>
    [System.Serializable]
    public class WeaponMaterialRequirement
    {
        public int materialId;
        public int amount;
    }
    
    /// <summary>
    /// 武器材料配置
    /// </summary>
    [CreateAssetMenu(fileName = "WeaponMaterial", menuName = "GunFire Heroes/Weapon Material")]
    public class WeaponMaterial : ScriptableObject
    {
        [Header("基础信息")]
        public int materialId;
        public string materialName;
        public WeaponMaterialRarity rarity;
        
        [Header("外观")]
        public Sprite materialIcon;
        
        [Header("描述")]
        [TextArea(2, 3)]
        public string description;
    }
    
    /// <summary>
    /// 武器材料稀有度
    /// </summary>
    public enum WeaponMaterialRarity
    {
        Common = 1,     // 普通
        Rare = 2,       // 稀有
        Epic = 3,       // 史诗
        Legendary = 4   // 传说
    }
    
    /// <summary>
    /// 武器属性
    /// </summary>
    [System.Serializable]
    public class WeaponStats
    {
        public float damage;
        public float attackSpeed;
        public float range;
        public float accuracy;
        public int magazineSize;
        public float reloadTime;
        public float criticalRate;
        public float criticalDamage;
        
        public WeaponStats()
        {
            damage = 0f;
            attackSpeed = 1f;
            range = 10f;
            accuracy = 0.9f;
            magazineSize = 30;
            reloadTime = 2f;
            criticalRate = 0.05f;
            criticalDamage = 1.5f;
        }
    }
}