using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GunFireHeroes.Core;

namespace GunFireHeroes.UI
{
    /// <summary>
    /// UI管理器，负责所有UI界面的管理
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("UI面板")]
        public GameObject mainMenuPanel;
        public GameObject gameplayPanel;
        public GameObject characterPanel;
        public GameObject weaponPanel;
        public GameObject shopPanel;
        public GameObject settingsPanel;
        
        [Header("HUD元素")]
        public Text goldText;
        public Text diamondText;
        public Text rankCoinText;
        public Text matchTokenText;
        public Slider healthBar;
        public Text levelText;
        
        [Header("弹窗")]
        public GameObject messagePopup;
        public Text messageText;
        public GameObject confirmPopup;
        public Text confirmText;
        
        private Dictionary<UIPanel, GameObject> panelDict;
        private UIPanel currentPanel = UIPanel.MainMenu;
        
        private void Awake()
        {
            InitializePanelDict();
            UpdateCurrencyDisplay();
        }
        
        private void Start()
        {
            ShowPanel(UIPanel.MainMenu);
        }
        
        private void InitializePanelDict()
        {
            panelDict = new Dictionary<UIPanel, GameObject>
            {
                { UIPanel.MainMenu, mainMenuPanel },
                { UIPanel.Gameplay, gameplayPanel },
                { UIPanel.Character, characterPanel },
                { UIPanel.Weapon, weaponPanel },
                { UIPanel.Shop, shopPanel },
                { UIPanel.Settings, settingsPanel }
            };
        }
        
        /// <summary>
        /// 显示指定面板
        /// </summary>
        public void ShowPanel(UIPanel panel)
        {
            // 隐藏所有面板
            foreach (var kvp in panelDict)
            {
                if (kvp.Value != null)
                    kvp.Value.SetActive(false);
            }
            
            // 显示目标面板
            if (panelDict.ContainsKey(panel) && panelDict[panel] != null)
            {
                panelDict[panel].SetActive(true);
                currentPanel = panel;
                OnPanelChanged(panel);
            }
        }
        
        /// <summary>
        /// 面板切换时的处理
        /// </summary>
        private void OnPanelChanged(UIPanel panel)
        {
            switch (panel)
            {
                case UIPanel.MainMenu:
                    UpdateCurrencyDisplay();
                    break;
                case UIPanel.Character:
                    RefreshCharacterPanel();
                    break;
                case UIPanel.Weapon:
                    RefreshWeaponPanel();
                    break;
                case UIPanel.Shop:
                    RefreshShopPanel();
                    break;
            }
        }
        
        /// <summary>
        /// 更新货币显示
        /// </summary>
        public void UpdateCurrencyDisplay()
        {
            var playerData = PlayerDataManager.PlayerData;
            
            if (goldText != null)
                goldText.text = playerData.gold.ToString();
            if (diamondText != null)
                diamondText.text = playerData.diamond.ToString();
            if (rankCoinText != null)
                rankCoinText.text = playerData.rankCoin.ToString();
            if (matchTokenText != null)
                matchTokenText.text = playerData.matchToken.ToString();
        }
        
        /// <summary>
        /// 更新血量条
        /// </summary>
        public void UpdateHealthBar(float currentHP, float maxHP)
        {
            if (healthBar != null)
            {
                healthBar.value = currentHP / maxHP;
            }
        }
        
        /// <summary>
        /// 更新等级显示
        /// </summary>
        public void UpdateLevelDisplay(int level)
        {
            if (levelText != null)
                levelText.text = $"Lv.{level}";
        }
        
        /// <summary>
        /// 刷新角色面板
        /// </summary>
        private void RefreshCharacterPanel()
        {
            // 这里可以添加角色面板刷新逻辑
            Debug.Log("刷新角色面板");
        }
        
        /// <summary>
        /// 刷新武器面板
        /// </summary>
        private void RefreshWeaponPanel()
        {
            // 这里可以添加武器面板刷新逻辑
            Debug.Log("刷新武器面板");
        }
        
        /// <summary>
        /// 刷新商店面板
        /// </summary>
        private void RefreshShopPanel()
        {
            // 这里可以添加商店面板刷新逻辑
            Debug.Log("刷新商店面板");
        }
        
        /// <summary>
        /// 显示消息弹窗
        /// </summary>
        public void ShowMessage(string message, float duration = 3f)
        {
            if (messagePopup != null && messageText != null)
            {
                messageText.text = message;
                messagePopup.SetActive(true);
                
                // 自动隐藏
                Invoke(nameof(HideMessage), duration);
            }
        }
        
        /// <summary>
        /// 隐藏消息弹窗
        /// </summary>
        public void HideMessage()
        {
            if (messagePopup != null)
                messagePopup.SetActive(false);
        }
        
        /// <summary>
        /// 显示确认弹窗
        /// </summary>
        public void ShowConfirm(string message, System.Action onConfirm, System.Action onCancel = null)
        {
            if (confirmPopup != null && confirmText != null)
            {
                confirmText.text = message;
                confirmPopup.SetActive(true);
                
                // 设置按钮回调
                var confirmButton = confirmPopup.transform.Find("ConfirmButton")?.GetComponent<Button>();
                var cancelButton = confirmPopup.transform.Find("CancelButton")?.GetComponent<Button>();
                
                if (confirmButton != null)
                {
                    confirmButton.onClick.RemoveAllListeners();
                    confirmButton.onClick.AddListener(() => {
                        onConfirm?.Invoke();
                        HideConfirm();
                    });
                }
                
                if (cancelButton != null)
                {
                    cancelButton.onClick.RemoveAllListeners();
                    cancelButton.onClick.AddListener(() => {
                        onCancel?.Invoke();
                        HideConfirm();
                    });
                }
            }
        }
        
        /// <summary>
        /// 隐藏确认弹窗
        /// </summary>
        public void HideConfirm()
        {
            if (confirmPopup != null)
                confirmPopup.SetActive(false);
        }
        
        // UI按钮事件处理
        public void OnMainMenuButtonClicked()
        {
            ShowPanel(UIPanel.MainMenu);
        }
        
        public void OnCharacterButtonClicked()
        {
            ShowPanel(UIPanel.Character);
        }
        
        public void OnWeaponButtonClicked()
        {
            ShowPanel(UIPanel.Weapon);
        }
        
        public void OnShopButtonClicked()
        {
            ShowPanel(UIPanel.Shop);
        }
        
        public void OnSettingsButtonClicked()
        {
            ShowPanel(UIPanel.Settings);
        }
        
        public void OnStartGameButtonClicked()
        {
            var gameplayManager = FindObjectOfType<Gameplay.GameplayManager>();
            if (gameplayManager != null)
            {
                var playerData = PlayerDataManager.PlayerData;
                gameplayManager.StartStage(playerData.currentStage);
            }
        }
        
        public void OnPVPButtonClicked()
        {
            var gameplayManager = FindObjectOfType<Gameplay.GameplayManager>();
            if (gameplayManager != null)
            {
                gameplayManager.StartPVPMatch();
            }
        }
        
        public void OnCoopButtonClicked()
        {
            var gameplayManager = FindObjectOfType<Gameplay.GameplayManager>();
            if (gameplayManager != null)
            {
                gameplayManager.StartCoopMatch();
            }
        }
        
        public void OnExitButtonClicked()
        {
            ShowConfirm("确定要退出游戏吗？", () => {
                Application.Quit();
            });
        }
    }
    
    /// <summary>
    /// UI面板枚举
    /// </summary>
    public enum UIPanel
    {
        MainMenu,   // 主菜单
        Gameplay,   // 游戏界面
        Character,  // 角色界面
        Weapon,     // 武器界面
        Shop,       // 商店界面
        Settings    // 设置界面
    }
}