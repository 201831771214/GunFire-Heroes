using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GunFireHeroes.Core
{
    /// <summary>
    /// 简单账户系统：本地注册/登录（基于PlayerPrefs存储），用于演示登录/注册流程
    /// </summary>
    public static class AuthManager
    {
        private const string AuthDataKey = "Auth_Data";

        [Serializable]
        private class UserRecord
        {
            public string username;
            public string passwordHash;  // SHA256
            public long createdAt;
            public long lastLoginAt;
        }

        [Serializable]
        private class AuthData
        {
            public List<UserRecord> users = new List<UserRecord>();
            public string currentUser = null;
            public bool isLoggedIn = false;
        }

        private static AuthData data;

        public static bool IsLoggedIn => EnsureLoaded().isLoggedIn && !string.IsNullOrEmpty(EnsureLoaded().currentUser);
        public static string CurrentUser => EnsureLoaded().currentUser;

        private static AuthData EnsureLoaded()
        {
            if (data != null) return data;
            var json = PlayerPrefs.GetString(AuthDataKey, "");
            if (string.IsNullOrEmpty(json))
            {
                data = new AuthData();
                Save();
            }
            else
            {
                try { data = JsonUtility.FromJson<AuthData>(json) ?? new AuthData(); }
                catch { data = new AuthData(); }
            }
            return data;
        }

        private static void Save()
        {
            if (data == null) return;
            var json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(AuthDataKey, json);
            PlayerPrefs.Save();
        }

        public static bool Register(string username, string password, out string message)
        {
            username = (username ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                message = "用户名或密码不能为空";
                return false;
            }
            if (username.Length < 3)
            {
                message = "用户名至少3个字符";
                return false;
            }
            if (password.Length < 6)
            {
                message = "密码至少6个字符";
                return false;
            }

            var d = EnsureLoaded();
            if (d.users.Any(u => string.Equals(u.username, username, StringComparison.OrdinalIgnoreCase)))
            {
                message = "该用户名已被注册";
                return false;
            }

            var record = new UserRecord
            {
                username = username,
                passwordHash = Sha256(password),
                createdAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                lastLoginAt = 0
            };
            d.users.Add(record);
            Save();
            message = "注册成功，请登录";
            return true;
        }

        public static bool TryLogin(string username, string password, out string message)
        {
            username = (username ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                message = "请输入用户名和密码";
                return false;
            }

            var d = EnsureLoaded();
            var user = d.users.FirstOrDefault(u => string.Equals(u.username, username, StringComparison.OrdinalIgnoreCase));
            if (user == null)
            {
                message = "用户不存在";
                return false;
            }

            var hash = Sha256(password);
            if (!string.Equals(hash, user.passwordHash, StringComparison.Ordinal))
            {
                message = "密码错误";
                return false;
            }

            d.currentUser = user.username;
            d.isLoggedIn = true;
            user.lastLoginAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Save();

            // 同步到玩家数据（昵称）
            try
            {
                PlayerDataManager.PlayerData.playerName = user.username;
                PlayerDataManager.SavePlayerData();
            }
            catch { }

            message = "登录成功";
            return true;
        }

        public static void Logout()
        {
            var d = EnsureLoaded();
            d.isLoggedIn = false;
            d.currentUser = null;
            Save();
        }

        public static bool HasAnyAccount()
        {
            return EnsureLoaded().users.Count > 0;
        }

        public static bool QuickGuestLogin(out string username)
        {
            username = $"游客_{UnityEngine.Random.Range(100000, 999999)}";
            string msg;
            // 为游客创建一个随机密码并注册
            if (!HasAnyAccount() || !EnsureLoaded().users.Any(u => u.username == username))
            {
                Register(username, Guid.NewGuid().ToString("N").Substring(0, 8), out msg);
            }
            return TryLogin(username, "wrong-password-will-be-ignored", out msg) || ForceLogin(username);
        }

        private static bool ForceLogin(string username)
        {
            var d = EnsureLoaded();
            if (!d.users.Any(u => u.username == username)) return false;
            d.currentUser = username;
            d.isLoggedIn = true;
            Save();
            try
            {
                PlayerDataManager.PlayerData.playerName = username;
                PlayerDataManager.SavePlayerData();
            }
            catch { }
            return true;
        }

        private static string Sha256(string input)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder();
                foreach (var b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}
