using UnityEngine;
using System.Collections.Generic;
using GunFireHeroes.Core;

namespace GunFireHeroes.Economy
{
    /// <summary>
    /// 经济系统管理器，负责充值、礼包、商店等功能
    /// </summary>
    public class EconomyManager : MonoBehaviour
    {
        [Header("充值配置")]
        public RechargePackage[] rechargePackages;
        
        [Header("礼包配置")]
        public GiftPackage[] giftPackages;
        
        [Header("商店配置")]
        public ShopItem[] shopItems;
        
        private Dictionary<int, RechargePackage> rechargeDict;
        private Dictionary<int, GiftPackage> giftDict;
        
        private void Awake()
        {
            InitializeDictionaries();
        }
        
        private void InitializeDictionaries()
        {
            // 初始化充值包字典
            rechargeDict = new Dictionary<int, RechargePackage>();
            foreach (var package in rechargePackages)
            {
                rechargeDict[package.packageId] = package;
            }
            
            // 初始化礼包字典
            giftDict = new Dictionary<int, GiftPackage>();
            foreach (var gift in giftPackages)
            {
                giftDict[gift.giftId] = gift;
            }
        }
        
        /// <summary>
        /// 处理充值请求
        /// </summary>
        public void RequestPayment(int packageId)
        {
            if (!rechargeDict.ContainsKey(packageId))
            {
                Debug.LogError($"充值包不存在: {packageId}");
                return;
            }
            
            var package = rechargeDict[packageId];
            
            #if UNITY_WEBGL && !UNITY_EDITOR
            // 微信支付
            ProcessWeChatPayment(package);
            #else
            // 编辑器模拟支付成功
            OnPaymentSuccess(package);
            #endif
        }
        
        private void ProcessWeChatPayment(RechargePackage package)
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
            // WX.RequestPayment(new PaymentParams{
            //     totalFee = package.price * 100, // 转换为分
            //     success = () => OnPaymentSuccess(package),
            //     fail = (res) => OnPaymentFailed(res.errMsg)
            // });
            #endif
        }
        
        private void OnPaymentSuccess(RechargePackage package)
        {
            var playerData = PlayerDataManager.PlayerData;
            
            // 计算获得的点券
            int baseDiamonds = package.baseDiamonds;
            int bonusDiamonds = package.bonusDiamonds;
            
            // 首充双倍
            if (!playerData.hasFirstRecharge)
            {
                baseDiamonds *= 2;
                bonusDiamonds *= 2;
                playerData.hasFirstRecharge = true;
                
                // 发放首充礼包
                GiveFirstRechargeGift();
            }
            
            // 添加点券
            PlayerDataManager.AddCurrency(CurrencyType.Diamond, baseDiamonds + bonusDiamonds);
            
            // 更新充值总额
            playerData.totalRechargeAmount += package.price;
            
            PlayerDataManager.SavePlayerData();
            
            Debug.Log($"充值成功！获得 {baseDiamonds + bonusDiamonds} 点券");
            
            // 触发充值成功事件
            OnRechargeSuccess?.Invoke(package);
        }
        
        private void OnPaymentFailed(string errorMsg)
        {
            Debug.LogError($"支付失败: {errorMsg}");
            OnRechargeFailed?.Invoke(errorMsg);
        }
        
        /// <summary>
        /// 发放首充礼包
        /// </summary>
        private void GiveFirstRechargeGift()
        {
            // 首充礼包：A+角色 + 金币 + 武器材料
            PlayerDataManager.AddCurrency(CurrencyType.Gold, 10000);
            
            // 这里可以添加具体的角色和武器材料发放逻辑
            Debug.Log("获得首充礼包：A+角色 + 10000金币 + 武器材料");
        }
        
        /// <summary>
        /// 购买礼包
        /// </summary>
        public bool PurchaseGiftPackage(int giftId)
        {
            if (!giftDict.ContainsKey(giftId))
            {
                Debug.LogError($"礼包不存在: {giftId}");
                return false;
            }
            
            var gift = giftDict[giftId];
            
            // 检查货币是否足够
            if (!PlayerDataManager.SpendCurrency(gift.currencyType, gift.price))
            {
                Debug.Log("货币不足，无法购买礼包");
                return false;
            }
            
            // 发放礼包奖励
            GiveGiftRewards(gift);
            
            Debug.Log($"成功购买礼包: {gift.giftName}");
            return true;
        }
        
        /// <summary>
        /// 发放礼包奖励
        /// </summary>
        private void GiveGiftRewards(GiftPackage gift)
        {
            if (gift.isRandomGift)
            {
                // 随机礼包
                var selectedReward = SelectRandomReward(gift.randomRewards);
                GiveReward(selectedReward);
            }
            else
            {
                // 固定礼包
                foreach (var reward in gift.fixedRewards)
                {
                    GiveReward(reward);
                }
            }
        }
        
        /// <summary>
        /// 选择随机奖励
        /// </summary>
        private GiftReward SelectRandomReward(GiftReward[] rewards)
        {
            float totalWeight = 0f;
            foreach (var reward in rewards)
            {
                totalWeight += reward.weight;
            }
            
            float randomValue = Random.Range(0f, totalWeight);
            float currentWeight = 0f;
            
            foreach (var reward in rewards)
            {
                currentWeight += reward.weight;
                if (randomValue <= currentWeight)
                {
                    return reward;
                }
            }
            
            return rewards[0]; // 默认返回第一个
        }
        
        /// <summary>
        /// 发放奖励
        /// </summary>
        private void GiveReward(GiftReward reward)
        {
            switch (reward.rewardType)
            {
                case RewardType.Currency:
                    PlayerDataManager.AddCurrency(reward.currencyType, reward.amount);
                    break;
                case RewardType.CharacterShard:
                    // 添加角色碎片逻辑
                    break;
                case RewardType.WeaponMaterial:
                    // 添加武器材料逻辑
                    break;
                case RewardType.Experience:
                    // 添加经验逻辑
                    break;
            }
        }
        
        /// <summary>
        /// 获取商店物品
        /// </summary>
        public ShopItem[] GetShopItems()
        {
            return shopItems;
        }
        
        /// <summary>
        /// 购买商店物品
        /// </summary>
        public bool PurchaseShopItem(int itemId)
        {
            var item = System.Array.Find(shopItems, x => x.itemId == itemId);
            if (item == null)
            {
                Debug.LogError($"商店物品不存在: {itemId}");
                return false;
            }
            
            // 检查货币是否足够
            if (!PlayerDataManager.SpendCurrency(item.currencyType, item.price))
            {
                Debug.Log("货币不足，无法购买物品");
                return false;
            }
            
            // 发放物品
            GiveReward(item.reward);
            
            Debug.Log($"成功购买物品: {item.itemName}");
            return true;
        }
        
        // 事件
        public System.Action<RechargePackage> OnRechargeSuccess;
        public System.Action<string> OnRechargeFailed;
    }
}