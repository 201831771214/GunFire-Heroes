using UnityEngine;
using System.Collections.Generic;
using GunFireHeroes.Core;

namespace GunFireHeroes.Social
{
    /// <summary>
    /// 社交系统管理器，负责排行榜、分享等社交功能
    /// </summary>
    public class SocialManager : MonoBehaviour
    {
        [Header("排行榜配置")]
        public LeaderboardConfig[] leaderboards;
        
        [Header("分享配置")]
        public ShareConfig shareConfig;
        
        private Dictionary<LeaderboardType, LeaderboardData> leaderboardData;
        
        private void Awake()
        {
            InitializeLeaderboards();
        }
        
        private void InitializeLeaderboards()
        {
            leaderboardData = new Dictionary<LeaderboardType, LeaderboardData>();
            
            foreach (var config in leaderboards)
            {
                leaderboardData[config.type] = new LeaderboardData
                {
                    type = config.type,
                    entries = new List<LeaderboardEntry>()
                };
            }
        }
        
        /// <summary>
        /// 获取排行榜数据
        /// </summary>
        public void GetLeaderboard(LeaderboardType type, System.Action<LeaderboardData> callback)
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
            // 微信小游戏排行榜
            GetWeChatLeaderboard(type, callback);
            #else
            // 本地模拟数据
            if (leaderboardData.ContainsKey(type))
            {
                callback?.Invoke(leaderboardData[type]);
            }
            #endif
        }
        
