using UnityEngine;
using UnityEngine.UI;
using GunFireHeroes.Core;

namespace GunFireHeroes.UI
{
    public class LoginPanel : MonoBehaviour
    {
        public InputField usernameInput;
        public InputField passwordInput;
        public Button loginButton;
        public Button toRegisterButton;
        public Button guestButton;
        public Text messageText;

        private void Awake()
        {
            if (loginButton) loginButton.onClick.AddListener(OnLoginClicked);
            if (toRegisterButton) toRegisterButton.onClick.AddListener(OnToRegisterClicked);
            if (guestButton) guestButton.onClick.AddListener(OnGuestClicked);
        }

        private void OnEnable()
        {
            messageText?.SetText("");
        }

        private void OnLoginClicked()
        {
            string msg;
            bool ok = AuthManager.TryLogin(usernameInput?.text, passwordInput?.text, out msg);
            messageText?.SetText(msg);
            if (ok)
            {
                FindObjectOfType<UIManager>()?.ShowPanel(UIPanel.MainMenu);
                GameManager.Instance.ChangeGameState(GameState.MainMenu);
            }
        }

        private void OnToRegisterClicked()
        {
            FindObjectOfType<UIManager>()?.ShowPanel(UIPanel.Register);
        }

        private void OnGuestClicked()
        {
            string name;
            if (AuthManager.QuickGuestLogin(out name))
            {
                messageText?.SetText($"已使用{name}进入游戏");
                FindObjectOfType<UIManager>()?.ShowPanel(UIPanel.MainMenu);
                GameManager.Instance.ChangeGameState(GameState.MainMenu);
            }
            else
            {
                messageText?.SetText("游客登录失败");
            }
        }
    }

    public class RegisterPanel : MonoBehaviour
    {
        public InputField usernameInput;
        public InputField passwordInput;
        public InputField confirmInput;
        public Button registerButton;
        public Button backToLoginButton;
        public Text messageText;

        private void Awake()
        {
            if (registerButton) registerButton.onClick.AddListener(OnRegisterClicked);
            if (backToLoginButton) backToLoginButton.onClick.AddListener(() =>
            {
                FindObjectOfType<UIManager>()?.ShowPanel(UIPanel.Login);
            });
        }

        private void OnEnable()
        {
            messageText?.SetText("");
        }

        private void OnRegisterClicked()
        {
            if (passwordInput?.text != confirmInput?.text)
            {
                messageText?.SetText("两次输入的密码不一致");
                return;
            }
            string msg;
            if (AuthManager.Register(usernameInput?.text, passwordInput?.text, out msg))
            {
                messageText?.SetText("注册成功，请登录");
                FindObjectOfType<UIManager>()?.ShowPanel(UIPanel.Login);
            }
            else
            {
                messageText?.SetText(msg);
            }
        }
    }

    internal static class UIExtensions
    {
        public static void SetText(this Text t, string content)
        {
            if (t != null) t.text = content ?? string.Empty;
        }
    }
}
