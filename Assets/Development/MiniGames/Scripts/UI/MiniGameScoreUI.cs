using UnityEngine;
using UnityEngine.UI;

namespace MiniGame.UI
{
    public class MiniGameScoreUI : AMiniGameWindow
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            mainButton.onClick.AddListener(baseMiniGameUIController.OnScoreConfirm);
            replayButton.onClick.AddListener(baseMiniGameUIController.OnReplayInstruction);

            if (baseMiniGameUIController is MiniGameUIController)
            {
                exitButton.onClick.AddListener((baseMiniGameUIController as MiniGameUIController).OnScoreCancel);
            }
            else if (baseMiniGameUIController is VirtualTourMiniGameUIController)
            {
                exitButton.onClick.AddListener((baseMiniGameUIController as VirtualTourMiniGameUIController).OnScoreCancel);
            }
            else
            {
                exitButton.onClick.AddListener(baseMiniGameUIController.OnScoreCancel);
            }
        }

        public override void LoadMiniGameConfig(MiniGameConfig config)
        {
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SetScoreSprite, "SetScoreSprite");

            miniGameTitleText.text = config.title;
            mainButtonText.text = mainButtonTextContent;
            exitButtonText.text = exitButtonTextContent;
            replayButtonText.text = replayButtonTextContent;
            texture.enabled = config.scoreSprite != null;
            texture.sprite = config.scoreSprite;
            texture.type = Image.Type.Filled;
            texture.preserveAspect = true;
            enableReplayButton = false;

            videoTexture.enabled = config.scoreVideoClip != null;
            videoPlayer.clip = config.scoreVideoClip;
            if (config.scoreVideoClip)
            {
                enableReplayButton = true;
                float videoAspect = (float)config.scoreVideoClip.width / (float)config.scoreVideoClip.height;

                float newWidth;
                float newHeight;

                if (videoPlayer.clip.width > videoPlayer.clip.height)
                {
                    newWidth = videoTextureWidth;
                    newHeight = newWidth / videoAspect;
                }
                else
                {
                    newHeight = videoTextureHeight;
                    newWidth = newHeight * videoAspect;
                }

                videoTextureTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
                videoTextureTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);

                if (renderTexture)
                {
                    if (renderTexture.IsCreated())
                    {
                        renderTexture.Release();
                    }
                }
                renderTexture = new RenderTexture((int)config.scoreVideoClip.width, (int)config.scoreVideoClip.height, 32);
                renderTexture.Create();

                videoTexture.texture = renderTexture;
                videoPlayer.targetTexture = renderTexture;

                videoPlayer.Play();

                mainButton.interactable = false;
                mainButtonImage.enabled = false;
                mainButtonText.enabled = false;
                exitButton.interactable = false;
                exitButtonImage.enabled = false;
                exitButtonText.enabled = false;
                replayButton.interactable = enableReplayButton;
                replayButtonImage.enabled = enableReplayButton;
                replayButtonIconImage.enabled = enableReplayButton;
                replayButtonText.enabled = enableReplayButton;
                replayButton.gameObject.SetActive(enableReplayButton);
            }
            else
            {
                videoTextureTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, videoTextureWidth);
                videoTextureTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, videoTextureHeight);
            }

            audioSource.clip = config.scoreAudioClip;
            if (config.scoreAudioClip)
            {
                enableReplayButton = true;
                audioSource.Play();

                mainButton.interactable = false;
                mainButtonImage.enabled = false;
                mainButtonText.enabled = false;
                exitButton.interactable = false;
                exitButtonImage.enabled = false;
                exitButtonText.enabled = false;
                replayButton.interactable = enableReplayButton;
                replayButtonImage.enabled = enableReplayButton;
                replayButtonIconImage.enabled = enableReplayButton;
                replayButtonText.enabled = enableReplayButton;
                replayButton.gameObject.SetActive(enableReplayButton);
            }

            if (string.IsNullOrEmpty(config.scoreText))
            {
                miniGameDescriptionText.text = "Geschafft!";
            }
            else
            {
                miniGameDescriptionText.text = config.scoreText;
            }
        }

        public override void UnloadMiniGameConfig()
        {
            base.UnloadMiniGameConfig();
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SetScoreSprite, "SetScoreSprite");
        }

        public void SetScoreSprite(Sprite scoreSprite)
        {
            texture.enabled = scoreSprite != null;
            texture.sprite = scoreSprite;
            texture.type = Image.Type.Filled;
            texture.preserveAspect = true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            mainButton.onClick.RemoveListener(baseMiniGameUIController.OnScoreConfirm);
            replayButton.onClick.RemoveListener(baseMiniGameUIController.OnReplayInstruction);

            if (baseMiniGameUIController is MiniGameUIController)
            {
                exitButton.onClick.RemoveListener((baseMiniGameUIController as MiniGameUIController).OnScoreCancel);
            }
            else if (baseMiniGameUIController is VirtualTourMiniGameUIController)
            {
                exitButton.onClick.RemoveListener((baseMiniGameUIController as VirtualTourMiniGameUIController).OnScoreCancel);
            }
            else
            {
                exitButton.onClick.RemoveListener(baseMiniGameUIController.OnScoreCancel);
            }
        }
    }
}