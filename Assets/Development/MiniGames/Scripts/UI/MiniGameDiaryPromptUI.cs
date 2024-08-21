using UnityEngine.Video;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MiniGame.UI
{
    public class MiniGameDiaryPromptUI : MonoBehaviour
    {
        public MiniGameDiaryUI miniGameDiaryUI;
        public TextMeshProUGUI promptText, diaryPromptYesButtonText, diaryPromptNoButtonText;
        public Button diaryPromptYesButton, diaryPromptNoButton;
        public Image diaryPromptYesButtonImage, diaryPromptNoButtonImage;
        public Image promptTexture;
        public RawImage promptVideoTexture;
        public VideoPlayer promptVideoPlayer;
        public AudioSource promptAudioSource;

        public RectTransform videoTextureTransform;
        public float videoTextureWidth;
        public float videoTextureHeight;

        private string defaultText = "Hast du die Aufgabe {0} im Lerntagebuch fertig bearbeitet?";

        private const float maxRadius = 100.0f;

        private bool admin = false;

        public void SetDefaultString(string diaryNumber)
        {
            promptText.text = string.Format(defaultText, diaryNumber);
        }

        private void Awake()
        {
            EventBus.SaveRegisterCallback(this, EventId.QuitAdminMode, "OnQuitAdminMode");

            videoTextureTransform = promptVideoTexture.GetComponent<RectTransform>();
            videoTextureWidth = videoTextureTransform.rect.width;
            videoTextureHeight = videoTextureTransform.rect.height;

            if (PlayerPrefs.HasKey(EventId.AdminMode))
            {
                admin = PlayerPrefs.GetInt(EventId.AdminMode) != 0;
            }
        }

        private void Update()
        {
            if (admin)
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
                    diaryPromptYesButton.interactable = true;
                    diaryPromptYesButtonImage.enabled = true;
                    diaryPromptYesButtonText.enabled = true;
                    diaryPromptNoButton.interactable = true;
                    diaryPromptNoButtonImage.enabled = true;
                    diaryPromptNoButtonText.enabled = true;
                }
            }
            else
            {
                if (enabled && !diaryPromptYesButton.interactable && !diaryPromptNoButton.interactable && !promptVideoPlayer.isPlaying && !promptAudioSource.isPlaying)
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
                        diaryPromptYesButton.interactable = true;
                        diaryPromptYesButtonImage.enabled = true;
                        diaryPromptYesButtonText.enabled = true;
                        diaryPromptNoButton.interactable = true;
                        diaryPromptNoButtonImage.enabled = true;
                        diaryPromptNoButtonText.enabled = true;
                    }
                }
                else if (promptVideoPlayer.isPlaying || promptAudioSource.isPlaying)
                {
                    diaryPromptYesButton.interactable = false;
                    diaryPromptYesButtonImage.enabled = false;
                    diaryPromptYesButtonText.enabled = false;
                    diaryPromptNoButton.interactable = false;
                    diaryPromptNoButtonImage.enabled = false;
                    diaryPromptNoButtonText.enabled = false;
                }
            }
        }

        private void OnEnable()
        {
            if (miniGameDiaryUI.baseMiniGameUIController is MiniGameUIController)
            {
                diaryPromptYesButton.onClick.AddListener((miniGameDiaryUI.baseMiniGameUIController as MiniGameUIController).OnDiaryPromptYes);
            }
            else if (miniGameDiaryUI.baseMiniGameUIController is VirtualTourMiniGameUIController)
            {
                diaryPromptYesButton.onClick.AddListener((miniGameDiaryUI.baseMiniGameUIController as VirtualTourMiniGameUIController).OnDiaryPromptYes);
            }
            else
            {
                diaryPromptYesButton.onClick.AddListener(miniGameDiaryUI.baseMiniGameUIController.OnDiaryPromptYes);
            }
            diaryPromptNoButton.onClick.AddListener(miniGameDiaryUI.baseMiniGameUIController.OnDiaryPromptNo);
        }

        private void OnDisable()
        {
            promptVideoPlayer.Stop();
            promptAudioSource.Stop();
            if (miniGameDiaryUI.baseMiniGameUIController is MiniGameUIController)
            {
                diaryPromptYesButton.onClick.RemoveListener((miniGameDiaryUI.baseMiniGameUIController as MiniGameUIController).OnDiaryPromptYes);
            }
            else if (miniGameDiaryUI.baseMiniGameUIController is VirtualTourMiniGameUIController)
            {
                diaryPromptYesButton.onClick.RemoveListener((miniGameDiaryUI.baseMiniGameUIController as VirtualTourMiniGameUIController).OnDiaryPromptYes);
            }
            else
            {
                diaryPromptYesButton.onClick.RemoveListener(miniGameDiaryUI.baseMiniGameUIController.OnDiaryPromptYes);
            }
            diaryPromptNoButton.onClick.RemoveListener(miniGameDiaryUI.baseMiniGameUIController.OnDiaryPromptNo);
        }

        public void OnQuitAdminMode()
        {
            admin = false;
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, EventId.QuitAdminMode, "OnQuitAdminMode");
        }
    }
}