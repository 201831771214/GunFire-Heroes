using UnityEngine;
using System;
using System.Collections.Generic;

namespace GunFireHeroes.Core
{
    /// <summary>
    /// 玩家数据管理器，负责数据的保存和加载
    /// </summary>
    public static class PlayerDataManager
    {
        private static PlayerData playerData;
        
        public static PlayerData PlayerData 
        { 
            get 
            { 
                if (playerData == null)
                    LoadPlayerData();
                return playerData; 
            } 
        }
        
        public static void LoadPlayerData()
        {
            string jsonData = PlayerPrefs.GetString("PlayerData", "");
            
            if (string.IsNullOrEmpty(jsonData))
            {
                // 创建新的玩家数据
                playerData = new PlayerData();
                SavePlayerData();
            }
            else
            {
                try
                {
                    playerData = JsonUtility.FromJson<PlayerData>(jsonData);
                }
                catch (Exception e)
                {
                    Debug.LogError("加载玩家数据失败: " + e.Message);
                    playerData = new PlayerData();
                }
            }
            
            // 检查登录天数
            CheckDailyLogin();
        }
        
        public static void SavePlayerData()
        {
            if (playerData != null)
            {
                string jsonData = JsonUtility.ToJson(playerData);
                PlayerPrefs.SetString("PlayerData", jsonData);
                PlayerPrefs.Save();
            }
        }
        
        private static void CheckDailyLogin()
        {
            DateTime today = DateTime.Today;
            DateTime lastLogin = DateTime.FromBinary(playerData.lastLoginDate);
            
            if (lastLogin.Date != today)
            {
                // 新的一天登录
                if (lastLogin.Date == today.AddDays(-1))
                {
                    // 连续登录
                    playerData.consecutiveLoginDays++;
                }
                else
                {
                    // 非连续登录，重置为1
                    playerData.consecutiveLoginDays = 1;
                }
                
                playerData.totalLoginDays++;
                playerData.lastLoginDate = today.ToBinary();
                
                // 检查7日登录奖励
                CheckSevenDayReward();
                
                SavePlayerData();
            }
        }
        
        private static void CheckSevenDayReward()
        {
            if (playerData.totalLoginDays >= 7 && !playerData.hasReceivedSevenDayReward)
            {
                // 发放免费S角色
                playerData.hasReceivedSevenDayReward = true;
                // 这里可以添加具体的奖励逻辑
                Debug.Log("获得7日登录奖励：免费S角色！");
            }
        }
        
        // 货币相关
        public static void AddCurrency(CurrencyType type, int amount)
        {
            switch (type)
            {
                case CurrencyType.Gold:
                    playerData.gold += amount;
                    break;
                case CurrencyType.Diamond:
                    playerData.diamond += amount;
                    break;
                case CurrencyType.RankCoin:
                    playerData.rankCoin += amount;
                    break;
                case CurrencyType.MatchToken:
                    playerData.matchToken += amount;
                    break;
            }
            SavePlayerData();
        }
        
        public static bool SpendCurrency(CurrencyType type, int amount)
        {
            int currentAmount = GetCurrency(type);
            if (currentAmount >= amount)
            {
                AddCurrency(type, -amount);
                return true;
            }
            return false;
        }
        
        public static int GetCurrency(CurrencyType type)
        {
            switch (type)
            {
                case CurrencyType.Gold:
                    return playerData.gold;
                case CurrencyType.Diamond:
                    return playerData.diamond;
                case CurrencyType.RankCoin:
                    return playerData.rankCoin;
                case CurrencyType.MatchToken:
                    return playerData.matchToken;
                default:
                    return 0;
            }
        }
    }
    
    [System.Serializable]
    public class PlayerData
    {
        [Header("基础信息")]
        public string playerName = "新手玩家";
        public int playerLevel = 1;
        public int experience = 0;
        
        [Header("登录信息")]
        public int totalLoginDays = 1;
        public int consecutiveLoginDays = 1;
        public long lastLoginDate;
        public bool hasReceivedSevenDayReward = false;
        
        [Header("货币")]
        public int gold = 1000;
        public int diamond = 100;
        public int rankCoin = 0;
        public int matchToken = 0;
        
        [Header("角色")]
        public List<CharacterData> ownedCharacters = new List<CharacterData>();
        public List<CharacterShardData> characterShards = new List<CharacterShardData>();
        
        [Header("武器")]
        public List<WeaponData> ownedWeapons = new List<WeaponData>();
        
        [Header("关卡进度")]
        public int currentStage = 1;
        public int maxUnlockedStage = 1;
        
        [Header("充值信息")]
        public bool hasFirstRecharge = false;
        public int totalRechargeAmount = 0;
        
        public PlayerData()
        {
            lastLoginDate = DateTime.Today.ToBinary();
        }
    }
    
    public enum CurrencyType
    {
        Gold,       // 金币
        Diamond,    // 钻石/点券
        RankCoin,   // 排位币
        MatchToken  // 匹配代币
    }
}