using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace MiniGame.DairyTour
{
    public class StorageLife : MonoBehaviour
    {
        public Sprite[] sprites;
        public AudioClip[] audioClips;
        public AudioSource audioSource;
        public Slider slider;
        public TextMeshProUGUI finishButtonText, replayButtonText;
        public Button finishButton, replayButton;
        public Image finishButtonImage, replayButtonImage, replayButtonIconImage, backgroundImage;

        private float waitTimer = 2.0f;
        private float waitTime = 0.0f;

        private int oldSliderValue = 0;

        private List<bool> sliderValuesSeen;

        private bool endAudioPlayed = false;

        private int step = 0;

        private bool admin = false;

        private void Awake()
        {
            sliderValuesSeen = new List<bool>();
            for (int i = Mathf.RoundToInt(slider.minValue); i <= Mathf.RoundToInt(slider.maxValue); i++)
            {
                sliderValuesSeen.Add(i == Mathf.RoundToInt(slider.value));
            }
            Debug.Assert(sprites.Length == Mathf.RoundToInt(slider.maxValue + 1.0f), "StorageLife needs the same amount of Sprites and possible Slider Values");
            Debug.Assert(audioClips.Length == 2, "StorageLife needs exactly 2 AudioClips");
            Debug.Assert(audioSource, "StorageLife is missing a reference to an AudioSource");
            Debug.Assert(slider, "StorageLife is missing a reference to a Slider");
            if (!finishButtonText)
            {
                finishButtonText = finishButton.GetComponentInChildren<TextMeshProUGUI>();
            }
            Debug.Assert(finishButtonText, "StorageLife is missing a reference to a TMPro");
            if (!replayButtonText)
            {
                replayButtonText = replayButton.GetComponentInChildren<TextMeshProUGUI>();
            }
            Debug.Assert(replayButtonText, "StorageLife is missing a reference to a TMPro");
            Debug.Assert(finishButton, "StorageLife is missing a reference to the finish Button");
            Debug.Assert(replayButton, "StorageLife is missing a reference to the replay Button");
            Debug.Assert(finishButtonImage, "StorageLife is missing a reference to an Image");
            Debug.Assert(replayButtonImage, "StorageLife is missing a reference to an Image");
            Debug.Assert(replayButtonIconImage, "StorageLife is missing a reference to an Image");
            Debug.Assert(backgroundImage, "StorageLife is missing a reference to an Image");

            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.RequestAbortWindow, "OnRequestAbortWindow");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");
            EventBus.SaveRegisterCallback(this, EventId.QuitAdminMode, "OnQuitAdminMode");

            if (PlayerPrefs.HasKey(EventId.AdminMode))
            {
                admin = PlayerPrefs.GetInt(EventId.AdminMode) != 0;
            }

            finishButton.interactable = false;
            finishButtonImage.enabled = false;
            finishButtonText.enabled = false;
            audioSource.clip = audioClips[0];
            audioSource.Play();
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
            slider.interactable = false;
            replayButton.interactable = false;
            replayButtonImage.enabled = false;
            replayButtonIconImage.enabled = false;
            replayButtonText.enabled = false;
            backgroundImage.sprite = sprites[0];
        }

        private void Update()
        {
            if (step == 0 && !audioSource.isPlaying)
            {
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, false));
                slider.interactable = true;
                replayButton.interactable = true;
                replayButtonImage.enabled = true;
                replayButtonIconImage.enabled = true;
                replayButtonText.enabled = true;
                step++;
            }

            if (step > 0 && !slider.interactable)
            {
                if (waitTime < waitTimer && !admin)
                {
                    waitTime += Time.deltaTime;
                }
                else if (!audioSource.isPlaying)
                {
                    replayButton.interactable = true;
                    replayButtonImage.enabled = true;
                    replayButtonIconImage.enabled = true;
                    replayButtonText.enabled = true;
                    StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, false));
                    waitTime = 0.0f;
                    slider.interactable = true;
                }
            }

            if (step == 2 && !audioSource.isPlaying && !finishButton.interactable)
            {
                replayButton.interactable = true;
                replayButtonImage.enabled = true;
                replayButtonIconImage.enabled = true;
                replayButtonText.enabled = true;
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, false));
                finishButton.interactable = true;
                finishButtonImage.enabled = true;
                finishButtonText.enabled = true;
                slider.interactable = true;
            }
        }

        public void OnSliderValueChanged()
        {
            if (Mathf.RoundToInt(slider.value) != oldSliderValue)
            {
                if (Mathf.RoundToInt(slider.value) - oldSliderValue > 0)
                {
                    slider.value = oldSliderValue + 1;
                }
                else
                {
                    slider.value = oldSliderValue - 1;
                }

                oldSliderValue = Mathf.RoundToInt(slider.value);

                sliderValuesSeen[Mathf.RoundToInt(slider.value)] = true;

                backgroundImage.sprite = sprites[Mathf.RoundToInt(slider.value)];

                slider.interactable = false;

                for (int i = 0; i < sliderValuesSeen.Count; i++)
                {
                    if (!sliderValuesSeen[i])
                    {
                        return;
                    }
                }

                if (!endAudioPlayed)
                {
                    endAudioPlayed = true;
                    audioSource.clip = audioClips[1];
                    audioSource.Play();
                    replayButton.interactable = false;
                    replayButtonImage.enabled = false;
                    replayButtonIconImage.enabled = false;
                    replayButtonText.enabled = false;
                    StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
                    step++;
                }
            }
        }

        public void OnFinishButtonClicked()
        {
            string sendName = gameObject.name;
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.SubMiniGameEnded, sendName));
        }

        public void OnRequestAbortWindow()
        {
            string sendName = gameObject.name;
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.SubMiniGameAborted, sendName));
        }

        public void OnReplayInstruction()
        {
            if (audioSource.clip)
            {
                audioSource.Stop();
                audioSource.Play();
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));

                slider.interactable = false;
                replayButton.interactable = false;
                replayButtonImage.enabled = false;
                replayButtonIconImage.enabled = false;
                replayButtonText.enabled = false;
                finishButton.interactable = false;
                finishButtonImage.enabled = false;
                finishButtonText.enabled = false;

                if (!endAudioPlayed)
                {
                    step = 0;
                }
            }
        }

        public void OnSkipAudioButtonPressed()
        {
            audioSource.Stop();
        }

        public void OnQuitAdminMode()
        {
            admin = false;
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.RequestAbortWindow, "OnRequestAbortWindow");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");
            EventBus.SaveDeregisterCallback(this, EventId.QuitAdminMode, "OnQuitAdminMode");
        }
    }
}
