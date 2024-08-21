using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine;

namespace MiniGame.DairyTour
{
    public class Centrifugation : MonoBehaviour
    {
        public Sprite[] sprites;
        public VideoClip[] videoClips;
        public AudioClip[] audioClips;
        public AudioSource audioSource;
        public Image backgroundImage;
        public GameObject rotationInteraction;
        public Button replayButton;

        private int step = 0;

        private void Awake()
        {
            Debug.Assert(sprites.Length == 2, "Centrifugation needs exactly 2 Sprites");
            Debug.Assert(videoClips.Length == 6, "Centrifugation needs exactly 6 VideoClips");
            Debug.Assert(audioClips.Length == 8, "Centrifugation needs exactly 8 AudioClips");
            Debug.Assert(audioSource, "Centrifugation is missing a reference to an AudioSource");
            Debug.Assert(backgroundImage, "Centrifugation is missing a reference to a Image");

            EventBus.SaveRegisterCallback(this, FullScreen.EventId.UIEvents.FullScreenPromptPanelClosed, "OnFullScreenPromptPanelClosed");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");

            backgroundImage.sprite = sprites[0];
            backgroundImage.gameObject.SetActive(false);
            audioSource.clip = audioClips[2];
            FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[0], audioClips[0]);
        }

        private void Update()
        {
            if (step == 2 && !audioSource.isPlaying && !rotationInteraction.activeSelf)
            {
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, false));
                rotationInteraction.SetActive(true);
                replayButton.gameObject.SetActive(true);
            }
        }

        public void StartVideo()
        {
            step++;
            backgroundImage.gameObject.SetActive(false);
            rotationInteraction.SetActive(false);
            replayButton.gameObject.SetActive(false);
            FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[2], audioClips[3]);
        }

        public void OnFullScreenPromptPanelClosed()
        {
            switch (step)
            {
                case 0:
                    FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[1], audioClips[1]);
                    break;
                case 1:
                    backgroundImage.gameObject.SetActive(true);
                    rotationInteraction.SetActive(false);
                    replayButton.gameObject.SetActive(false);
                    audioSource.Play();
                    StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
                    break;
                case 3:
                    FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[3], audioClips[4]);
                    break;
                case 4:
                    FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[4], audioClips[5]);
                    break;
                case 5:
                    FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[5], audioClips[6]);
                    break;
                case 6:
                    string sendName = gameObject.name;
                    StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.SubMiniGameEnded, sendName));
                    break;

            }
            step++;
        }

        public void OnReplayInstruction()
        {
            if (step == 2)
            {
                audioSource.Stop();
                audioSource.Play();
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));

                rotationInteraction.SetActive(false);
                replayButton.gameObject.SetActive(false);

                step = 2;
            }
        }

        public void OnSkipAudioButtonPressed()
        {
            audioSource.Stop();
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, FullScreen.EventId.UIEvents.FullScreenPromptPanelClosed, "OnFullScreenPromptPanelClosed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");
        }
    }
}
