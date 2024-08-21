using TMPro;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine;
using System.Collections;

namespace MiniGame.DairyTour
{
    public class FillingPlant : MonoBehaviour
    {
        public RawImage videoTexture;
        public VideoPlayer videoPlayer;
        public Slider videoSlider;
        public Button startStopButton;
        public TextMeshProUGUI startStopButtonText;
        public string startButtonText = "Starten";
        public string stopButtonText = "Stoppen";

        private RenderTexture renderTexture;

        private int highestSliderValue = 0;
        private bool started = false;
        private bool setSlider = false;
        private bool seekCompleted = true;
        private bool sliderValueChanged = false;

        private bool admin = false;

        private void Awake()
        {
            Debug.Assert(videoTexture, "FillingPlant is missing a reference to a RawImage");
            Debug.Assert(videoPlayer, "FillingPlant is missing a reference to a VideoPlayer");
            Debug.Assert(videoSlider, "FillingPlant is missing a reference to a Slider");
            Debug.Assert(startStopButtonText, "FillingPlant is missing a reference to a TextMeshProUGUI");

            EventBus.SaveRegisterCallback(this, EventId.QuitAdminMode, "OnQuitAdminMode");

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

            startStopButtonText.text = startButtonText;
            startStopButton.gameObject.SetActive(false);
            videoSlider.gameObject.SetActive(false);
            videoPlayer.prepareCompleted += PrepareCompleted;
            videoPlayer.seekCompleted += SeekCompleted;
            videoPlayer.loopPointReached += LoopPointReached;
            videoPlayer.sendFrameReadyEvents = true;
            videoPlayer.frameReady += FrameReady;
            videoPlayer.Prepare();

            if (PlayerPrefs.HasKey(EventId.AdminMode))
            {
                admin = PlayerPrefs.GetInt(EventId.AdminMode) != 0;
            }
        }

        public void OnStartStopButtonToggle()
        {
            if (started)
            {
                started = false;
                startStopButtonText.text = startButtonText;
                videoPlayer.Pause();
            }
            else
            {
                started = true;
                startStopButtonText.text = stopButtonText;
                videoPlayer.Play();
            }
        }

        public void OnSliderValueChanged()
        {
            if (setSlider)
            {
                setSlider = false;
            }
            else if (seekCompleted)
            {
                if (Mathf.RoundToInt(videoSlider.value) > highestSliderValue && !admin)
                {
                    videoSlider.value = highestSliderValue;
                }

                if (videoPlayer.frame != Mathf.RoundToInt(videoSlider.value))
                {
                    seekCompleted = false;
                    videoPlayer.frame = Mathf.RoundToInt(videoSlider.value);

                    if (started)
                    {
                        started = false;
                        startStopButtonText.text = startButtonText;
                        videoPlayer.Pause();
                    }
                }
            }
            else
            {
                sliderValueChanged = true;
            }
        }

        public void OnQuitAdminMode()
        {
            admin = false;
        }

        private void PrepareCompleted(VideoPlayer source)
        {
            videoSlider.maxValue = videoPlayer.frameCount - 1;
            startStopButton.gameObject.SetActive(true);
            videoSlider.gameObject.SetActive(true);
            videoPlayer.Play();
            videoPlayer.Pause();
            seekCompleted = false;
            videoPlayer.frame = 0;
            highestSliderValue = 0;
            setSlider = true;
            videoSlider.value = highestSliderValue;
            seekCompleted = true;
        }

        private void SeekCompleted(VideoPlayer source)
        {
            StartCoroutine(SetSeekCompleted());
        }

        IEnumerator SetSeekCompleted()
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

        private void LoopPointReached(VideoPlayer source)
        {
            string sendName = gameObject.name;
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.SubMiniGameEnded, sendName));
        }

        private void FrameReady(VideoPlayer source, long frameIdx)
        {
            if (seekCompleted)
            {
                if (Mathf.RoundToInt(frameIdx) > highestSliderValue)
                {
                    highestSliderValue = Mathf.RoundToInt(frameIdx);
                }
                setSlider = true;
                videoSlider.value = Mathf.RoundToInt(frameIdx);
            }
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, EventId.QuitAdminMode, "OnQuitAdminMode");
            videoPlayer.prepareCompleted -= PrepareCompleted;
            videoPlayer.loopPointReached -= LoopPointReached;
            videoPlayer.frameReady -= FrameReady;

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
