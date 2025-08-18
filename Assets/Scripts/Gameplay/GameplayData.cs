using UnityEngine;
using GunFireHeroes.Economy;

namespace GunFireHeroes.Gameplay
{
    /// <summary>
    /// 关卡配置
    /// </summary>
    [CreateAssetMenu(fileName = "StageConfig", menuName = "GunFire Heroes/Stage Config")]
    public class StageConfig : ScriptableObject
    {
        [Header("基础信息")]
        public int stageId;
        public string stageName;
        public StageDifficulty difficulty;
        
        [Header("场景设置")]
        public GameObject stagePrefab;
        public Sprite backgroundSprite;
        public AudioClip backgroundMusic;
        
        [Header("敌人配置")]
        public EnemyWave[] enemyWaves;
        
        [Header("奖励")]
        public int goldReward = 100;
        public int experienceReward = 50;
        public GiftReward[] rewards;
        
        [Header("解锁条件")]
        public int requiredStage = 0; // 需要完成的前置关卡
        public int requiredPlayerLevel = 1;
        
        [Header("描述")]
        [TextArea(2, 3)]
        public string description;
    }
    
    /// <summary>
    /// 关卡难度
    /// </summary>
    public enum StageDifficulty
    {
        Easy,       // 简单
        Normal,     // 普通
        Hard,       // 困难
        Hell,       // 地狱
        Inferno     // 炼狱
    }
    
    /// <summary>
    /// 敌人波次
    /// </summary>
    [System.Serializable]
    public class EnemyWave
    {
        [Header("波次信息")]
        public int waveIndex;
        public float delayTime = 0f;
        
        [Header("敌人配置")]
        public EnemySpawnData[] enemies;
        
        [Header("Boss")]
        public bool hasBoss = false;
        public EnemySpawnData bossData;
    }
    
    /// <summary>
    /// 敌人生成数据
    /// </summary>
    [System.Serializable]
    public class EnemySpawnData
    {
        public int enemyId;
        public int count = 1;
        public Vector2 spawnPosition;
        public float spawnInterval = 1f;
    }
    
    /// <summary>
    /// 副本配置
    /// </summary>
    [CreateAssetMenu(fileName = "DungeonConfig", menuName = "GunFire Heroes/Dungeon Config")]
    public class DungeonConfig : ScriptableObject
    {
        [Header("基础信息")]
        public int dungeonId;
        public string dungeonName;
        public DungeonType dungeonType;
        public StageDifficulty difficulty;
        
        [Header("挑战限制")]
        public int maxChallengesPerDay = 3;
        public int energyCost = 10;
        
        [Header("场景设置")]
        public GameObject dungeonPrefab;
        public Sprite backgroundSprite;
        
        [Header("敌人配置")]
        public EnemyWave[] enemyWaves;
        
        [Header("掉落配置")]
        public DropItem[] dropItems;
        
        [Header("解锁条件")]
        public int requiredStage = 1;
        public int requiredPlayerLevel = 1;
        
        [Header("描述")]
        [TextArea(3, 5)]
        public string description;
    }
    
    /// <summary>
    /// 副本类型
    /// </summary>
    public enum DungeonType
    {
        CharacterShard, // 角色碎片副本
        WeaponMaterial, // 武器材料副本
        Gold,           // 金币副本
        Experience      // 经验副本
    }
    
    /// <summary>
    /// 掉落物品
    /// </summary>
    [System.Serializable]
    public class DropItem
    {
        [Header("物品信息")]
        public string itemName;
        public DropItemType itemType;
        public int itemId;
        public int amount = 1;
        
        [Header("掉落概率")]
        [Range(0f, 1f)]
        public float dropRate = 0.1f;
        
        [Header("显示")]
        public Sprite itemIcon;
    }
    
    /// <summary>
    /// 掉落物品类型
    /// </summary>
    public enum DropItemType
    {
        CharacterShard, // 角色碎片
        WeaponMaterial, // 武器材料
        Currency,       // 货币
        Experience      // 经验
    }
    
