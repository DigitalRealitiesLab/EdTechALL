using UnityEngine.Video;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGame.UI
{
    public class BaseMiniGameUIController : MonoBehaviour
    {
        public AudioSource audioSource;
        public BaseMiniGameManager baseMiniGameManager;
        public Button abortButton;
        public MiniGameStartUI startUI;
        public MiniGameAbortUI abortUI;
        public MiniGameScoreUI scoreUI;
        public MiniGamePromptUI promptUI;
        public MiniGameQuestionUI questionUI;
        public MiniGameDiaryUI diaryUI;
        public VisualGuidanceUIPanel visualGuidanceUIPanel;
        protected AMiniGameWindow _activeMiniGameWindow = null;
        public AMiniGameWindow activeMiniGameWindow
        {
            get
            {
                return _activeMiniGameWindow;
            }
            set
            {
                audioSource.Stop();
                if (_activeMiniGameWindow == value)
                {
                    return;
                }
                if (_activeMiniGameWindow)
                {
                    _activeMiniGameWindow.gameObject.SetActive(false);
                    _activeMiniGameWindow.enabled = false;
                    _activeMiniGameWindow.UnloadMiniGameConfig();
                }
                _activeMiniGameWindow = value;
                if (_activeMiniGameWindow)
                {
                    _activeMiniGameWindow.gameObject.SetActive(true);
                    _activeMiniGameWindow.enabled = true;
                    _activeMiniGameWindow.LoadMiniGameConfig(baseMiniGameManager.miniGameConfig);
                }
            }
        }

        public GameObject gameUIPanelPrefab;

        public GameObject gameUIPanelParent;

        public AMiniGameWindow[] miniGameWindows => new AMiniGameWindow[] { startUI, abortUI, scoreUI, promptUI, questionUI, diaryUI };

        protected GameObject gameUIPanel;

        protected RenderTexture renderTexture;
        protected RenderTexture promptRenderTexture;

        protected bool abortWindowOpen = false;
        protected bool startWindowOpen = false;

        protected bool visualGuidanceEnabled = true;
        protected bool visualGuidanceState = true;

        protected bool startWindowFirstTime = false;

        protected void Awake()
        {
            if (!_activeMiniGameWindow)
            {
                foreach (AMiniGameWindow window in miniGameWindows)
                {
                    window.enabled = false;
                    window.gameObject.SetActive(false);
                }
            }

            gameUIPanel = Instantiate(gameUIPanelPrefab, gameUIPanelParent.transform);

            gameUIPanel.SetActive(false);
        }

        protected virtual void OnEnable()
        {
            abortWindowOpen = false;
            startWindowOpen = false;
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");
        }

        public virtual void OnRequestStartWindow(bool firstTime = true)
        {
            if (!firstTime)
            {
                startWindowOpen = true;
            }
            startWindowFirstTime = firstTime;
            visualGuidanceState = false;
            visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState);
            activeMiniGameWindow = startUI;
        }

        public void BaseOnRequestPromptWindow(string text, float buttonTime, Sprite sprite, VideoClip videoClip, AudioClip audioClip)
        {
            activeMiniGameWindow = promptUI;
            questionUI.answered = false;
            promptUI.timer = buttonTime;

            if (string.IsNullOrEmpty(text))
            {
                promptUI.miniGameDescriptionText.text = baseMiniGameManager.activeMiniGame.windowText;
            }
            else
            {
                promptUI.miniGameDescriptionText.text = text;
            }

            promptUI.texture.enabled = sprite != null;
            promptUI.texture.sprite = sprite;
            promptUI.texture.type = Image.Type.Filled;
            promptUI.texture.preserveAspect = true;
            bool enableReplayButton = false;

            promptUI.videoTexture.enabled = videoClip != null;
            promptUI.videoPlayer.clip = videoClip;

            if (videoClip)
            {
                enableReplayButton = true;
                float videoAspect = (float)videoClip.width / (float)videoClip.height;

                float newWidth;
                float newHeight;

                if (promptUI.videoPlayer.clip.width > promptUI.videoPlayer.clip.height)
                {
                    newWidth = promptUI.videoTextureWidth;
                    newHeight = newWidth / videoAspect;
                }
                else
                {
                    newHeight = promptUI.videoTextureHeight;
                    newWidth = newHeight * videoAspect;
                }

                promptUI.videoTextureTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
                promptUI.videoTextureTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);

                if (renderTexture)
                {
                    if (renderTexture.IsCreated())
                    {
                        renderTexture.Release();
                    }
                }
                renderTexture = new RenderTexture((int)videoClip.width, (int)videoClip.height, 32);
                renderTexture.Create();

                promptUI.videoTexture.texture = renderTexture;
                promptUI.videoPlayer.targetTexture = renderTexture;

                promptUI.videoPlayer.Play();
            }
            else
            {
                promptUI.videoTextureTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, promptUI.videoTextureWidth);
                promptUI.videoTextureTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, promptUI.videoTextureHeight);
            }

            promptUI.audioSource.clip = audioClip;

            if (audioClip)
            {
                enableReplayButton = true;
                promptUI.audioSource.Play();
            }

            promptUI.enableReplayButton = enableReplayButton;

            if (enableReplayButton)
            {
                promptUI.mainButton.interactable = false;
                promptUI.mainButtonImage.enabled = false;
                promptUI.mainButtonText.enabled = false;
                promptUI.replayButton.interactable = promptUI.enableReplayButton;
                promptUI.replayButtonImage.enabled = promptUI.enableReplayButton;
                promptUI.replayButtonIconImage.enabled = promptUI.enableReplayButton;
                promptUI.replayButtonText.enabled = promptUI.enableReplayButton;
                promptUI.replayButton.gameObject.SetActive(promptUI.enableReplayButton);
            }
        }

        public void BaseOnRequestQuestionWindow(string text, float buttonTime, Sprite sprite, VideoClip videoClip, AudioClip audioClip)
        {
            activeMiniGameWindow = questionUI;
            questionUI.answered = false;
            questionUI.timer = buttonTime;

            if (string.IsNullOrEmpty(text))
            {
                questionUI.miniGameDescriptionText.text = baseMiniGameManager.activeMiniGame.windowText;
            }
            else
            {
                questionUI.miniGameDescriptionText.text = text;
            }
            questionUI.texture.enabled = sprite != null;
            questionUI.texture.sprite = sprite;
            questionUI.texture.type = Image.Type.Filled;
            questionUI.texture.preserveAspect = true;
            bool enableReplayButton = false;

            questionUI.videoTexture.enabled = videoClip != null;
            questionUI.videoPlayer.clip = videoClip;
            if (videoClip)
            {
                enableReplayButton = true;
                float videoAspect = (float)videoClip.width / (float)videoClip.height;

                float newWidth;
                float newHeight;

                if (questionUI.videoPlayer.clip.width > questionUI.videoPlayer.clip.height)
                {
                    newWidth = questionUI.videoTextureWidth;
                    newHeight = newWidth / videoAspect;
                }
                else
                {
                    newHeight = questionUI.videoTextureHeight;
                    newWidth = newHeight * videoAspect;
                }

                questionUI.videoTextureTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
                questionUI.videoTextureTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);

                if (renderTexture)
                {
                    if (renderTexture.IsCreated())
                    {
                        renderTexture.Release();
                    }
                }
                renderTexture = new RenderTexture((int)videoClip.width, (int)videoClip.height, 32);
                renderTexture.Create();

                questionUI.videoTexture.texture = renderTexture;
                questionUI.videoPlayer.targetTexture = renderTexture;

                questionUI.videoPlayer.Play();
            }
            else
            {
                questionUI.videoTextureTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, questionUI.videoTextureWidth);
                questionUI.videoTextureTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, questionUI.videoTextureHeight);
            }

            questionUI.audioSource.clip = audioClip;
            if (audioClip)
            {
                enableReplayButton = true;
                questionUI.audioSource.Play();
            }

            questionUI.enableReplayButton = enableReplayButton;

            if (enableReplayButton)
            {
                questionUI.mainButton.interactable = false;
                questionUI.mainButtonImage.enabled = false;
                questionUI.mainButtonText.enabled = false;
                questionUI.replayButton.interactable = questionUI.enableReplayButton;
                questionUI.replayButtonImage.enabled = questionUI.enableReplayButton;
                questionUI.replayButtonIconImage.enabled = questionUI.enableReplayButton;
                questionUI.replayButtonText.enabled = questionUI.enableReplayButton;
                questionUI.replayButton.gameObject.SetActive(questionUI.enableReplayButton);
            }
        }

        public void BaseOnRequestDiaryWindow(string text, string diaryNumberText, float buttonTime, Sprite sprite, VideoClip videoClip, AudioClip audioClip, string promptText, Sprite promptSprite, VideoClip promptVideoClip, AudioClip promptAudioClip)
        {
            diaryUI.miniGameDiaryPromptUI.gameObject.SetActive(false);
            activeMiniGameWindow = diaryUI;
            diaryUI.answered = false;
            diaryUI.timer = buttonTime;
            diaryUI.diaryNumberText.text = diaryNumberText;
            if (string.IsNullOrEmpty(text))
            {
                diaryUI.miniGameDescriptionText.text = baseMiniGameManager.activeMiniGame.windowText;
            }
            else
            {
                diaryUI.miniGameDescriptionText.text = text;
            }
            diaryUI.texture.enabled = sprite != null;
            diaryUI.texture.sprite = sprite;
            diaryUI.texture.type = Image.Type.Filled;
            diaryUI.texture.preserveAspect = true;
            bool enableReplayButton = false;

            diaryUI.videoTexture.enabled = videoClip != null;
            diaryUI.videoPlayer.clip = videoClip;
            if (videoClip)
            {
                enableReplayButton = true;
                float videoAspect = (float)videoClip.width / (float)videoClip.height;

                float newWidth;
                float newHeight;

                if (diaryUI.videoPlayer.clip.width > diaryUI.videoPlayer.clip.height)
                {
                    newWidth = diaryUI.videoTextureWidth;
                    newHeight = newWidth / videoAspect;
                }
                else
                {
                    newHeight = diaryUI.videoTextureHeight;
                    newWidth = newHeight * videoAspect;
                }

                diaryUI.videoTextureTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
                diaryUI.videoTextureTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);

                if (renderTexture)
                {
                    if (renderTexture.IsCreated())
                    {
                        renderTexture.Release();
                    }
                }
                renderTexture = new RenderTexture((int)videoClip.width, (int)videoClip.height, 32);
                renderTexture.Create();

                diaryUI.videoTexture.texture = renderTexture;
                diaryUI.videoPlayer.targetTexture = renderTexture;

                diaryUI.videoPlayer.Play();
            }
            else
            {
                diaryUI.videoTextureTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, diaryUI.videoTextureWidth);
                diaryUI.videoTextureTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, diaryUI.videoTextureHeight);
            }

            diaryUI.audioSource.clip = audioClip;
            if (audioClip)
            {
                enableReplayButton = true;
                diaryUI.audioSource.Play();
            }

            diaryUI.enableReplayButton = enableReplayButton;

            if (enableReplayButton)
            {
                diaryUI.mainButton.interactable = false;
                diaryUI.mainButtonImage.enabled = false;
                diaryUI.mainButtonText.enabled = false;
                diaryUI.replayButton.interactable = diaryUI.enableReplayButton;
                diaryUI.replayButtonImage.enabled = diaryUI.enableReplayButton;
                diaryUI.replayButtonIconImage.enabled = diaryUI.enableReplayButton;
                diaryUI.replayButtonText.enabled = diaryUI.enableReplayButton;
            }

            if (!string.IsNullOrEmpty(promptText))
            {
                diaryUI.miniGameDiaryPromptUI.promptText.text = promptText;
            }
            else
            {
                diaryUI.miniGameDiaryPromptUI.SetDefaultString(diaryUI.diaryNumberText.text);
            }

            diaryUI.miniGameDiaryPromptUI.promptTexture.enabled = promptSprite != null;
            diaryUI.miniGameDiaryPromptUI.promptTexture.sprite = promptSprite;
            diaryUI.miniGameDiaryPromptUI.promptTexture.type = Image.Type.Filled;
            diaryUI.miniGameDiaryPromptUI.promptTexture.preserveAspect = true;

            diaryUI.miniGameDiaryPromptUI.promptVideoTexture.enabled = promptVideoClip != null;
            diaryUI.miniGameDiaryPromptUI.promptVideoPlayer.clip = promptVideoClip;

            if (promptVideoClip)
            {
                float videoAspect = (float)promptVideoClip.width / (float)promptVideoClip.height;

                float newWidth;
                float newHeight;

                if (diaryUI.miniGameDiaryPromptUI.promptVideoPlayer.clip.width > diaryUI.miniGameDiaryPromptUI.promptVideoPlayer.clip.height)
                {
                    newWidth = diaryUI.miniGameDiaryPromptUI.videoTextureWidth;
                    newHeight = newWidth / videoAspect;
                }
                else
                {
                    newHeight = diaryUI.miniGameDiaryPromptUI.videoTextureHeight;
                    newWidth = newHeight * videoAspect;
                }

                diaryUI.miniGameDiaryPromptUI.videoTextureTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
                diaryUI.miniGameDiaryPromptUI.videoTextureTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);

                if (promptRenderTexture)
                {
                    if (promptRenderTexture.IsCreated())
                    {
                        promptRenderTexture.Release();
                    }
                }
                promptRenderTexture = new RenderTexture((int)promptVideoClip.width, (int)promptVideoClip.height, 32);
                promptRenderTexture.Create();

                diaryUI.miniGameDiaryPromptUI.promptVideoTexture.texture = promptRenderTexture;
                diaryUI.miniGameDiaryPromptUI.promptVideoPlayer.targetTexture = promptRenderTexture;

                diaryUI.miniGameDiaryPromptUI.diaryPromptYesButton.interactable = false;
                diaryUI.miniGameDiaryPromptUI.diaryPromptYesButtonImage.enabled = false;
                diaryUI.miniGameDiaryPromptUI.diaryPromptYesButtonText.enabled = false;
                diaryUI.miniGameDiaryPromptUI.diaryPromptNoButton.interactable = false;
                diaryUI.miniGameDiaryPromptUI.diaryPromptNoButtonImage.enabled = false;
                diaryUI.miniGameDiaryPromptUI.diaryPromptNoButtonText.enabled = false;
            }
            else
            {
                diaryUI.miniGameDiaryPromptUI.videoTextureTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, diaryUI.miniGameDiaryPromptUI.videoTextureWidth);
                diaryUI.miniGameDiaryPromptUI.videoTextureTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, diaryUI.miniGameDiaryPromptUI.videoTextureHeight);
            }

            diaryUI.miniGameDiaryPromptUI.promptAudioSource.clip = promptAudioClip;

            if (promptAudioClip)
            {
                diaryUI.miniGameDiaryPromptUI.diaryPromptYesButton.interactable = false;
                diaryUI.miniGameDiaryPromptUI.diaryPromptYesButtonImage.enabled = false;
                diaryUI.miniGameDiaryPromptUI.diaryPromptYesButtonText.enabled = false;
                diaryUI.miniGameDiaryPromptUI.diaryPromptNoButton.interactable = false;
                diaryUI.miniGameDiaryPromptUI.diaryPromptNoButtonImage.enabled = false;
                diaryUI.miniGameDiaryPromptUI.diaryPromptNoButtonText.enabled = false;
            }
        }

        public virtual void OnStartConfirm()
        {
            startWindowOpen = false;

            if (startWindowFirstTime)
            {
                baseMiniGameManager.StartMiniGame();
                gameUIPanel.SetActive(true);
                EventBus.Publish(EventId.MiniGameEvents.MiniGameStarted);
            }
            else
            {
                EventBus.Publish(EventId.MiniGameEvents.MiniGameResumed);
            }
            if (activeMiniGameWindow)
            {
                activeMiniGameWindow.videoPlayer.Stop();
                activeMiniGameWindow.audioSource.Stop();
            }
            activeMiniGameWindow = null;
        }

        public void OnStartAbort()
        {
            abortWindowOpen = false;
            baseMiniGameManager.AbortActiveMiniGame();
            if (activeMiniGameWindow)
            {
                activeMiniGameWindow.videoPlayer.Stop();
                activeMiniGameWindow.audioSource.Stop();
            }
            EventBus.Publish(EventId.MiniGameEvents.MiniGameAborted);
        }

        public void OnAbortConfirm()
        {
            abortWindowOpen = false;
            baseMiniGameManager.AbortActiveMiniGame();
            if (activeMiniGameWindow)
            {
                activeMiniGameWindow.videoPlayer.Stop();
                activeMiniGameWindow.audioSource.Stop();
            }
            EventBus.Publish(EventId.MiniGameEvents.MiniGameAborted);
        }

        public virtual void OnAbortCancel()
        {
            abortWindowOpen = false;
            if (activeMiniGameWindow)
            {
                activeMiniGameWindow.videoPlayer.Stop();
                activeMiniGameWindow.audioSource.Stop();
            }
            activeMiniGameWindow = null;
            EventBus.Publish(EventId.MiniGameEvents.MiniGameResumed);
        }

        public virtual void CloseActivePanel()
        {
            if (activeMiniGameWindow)
            {
                activeMiniGameWindow.videoPlayer.Stop();
                activeMiniGameWindow.audioSource.Stop();
            }
            activeMiniGameWindow = null;
        }
        public virtual void OnQuestionWindowYes()
        {
            questionUI.answered = true;

            CloseActivePanel();

            EventBus.Publish(EventId.MiniGameEvents.QuestionWindowYes);
        }

        public virtual void OnQuestionWindowNo()
        {
            questionUI.answered = true;

            CloseActivePanel();

            EventBus.Publish(EventId.MiniGameEvents.QuestionWindowNo);
        }

        public void OnDiaryContinue()
        {
            diaryUI.miniGameDiaryPromptUI.gameObject.SetActive(true);
            diaryUI.videoPlayer.Stop();
            diaryUI.audioSource.Stop();
            if (diaryUI.miniGameDiaryPromptUI.promptVideoPlayer.clip)
            {
                diaryUI.miniGameDiaryPromptUI.promptVideoPlayer.Play();
            }
            if (diaryUI.miniGameDiaryPromptUI.promptAudioSource.clip)
            {
                diaryUI.miniGameDiaryPromptUI.promptAudioSource.Play();
            }
        }

        public virtual void OnDiaryPromptYes()
        {
            diaryUI.miniGameDiaryPromptUI.promptVideoPlayer.Stop();
            diaryUI.miniGameDiaryPromptUI.promptAudioSource.Stop();
            diaryUI.answered = true;
            EventBus.Publish(EventId.MiniGameEvents.DiaryContinue);
        }

        public void OnDiaryPromptNo()
        {
            diaryUI.miniGameDiaryPromptUI.promptVideoPlayer.Stop();
            diaryUI.miniGameDiaryPromptUI.promptAudioSource.Stop();
            diaryUI.miniGameDiaryPromptUI.gameObject.SetActive(false);
        }

        public void OnScoreConfirm()
        {
            abortWindowOpen = false;

            int gameMode = 0;

            if (PlayerPrefs.HasKey(EventId.GameMode))
            {
                gameMode = PlayerPrefs.GetInt(EventId.GameMode);
            }

            if (gameMode == 0)
            {
                int progress = 0;

                if (PlayerPrefs.HasKey(EventId.GameModeProgress))
                {
                    progress = PlayerPrefs.GetInt(EventId.GameModeProgress);
                }

                int nextMiniGame = (int)baseMiniGameManager.miniGameConfig.type + 1;

                if (progress < nextMiniGame)
                {
                    PlayerPrefs.SetInt(EventId.GameModeProgress, nextMiniGame);
                }
            }

            baseMiniGameManager.ExitActiveMiniGame();
            if (activeMiniGameWindow)
            {
                activeMiniGameWindow.videoPlayer.Stop();
                activeMiniGameWindow.audioSource.Stop();
            }
            EventBus.Publish(EventId.MiniGameEvents.MiniGameEnded);
        }

        public void OnReplayInstruction()
        {
            audioSource.Stop();
            if (activeMiniGameWindow)
            {
                if (activeMiniGameWindow == diaryUI && diaryUI.miniGameDiaryPromptUI.gameObject.activeSelf)
                {
                    bool replayed = false;
                    if (diaryUI.miniGameDiaryPromptUI.promptVideoPlayer.clip)
                    {
                        replayed = true;
                        diaryUI.miniGameDiaryPromptUI.promptVideoPlayer.Stop();
                        diaryUI.miniGameDiaryPromptUI.promptVideoPlayer.Play();
                    }
                    if (diaryUI.miniGameDiaryPromptUI.promptAudioSource.clip)
                    {
                        replayed = true;
                        diaryUI.miniGameDiaryPromptUI.promptAudioSource.Stop();
                        diaryUI.miniGameDiaryPromptUI.promptAudioSource.Play();
                    }
                    if (!replayed)
                    {
                        audioSource.Play();
                    }
                }
                else
                {
                    bool replayed = false;
                    if (activeMiniGameWindow.videoPlayer.clip)
                    {
                        replayed = true;
                        activeMiniGameWindow.videoPlayer.Stop();
                        activeMiniGameWindow.videoPlayer.Play();
                    }
                    if (activeMiniGameWindow.audioSource.clip)
                    {
                        replayed = true;
                        activeMiniGameWindow.audioSource.Stop();
                        activeMiniGameWindow.audioSource.Play();
                    }
                    if (!replayed)
                    {
                        audioSource.Play();
                    }
                }
            }
        }

        public virtual void OnScoreCancel()
        {
            abortWindowOpen = false;
            if (activeMiniGameWindow)
            {
                activeMiniGameWindow.videoPlayer.Stop();
                activeMiniGameWindow.audioSource.Stop();
            }
            activeMiniGameWindow = null;
            EventBus.Publish(EventId.MiniGameEvents.MiniGameResumed);
        }

        protected virtual void OnDisable()
        {
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");
        }

        private void OnDestroy()
        {
            if (renderTexture)
            {
                if (renderTexture.IsCreated())
                {
                    renderTexture.Release();
                }
            }

            if (promptRenderTexture)
            {
                if (promptRenderTexture.IsCreated())
                {
                    promptRenderTexture.Release();
                }
            }
        }
    }
}