using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

namespace MiniGame.FarmTour
{
    public class VirtualTourInfo : MonoBehaviour
    {
        public TextMeshProUGUI mainButtonText, exitButtonText, replayButtonText;
        public Button mainButton, exitButton, replayButton;
        public Image mainButtonImage, exitButtonImage, replayButtonImage, replayButtonIconImage;
        public AudioSource audioSource;
        public RawImage videoTexture;
        public VideoPlayer videoPlayer;

        private RenderTexture renderTexture;

        private bool enableReplayButton = false;
        private bool admin = false;

        private void Awake()
        {
            if (!mainButtonText)
            {
                mainButtonText = mainButton.GetComponentInChildren<TextMeshProUGUI>();
            }
            Debug.Assert(mainButtonText, "VirtualTourInfo is missing a reference to a TMPro");
            if (!replayButtonText)
            {
                replayButtonText = replayButton.GetComponentInChildren<TextMeshProUGUI>();
            }
            Debug.Assert(replayButtonText, "VirtualTourInfo is missing a reference to a TMPro");
            Debug.Assert(mainButton, "VirtualTourInfo is missing a reference to the main Button");
            Debug.Assert(replayButton, "VirtualTourInfo is missing a reference to the replay Button");
            Debug.Assert(mainButtonImage, "VirtualTourInfo is missing a reference to an Image");
            Debug.Assert(replayButtonImage, "VirtualTourInfo is missing a reference to an Image");
            Debug.Assert(replayButtonIconImage, "VirtualTourInfo is missing a reference to an Image");
            Debug.Assert(audioSource, "VirtualTourInfo is missing a reference to an AudioSource");
            Debug.Assert(videoTexture, "VirtualTourInfo is missing a reference to a RawImage");
            Debug.Assert(videoPlayer, "VirtualTourInfo is missing a reference to an VideoPlayer");

            EventBus.SaveRegisterCallback(this, EventId.QuitAdminMode, "OnQuitAdminMode");

            if (audioSource.clip)
            {
                enableReplayButton = true;
                audioSource.Play();
            }

            if (videoPlayer.clip)
            {
                if (renderTexture)
                {
                    if (renderTexture.IsCreated())
                    {
                        renderTexture.Release();
                    }
                }
                renderTexture = new RenderTexture((int)videoPlayer.clip.width, (int)videoPlayer.clip.height, 32);
                renderTexture.Create();

                videoTexture.texture = renderTexture;
                videoPlayer.targetTexture = renderTexture;
                enableReplayButton = true;
                videoPlayer.Play();
            }

            if (PlayerPrefs.HasKey(EventId.AdminMode))
            {
                admin = PlayerPrefs.GetInt(EventId.AdminMode) != 0;
            }

            if (!admin)
            {
                mainButton.interactable = false;
                mainButtonImage.enabled = false;
                mainButtonText.enabled = false;
                if (exitButton)
                {
                    exitButton.interactable = false;
                    exitButtonImage.enabled = false;
                    exitButtonText.enabled = false;
                }
            }
            replayButton.interactable = enableReplayButton;
            replayButtonImage.enabled = enableReplayButton;
            replayButtonIconImage.enabled = enableReplayButton;
            replayButtonText.enabled = enableReplayButton;
            replayButton.gameObject.SetActive(enableReplayButton);
        }

        private void Update()
        {
            if (!audioSource.isPlaying && !videoPlayer.isPlaying)
            {
                mainButton.interactable = true;
                mainButtonImage.enabled = true;
                mainButtonText.enabled = true;
                if (exitButton)
                {
                    exitButton.interactable = true;
                    exitButtonImage.enabled = true;
                    exitButtonText.enabled = true;
                }
            }
        }

        public void OnReplayInstruction()
        {
            if (videoPlayer.clip)
            {
                videoPlayer.Stop();
                videoPlayer.Play();
            }
            if (audioSource.clip)
            {
                audioSource.Stop();
                audioSource.Play();
            }
        }

        public void EndSubMiniGame()
        {
            string sendName = gameObject.name;
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.SubMiniGameEnded, sendName));
        }

        public void EndVirtualTour()
        {
            string sendName = gameObject.name;
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.SubMiniGameEnded, sendName));
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.VirtualTourStep, 5));
            EventBus.Publish(EventId.MiniGameEvents.NextStep);
        }

        public void OnQuitAdminMode()
        {
            admin = false;
            if (audioSource.isPlaying || videoPlayer.isPlaying)
            {
                mainButton.interactable = false;
                mainButtonImage.enabled = false;
                mainButtonText.enabled = false;
                if (exitButton)
                {
                    exitButton.interactable = false;
                    exitButtonImage.enabled = false;
                    exitButtonText.enabled = false;
                }
            }
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, EventId.QuitAdminMode, "OnQuitAdminMode");

            if (renderTexture)
            {
                if (renderTexture.IsCreated())
                {
                    renderTexture.Release();
                }
            }
        }
    }
}
