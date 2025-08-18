using UnityEngine;
using System.Collections.Generic;

namespace GunFireHeroes.Social
{
    /// <summary>
    /// 排行榜配置
    /// </summary>
    [CreateAssetMenu(fileName = "LeaderboardConfig", menuName = "GunFire Heroes/Leaderboard Config")]
    public class LeaderboardConfig : ScriptableObject
    {
        [Header("基础信息")]
        public LeaderboardType type;
        public string leaderboardName;
        public string description;
        
        [Header("显示设置")]
        public Sprite leaderboardIcon;
        public int maxDisplayEntries = 100;
        public bool showPlayerRank = true;
        
        [Header("更新设置")]
        public bool autoUpdate = true;
        public float updateInterval = 60f; // 秒
    }
    
    /// <summary>
    /// 排行榜类型
    /// </summary>
    public enum LeaderboardType
    {
        HighScore,      // 最高分
        StageProgress,  // 关卡进度
        PVPRank        // PVP排名
    }
    
    /// <summary>
    /// 排行榜数据
    /// </summary>
    [System.Serializable]
    public class LeaderboardData
    {
        public LeaderboardType type;
        public List<LeaderboardEntry> entries;
        public long lastUpdateTime;
        
        public LeaderboardData()
        {
            entries = new List<LeaderboardEntry>();
            lastUpdateTime = System.DateTimeOffset.Now.ToUnixTimeSeconds();
        }
    }
    
    /// <summary>
    /// 排行榜条目
    /// </summary>
    [System.Serializable]
    public class LeaderboardEntry
    {
        public string playerId;
        public string playerName;
        public int score;
        public int rank;
        public string avatarUrl;
        public long timestamp;
        
        public LeaderboardEntry()
        {
            timestamp = System.DateTimeOffset.Now.ToUnixTimeSeconds();
        }
    }
    
    /// <summary>
    /// 分享配置
    /// </summary>
    [CreateAssetMenu(fileName = "ShareConfig", menuName = "GunFire Heroes/Share Config")]
    public class ShareConfig : ScriptableObject
    {
        [Header("成就分享")]
        public string achievementTitle = "我在GunFire Heroes中获得了新成就！";
        public string achievementImage = "";
        
        [Header("高分分享")]
        public string highScoreTitle = "我在GunFire Heroes中创造了新纪录！";
        public string highScoreImage = "";
        
        [Header("邀请分享")]
        public string invitationTitle = "快来和我一起玩GunFire Heroes吧！";
        public string invitationImage = "";
        
        [Header("通用设置")]
        public string gameUrl = "";
        public string gameDescription = "最刺激的横版射击游戏";
    }
    
    /// <summary>
    /// 分享类型
    /// </summary>
    public enum ShareType
    {
        Achievement,    // 成就分享
        HighScore,      // 高分分享
        Invitation      // 邀请分享
    }
    
    /// <summary>
    /// 好友数据
    /// </summary>
    [System.Serializable]
    public class FriendData
    {
        public string friendId;
        public string friendName;
        public string avatarUrl;
        public int level;
        public int highScore;
        public bool isOnline;
        public long lastActiveTime;
        
        public FriendData()
        {
            lastActiveTime = System.DateTimeOffset.Now.ToUnixTimeSeconds();
        }
    }
    
    /// <summary>
    /// 社交事件数据
    /// </summary>
    [System.Serializable]
    public class SocialEvent
    {
        public SocialEventType eventType;
        public string playerId;
        public string playerName;
        public string eventData;
        public long timestamp;
        
        public SocialEvent()
        {
            timestamp = System.DateTimeOffset.Now.ToUnixTimeSeconds();
        }
    }
    
    /// <summary>
    /// 社交事件类型
    /// </summary>
    public enum SocialEventType
    {
        NewHighScore,       // 新高分
        StageComplete,      // 完成关卡
        CharacterUnlock,    // 解锁角色
        AchievementUnlock,  // 解锁成就
        PVPVictory         // PVP胜利
    }
    
    /// <summary>
    /// 成就数据
    /// </summary>
    [CreateAssetMenu(fileName = "AchievementConfig", menuName = "GunFire Heroes/Achievement Config")]
    public class AchievementConfig : ScriptableObject
    {
        [Header("基础信息")]
        public int achievementId;
        public string achievementName;
        public AchievementType type;
        
        [Header("条件")]
        public int targetValue;
        public string conditionDescription;
        
        [Header("奖励")]
        public AchievementReward[] rewards;
        
        [Header("显示")]
        public Sprite achievementIcon;
        public bool isHidden = false; // 隐藏成就
        
        [Header("描述")]
        [TextArea(3, 5)]
        public string description;
    }
    
    /// <summary>
    /// 成就类型
    /// </summary>
    public enum AchievementType
    {
        KillEnemies,        // 击杀敌人
        CompleteStages,     // 完成关卡
        CollectGold,        // 收集金币
        UpgradeCharacter,   // 升级角色
        WinPVP,            // PVP胜利
        LoginDays,         // 登录天数
        ShareGame          // 分享游戏
    }
    
    /// <summary>
    /// 成就奖励
    /// </summary>
    [System.Serializable]
    public class AchievementReward
    {
        public AchievementRewardType rewardType;
        public int amount;
        public int itemId; // 用于角色、武器等特定物品
    }
    
    /// <summary>
    /// 成就奖励类型
    /// </summary>
    public enum AchievementRewardType
    {
        Gold,           // 金币
        Diamond,        // 钻石
        Character,      // 角色
        Weapon,         // 武器
        Title          // 称号
    }
    
    /// <summary>
    /// 玩家成就进度
    /// </summary>
    [System.Serializable]
    public class PlayerAchievement
    {
        public int achievementId;
        public int currentProgress;
        public bool isCompleted;
        public bool isRewarded;
        public long completedTime;
        
        public PlayerAchievement()
        {
            currentProgress = 0;
            isCompleted = false;
            isRewarded = false;
            completedTime = 0;
        }
    }
}