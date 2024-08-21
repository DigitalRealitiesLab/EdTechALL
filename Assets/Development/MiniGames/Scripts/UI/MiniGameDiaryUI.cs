using TMPro;
using UnityEngine;

namespace MiniGame.UI
{
    public class MiniGameDiaryUI : AMiniGameWindow
    {
        public TextMeshProUGUI diaryNumberText;
        public MiniGameDiaryPromptUI miniGameDiaryPromptUI;

        protected override void OnEnable()
        {
            base.OnEnable();
            mainButton.onClick.AddListener(baseMiniGameUIController.OnDiaryContinue);
            replayButton.onClick.AddListener(baseMiniGameUIController.OnReplayInstruction);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (!answered)
            {
                videoPlayer.Stop();
                audioSource.Stop();
                EventBus.Publish(EventId.MiniGameEvents.DiaryWindowClosed);
            }
            mainButton.onClick.RemoveListener(baseMiniGameUIController.OnDiaryContinue);
            replayButton.onClick.RemoveListener(baseMiniGameUIController.OnReplayInstruction);
        }

        public override void LoadMiniGameConfig(MiniGameConfig config)
        {
            mainButtonText.text = mainButtonTextContent;
            exitButtonText.text = exitButtonTextContent;
            replayButtonText.text = replayButtonTextContent;

            if (!miniGameDiaryPromptUI.videoTextureTransform)
            {
                miniGameDiaryPromptUI.videoTextureTransform = miniGameDiaryPromptUI.promptVideoTexture.GetComponent<RectTransform>();
                miniGameDiaryPromptUI.videoTextureWidth = miniGameDiaryPromptUI.videoTextureTransform.rect.width;
                miniGameDiaryPromptUI.videoTextureHeight = miniGameDiaryPromptUI.videoTextureTransform.rect.height;
            }
            miniGameDiaryPromptUI.videoTextureTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, miniGameDiaryPromptUI.videoTextureWidth);
            miniGameDiaryPromptUI.videoTextureTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, miniGameDiaryPromptUI.videoTextureHeight);
        }
    }
}