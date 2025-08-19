using UnityEngine;
using GunFireHeroes.UI;

using System.Collections.Generic;
using GunFireHeroes.Core;

namespace GunFireHeroes.Gameplay
{
    /// <summary>
    /// 游戏玩法管理器，负责关卡、副本、PVP等玩法
    /// </summary>
    public class GameplayManager : MonoBehaviour
    {
        [Header("关卡配置")]
        public StageConfig[] allStages;
        
        [Header("副本配置")]
        public DungeonConfig[] allDungeons;
        
        [Header("PVP配置")]
        public PVPConfig pvpConfig;
        
        [Header("双人匹配配置")]
        public MatchConfig matchConfig;
        
        private Dictionary<int, StageConfig> stageDict;
        private Dictionary<int, DungeonConfig> dungeonDict;
        
        private void Awake()
        {
            InitializeDictionaries();
        }
        
        private void InitializeDictionaries()
        {
            // 初始化关卡字典
            stageDict = new Dictionary<int, StageConfig>();
            foreach (var stage in allStages)
            {
                stageDict[stage.stageId] = stage;
            }
            
            // 初始化副本字典
            dungeonDict = new Dictionary<int, DungeonConfig>();
            foreach (var dungeon in allDungeons)
            {
                dungeonDict[dungeon.dungeonId] = dungeon;
            }
        }
        
        /// <summary>
        /// 开始关卡
        /// </summary>
        public bool StartStage(int stageId)
        {
            var ui = Object.FindObjectOfType<UIManager>();
            ui?.ShowLoading("正在加载关卡...");

            if (!stageDict.ContainsKey(stageId))
            {
                Debug.LogError($"关卡不存在: {stageId}");
                return false;
            }

            var stage = stageDict[stageId];
            var playerData = PlayerDataManager.PlayerData;

            // 检查关卡是否解锁
            if (stageId > playerData.maxUnlockedStage)
            {
                Debug.Log("关卡未解锁");
                return false;
            }

            // 加载并进入关卡
            GameManager.Instance.ChangeGameState(GameState.Loading);
            ui?.SetLoadingProgress(0.3f);
            LoadStage(stage);
            ui?.SetLoadingProgress(1f);

            GameManager.Instance.ChangeGameState(GameState.InGame);
            Debug.Log($"开始关卡: {stage.stageName}");
            return true;
        }
        
        /// <summary>
        /// 加载关卡
        /// </summary>
        private void LoadStage(StageConfig stage)
        {
            // 这里实现关卡加载逻辑
            // 生成敌人、设置背景等
        }
        
        /// <summary>
        /// 完成关卡
        /// </summary>
        public void CompleteStage(int stageId, bool isVictory, int score = 0)
        {
            if (!stageDict.ContainsKey(stageId))
                return;
                
            var stage = stageDict[stageId];
            var playerData = PlayerDataManager.PlayerData;
            
            if (isVictory)
            {
                // 解锁下一关
                if (stageId == playerData.maxUnlockedStage && stageId < allStages.Length)
                {
                    playerData.maxUnlockedStage = stageId + 1;
                }
                
                // 发放奖励
                GiveStageRewards(stage);
                
                Debug.Log($"关卡 {stage.stageName} 完成！");
            }
            else
            {
                Debug.Log($"关卡 {stage.stageName} 失败！");
            }
            
            PlayerDataManager.SavePlayerData();
            GameManager.Instance.ChangeGameState(GameState.MainMenu);
        }
        
        /// <summary>
        /// 发放关卡奖励
        /// </summary>
        private void GiveStageRewards(StageConfig stage)
        {
            // 发放金币
            PlayerDataManager.AddCurrency(CurrencyType.Gold, stage.goldReward);
            
            // 发放经验
            // 这里可以添加经验奖励逻辑
            
            // 发放其他奖励
            foreach (var reward in stage.rewards)
            {
                // 处理奖励
            }
        }
        
        /// <summary>
        /// 开始副本
        /// </summary>
        public bool StartDungeon(int dungeonId)
        {
            if (!dungeonDict.ContainsKey(dungeonId))
            {
                Debug.LogError($"副本不存在: {dungeonId}");
                return false;
            }
            
            var dungeon = dungeonDict[dungeonId];
            
            // 检查挑战次数
            if (!CanChallengeDungeon(dungeonId))
            {
                Debug.Log("副本挑战次数不足");
                return false;
            }
            
            // 开始副本
            GameManager.Instance.ChangeGameState(GameState.InGame);
            LoadDungeon(dungeon);
            
            Debug.Log($"开始副本: {dungeon.dungeonName}");
            return true;
        }
        
