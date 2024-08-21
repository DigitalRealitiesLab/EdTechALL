using UnityEngine;
using UnityEngine.UI;

namespace MiniGame.UI
{
    public class MiniGameAbortUI : AMiniGameWindow
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            mainButton.onClick.AddListener(baseMiniGameUIController.OnAbortConfirm);
            replayButton.onClick.AddListener(baseMiniGameUIController.OnReplayInstruction);

            if (baseMiniGameUIController is MiniGameUIController)
            {
                exitButton.onClick.AddListener((baseMiniGameUIController as MiniGameUIController).OnAbortCancel);
            }
            else if (baseMiniGameUIController is VirtualTourMiniGameUIController)
            {
                exitButton.onClick.AddListener((baseMiniGameUIController as VirtualTourMiniGameUIController).OnAbortCancel);
            }
            else
            {
                exitButton.onClick.AddListener(baseMiniGameUIController.OnAbortCancel);
            }
        }

        public override void LoadMiniGameConfig(MiniGameConfig config)
        {
            miniGameTitleText.text = config.title;
            mainButtonText.text = mainButtonTextContent;
            exitButtonText.text = exitButtonTextContent;
            replayButtonText.text = replayButtonTextContent;
            texture.enabled = config.abortSprite != null;
            texture.sprite = config.abortSprite;
            texture.type = Image.Type.Filled;
            texture.preserveAspect = true;
            enableReplayButton = false;

            videoTexture.enabled = config.abortVideoClip != null;
            videoPlayer.clip = config.abortVideoClip;
            if (config.abortVideoClip)
            {
                enableReplayButton = true;
                float videoAspect = (float)config.abortVideoClip.width / (float)config.abortVideoClip.height;

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
                renderTexture = new RenderTexture((int)config.abortVideoClip.width, (int)config.abortVideoClip.height, 32);
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

            audioSource.clip = config.abortAudioClip;
            if (config.abortAudioClip)
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

            if (string.IsNullOrEmpty(config.abortText))
            {
                miniGameDescriptionText.text = "Willst du das Mini Spiel wirklich verlassen?";
            }
            else
            {
                miniGameDescriptionText.text = config.abortText;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            mainButton.onClick.RemoveListener(baseMiniGameUIController.OnAbortConfirm);
            replayButton.onClick.RemoveListener(baseMiniGameUIController.OnReplayInstruction);

            if (baseMiniGameUIController is MiniGameUIController)
            {
                exitButton.onClick.RemoveListener((baseMiniGameUIController as MiniGameUIController).OnAbortCancel);
            }
            else if (baseMiniGameUIController is VirtualTourMiniGameUIController)
            {
                exitButton.onClick.RemoveListener((baseMiniGameUIController as VirtualTourMiniGameUIController).OnAbortCancel);
            }
            else
            {
                exitButton.onClick.RemoveListener(baseMiniGameUIController.OnAbortCancel);
            }
        }
    }
}