using UnityEngine;
using UnityEngine.UI;
using System.Text;
using GunFireHeroes.Core;
using GunFireHeroes.Gameplay;

namespace GunFireHeroes.UI
{
    public class StageSelectPanel : MonoBehaviour
    {
        public Text titleText;
        public Text progressText;
        public Button prevButton;
        public Button nextButton;
        public Button startButton;
        public Transform stageGrid; // 可用于实例化关卡按钮
        public GameObject stageItemPrefab; // 简易关卡项

        private int currentPage = 0;
        private const int PageSize = 10;

        private void Awake()
        {
            if (prevButton) prevButton.onClick.AddListener(() => { currentPage = Mathf.Max(0, currentPage - 1); Refresh(); });
            if (nextButton) nextButton.onClick.AddListener(() => { currentPage++; Refresh(); });
            if (startButton) startButton.onClick.AddListener(StartSelectedStage);
        }

        private void OnEnable()
        {
            Refresh();
        }

        private void Refresh()
        {
            var pd = PlayerDataManager.PlayerData;
            if (titleText) titleText.text = "关卡选择";
            if (progressText) progressText.text = $"当前关卡: {pd.currentStage} 最高解锁: {pd.maxUnlockedStage}";

            // 清理Grid
            if (stageGrid)
            {
                for (int i = stageGrid.childCount - 1; i >= 0; i--)
                {
                    Destroy(stageGrid.GetChild(i).gameObject);
                }
            }

            int start = currentPage * PageSize + 1;
            for (int i = 0; i < PageSize; i++)
            {
                int stageId = start + i;
                var go = Instantiate(stageItemPrefab, stageGrid);
                var label = go.GetComponentInChildren<Text>();
                if (label) label.text = $"关卡 {stageId}";
                var btn = go.GetComponentInChildren<Button>();
                bool unlocked = stageId <= pd.maxUnlockedStage;
                if (btn)
                {
                    btn.interactable = unlocked;
                    btn.onClick.AddListener(() => { pd.currentStage = stageId; PlayerDataManager.SavePlayerData(); RefreshHeaderOnly(); });
                }
            }
        }

        private void RefreshHeaderOnly()
        {
            var pd = PlayerDataManager.PlayerData;
            if (progressText) progressText.text = $"当前关卡: {pd.currentStage} 最高解锁: {pd.maxUnlockedStage}";
        }

        private void StartSelectedStage()
        {
            var gm = Object.FindObjectOfType<GameplayManager>();
            if (gm)
            {
                gm.StartStage(PlayerDataManager.PlayerData.currentStage);
            }
        }
    }
}
