using UnityEngine;
using GunFireHeroes.Core;
using GunFireHeroes.Character;
using GunFireHeroes.Weapon;
using GunFireHeroes.Economy;

namespace GunFireHeroes.Core
{
    /// <summary>
    /// 测试场景管理器，用于演示和测试游戏功能
    /// </summary>
    public class TestSceneManager : MonoBehaviour
    {
        [Header("测试设置")]
        public bool enableDebugUI = true;
        public KeyCode testKey1 = KeyCode.F1;
        public KeyCode testKey2 = KeyCode.F2;
        public KeyCode testKey3 = KeyCode.F3;
        public KeyCode testKey4 = KeyCode.F4;
        
        private CharacterManager characterManager;
        private WeaponManager weaponManager;
        private EconomyManager economyManager;
        
        private void Start()
        {
            InitializeTestScene();
        }
        
        private void Update()
        {
            HandleTestInput();
        }
        
        private void InitializeTestScene()
        {
            // 获取管理器引用
            characterManager = FindObjectOfType<CharacterManager>();
            weaponManager = FindObjectOfType<WeaponManager>();
            economyManager = FindObjectOfType<EconomyManager>();
            
            // 初始化测试数据
            InitializeTestData();
            
            Debug.Log("=== GunFire Heroes 测试场景已启动 ===");
            Debug.Log("按键说明：");
            Debug.Log("F1 - 测试角色系统");
            Debug.Log("F2 - 测试武器系统");
            Debug.Log("F3 - 测试经济系统");
            Debug.Log("F4 - 显示玩家数据");
        }
        
        private void InitializeTestData()
        {
            var playerData = PlayerDataManager.PlayerData;
            
            // 如果是新玩家，给予初始资源
            if (playerData.ownedCharacters.Count == 0)
            {
                Debug.Log("初始化新玩家数据...");
                
                // 添加初始货币
                PlayerDataManager.AddCurrency(CurrencyType.Gold, 5000);
                PlayerDataManager.AddCurrency(CurrencyType.Diamond, 500);
                
                // 添加初始角色（如果有角色管理器）
                if (characterManager != null)
                {
                    // 这里可以添加初始角色
                    Debug.Log("添加初始角色...");
                }
                
                // 添加初始武器（如果有武器管理器）
                if (weaponManager != null)
                {
                    // 这里可以添加初始武器
                    Debug.Log("添加初始武器...");
                }
            }
        }
        
        private void HandleTestInput()
        {
            if (Input.GetKeyDown(testKey1))
            {
                TestCharacterSystem();
            }
            else if (Input.GetKeyDown(testKey2))
            {
                TestWeaponSystem();
            }
            else if (Input.GetKeyDown(testKey3))
            {
                TestEconomySystem();
            }
            else if (Input.GetKeyDown(testKey4))
            {
                ShowPlayerData();
            }
        }
        
        private void TestCharacterSystem()
        {
            Debug.Log("=== 测试角色系统 ===");
            
            if (characterManager == null)
            {
                Debug.LogWarning("角色管理器未找到！");
                return;
            }
            
            // 测试添加角色碎片
            Debug.Log("添加角色碎片...");
            characterManager.AddCharacterShard(1001, 10); // 假设1001是某个角色ID
            
            // 测试角色升星
            var ownedCharacters = characterManager.GetOwnedCharacters();
            if (ownedCharacters.Count > 0)
            {
                var character = ownedCharacters[0];
                Debug.Log($"尝试升星角色 {character.characterId}...");
                bool success = characterManager.UpgradeCharacterStar(character.characterId);
                Debug.Log($"升星结果: {(success ? "成功" : "失败")}");
                
                // 显示角色属性
                var stats = characterManager.CalculateCharacterStats(character);
                Debug.Log($"角色属性 - 攻击力: {stats.attack}, 生命值: {stats.hp}, 防御力: {stats.defense}");
            }
        }
        
        private void TestWeaponSystem()
        {
            Debug.Log("=== 测试武器系统 ===");
            
            if (weaponManager == null)
            {
                Debug.LogWarning("武器管理器未找到！");
                return;
            }
            
            // 测试添加武器
            Debug.Log("添加测试武器...");
            weaponManager.AddWeapon(2001); // 假设2001是某个武器ID
            
            // 测试武器强化
            var ownedWeapons = weaponManager.GetOwnedWeapons();
            if (ownedWeapons.Count > 0)
            {
                var weapon = ownedWeapons[0];
                Debug.Log($"尝试强化武器 {weapon.weaponId}...");
                bool success = weaponManager.EnhanceWeapon(weapon.weaponId);
                Debug.Log($"强化结果: {(success ? "成功" : "失败")}");
                
                // 显示武器属性
                var stats = weaponManager.CalculateWeaponStats(weapon);
                Debug.Log($"武器属性 - 伤害: {stats.damage}, 攻击速度: {stats.attackSpeed}, 射程: {stats.range}");
            }
        }
        