        /// <summary>
        /// 检查是否可以挑战副本
        /// </summary>
        private bool CanChallengeDungeon(int dungeonId)
        {
            // 这里可以添加挑战次数检查逻辑
            // 暂时返回true
            return true;
        }
        
        /// <summary>
        /// 加载副本
        /// </summary>
        private void LoadDungeon(DungeonConfig dungeon)
        {
            // 这里实现副本加载逻辑
        }
        
        /// <summary>
        /// 完成副本
        /// </summary>
        public void CompleteDungeon(int dungeonId, bool isVictory)
        {
            if (!dungeonDict.ContainsKey(dungeonId))
                return;
                
            var dungeon = dungeonDict[dungeonId];
            
            if (isVictory)
            {
                // 计算掉落
                CalculateDungeonDrops(dungeon);
                Debug.Log($"副本 {dungeon.dungeonName} 完成！");
            }
            else
            {
                Debug.Log($"副本 {dungeon.dungeonName} 失败！");
            }
            
            GameManager.Instance.ChangeGameState(GameState.MainMenu);
        }
        
        /// <summary>
        /// 计算副本掉落
        /// </summary>
        private void CalculateDungeonDrops(DungeonConfig dungeon)
        {
            foreach (var drop in dungeon.dropItems)
            {
                if (Random.value < drop.dropRate)
                {
                    // 发放掉落物品
                    switch (drop.itemType)
                    {
                        case DropItemType.CharacterShard:
                            // 添加角色碎片
                            var characterManager = FindObjectOfType<Character.CharacterManager>();
                            if (characterManager != null)
                            {
                                characterManager.AddCharacterShard(drop.itemId, drop.amount);
                            }
                            break;
                        case DropItemType.WeaponMaterial:
                            // 添加武器材料
                            var weaponManager = FindObjectOfType<Weapon.WeaponManager>();
                            if (weaponManager != null)
                            {
                                weaponManager.AddWeaponMaterial(drop.itemId, drop.amount);
                            }
                            break;
                        case DropItemType.Currency:
                            // 添加货币
                            PlayerDataManager.AddCurrency((CurrencyType)drop.itemId, drop.amount);
                            break;
                    }
                    
                    Debug.Log($"获得掉落: {drop.itemName} x{drop.amount}");
                }
            }
        }
        
        /// <summary>
        /// 开始PVP匹配
        /// </summary>
        public void StartPVPMatch()
        {
            // 检查时间限制
            if (!IsInPVPTime())
            {
                Debug.Log("不在PVP时间内");
                return;
            }
            
            Debug.Log("开始PVP匹配...");
            // 这里可以集成Photon等网络框架
        }
        
        /// <summary>
        /// 检查是否在PVP时间内
        /// </summary>
        private bool IsInPVPTime()
        {
            // 这里可以添加时间检查逻辑
            return true;
        }
        
        /// <summary>
        /// 开始双人匹配
        /// </summary>
        public void StartCoopMatch()
        {
            // 检查时间限制 (8:00-23:00)
            var currentTime = System.DateTime.Now;
            if (currentTime.Hour < 8 || currentTime.Hour >= 23)
            {
                Debug.Log("双人匹配时间：8:00-23:00");
                return;
            }
            
            // 随机选择关卡
            int randomStageId = Random.Range(1, 521); // 从520关卡中随机选取
            
            Debug.Log($"开始双人匹配，关卡: {randomStageId}");
            // 这里可以实现双人匹配逻辑
        }
        
        /// <summary>
        /// 获取关卡配置
        /// </summary>
        public StageConfig GetStageConfig(int stageId)
        {
            stageDict.TryGetValue(stageId, out StageConfig config);
            return config;
        }
        
        /// <summary>
        /// 获取副本配置
        /// </summary>
        public DungeonConfig GetDungeonConfig(int dungeonId)
        {
            dungeonDict.TryGetValue(dungeonId, out DungeonConfig config);
            return config;
        }
    }
}