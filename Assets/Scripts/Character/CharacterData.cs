using UnityEngine;
using System;

namespace GunFireHeroes.Character
{
    /// <summary>
    /// 角色数据类
    /// </summary>
    [System.Serializable]
    public class CharacterData
    {
        public int characterId;
        public int level = 1;
        public int starLevel = 1;
        public int experience = 0;
        public bool hasQualityBreakthrough = false;
        
        public CharacterData()
        {
        }
        
        public CharacterData(int id)
        {
            characterId = id;
        }
    }
    
    /// <summary>
    /// 角色碎片数据
    /// </summary>
    [System.Serializable]
    public class CharacterShardData
    {
        public int characterId;
        public int shardCount;
    }
    
    /// <summary>
    /// 角色配置
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterConfig", menuName = "GunFire Heroes/Character Config")]
    public class CharacterConfig : ScriptableObject
    {
        [Header("基础信息")]
        public int characterId;
        public string characterName;
        public CharacterRarity rarity;
        public CharacterType type;
        
        [Header("外观")]
        public Sprite characterIcon;
        public Sprite characterPortrait;
        public RuntimeAnimatorController animatorController;
        
        [Header("基础属性")]
        public float baseAttack = 100f;
        public float baseHP = 500f;
        public float baseDefense = 50f;
        public float moveSpeed = 5f;
        public float attackSpeed = 1f;
        
        [Header("技能")]
        public SkillConfig normalAttack;
        public SkillConfig specialSkill;
        public SkillConfig ultimateSkill;
        
        [Header("合成需求")]
        public int synthesisRequiredShards = 30;
        
        [Header("描述")]
        [TextArea(3, 5)]
        public string description;
    }
    
    /// <summary>
    /// 角色稀有度
    /// </summary>
    public enum CharacterRarity
    {
        A = 1,      // A级
        S = 2,      // S级
        SS = 3,     // SS级
        SSR = 4,    // SSR级
        SR = 5,     // SR级
        UR = 6      // UR级
    }
    
    /// <summary>
    /// 角色类型
    /// </summary>
    public enum CharacterType
    {
        Assault,    // 突击型
        Defense,    // 防御型
        Support,    // 支援型
        Sniper      // 狙击型
    }
    
    /// <summary>
    /// 角色属性
    /// </summary>
    [System.Serializable]
    public class CharacterStats
    {
        public int attack;
        public int hp;
        public int defense;
        public float moveSpeed;
        public float attackSpeed;
        public float criticalRate;
        public float criticalDamage;
        
        public CharacterStats()
        {
            attack = 0;
            hp = 0;
            defense = 0;
            moveSpeed = 5f;
            attackSpeed = 1f;
            criticalRate = 0.05f;
            criticalDamage = 1.5f;
        }
    }
    
    /// <summary>
    /// 技能配置
    /// </summary>
    [System.Serializable]
    public class SkillConfig
    {
        public string skillName;
        public SkillType skillType;
        public float damage;
        public float cooldown;
        public float range;
        public GameObject effectPrefab;
        
        [TextArea(2, 3)]
        public string description;
    }
    
    /// <summary>
    /// 技能类型
    /// </summary>
    public enum SkillType
    {
        NormalAttack,   // 普通攻击
        Special,        // 特殊技能
        Ultimate        // 终极技能
    }
}