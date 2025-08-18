using UnityEngine;
using GunFireHeroes.Core;

namespace GunFireHeroes.Economy
{
    /// <summary>
    /// 充值包配置
    /// </summary>
    [CreateAssetMenu(fileName = "RechargePackage", menuName = "GunFire Heroes/Recharge Package")]
    public class RechargePackage : ScriptableObject
    {
        [Header("基础信息")]
        public int packageId;
        public string packageName;
        public int price; // 人民币价格（元）
        
        [Header("奖励")]
        public int baseDiamonds; // 基础点券
        public int bonusDiamonds; // 赠送点券
        
        [Header("显示")]
        public Sprite packageIcon;
        public bool isHotSale = false; // 是否热销
        public bool isFirstRechargeDouble = true; // 首充双倍
        
        [Header("描述")]
        [TextArea(2, 3)]
        public string description;
    }
    
    /// <summary>
    /// 礼包配置
    /// </summary>
    [CreateAssetMenu(fileName = "GiftPackage", menuName = "GunFire Heroes/Gift Package")]
    public class GiftPackage : ScriptableObject
    {
        [Header("基础信息")]
        public int giftId;
        public string giftName;
        public CurrencyType currencyType;
        public int price;
        
        [Header("奖励类型")]
        public bool isRandomGift = false;
        
        [Header("固定奖励")]
        public GiftReward[] fixedRewards;
        
        [Header("随机奖励")]
        public GiftReward[] randomRewards;
        
        [Header("显示")]
        public Sprite giftIcon;
        public bool isLimitedTime = false;
        public float limitedTimeHours = 24f;
        
        [Header("描述")]
        [TextArea(2, 3)]
        public string description;
    }
    
    /// <summary>
    /// 礼包奖励
    /// </summary>
    [System.Serializable]
    public class GiftReward
    {
        [Header("奖励信息")]
        public RewardType rewardType;
        public int amount;
        public float weight = 1f; // 随机权重
        
        [Header("货币奖励")]
        public CurrencyType currencyType;
        
        [Header("角色碎片奖励")]
        public int characterId;
        
        [Header("武器材料奖励")]
        public int weaponMaterialId;
        
        [Header("显示")]
        public Sprite rewardIcon;
        public string rewardName;
    }
    
    /// <summary>
    /// 奖励类型
    /// </summary>
    public enum RewardType
    {
        Currency,       // 货币
        CharacterShard, // 角色碎片
        WeaponMaterial, // 武器材料
        Experience      // 经验
    }
    
    /// <summary>
    /// 商店物品
    /// </summary>
    [CreateAssetMenu(fileName = "ShopItem", menuName = "GunFire Heroes/Shop Item")]
    public class ShopItem : ScriptableObject
    {
        [Header("基础信息")]
        public int itemId;
        public string itemName;
        public CurrencyType currencyType;
        public int price;
        
        [Header("奖励")]
        public GiftReward reward;
        
        [Header("限制")]
        public bool hasLimitedQuantity = false;
        public int maxQuantity = 1;
        public bool refreshDaily = false;
        
        [Header("显示")]
        public Sprite itemIcon;
        public bool isRecommended = false;
        
        [Header("描述")]
        [TextArea(2, 3)]
        public string description;
    }
    
    /// <summary>
    /// 权重随机选择器
    /// </summary>
    public static class WeightedRandom
    {
        public static T Select<T>(System.Collections.Generic.List<T> items) where T : IWeighted
        {
            if (items == null || items.Count == 0)
                return default(T);
                
            float totalWeight = 0f;
            foreach (var item in items)
            {
                totalWeight += item.Weight;
            }
            
            float randomValue = Random.Range(0f, totalWeight);
            float currentWeight = 0f;
            
            foreach (var item in items)
            {
                currentWeight += item.Weight;
                if (randomValue <= currentWeight)
                {
                    return item;
                }
            }
            
            return items[0];
        }
    }
    
    /// <summary>
    /// 权重接口
    /// </summary>
    public interface IWeighted
    {
        float Weight { get; }
    }
    
    /// <summary>
    /// 权重礼包类（用于随机礼包示例）
    /// </summary>
    [System.Serializable]
    public class WeightedGiftPack : IWeighted
    {
        public int roleShards;
        public int weaponMaterials;
        public float weight;
        
        public float Weight => weight;
        
        public WeightedGiftPack(int shards, int materials, float w)
        {
            roleShards = shards;
            weaponMaterials = materials;
            weight = w;
        }
    }
}