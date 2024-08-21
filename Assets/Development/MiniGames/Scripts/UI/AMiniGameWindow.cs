using UnityEngine.Video;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MiniGame.UI
{
    public abstract class AMiniGameWindow : MonoBehaviour
    {
        public BaseMiniGameUIController baseMiniGameUIController;
        public TextMeshProUGUI miniGameTitleText, miniGameDescriptionText, mainButtonText, exitButtonText, replayButtonText;
        public Button mainButton, exitButton, replayButton;
        public Image mainButtonImage, exitButtonImage, replayButtonImage, replayButtonIconImage;
        public Image texture;
        public RawImage videoTexture;
        public VideoPlayer videoPlayer;
        public AudioSource audioSource;

        public RectTransform videoTextureTransform, miniGameDescriptionTransform;
        public float videoTextureWidth;
        public float videoTextureHeight;

        public string mainButtonTextContent = "";
        public string exitButtonTextContent = "";
        public string replayButtonTextContent = "";

        public float timer = 0.0f;
        public float time = 0.0f;

        public bool closeWindow = false;
        public bool answered = false;
        public bool enableReplayButton = false;

        protected RenderTexture renderTexture;

        protected const float maxRadius = 100.0f;

        protected bool admin = false;

        protected virtual void Awake()
        {
            Debug.Assert(baseMiniGameUIController, "MiniGameWindow is missing a reference to a BaseMiniGameUIController");
            Debug.Assert(miniGameTitleText, "MiniGameWindow is missing a reference to a TMPro");
            Debug.Assert(miniGameDescriptionText, "MiniGameWindow is missing a reference to a TMPro");
            Debug.Assert(texture, "MiniGameWindow is missing a reference to an Image");
            if (!mainButtonText)
            {
                mainButtonText = mainButton.GetComponentInChildren<TextMeshProUGUI>();
            }
            Debug.Assert(mainButtonText, "MiniGameWindow is missing a reference to a TMPro");
            if (!exitButtonText)
            {
                exitButtonText = exitButton.GetComponentInChildren<TextMeshProUGUI>();
            }
            Debug.Assert(exitButtonText, "MiniGameWindow is missing a reference to a TMPro");
            if (!replayButtonText)
            {
                replayButtonText = replayButton.GetComponentInChildren<TextMeshProUGUI>();
            }
            Debug.Assert(replayButtonText, "MiniGameWindow is missing a reference to a TMPro");
            Debug.Assert(mainButton, "MiniGameWindow is missing a reference to the main Button");
            Debug.Assert(exitButton, "MiniGameWindow is missing a reference to the exit Button");
            Debug.Assert(replayButton, "MiniGameWindow is missing a reference to the replay Button");
            Debug.Assert(mainButtonImage, "MiniGameWindow is missing a reference to an Image");
            Debug.Assert(exitButtonImage, "MiniGameWindow is missing a reference to an Image");
            Debug.Assert(replayButtonImage, "MiniGameWindow is missing a reference to an Image");
            Debug.Assert(replayButtonIconImage, "MiniGameWindow is missing a reference to an Image");
            Debug.Assert(videoTexture, "MiniGameWindow is missing a reference to an RawImage");
            Debug.Assert(videoPlayer, "MiniGameWindow is missing a reference to an VideoPlayer");
            Debug.Assert(audioSource, "MiniGameWindow is missing a reference to an AudioSource");

            videoTextureTransform = videoTexture.GetComponent<RectTransform>();
            videoTextureWidth = videoTextureTransform.rect.width;
            videoTextureHeight = videoTextureTransform.rect.height;

            miniGameDescriptionTransform = miniGameDescriptionText.GetComponent<RectTransform>();

            EventBus.SaveRegisterCallback(this, EventId.QuitAdminMode, "OnQuitAdminMode");

            if (PlayerPrefs.HasKey(EventId.AdminMode))
            {
                admin = PlayerPrefs.GetInt(EventId.AdminMode) != 0;
            }
        }

        protected virtual void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, EventId.QuitAdminMode, "OnQuitAdminMode");

            if(renderTexture)
            {
                if (renderTexture.IsCreated())
                {
                    renderTexture.Release();
                }
            }
        }

        protected virtual void Update()
        {
            if (closeWindow)
            {
                closeWindow = false;
                answered = true;
                if (baseMiniGameUIController.activeMiniGameWindow == this)
                {
                    if (baseMiniGameUIController is MiniGameUIController)
                    {
                        (baseMiniGameUIController as MiniGameUIController).CloseActivePanel();
                    }
                    else if (baseMiniGameUIController is VirtualTourMiniGameUIController)
                    {
                        (baseMiniGameUIController as VirtualTourMiniGameUIController).CloseActivePanel();
                    }
                    else
                    {
                        baseMiniGameUIController.CloseActivePanel();
                    }
                }
                else
                {
                    videoPlayer.Stop();
                    audioSource.Stop();
                }
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
                    exitButton.interactable = true;
                    exitButtonImage.enabled = true;
                    exitButtonText.enabled = true;
                    replayButton.interactable = enableReplayButton;
                    replayButtonImage.enabled = enableReplayButton;
                    replayButtonIconImage.enabled = enableReplayButton;
                    replayButtonText.enabled = enableReplayButton;
                    replayButton.gameObject.SetActive(enableReplayButton);
                }
            }
            else if (time >= timer)
            {
                if (enabled && !exitButton.interactable && !mainButton.interactable && !videoPlayer.isPlaying && !audioSource.isPlaying)
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
                        exitButton.interactable = true;
                        exitButtonImage.enabled = true;
                        exitButtonText.enabled = true;
                        replayButton.interactable = enableReplayButton;
                        replayButtonImage.enabled = enableReplayButton;
                        replayButtonIconImage.enabled = enableReplayButton;
                        replayButtonText.enabled = enableReplayButton;
                        replayButton.gameObject.SetActive(enableReplayButton);
                    }
                }
                else if (videoPlayer.isPlaying || audioSource.isPlaying)
                {
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
            }
            else if (!audioSource.isPlaying && !videoPlayer.isPlaying)
            {
                time += Time.deltaTime;
            }
        }

        protected virtual void OnEnable()
        {
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
            time = 0.0f;
            answered = false;
            closeWindow = false;
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.NextStep, "OnNextStep");
        }

        protected virtual void OnDisable()
        {
            videoPlayer.Stop();
            audioSource.Stop();
            timer = 0.0f;
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.NextStep, "OnNextStep");
        }

        public virtual void LoadMiniGameConfig(MiniGameConfig config)
        {
            miniGameTitleText.text = config.title;
            mainButtonText.text = mainButtonTextContent;
            exitButtonText.text = exitButtonTextContent;
            replayButtonText.text = replayButtonTextContent;
            texture.enabled = config.sprite != null;
            texture.sprite = config.sprite;
            texture.type = Image.Type.Filled;
            texture.preserveAspect = true;
            enableReplayButton = false;

            videoTexture.enabled = config.videoClip != null;
            videoPlayer.clip = config.videoClip;
            if (config.videoClip)
            {
                enableReplayButton = true;
                float videoAspect = (float)config.videoClip.width / (float)config.videoClip.height;

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
                renderTexture = new RenderTexture((int)config.videoClip.width, (int)config.videoClip.height, 32);
                renderTexture.Create();

                videoTexture.texture = renderTexture;
                videoPlayer.targetTexture = renderTexture;

                videoPlayer.Play();
            }
            else
            {
                videoTextureTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, videoTextureWidth);
                videoTextureTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, videoTextureHeight);
            }

            audioSource.clip = config.audioClip;
            if (config.audioClip)
            {
                enableReplayButton = true;
                audioSource.Play();
            }

            if (enableReplayButton)
            {
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
        }

        public virtual void UnloadMiniGameConfig()
        {
            miniGameTitleText.text = string.Empty;
            mainButtonText.text = string.Empty;
            exitButtonText.text = string.Empty;
            miniGameDescriptionText.text = string.Empty;
            texture.enabled = false;
            videoPlayer.Stop();
            videoTexture.enabled = false;
            audioSource.Stop();
        }

        public void OnPreviousStep()
        {
            closeWindow = true;
        }

        public void OnNextStep()
        {
            closeWindow = true;
        }

        public void OnQuitAdminMode()
        {
            admin = false;
        }
    }
}