        private void GetWeChatLeaderboard(LeaderboardType type, System.Action<LeaderboardData> callback)
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
            // WX.GetFriendCloudStorage(new GetFriendCloudStorageParam
            // {
            //     keyList = new string[] { GetLeaderboardKey(type) },
            //     success = (res) => {
            //         var data = ParseLeaderboardData(res.data, type);
            //         callback?.Invoke(data);
            //     },
            //     fail = (res) => {
            //         Debug.LogError("获取排行榜失败: " + res.errMsg);
            //         callback?.Invoke(null);
            //     }
            // });
            #endif
        }
        
        /// <summary>
        /// 上传分数到排行榜
        /// </summary>
        public void SubmitScore(LeaderboardType type, int score)
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
            SubmitWeChatScore(type, score);
            #else
            // 本地模拟
            UpdateLocalLeaderboard(type, score);
            #endif
        }
        
        private void SubmitWeChatScore(LeaderboardType type, int score)
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
            // var data = new Dictionary<string, object>
            // {
            //     { GetLeaderboardKey(type), score }
            // };
            // 
            // WX.SetUserCloudStorage(new SetUserCloudStorageParam
            // {
            //     KVDataList = data,
            //     success = () => {
            //         Debug.Log($"分数上传成功: {type} - {score}");
            //     },
            //     fail = (res) => {
            //         Debug.LogError("分数上传失败: " + res.errMsg);
            //     }
            // });
            #endif
        }
        
        private void UpdateLocalLeaderboard(LeaderboardType type, int score)
        {
            if (!leaderboardData.ContainsKey(type))
                return;
                
            var data = leaderboardData[type];
            var playerData = PlayerDataManager.PlayerData;
            
            // 查找或创建玩家条目
            var entry = data.entries.Find(e => e.playerId == playerData.playerName);
            if (entry == null)
            {
                entry = new LeaderboardEntry
                {
                    playerId = playerData.playerName,
                    playerName = playerData.playerName,
                    score = score,
                    rank = 0
                };
                data.entries.Add(entry);
            }
            else
            {
                // 更新最高分
                if (score > entry.score)
                {
                    entry.score = score;
                }
            }
            
            // 重新排序
            data.entries.Sort((a, b) => b.score.CompareTo(a.score));
            
            // 更新排名
            for (int i = 0; i < data.entries.Count; i++)
            {
                data.entries[i].rank = i + 1;
            }
        }
        
        /// <summary>
        /// 分享游戏
        /// </summary>
        public void ShareGame(ShareType shareType, string customMessage = "")
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
            ShareToWeChat(shareType, customMessage);
            #else
            Debug.Log($"分享游戏: {shareType} - {customMessage}");
            #endif
        }
        
        private void ShareToWeChat(ShareType shareType, string customMessage)
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
            string title = "";
            string imageUrl = "";
            
            switch (shareType)
            {
                case ShareType.Achievement:
                    title = shareConfig.achievementTitle;
                    imageUrl = shareConfig.achievementImage;
                    break;
                case ShareType.HighScore:
                    title = shareConfig.highScoreTitle;
                    imageUrl = shareConfig.highScoreImage;
                    break;
                case ShareType.Invitation:
                    title = shareConfig.invitationTitle;
                    imageUrl = shareConfig.invitationImage;
                    break;
            }
            
            if (!string.IsNullOrEmpty(customMessage))
            {
                title = customMessage;
            }
            
            // WX.ShareAppMessage(new ShareAppMessageParam
            // {
            //     title = title,
            //     imageUrl = imageUrl,
            //     success = () => {
            //         Debug.Log("分享成功");
            //         OnShareSuccess(shareType);
            //     },
            //     fail = (res) => {
            //         Debug.LogError("分享失败: " + res.errMsg);
            //     }
            // });
            #endif
        }
        
        /// <summary>
        /// 分享成功回调
        /// </summary>
        private void OnShareSuccess(ShareType shareType)
        {
            // 可以给予分享奖励
            switch (shareType)
            {
                case ShareType.Achievement:
                    PlayerDataManager.AddCurrency(CurrencyType.Gold, 100);
                    break;
                case ShareType.HighScore:
                    PlayerDataManager.AddCurrency(CurrencyType.Diamond, 10);
                    break;
                case ShareType.Invitation:
                    PlayerDataManager.AddCurrency(CurrencyType.Gold, 200);
                    break;
            }
            
            var uiManager = FindObjectOfType<UI.UIManager>();
            if (uiManager != null)
            {
                uiManager.ShowMessage("分享成功，获得奖励！");
                uiManager.UpdateCurrencyDisplay();
            }
        }
        
        /// <summary>
        /// 获取排行榜键名
        /// </summary>
        private string GetLeaderboardKey(LeaderboardType type)
        {
            switch (type)
            {
                case LeaderboardType.HighScore:
                    return "high_score";
                case LeaderboardType.StageProgress:
                    return "stage_progress";
                case LeaderboardType.PVPRank:
                    return "pvp_rank";
                default:
                    return "unknown";
            }
        }
        
        /// <summary>
        /// 解析排行榜数据
        /// </summary>
        private LeaderboardData ParseLeaderboardData(object rawData, LeaderboardType type)
        {
            // 这里需要根据微信返回的数据格式进行解析
            // 暂时返回空数据
            return new LeaderboardData
            {
                type = type,
                entries = new List<LeaderboardEntry>()
            };
        }
        
        /// <summary>
        /// 显示排行榜UI
        /// </summary>
        public void ShowLeaderboard(LeaderboardType type)
        {
            GetLeaderboard(type, (data) => {
                if (data != null)
                {
                    // 显示排行榜UI
                    Debug.Log($"显示排行榜: {type}");
                    // 这里可以调用UI管理器显示排行榜界面
                }
            });
        }
        
        /// <summary>
        /// 邀请好友
        /// </summary>
        public void InviteFriends()
        {
            ShareGame(ShareType.Invitation, "快来和我一起玩GunFire Heroes吧！");
        }
        
        /// <summary>
        /// 炫耀成就
        /// </summary>
        public void ShowOffAchievement(string achievementName)
        {
            string message = $"我在GunFire Heroes中获得了成就：{achievementName}！";
            ShareGame(ShareType.Achievement, message);
        }
        
        /// <summary>
        /// 炫耀高分
        /// </summary>
        public void ShowOffHighScore(int score)
        {
            string message = $"我在GunFire Heroes中获得了{score}分的高分！";
            ShareGame(ShareType.HighScore, message);
        }
    }
}