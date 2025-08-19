using UnityEngine;
using System.Collections;
using GunFireHeroes.UI;
using GunFireHeroes.Character;
using GunFireHeroes.Economy;


namespace GunFireHeroes.Core
{
    /// <summary>
    /// 游戏主管理器，负责游戏整体流程控制
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [Header("游戏状态")]
        public GameState currentState = GameState.Loading;
        
        [Header("系统管理器")]
        public CharacterManager characterManager;
        public EconomyManager economyManager;
        public UIManager uiManager;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeGame();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeGame()
        {
            // 初始化微信小游戏环境
            InitializeWeChatMiniGame();
            
            // 初始化各个系统
            InitializeSystems();
            
            // 加载玩家数据后，显示启动加载界面 -> 然后进入登录/主菜单
            LoadPlayerData();
            StartCoroutine(BootstrapFlow());
        }
        
        private void InitializeWeChatMiniGame()
        {
            // 微信小游戏初始化
            #if UNITY_WEBGL && !UNITY_EDITOR
            // WX.InitSDK(() => {
            //     Debug.Log("微信SDK初始化成功");
            //     LoginToWeChat();
            // });
            #endif
        }
        
        private void InitializeSystems()
        {
            // 初始化角色系统
            if (characterManager == null)
                characterManager = FindObjectOfType<CharacterManager>();
            
            // 初始化经济系统
            if (economyManager == null)
                economyManager = FindObjectOfType<EconomyManager>();
                
            // 初始化UI系统
            if (uiManager == null)
                uiManager = FindObjectOfType<UIManager>();
        }
        
        private void LoadPlayerData()
        {
            // 从PlayerPrefs加载玩家数据
            PlayerDataManager.LoadPlayerData();
        }
        
        public void ChangeGameState(GameState newState)
        {
            currentState = newState;
            OnGameStateChanged(newState);
        }
        
        private void OnGameStateChanged(GameState newState)
        {
            switch (newState)
            {
                case GameState.MainMenu:
                    // 显示主菜单UI
                    if (uiManager != null) uiManager.ShowPanel(GunFireHeroes.UI.UIPanel.MainMenu);
                    Time.timeScale = 1f;
                    break;
                case GameState.InGame:
                    // 进入游戏状态
                    if (uiManager != null) uiManager.ShowPanel(GunFireHeroes.UI.UIPanel.Gameplay);
                    Time.timeScale = 1f;
                    break;
                case GameState.Paused:
                    // 暂停游戏
                    Time.timeScale = 0f;
                    break;
                case GameState.Loading:
                    if (uiManager != null) uiManager.ShowPanel(GunFireHeroes.UI.UIPanel.Loading);
                    Time.timeScale = 1f;
                    break;
                case GameState.GameOver:
                    // 游戏结束
                    Time.timeScale = 1f;
                    break;
            }
        }
        
        private void LoginToWeChat()
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
            // WX.Login(res => {
            //     if(res.code != null) 
            //     {
            //         Debug.Log("微信登录成功，code: " + res.code);
            //         // 向服务器验证
            //         // Server.VerifyCode(res.code);
            //     }
            // });
            #endif
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                // 游戏暂停时保存数据
                PlayerDataManager.SavePlayerData();
            }
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                // 失去焦点时保存数据
                PlayerDataManager.SavePlayerData();
            }

        }

        private IEnumerator BootstrapFlow()
        {
            // 进入加载界面
            ChangeGameState(GameState.Loading);
            var startTime = Time.time;

            // 模拟资源加载进度
            float progress = 0f;
            while (progress < 1f)
            {
                progress += Time.deltaTime * 0.5f; // 2秒完成
                var ui = uiManager;
                if (ui != null)
                {
                    var lp = ui.loadingPanel ? ui.loadingPanel.GetComponent<GunFireHeroes.UI.LoadingPanel>() : null;
                    if (lp != null) lp.SetProgress(progress);
                }
                yield return null;
            }

            // 判断是否有账号，未登录则进入登录界面，否则主菜单
            if (!AuthManager.IsLoggedIn)
            {
                if (AuthManager.HasAnyAccount())
                {
                    uiManager?.ShowPanel(GunFireHeroes.UI.UIPanel.Login);
                }
                else
                {
                    uiManager?.ShowPanel(GunFireHeroes.UI.UIPanel.Register);
                }
            }
            else
            {
                ChangeGameState(GameState.MainMenu);
            }
        }
    }
    
    public enum GameState
    {
        MainMenu,
        InGame,
        Paused,
        GameOver,
        Loading
    }
}