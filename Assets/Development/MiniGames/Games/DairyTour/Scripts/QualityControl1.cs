using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections.Generic;

namespace MiniGame.DairyTour
{
    public class QualityControl1 : MonoBehaviour
    {
        public Image magnifyingGlassImage;
        public Texture[] magnifyingGlassTextures;
        public Button[] OKButtons;
        public Button[] notOKButtons;
        public VideoClip[] videoClips;
        public AudioClip[] audioClips;
        public AudioSource audioSource;

        private int chosenTexture;
        private int playNext = -1;
        private bool inactive = false;
        private bool finished = false;
        private List<Button> activeButtons = new List<Button>();

        private void Awake()
        {
            Debug.Assert(magnifyingGlassImage, "QualityControl1 is missing a reference to an Image");
            Debug.Assert(magnifyingGlassTextures.Length == 12, "QualityControl1 needs 12 Textures");
            Debug.Assert(OKButtons.Length == 4, "QualityControl1 needs 4 OK Buttons");
            Debug.Assert(notOKButtons.Length == 4, "QualityControl1 needs 4 Not OK Buttons");
            Debug.Assert(videoClips.Length == 1, "QualityControl1 needs exactly 1 VideoClip");
            Debug.Assert(audioClips.Length == 7, "QualityControl1 needs exactly 7 AudioClips");
            Debug.Assert(audioSource, "QualityControl1 is missing a reference to an AudioSource");

            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");

            chosenTexture = Random.Range(0, magnifyingGlassTextures.Length);
            magnifyingGlassImage.material.mainTexture = magnifyingGlassTextures[chosenTexture];
            FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[0], audioClips[0]);

            activeButtons.AddRange(OKButtons);
            activeButtons.AddRange(notOKButtons);
        }

        private void Update()
        {
            if (playNext >= 0 && !audioSource.isPlaying)
            {
                audioSource.clip = audioClips[playNext];
                audioSource.Play();
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
                if (finished && playNext != 6)
                {
                    playNext = 6;
                }
                else
                {
                    playNext = -1;
                }
            }
            else if (playNext < 0 && !audioSource.isPlaying && inactive)
            {
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, false));
                inactive = false;
                foreach (Button button in activeButtons)
                {
                    button.gameObject.SetActive(true);
                }
            }
            else if (playNext < 0 && !audioSource.isPlaying && finished)
            {
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, false));
                string sendName = gameObject.name;
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.SubMiniGameEnded, sendName));
            }
        }

        public void OnOKButtonpClicked(int number)
        {
            foreach (Button button in activeButtons)
            {
                button.gameObject.SetActive(false);
            }

            if (number == Mathf.FloorToInt(chosenTexture / 3.0f))
            {
                audioSource.clip = audioClips[2];
                audioSource.Play();
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
                inactive = true;
            }
            else
            {
                if ((number == 0 && (chosenTexture == 3 || chosenTexture == 6 || chosenTexture == 9))
                    || (number == 1 && (chosenTexture == 0 || chosenTexture == 7 || chosenTexture == 10))
                    || (number == 2 && (chosenTexture == 1 || chosenTexture == 4 || chosenTexture == 11))
                    || (number == 3 && (chosenTexture == 2 || chosenTexture == 5 || chosenTexture == 8)))
                {
                    audioSource.clip = audioClips[2];
                    audioSource.Play();
                    StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
                    inactive = true;
                }
                else
                {
                    audioSource.clip = audioClips[1];
                    audioSource.Play();
                    StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
                    playNext = 3;
                    activeButtons.Remove(OKButtons[number]);
                    activeButtons.Remove(notOKButtons[number]);
                    inactive = true;
                    CheckButtons();
                }
            }
        }

        public void OnNotOKButtonpClicked(int number)
        {
            foreach (Button button in activeButtons)
            {
                button.gameObject.SetActive(false);
            }

            if (number == Mathf.FloorToInt(chosenTexture / 3.0f))
            {
                audioSource.clip = audioClips[1];
                audioSource.Play();
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
                playNext = 4;
                activeButtons.Remove(OKButtons[number]);
                activeButtons.Remove(notOKButtons[number]);
                inactive = true;
                CheckButtons();
            }
            else
            {
                if ((number == 0 && (chosenTexture == 3 || chosenTexture == 6 || chosenTexture == 9))
                    || (number == 1 && (chosenTexture == 0 || chosenTexture == 7 || chosenTexture == 10))
                    || (number == 2 && (chosenTexture == 1 || chosenTexture == 4 || chosenTexture == 11))
                    || (number == 3 && (chosenTexture == 2 || chosenTexture == 5 || chosenTexture == 8)))
                {
                    audioSource.clip = audioClips[1];
                    audioSource.Play();
                    StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
                    playNext = 5;
                    activeButtons.Remove(OKButtons[number]);
                    activeButtons.Remove(notOKButtons[number]);
                    inactive = true;
                    CheckButtons();
                }
                else
                {
                    audioSource.clip = audioClips[2];
                    audioSource.Play();
                    StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
                    inactive = true;
                }
            }
        }

        public void OnSkipAudioButtonPressed()
        {
            audioSource.Stop();
        }

        private void CheckButtons()
        {
            finished = activeButtons.Count == 0;
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");
        }
    }
}
