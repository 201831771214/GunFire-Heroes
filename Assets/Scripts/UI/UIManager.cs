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
        public GameObject loginPanel;
        public GameObject registerPanel;
        public GameObject loadingPanel;
        public GameObject stageSelectPanel;
        
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
        private LoadingPanel loadingPanelComp;
        
        private void Awake()
        {
            InitializePanelDict();
            loadingPanelComp = loadingPanel ? loadingPanel.GetComponent<LoadingPanel>() : null;
            UpdateCurrencyDisplay();
        }
        
        private void Start()
        {
            // 初始界面由GameManager控制
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
                { UIPanel.Settings, settingsPanel },
                { UIPanel.Login, loginPanel },
                { UIPanel.Register, registerPanel },
                { UIPanel.Loading, loadingPanel },
                { UIPanel.StageSelect, stageSelectPanel }
            };
        }
        
        /// <summary>
        /// 显示指定面板
        /// </summary>
        public void ShowPanel(UIPanel panel)
        {
            foreach (var kvp in panelDict)
            {
                if (kvp.Value != null) kvp.Value.SetActive(false);
            }
            if (panelDict.ContainsKey(panel) && panelDict[panel] != null)
            {
                panelDict[panel].SetActive(true);
                currentPanel = panel;
                OnPanelChanged(panel);
            }
        }

        /// <summary>
        /// 显示加载界面（可选提示）
        /// </summary>
        public void ShowLoading(string tip = null)
        {
            ShowPanel(UIPanel.Loading);
            if (loadingPanelComp != null && !string.IsNullOrEmpty(tip))
                loadingPanelComp.SetTip(tip);
        }

        /// <summary>
        /// 更新加载进度
        /// </summary>
        public void SetLoadingProgress(float value)
        {
            if (loadingPanelComp != null) loadingPanelComp.SetProgress(value);
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
                case UIPanel.Loading:
                    if (loadingPanelComp != null) loadingPanelComp.SetTip("正在载入，请稍候...");
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
        Settings,   // 设置界面
        Login,      // 登录界面
        Register,   // 注册界面
        Loading,    // 加载界面
        StageSelect // 关卡选择
    }
}