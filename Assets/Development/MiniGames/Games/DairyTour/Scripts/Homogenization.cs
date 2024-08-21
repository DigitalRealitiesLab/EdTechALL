using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

namespace MiniGame.DairyTour
{
    public class Homogenization : MonoBehaviour
    {
        public Sprite[] sprites;
        public VideoClip[] videoClips;
        public AudioClip[] audioClips;
        public RawImage videoTexture;
        public VideoPlayer videoPlayer;
        public AudioSource audioSource;
        public Slider videoSlider;
        public Button replayButton;

        private RenderTexture renderTexture;

        private int step = 0;
        private int highestSliderValue = 0;

        private bool seekCompleted = true;
        private bool sliderValueChanged = false;

        private void Awake()
        {
            Debug.Assert(sprites.Length == 1, "Homogenization needs exactly 1 Sprite");
            Debug.Assert(videoClips.Length == 4, "Homogenization needs exactly 4 VideoClips");
            Debug.Assert(audioClips.Length == 5, "Homogenization needs exactly 5 AudioClips");
            Debug.Assert(videoTexture, "Homogenization is missing a reference to a RawImage");
            Debug.Assert(videoPlayer, "Homogenization is missing a reference to a VideoPlayer");
            Debug.Assert(audioSource, "Homogenization is missing a reference to an AudioSource");
            Debug.Assert(videoSlider, "Homogenization is missing a reference to a Slider");

            EventBus.SaveRegisterCallback(this, FullScreen.EventId.UIEvents.FullScreenPromptPanelClosed, "OnFullScreenPromptPanelClosed");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");

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

            videoPlayer.prepareCompleted += PrepareCompleted;
            videoPlayer.seekCompleted += SeekCompleted;
            videoPlayer.gameObject.SetActive(false);
            videoSlider.gameObject.SetActive(false);
            replayButton.gameObject.SetActive(false);
            FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[0], audioClips[0]);
        }

        private void Update()
        {
            if (step == 2 && !audioSource.isPlaying && videoPlayer.isPlaying && seekCompleted && !videoSlider.gameObject.activeSelf)
            {
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, false));
                highestSliderValue = 0;
                videoSlider.value = 0.0f;
                videoSlider.gameObject.SetActive(true);
                replayButton.gameObject.SetActive(true);
                seekCompleted = false;
                videoPlayer.frame = 0;
            }
        }

        public void OnFullScreenPromptPanelClosed()
        {
            switch (step)
            {
                case 0:
                    FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[1], audioClips[1]);
                    break;
                case 1:
                    videoPlayer.gameObject.SetActive(true);
                    videoPlayer.clip = videoClips[2];
                    videoPlayer.Prepare();
                    audioSource.clip = audioClips[2];
                    audioSource.Play();
                    StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
                    break;
                case 3:
                    string sendName = gameObject.name;
                    StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.SubMiniGameEnded, sendName));
                    break;
            }
            step++;
        }

        public void OnSliderValueChanged()
        {
            if (seekCompleted)
            {
                if (Mathf.RoundToInt(videoSlider.value) < highestSliderValue)
                {
                    videoSlider.value = highestSliderValue;
                }
                else if (Mathf.RoundToInt(videoSlider.value) > highestSliderValue)
                {
                    highestSliderValue = Mathf.RoundToInt(videoSlider.value);

                    seekCompleted = false;
                    videoPlayer.frame = Mathf.RoundToInt(highestSliderValue);

                    if (highestSliderValue == videoSlider.maxValue)
                    {
                        step++;
                        FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[3], audioClips[3]);
                    }
                }
            }
            else
            {
                sliderValueChanged = true;
            }
        }

        public void OnReplayInstruction()
        {
            if (step == 2)
            {
                audioSource.Stop();
                audioSource.Play();
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));

                videoSlider.gameObject.SetActive(false);
                replayButton.gameObject.SetActive(false);
            }
        }

        public void OnSkipAudioButtonPressed()
        {
            audioSource.Stop();
        }

        private void PrepareCompleted(VideoPlayer source)
        {
            videoSlider.maxValue = videoPlayer.frameCount - 1;
            videoPlayer.Play();
        }

        private void SeekCompleted(VideoPlayer source)
        {
            StartCoroutine(SetSeekCompleted());
        }

        private IEnumerator SetSeekCompleted()
        {
            // wait until the set frame of the videoPlayer is visible to prevent the image from being stuck
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            seekCompleted = true;

            if (sliderValueChanged)
            {
                OnSliderValueChanged();
            }
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, FullScreen.EventId.UIEvents.FullScreenPromptPanelClosed, "OnFullScreenPromptPanelClosed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");

            videoPlayer.prepareCompleted -= PrepareCompleted;

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