        private void TestEconomySystem()
        {
            Debug.Log("=== 测试经济系统 ===");
            
            if (economyManager == null)
            {
                Debug.LogWarning("经济管理器未找到！");
                return;
            }
            
            // 测试模拟充值
            Debug.Log("模拟充值测试...");
            // economyManager.RequestPayment(1); // 假设1是某个充值包ID
            
            // 测试购买礼包
            Debug.Log("测试购买礼包...");
            bool success = economyManager.PurchaseGiftPackage(1001); // 假设1001是某个礼包ID
            Debug.Log($"购买结果: {(success ? "成功" : "失败")}");
            
            // 显示货币状态
            var playerData = PlayerDataManager.PlayerData;
            Debug.Log($"当前货币 - 金币: {playerData.gold}, 钻石: {playerData.diamond}");
        }
        
        private void ShowPlayerData()
        {
            Debug.Log("=== 玩家数据 ===");
            
            var playerData = PlayerDataManager.PlayerData;
            
            Debug.Log($"玩家名称: {playerData.playerName}");
            Debug.Log($"玩家等级: {playerData.playerLevel}");
            Debug.Log($"总登录天数: {playerData.totalLoginDays}");
            Debug.Log($"连续登录天数: {playerData.consecutiveLoginDays}");
            Debug.Log($"当前关卡: {playerData.currentStage}");
            Debug.Log($"最高解锁关卡: {playerData.maxUnlockedStage}");
            
            Debug.Log("=== 货币 ===");
            Debug.Log($"金币: {playerData.gold}");
            Debug.Log($"钻石: {playerData.diamond}");
            Debug.Log($"排位币: {playerData.rankCoin}");
            Debug.Log($"匹配代币: {playerData.matchToken}");
            
            Debug.Log("=== 角色 ===");
            Debug.Log($"拥有角色数量: {playerData.ownedCharacters.Count}");
            foreach (var character in playerData.ownedCharacters)
            {
                Debug.Log($"角色ID: {character.characterId}, 等级: {character.level}, 星级: {character.starLevel}");
            }
            
            Debug.Log("=== 角色碎片 ===");
            Debug.Log($"角色碎片种类: {playerData.characterShards.Count}");
            foreach (var shard in playerData.characterShards)
            {
                Debug.Log($"角色ID: {shard.characterId}, 碎片数量: {shard.shardCount}");
            }
            
            Debug.Log("=== 武器 ===");
            Debug.Log($"拥有武器数量: {playerData.ownedWeapons.Count}");
            foreach (var weapon in playerData.ownedWeapons)
            {
                Debug.Log($"武器ID: {weapon.weaponId}, 等级: {weapon.level}, 强化等级: {weapon.enhanceLevel}, 已装备: {weapon.isEquipped}");
            }
        }
        
        private void OnGUI()
        {
            if (!enableDebugUI)
                return;
                
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label("=== GunFire Heroes 调试面板 ===");
            
            if (GUILayout.Button("测试角色系统 (F1)"))
            {
                TestCharacterSystem();
            }
            
            if (GUILayout.Button("测试武器系统 (F2)"))
            {
                TestWeaponSystem();
            }
            
            if (GUILayout.Button("测试经济系统 (F3)"))
            {
                TestEconomySystem();
            }
            
            if (GUILayout.Button("显示玩家数据 (F4)"))
            {
                ShowPlayerData();
            }
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("添加1000金币"))
            {
                PlayerDataManager.AddCurrency(CurrencyType.Gold, 1000);
                Debug.Log("添加1000金币");
            }
            
            if (GUILayout.Button("添加100钻石"))
            {
                PlayerDataManager.AddCurrency(CurrencyType.Diamond, 100);
                Debug.Log("添加100钻石");
            }
            
            if (GUILayout.Button("保存数据"))
            {
                PlayerDataManager.SavePlayerData();
                Debug.Log("数据已保存");
            }
            
            GUILayout.EndArea();
        }
    }
}