    /// <summary>
    /// PVP配置
    /// </summary>
    [CreateAssetMenu(fileName = "PVPConfig", menuName = "GunFire Heroes/PVP Config")]
    public class PVPConfig : ScriptableObject
    {
        [Header("匹配设置")]
        public float matchmakingTimeout = 30f;
        public int maxRankDifference = 200;
        
        [Header("战斗设置")]
        public float battleDuration = 180f; // 3分钟
        public GameObject[] pvpMaps;
        
        [Header("奖励设置")]
        public PVPReward[] rankRewards;
        public int baseRankCoinReward = 10;
        public int winBonusRankCoin = 5;
        
        [Header("赛季设置")]
        public int seasonDurationDays = 30;
        public PVPRank[] rankTiers;
    }
    
    /// <summary>
    /// PVP奖励
    /// </summary>
    [System.Serializable]
    public class PVPReward
    {
        public PVPRank requiredRank;
        public GiftReward[] rewards;
    }
    
    /// <summary>
    /// PVP段位
    /// </summary>
    public enum PVPRank
    {
        Bronze,     // 青铜
        Silver,     // 白银
        Gold,       // 黄金
        Platinum,   // 铂金
        Diamond,    // 钻石
        Master,     // 大师
        Grandmaster // 宗师
    }
    
    /// <summary>
    /// 双人匹配配置
    /// </summary>
    [CreateAssetMenu(fileName = "MatchConfig", menuName = "GunFire Heroes/Match Config")]
    public class MatchConfig : ScriptableObject
    {
        [Header("时间限制")]
        public int startHour = 8;   // 8:00
        public int endHour = 23;    // 23:00
        
        [Header("关卡设置")]
        public int totalStageCount = 520;
        public StageConfig[] matchStages;
        
        [Header("奖励设置")]
        public int baseTokenReward = 1;
        public int perfectClearBonus = 2;
        
        [Header("兑换商店")]
        public MatchShopItem[] shopItems;
    }
    
    /// <summary>
    /// 匹配商店物品
    /// </summary>
    [System.Serializable]
    public class MatchShopItem
    {
        public string itemName;
        public int itemId;
        public int tokenCost;
        public DropItemType itemType;
        public int amount;
        public Sprite itemIcon;
        
        [Header("限制")]
        public bool hasLimitedQuantity = false;
        public int maxQuantity = 1;
        public bool refreshDaily = false;
    }
    
    /// <summary>
    /// 敌人配置
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "GunFire Heroes/Enemy Config")]
    public class EnemyConfig : ScriptableObject
    {
        [Header("基础信息")]
        public int enemyId;
        public string enemyName;
        public EnemyType enemyType;
        
        [Header("外观")]
        public Sprite enemySprite;
        public RuntimeAnimatorController animatorController;
        
        [Header("属性")]
        public float maxHP = 100f;
        public float attack = 20f;
        public float defense = 10f;
        public float moveSpeed = 3f;
        public float attackSpeed = 1f;
        
        [Header("AI行为")]
        public EnemyAIType aiType;
        public float detectionRange = 10f;
        public float attackRange = 2f;
        
        [Header("掉落")]
        public DropItem[] dropItems;
        
        [Header("特殊能力")]
        public EnemySkill[] skills;
    }
    
    /// <summary>
    /// 敌人类型
    /// </summary>
    public enum EnemyType
    {
        Normal,     // 普通敌人
        Elite,      // 精英敌人
        Boss        // Boss
    }
    
    /// <summary>
    /// 敌人AI类型
    /// </summary>
    public enum EnemyAIType
    {
        Aggressive, // 主动攻击
        Defensive,  // 防御型
        Patrol,     // 巡逻型
        Ranged      // 远程型
    }
    
    /// <summary>
    /// 敌人技能
    /// </summary>
    [System.Serializable]
    public class EnemySkill
    {
        public string skillName;
        public float damage;
        public float cooldown;
        public float range;
        public GameObject effectPrefab;
    }
}