using UnityEngine;
using UnityEngine.UI;

namespace GunFireHeroes.UI
{
    public class LoadingPanel : MonoBehaviour
    {
        public Slider progressBar;
        public Text progressText;
        public Text tipText;

        public void SetProgress(float value)
        {
            value = Mathf.Clamp01(value);
            if (progressBar) progressBar.value = value;
            if (progressText) progressText.text = $"{Mathf.RoundToInt(value * 100)}%";
        }

        public void SetTip(string tip)
        {
            if (tipText) tipText.text = tip;
        }

        private void OnEnable()
        {
            // 重置显示
            SetProgress(0f);
        }
    }
}
