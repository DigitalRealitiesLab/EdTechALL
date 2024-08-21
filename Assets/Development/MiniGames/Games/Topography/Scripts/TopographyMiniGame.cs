using System.Collections;
using UnityEngine;
using UnityEngine.Video;

namespace MiniGame.Topography
{
    public class TopographyMiniGame : AMiniGame
    {
        public GameObject landmarksObject;
        public Sprite[] sprites;
        public VideoClip[] videoClips;
        public AudioClip[] audioClips;
        public AudioSource audioSource;
        public string[] landmarks;
        private int step = 0;
        private bool popUpOpened = true;
        private bool abortWindowOpen = false;
        private bool scoreWindowOpen = false;
        private bool startWindowOpen = false;
        private bool nextStepOnClose = false;
        private bool popUpActive = false;
        private bool sideViewExplained = false;
        private bool sideViewExplaining = false;

        private void Awake()
        {
            Debug.Assert(landmarksObject, "TopographyMiniGame is missing a reference to a GameObject");
            Debug.Assert(sprites.Length == 12, "TopographyMiniGame needs exactly 12 Sprites");
            Debug.Assert(videoClips.Length == 1, "TopographyMiniGame needs exactly 1 VideoClip");
            Debug.Assert(audioClips.Length == 14, "TopographyMiniGame needs exactly 14 AudioClips");
            Debug.Assert(audioSource, "TopographyMiniGame is missing a reference to an AudioSource");
        }

        public override void AbortMiniGame()
        {
            landmarksObject.SetActive(true);
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.PromptWindowClosed, "OnPromptWindowClosed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.LandmarkTouch, "OnLandmarkTouch");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.NextStep, "OnNextStep");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.RequestAbortWindow, "OnRequestAbortWindow");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.RequestScoreWindow, "OnRequestScoreWindow");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.RequestStartWindow, "OnRequestStartWindow");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");
        }

        public override void ExitMiniGame()
        {
            landmarksObject.SetActive(true);
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.PromptWindowClosed, "OnPromptWindowClosed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.LandmarkTouch, "OnLandmarkTouch");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.NextStep, "OnNextStep");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.RequestAbortWindow, "OnRequestAbortWindow");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.RequestScoreWindow, "OnRequestScoreWindow");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.RequestStartWindow, "OnRequestStartWindow");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");
        }

        public override void StartMiniGame()
        {
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.PromptWindowClosed, "OnPromptWindowClosed");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.LandmarkTouch, "OnLandmarkTouch");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.NextStep, "OnNextStep");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.RequestAbortWindow, "OnRequestAbortWindow");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.RequestScoreWindow, "OnRequestScoreWindow");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.RequestStartWindow, "OnRequestStartWindow");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");

            landmarksObject.SetActive(false);
            audioSource.clip = audioClips[0];
            audioSource.Play();
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
            popUpOpened = false;
        }

        private void Update()
        {
            if (!popUpOpened && !audioSource.isPlaying && !abortWindowOpen && !startWindowOpen)
            {
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, false));
                popUpOpened = true;
                StartCoroutine(RequestPromptCoroutine("", sprites[step * 2], null, audioClips[2 + step * 2]));
            }
        }

        private IEnumerator RequestPromptCoroutine(string text = "", Sprite sprite = null, VideoClip videoClip = null, AudioClip audioClip = null)
        {
            yield return new WaitForSeconds(0.1f);
            popUpActive = true;
            RequestPromptWindow(text, 2.0f, sprite, videoClip, audioClip);
        }

        public void OnPromptWindowClosed()
        {
            popUpActive = false;
            if (nextStepOnClose)
            {
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.NextStep));
            }
            else
            {
                StartCoroutine(CheckSideViewCoroutine());
            }
        }

        public IEnumerator CheckSideViewCoroutine()
        {
            yield return new WaitForSeconds(0.1f);

            if (!abortWindowOpen && !startWindowOpen && !popUpActive)
            {
                if (!sideViewExplained)
                {
                    if (!sideViewExplaining)
                    {
                        sideViewExplaining = true;
                        StartCoroutine(RequestPromptCoroutine("", null, videoClips[0], audioClips[1]));
                    }
                    else
                    {
                        sideViewExplaining = false;
                        sideViewExplained = true;
                        landmarksObject.SetActive(true);
                    }
                }
                else
                {
                    landmarksObject.SetActive(true);
                }
            }
        }

        public void OnLandmarkTouch(string touchedName)
        {
            if (touchedName.Contains(landmarks[step]))
            {
                landmarksObject.SetActive(false);
                nextStepOnClose = true;
                StartCoroutine(RequestPromptCoroutine("", sprites[1 + step * 2], null, audioClips[3 + step * 2]));
            }
        }

        public void OnRequestAbortWindow()
        {
            abortWindowOpen = true;
            audioSource.Stop();
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, false));
        }

        public void OnRequestScoreWindow()
        {
            scoreWindowOpen = true;
        }

        public void OnRequestStartWindow(bool firstTime)
        {
            if (!firstTime)
            {
                startWindowOpen = true;
            }
        }

        public void OnMiniGameResumed()
        {
            abortWindowOpen = false;
            startWindowOpen = false;
            if (!scoreWindowOpen && nextStepOnClose)
            {
                OnNextStep();
            }
            else
            {
                nextStepOnClose = false;
                scoreWindowOpen = false;

                if (sideViewExplaining)
                {
                    sideViewExplained = false;
                    sideViewExplaining = false;
                }

                landmarksObject.SetActive(false);

                if (popUpOpened)
                {
                    StartCoroutine(RequestPromptCoroutine("", sprites[step * 2], null, audioClips[2 + step * 2]));
                }
                else
                {
                    audioSource.clip = audioClips[0];
                    audioSource.Play();
                    StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
                }
            }
        }

        public void OnNextStep()
        {
            popUpActive = false;
            popUpOpened = true;

            if (scoreWindowOpen)
            {
                nextStepOnClose = false;
            }

            if (miniGameConfig.steps.Length <= step + 1)
            {
                audioSource.Stop();
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, false));
                nextStepOnClose = false;
                step = miniGameConfig.steps.Length - 1;
                RequestScoreWindow();
            }
            else if (!abortWindowOpen)
            {
                audioSource.Stop();
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, false));
                nextStepOnClose = false;
                step++;
                landmarksObject.SetActive(false);
                popUpOpened = false;

                if (sideViewExplaining)
                {
                    sideViewExplained = false;
                    sideViewExplaining = false;
                }
            }
        }

        public void OnPreviousStep()
        {
            popUpActive = false;
            nextStepOnClose = false;
            audioSource.Stop();
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, false));
            step--;

            if (sideViewExplaining)
            {
                sideViewExplained = false;
                sideViewExplaining = false;
            }

            if (step < 0)
            {
                step = 0;
                RequestStartWindow();
            }
            else
            {
                landmarksObject.SetActive(false);
                popUpOpened = false;
            }
        }

        public void OnReplayInstruction()
        {
            audioSource.Stop();
            if (!popUpActive)
            {
                popUpOpened = false;
                sideViewExplained = false;
                sideViewExplaining = false;
                landmarksObject.SetActive(false);
                audioSource.clip = audioClips[0];
                audioSource.Play();
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
            }
            else
            {
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, false));
            }
        }

        public void OnSkipAudioButtonPressed()
        {
            audioSource.Stop();
        }
    }
}