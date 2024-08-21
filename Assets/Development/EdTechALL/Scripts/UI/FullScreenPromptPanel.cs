using UnityEngine.Video;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace FullScreen
{
    public class FullScreenPromptPanel : MonoBehaviour
    {
        public GameObject panel;
        public TextMeshProUGUI displayText;
        public TextMeshProUGUI mainButtonText;
        public Button mainButton;
        public Image mainButtonImage;
        public TextMeshProUGUI replayButtonText;
        public Button replayButton;
        public Image replayButtonImage, replayButtonIconImage;
        public Image texture;
        public RawImage videoTexture;
        public VideoPlayer videoPlayer;
        public AudioSource audioSource;

        private RenderTexture renderTexture;
        private RectTransform videoTextureTransform;
        private float videoTextureWidth;
        private float videoTextureHeight;

        private float timer = 0.0f;
        private float time = 0.0f;

        private bool closeWindow = false;
        private bool enableReplayButton = false;

        private const float maxRadius = 100.0f;

        private bool admin = false;
        private bool credits = false;

        public static void RequestFullScreenPromptPanel(string text, float buttonTime = 0.0f, Sprite sprite = null, VideoClip videoClip = null, AudioClip audioClip = null, bool isCredits = false)
        {
            EventBus.Publish(EventId.UIEvents.FullScreenPromptPanel, text, buttonTime, sprite, videoClip, audioClip, isCredits);
        }

        public static IEnumerator RequestFullScreenPromptPanelCoroutine(string text, float buttonTime = 0.0f, Sprite sprite = null, VideoClip videoClip = null, AudioClip audioClip = null)
        {
            yield return new WaitForSeconds(0.1f);

            RequestFullScreenPromptPanel(text, buttonTime, sprite, videoClip, audioClip);
        }

        private void Awake()
        {
            Debug.Assert(panel, "FullScreenPromptPanel is missing a reference to a GameObject");
            Debug.Assert(displayText, "FullScreenPromptPanel is missing a reference to a TMPro");
            if (!mainButtonText)
            {
                mainButtonText = mainButton.GetComponentInChildren<TextMeshProUGUI>();
            }
            Debug.Assert(mainButtonText, "FullScreenPromptPanel is missing a reference to a TMPro");
            if (!replayButtonText)
            {
                replayButtonText = replayButton.GetComponentInChildren<TextMeshProUGUI>();
            }
            Debug.Assert(replayButtonText, "FullScreenPromptPanel is missing a reference to a TMPro");
            Debug.Assert(mainButton, "FullScreenPromptPanel is missing a reference to the main Button");
            Debug.Assert(replayButton, "FullScreenPromptPanel is missing a reference to the replay Button");
            Debug.Assert(mainButtonImage, "FullScreenPromptPanel is missing a reference to an Image");
            Debug.Assert(replayButtonImage, "FullScreenPromptPanel is missing a reference to an Image");
            Debug.Assert(replayButtonIconImage, "FullScreenPromptPanel is missing a reference to an Image");
            Debug.Assert(texture, "FullScreenPromptPanel is missing a reference to an Image");
            Debug.Assert(videoTexture, "FullScreenPromptPanel is missing a reference to a RawImage");
            Debug.Assert(videoPlayer, "FullScreenPromptPanel is missing a reference to a VideoPlayer");
            Debug.Assert(audioSource, "FullScreenPromptPanel is missing a reference to an AudioSource");

            EventBus.SaveRegisterCallback(this, EventId.UIEvents.FullScreenPromptPanel, "OnFullScreenPromptPanel");
            EventBus.SaveRegisterCallback(this, MiniGame.EventId.QuitAdminMode, "OnQuitAdminMode");
            EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.SubMiniGameAborted, "OnSubMiniGameAborted");

            videoTextureTransform = videoTexture.GetComponent<RectTransform>();
            videoTextureWidth = videoTextureTransform.rect.width;
            videoTextureHeight = videoTextureTransform.rect.height;

            panel.SetActive(false);

            if (PlayerPrefs.HasKey(MiniGame.EventId.AdminMode))
            {
                admin = PlayerPrefs.GetInt(MiniGame.EventId.AdminMode) != 0;
            }
        }

        private void Update()
        {
            if (closeWindow)
            {
                ClosePanel();
            }
            else if (admin)
            {
                int relevantTouchCount = 0;

                for (int i = 0; i < Input.touchCount; i++)
                {
                    if (Input.touches[i].radius <= maxRadius)
                    {
                        relevantTouchCount++;
                    }
                }

                if (relevantTouchCount == 0)
                {
                    mainButton.interactable = true;
                    mainButtonImage.enabled = true;
                    mainButtonText.enabled = true;

                    if (!credits)
                    {
                        replayButton.interactable = enableReplayButton;
                        replayButtonImage.enabled = enableReplayButton;
                        replayButtonIconImage.enabled = enableReplayButton;
                        replayButtonText.enabled = enableReplayButton;
                        replayButton.gameObject.SetActive(enableReplayButton);
                    }
                    else
                    {
                        replayButton.interactable = false;
                        replayButtonImage.enabled = false;
                        replayButtonIconImage.enabled = false;
                        replayButtonText.enabled = false;
                        replayButton.gameObject.SetActive(false);
                    }
                }
            }
            else if (time >= timer)
            {
                if (enabled && !mainButton.interactable && !videoPlayer.isPlaying && !audioSource.isPlaying)
                {
                    int relevantTouchCount = 0;

                    for (int i = 0; i < Input.touchCount; i++)
                    {
                        if (Input.touches[i].radius <= maxRadius)
                        {
                            relevantTouchCount++;
                        }
                    }

                    if (relevantTouchCount == 0)
                    {
                        mainButton.interactable = true;
                        mainButtonImage.enabled = true;
                        mainButtonText.enabled = true;

                        if (!credits)
                        {
                            replayButton.interactable = enableReplayButton;
                            replayButtonImage.enabled = enableReplayButton;
                            replayButtonIconImage.enabled = enableReplayButton;
                            replayButtonText.enabled = enableReplayButton;
                            replayButton.gameObject.SetActive(enableReplayButton);
                        }
                        else
                        {
                            replayButton.interactable = false;
                            replayButtonImage.enabled = false;
                            replayButtonIconImage.enabled = false;
                            replayButtonText.enabled = false;
                            replayButton.gameObject.SetActive(false);
                        }
                    }
                }
                else if (videoPlayer.isPlaying || audioSource.isPlaying)
                {
                    if (!credits)
                    {
                        mainButton.interactable = false;
                        mainButtonImage.enabled = false;
                        mainButtonText.enabled = false;
                        replayButton.interactable = enableReplayButton;
                        replayButtonImage.enabled = enableReplayButton;
                        replayButtonIconImage.enabled = enableReplayButton;
                        replayButtonText.enabled = enableReplayButton;
                        replayButton.gameObject.SetActive(enableReplayButton);
                    }
                    else
                    {
                        mainButton.interactable = true;
                        mainButtonImage.enabled = true;
                        mainButtonText.enabled = true;
                        replayButton.interactable = false;
                        replayButtonImage.enabled = false;
                        replayButtonIconImage.enabled = false;
                        replayButtonText.enabled = false;
                        replayButton.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                time += Time.deltaTime;
            }
        }

        private void OnEnable()
        {
            if (!credits)
            {
                mainButton.interactable = false;
                mainButtonImage.enabled = false;
                mainButtonText.enabled = false;
                replayButton.interactable = enableReplayButton;
                replayButtonImage.enabled = enableReplayButton;
                replayButtonIconImage.enabled = enableReplayButton;
                replayButtonText.enabled = enableReplayButton;
                replayButton.gameObject.SetActive(enableReplayButton);
            }
            else
            {
                mainButton.interactable = true;
                mainButtonImage.enabled = true;
                mainButtonText.enabled = true;
                replayButton.interactable = false;
                replayButtonImage.enabled = false;
                replayButtonIconImage.enabled = false;
                replayButtonText.enabled = false;
                replayButton.gameObject.SetActive(false);
            }
            time = 0.0f;
            closeWindow = false;
        }

        private void OnDisable()
        {
            timer = 0.0f;
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, EventId.UIEvents.FullScreenPromptPanel, "OnFullScreenPromptPanel");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.QuitAdminMode, "OnQuitAdminMode");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.SubMiniGameAborted, "OnSubMiniGameAborted");

            if (renderTexture)
            {
                if (renderTexture.IsCreated())
                {
                    renderTexture.Release();
                }
            }
        }

        public void OnFullScreenPromptPanel(string text, float buttonTime, Sprite sprite, VideoClip videoClip, AudioClip audioClip, bool isCredits)
        {
            credits = isCredits;
            panel.SetActive(true);
            mainButton.onClick.AddListener(ClosePanel);
            replayButton.onClick.AddListener(OnReplay);

            timer = buttonTime;

            displayText.text = text;
            texture.enabled = sprite != null;
            texture.sprite = sprite;
            texture.type = Image.Type.Filled;
            texture.preserveAspect = true;
            enableReplayButton = false;

            videoTexture.enabled = videoClip != null;
            videoPlayer.clip = videoClip;
            if (videoClip)
            {
                enableReplayButton = true;
                float videoAspect = (float)videoClip.width / (float)videoClip.height;

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
                renderTexture = new RenderTexture((int)videoClip.width, (int)videoClip.height, 32);
                renderTexture.Create();

                videoTexture.texture = renderTexture;
                videoPlayer.targetTexture = renderTexture;

                videoPlayer.Play();

                if (!credits)
                {
                    mainButton.interactable = false;
                    mainButtonImage.enabled = false;
                    mainButtonText.enabled = false;
                    replayButton.interactable = enableReplayButton;
                    replayButtonImage.enabled = enableReplayButton;
                    replayButtonIconImage.enabled = enableReplayButton;
                    replayButtonText.enabled = enableReplayButton;
                    replayButton.gameObject.SetActive(enableReplayButton);
                }
                else
                {
                    mainButton.interactable = true;
                    mainButtonImage.enabled = true;
                    mainButtonText.enabled = true;
                    replayButton.interactable = false;
                    replayButtonImage.enabled = false;
                    replayButtonIconImage.enabled = false;
                    replayButtonText.enabled = false;
                    replayButton.gameObject.SetActive(false);
                }
            }
            else
            {
                videoTextureTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, videoTextureWidth);
                videoTextureTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, videoTextureHeight);

                if (credits)
                {
                    mainButton.interactable = true;
                    mainButtonImage.enabled = true;
                    mainButtonText.enabled = true;
                    replayButton.interactable = false;
                    replayButtonImage.enabled = false;
                    replayButtonIconImage.enabled = false;
                    replayButtonText.enabled = false;
                    replayButton.gameObject.SetActive(false);
                }
            }

            audioSource.clip = audioClip;
            if (audioClip)
            {
                enableReplayButton = true;
                audioSource.Play();

                if (!credits)
                {
                    mainButton.interactable = false;
                    mainButtonImage.enabled = false;
                    mainButtonText.enabled = false;
                    replayButton.interactable = enableReplayButton;
                    replayButtonImage.enabled = enableReplayButton;
                    replayButtonIconImage.enabled = enableReplayButton;
                    replayButtonText.enabled = enableReplayButton;
                    replayButton.gameObject.SetActive(enableReplayButton);
                }
                else
                {
                    mainButton.interactable = true;
                    mainButtonImage.enabled = true;
                    mainButtonText.enabled = true;
                    replayButton.interactable = false;
                    replayButtonImage.enabled = false;
                    replayButtonIconImage.enabled = false;
                    replayButtonText.enabled = false;
                    replayButton.gameObject.SetActive(false);
                }
            }
            else if (credits)
            {
                mainButton.interactable = true;
                mainButtonImage.enabled = true;
                mainButtonText.enabled = true;
                replayButton.interactable = false;
                replayButtonImage.enabled = false;
                replayButtonIconImage.enabled = false;
                replayButtonText.enabled = false;
                replayButton.gameObject.SetActive(false);
            }
        }

        public void OnQuitAdminMode()
        {
            admin = false;
        }

        public void OnSubMiniGameAborted(string subMiniGameName)
        {
            if (panel.activeSelf)
            {
                credits = false;
                panel.SetActive(false);
                closeWindow = false;
                timer = 0.0f;
                time = 0.0f;
                displayText.text = string.Empty;
                texture.enabled = false;
                videoPlayer.Stop();
                videoTexture.enabled = false;
                audioSource.Stop();

                mainButton.onClick.RemoveAllListeners();
                replayButton.onClick.RemoveAllListeners();
                panel.SetActive(false);
            }
        }

        public void ClosePanel()
        {
            if (panel.activeSelf)
            {
                credits = false;
                panel.SetActive(false);
                closeWindow = false;
                timer = 0.0f;
                time = 0.0f;
                displayText.text = string.Empty;
                texture.enabled = false;
                videoPlayer.Stop();
                videoTexture.enabled = false;
                audioSource.Stop();

                mainButton.onClick.RemoveAllListeners();
                replayButton.onClick.RemoveAllListeners();
                panel.SetActive(false);

                EventBus.Publish(EventId.UIEvents.FullScreenPromptPanelClosed);
            }
        }

        public void OnReplay()
        {
            if (videoPlayer.clip)
            {
                if (videoPlayer.isPlaying)
                {
                    videoPlayer.Stop();
                }
                videoPlayer.Play();
            }
            if (audioSource.clip)
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
                audioSource.Play();
            }
        }
    }

    public partial class EventId
    {
        public static class UIEvents
        {
            public const string FullScreenPromptPanel = "FullScreenPromptPanel";
            public const string FullScreenPromptPanelClosed = "FullScreenPromptPanelClosed";
        }
    }
}