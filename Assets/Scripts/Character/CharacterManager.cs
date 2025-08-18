using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using GunFireHeroes.Core;

namespace GunFireHeroes.Character
{
    /// <summary>
    /// 角色管理器，负责角色的获取、升星、突破等功能
    /// </summary>
    public class CharacterManager : MonoBehaviour
    {
        [Header("角色配置")]
        public CharacterConfig[] allCharacters;
        
        [Header("掉落配置")]
        public float baseSShardDropRate = 0.05f; // S角色碎片基础掉率5%
        
        private Dictionary<int, CharacterConfig> characterConfigDict;
        
        private void Awake()
        {
            InitializeCharacterDict();
        }
        
        private void InitializeCharacterDict()
        {
            characterConfigDict = new Dictionary<int, CharacterConfig>();
            foreach (var config in allCharacters)
            {
                characterConfigDict[config.characterId] = config;
            }
        }
        
        /// <summary>
        /// 获取角色碎片（炼狱副本掉落）
        /// </summary>
        public bool TryGetCharacterShard(int characterId, float luckBonus = 0f)
        {
            if (!characterConfigDict.ContainsKey(characterId))
                return false;
                
            var config = characterConfigDict[characterId];
            if (config.rarity != CharacterRarity.S)
                return false;
                
            float dropRate = baseSShardDropRate + luckBonus;
            if (Random.value < dropRate)
            {
                AddCharacterShard(characterId, 1);
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// 添加角色碎片
        /// </summary>
        public void AddCharacterShard(int characterId, int amount)
        {
            var playerData = PlayerDataManager.PlayerData;
            var existingShard = playerData.characterShards.FirstOrDefault(s => s.characterId == characterId);
            
            if (existingShard != null)
            {
                existingShard.shardCount += amount;
            }
            else
            {
                playerData.characterShards.Add(new CharacterShardData
                {
                    characterId = characterId,
                    shardCount = amount
                });
            }
            
            PlayerDataManager.SavePlayerData();
            
            // 检查是否可以合成角色
            CheckCharacterSynthesis(characterId);
        }
        
        /// <summary>
        /// 检查角色合成
        /// </summary>
        private void CheckCharacterSynthesis(int characterId)
        {
            if (!characterConfigDict.ContainsKey(characterId))
                return;
                
            var config = characterConfigDict[characterId];
            var playerData = PlayerDataManager.PlayerData;
            var shard = playerData.characterShards.FirstOrDefault(s => s.characterId == characterId);
            
            if (shard != null && shard.shardCount >= config.synthesisRequiredShards)
            {
                // 检查是否已拥有该角色
                var ownedCharacter = playerData.ownedCharacters.FirstOrDefault(c => c.characterId == characterId);
                if (ownedCharacter == null)
                {
                    // 合成新角色
                    SynthesizeCharacter(characterId);
                }
            }
        }
        
        /// <summary>
        /// 合成角色
        /// </summary>
        public bool SynthesizeCharacter(int characterId)
        {
            if (!characterConfigDict.ContainsKey(characterId))
                return false;
                
            var config = characterConfigDict[characterId];
            var playerData = PlayerDataManager.PlayerData;
            var shard = playerData.characterShards.FirstOrDefault(s => s.characterId == characterId);
            
            if (shard == null || shard.shardCount < config.synthesisRequiredShards)
                return false;
                
            // 消耗碎片
            shard.shardCount -= config.synthesisRequiredShards;
            
            // 添加角色
            playerData.ownedCharacters.Add(new CharacterData
            {
                characterId = characterId,
                level = 1,
                starLevel = 1,
                experience = 0
            });
            
            PlayerDataManager.SavePlayerData();
            
            Debug.Log($"成功合成角色: {config.characterName}");
            return true;
        }
        
        /// <summary>
        /// 角色升星
        /// </summary>
        public bool UpgradeCharacterStar(int characterId)
        {
            if (!characterConfigDict.ContainsKey(characterId))
                return false;
                
            var config = characterConfigDict[characterId];
            var playerData = PlayerDataManager.PlayerData;
            var character = playerData.ownedCharacters.FirstOrDefault(c => c.characterId == characterId);
            var shard = playerData.characterShards.FirstOrDefault(s => s.characterId == characterId);
            
            if (character == null || shard == null)
                return false;
                
            // 计算升星所需碎片数量
            int requiredShards = GetStarUpgradeRequiredShards(character.starLevel, config.rarity);
            
            if (shard.shardCount < requiredShards)
                return false;
                
            // 消耗碎片
            shard.shardCount -= requiredShards;
            character.starLevel++;
            
            // 检查品质突破
            if (character.starLevel == 3)
            {
                CheckQualityBreakthrough(character, config);
            }
            
            PlayerDataManager.SavePlayerData();
            
            Debug.Log($"角色 {config.characterName} 升星至 {character.starLevel} 星");
            return true;
        }
        
        /// <summary>
        /// 获取升星所需碎片数量
        /// </summary>
        private int GetStarUpgradeRequiredShards(int currentStar, CharacterRarity rarity)
        {
            // S→SS需60碎片，等差递增
            int baseShards = 60;
            return baseShards + (currentStar - 1) * 20;
        }
        
        /// <summary>
        /// 检查品质突破
        /// </summary>
        private void CheckQualityBreakthrough(CharacterData character, CharacterConfig config)
        {
            if (character.starLevel >= 3 && !character.hasQualityBreakthrough)
            {
                // 这里可以添加突破材料检查逻辑
                // 暂时自动突破
                character.hasQualityBreakthrough = true;
                Debug.Log($"角色 {config.characterName} 品质突破成功！");
            }
        }
        
        /// <summary>
        /// 计算角色属性
        /// </summary>
        public CharacterStats CalculateCharacterStats(CharacterData character)
        {
            if (!characterConfigDict.ContainsKey(character.characterId))
                return new CharacterStats();
                
            var config = characterConfigDict[character.characterId];
            var stats = new CharacterStats();
            
            // 基础属性
            float baseAttack = config.baseAttack;
            float baseHP = config.baseHP;
            float baseDefense = config.baseDefense;
            
            // 等级加成
            float levelMultiplier = 1f + (character.level - 1) * 0.1f;
            
            // 星级加成
            float starMultiplier = 1f + (character.starLevel - 1) * 0.2f;
            
            // 品质突破加成
            float breakthroughBonus = 0f;
            if (character.hasQualityBreakthrough)
            {
                // 原生SR比突破角色高5%属性
                bool isNaturalSR = config.rarity >= CharacterRarity.SR;
                breakthroughBonus = isNaturalSR ? 0.2f : 0.15f;
            }
            
            stats.attack = Mathf.RoundToInt(baseAttack * levelMultiplier * starMultiplier * (1f + breakthroughBonus));
            stats.hp = Mathf.RoundToInt(baseHP * levelMultiplier * starMultiplier * (1f + breakthroughBonus));
            stats.defense = Mathf.RoundToInt(baseDefense * levelMultiplier * starMultiplier * (1f + breakthroughBonus));
            
            return stats;
        }
        
        /// <summary>
        /// 获取玩家拥有的角色列表
        /// </summary>
        public List<CharacterData> GetOwnedCharacters()
        {
            return PlayerDataManager.PlayerData.ownedCharacters;
        }
        
        /// <summary>
        /// 获取角色配置
        /// </summary>
        public CharacterConfig GetCharacterConfig(int characterId)
        {
            characterConfigDict.TryGetValue(characterId, out CharacterConfig config);
            return config;
        }
    }
}