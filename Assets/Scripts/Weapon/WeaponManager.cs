using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using GunFireHeroes.Core;

namespace GunFireHeroes.Weapon
{
    /// <summary>
    /// 武器管理器，负责武器的获取、强化、装备等功能
    /// </summary>
    public class WeaponManager : MonoBehaviour
    {
        [Header("武器配置")]
        public WeaponConfig[] allWeapons;
        
        [Header("武器材料配置")]
        public WeaponMaterial[] allMaterials;
        
        private Dictionary<int, WeaponConfig> weaponConfigDict;
        private Dictionary<int, WeaponMaterial> materialDict;
        
        private void Awake()
        {
            InitializeDictionaries();
        }
        
        private void InitializeDictionaries()
        {
            // 初始化武器配置字典
            weaponConfigDict = new Dictionary<int, WeaponConfig>();
            foreach (var config in allWeapons)
            {
                weaponConfigDict[config.weaponId] = config;
            }
            
            // 初始化材料字典
            materialDict = new Dictionary<int, WeaponMaterial>();
            foreach (var material in allMaterials)
            {
                materialDict[material.materialId] = material;
            }
        }
        
        /// <summary>
        /// 获取武器
        /// </summary>
        public void AddWeapon(int weaponId)
        {
            if (!weaponConfigDict.ContainsKey(weaponId))
            {
                Debug.LogError($"武器配置不存在: {weaponId}");
                return;
            }
            
            var playerData = PlayerDataManager.PlayerData;
            var existingWeapon = playerData.ownedWeapons.FirstOrDefault(w => w.weaponId == weaponId);
            
            if (existingWeapon == null)
            {
                // 添加新武器
                playerData.ownedWeapons.Add(new WeaponData(weaponId));
                PlayerDataManager.SavePlayerData();
                
                var config = weaponConfigDict[weaponId];
                Debug.Log($"获得新武器: {config.weaponName}");
            }
            else
            {
                Debug.Log("已拥有该武器");
            }
        }
        
        /// <summary>
        /// 强化武器
        /// </summary>
        public bool EnhanceWeapon(int weaponId)
        {
            if (!weaponConfigDict.ContainsKey(weaponId))
            {
                Debug.LogError($"武器配置不存在: {weaponId}");
                return false;
            }
            
            var config = weaponConfigDict[weaponId];
            var playerData = PlayerDataManager.PlayerData;
            var weapon = playerData.ownedWeapons.FirstOrDefault(w => w.weaponId == weaponId);
            
            if (weapon == null)
            {
                Debug.LogError("未拥有该武器");
                return false;
            }
            
            if (weapon.enhanceLevel >= config.maxEnhanceLevel)
            {
                Debug.Log("武器已达到最大强化等级");
                return false;
            }
            
            // 获取强化配置
            var enhanceConfig = config.enhanceConfigs.FirstOrDefault(e => e.enhanceLevel == weapon.enhanceLevel + 1);
            if (enhanceConfig == null)
            {
                Debug.LogError($"强化配置不存在: 等级 {weapon.enhanceLevel + 1}");
                return false;
            }
            
            // 检查金币是否足够
            if (!PlayerDataManager.SpendCurrency(CurrencyType.Gold, enhanceConfig.goldCost))
            {
                Debug.Log("金币不足，无法强化武器");
                return false;
            }
            
            // 检查材料是否足够
            foreach (var requirement in enhanceConfig.materialRequirements)
            {
                // 这里需要实现材料检查和消耗逻辑
                // 暂时跳过材料检查
            }
            
            // 强化成功
            weapon.enhanceLevel++;
            PlayerDataManager.SavePlayerData();
            
            Debug.Log($"武器 {config.weaponName} 强化至 +{weapon.enhanceLevel}");
            return true;
        }
        
        /// <summary>
        /// 装备武器
        /// </summary>
        public bool EquipWeapon(int weaponId)
        {
            var playerData = PlayerDataManager.PlayerData;
            var weapon = playerData.ownedWeapons.FirstOrDefault(w => w.weaponId == weaponId);
            
            if (weapon == null)
            {
                Debug.LogError("未拥有该武器");
                return false;
            }
            
            // 卸下当前装备的武器
            foreach (var w in playerData.ownedWeapons)
            {
                w.isEquipped = false;
            }
            
            // 装备新武器
            weapon.isEquipped = true;
            PlayerDataManager.SavePlayerData();
            
            var config = weaponConfigDict[weaponId];
            Debug.Log($"装备武器: {config.weaponName}");
            return true;
        }
        
        /// <summary>
        /// 计算武器属性
        /// </summary>
        public WeaponStats CalculateWeaponStats(WeaponData weapon)
        {
            if (!weaponConfigDict.ContainsKey(weapon.weaponId))
                return new WeaponStats();
                
            var config = weaponConfigDict[weapon.weaponId];
            var stats = new WeaponStats();
            
            // 基础属性
            stats.damage = config.baseDamage;
            stats.attackSpeed = config.attackSpeed;
            stats.range = config.range;
            stats.accuracy = config.accuracy;
            stats.magazineSize = config.magazineSize;
            stats.reloadTime = config.reloadTime;
            
            // 强化加成
            if (weapon.enhanceLevel > 0)
            {
                var enhanceConfig = config.enhanceConfigs.FirstOrDefault(e => e.enhanceLevel == weapon.enhanceLevel);
                if (enhanceConfig != null)
                {
                    stats.damage += stats.damage * enhanceConfig.damageBonus;
                    stats.attackSpeed += stats.attackSpeed * enhanceConfig.attackSpeedBonus;
                    stats.range += stats.range * enhanceConfig.rangeBonus;
                    stats.accuracy += stats.accuracy * enhanceConfig.accuracyBonus;
                }
            }
            
            return stats;
        }
        
        /// <summary>
        /// 获取当前装备的武器
        /// </summary>
        public WeaponData GetEquippedWeapon()
        {
            var playerData = PlayerDataManager.PlayerData;
            return playerData.ownedWeapons.FirstOrDefault(w => w.isEquipped);
        }
        
        /// <summary>
        /// 获取玩家拥有的武器列表
        /// </summary>
        public List<WeaponData> GetOwnedWeapons()
        {
            return PlayerDataManager.PlayerData.ownedWeapons;
        }
        
        /// <summary>
        /// 获取武器配置
        /// </summary>
        public WeaponConfig GetWeaponConfig(int weaponId)
        {
            weaponConfigDict.TryGetValue(weaponId, out WeaponConfig config);
            return config;
        }
        
        /// <summary>
        /// 获取材料配置
        /// </summary>
        public WeaponMaterial GetMaterialConfig(int materialId)
        {
            materialDict.TryGetValue(materialId, out WeaponMaterial material);
            return material;
        }
        
        /// <summary>
        /// 添加武器材料
        /// </summary>
        public void AddWeaponMaterial(int materialId, int amount)
        {
            // 这里需要实现材料存储逻辑
            // 可以在PlayerData中添加材料字典
            Debug.Log($"获得武器材料: {materialId} x{amount}");
        }
    }
}