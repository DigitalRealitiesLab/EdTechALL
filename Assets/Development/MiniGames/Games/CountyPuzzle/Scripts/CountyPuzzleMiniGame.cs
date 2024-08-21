using UnityEngine;
using UnityEngine.Video;
using System;
using System.Collections.Generic;

namespace MiniGame.CountyPuzzle
{
    public class CountyPuzzleMiniGame : AMiniGame
    {
        public Sprite[] sprites;
        public VideoClip[] videoClips;
        public AudioClip[] audioClips;
        public AudioSource audioSource;
        private Sprite snapshotSprite;
        private Texture2D snapshot;
        private Rect snapshotUIRect = new Rect(-Screen.width / 8.0f, 37.5f, Screen.width * 3.0f / 4.0f, Screen.height * 3.0f / 4.0f - 25.0f);
        private Vector2 snapshotPaddingX = new Vector2(0.0f, 0.0f);
        private Vector2 snapshotPaddingY = new Vector2(400.0f, 100.0f);
        private Salzburg salzburg;
        private Dictionary<CountyType, bool> countySuccesses = new Dictionary<CountyType, bool>();
        private int step = 0;
        private bool takeSnapshot = false;
        private bool enablePhotoButton = false;
        private bool nextStepOnClose = false;
        private bool abortWindowOpen = false;
        private bool startWindowOpen = false;
        private bool popUpActive = false;
        private bool visualGuidanceExplained = false;
        private bool previous = false;

        private void Awake()
        {
            Debug.Assert(sprites.Length == 3, "CountyPuzzleMiniGame needs exactly 3 Sprites");
            Debug.Assert(videoClips.Length == 1, "CountyPuzzleMiniGame needs exactly 1 VideoClip");
            Debug.Assert(audioClips.Length == 6, "CountyPuzzleMiniGame needs exactly 6 AudioClips");
            Debug.Assert(audioSource, "CountyPuzzleMiniGame is missing a reference to an AudioSource");
        }

        private void OnGUI()
        {
            if (takeSnapshot && !abortWindowOpen)
            {
                takeSnapshot = false;

                if (snapshot)
                {
                    Destroy(snapshot);
                }
                snapshot = new Texture2D((int)snapshotUIRect.width + (int)snapshotPaddingX.x + (int)snapshotPaddingX.y, (int)snapshotUIRect.height + (int)snapshotPaddingY.x + (int)snapshotPaddingY.y, TextureFormat.RGB24, false);

                Color[] fillColorArray = new Color[snapshot.width * snapshot.height];
                Array.Fill(fillColorArray, Color.white);
                snapshot.SetPixels(fillColorArray);
                snapshot.ReadPixels(new Rect(Screen.width - (int)snapshotUIRect.width + snapshotUIRect.position.x, snapshotUIRect.position.y, (int)snapshotUIRect.width, (int)snapshotUIRect.height), (int)snapshotPaddingX.x, (int)snapshotPaddingY.x);
                snapshot.Apply();

                if (snapshotSprite)
                {
                    Destroy(snapshotSprite);
                }
                snapshotSprite = Sprite.Create(snapshot, new Rect(0.0f, 0.0f, snapshot.width, snapshot.height), new Vector2(0.5f, 0.5f));

                popUpActive = true;
                RequestDiaryWindow("", "1B", 10.0f, snapshotSprite, null, audioClips[4], "", sprites[2], null, audioClips[5]);
                StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableVisualGuidance, true));
            }
        }

        private void Update()
        {
            if (enablePhotoButton && !audioSource.isPlaying && !abortWindowOpen)
            {
                enablePhotoButton = false;
                StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnablePhotoButton, true));
                StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, false));
            }

            if (previous)
            {
                StartCoroutine(EventBus.PublishCoroutine(EventId.CountyPuzzleMiniGameEvents.SetCounty, true));
                foreach (CountyType countyType in Enum.GetValues(typeof(CountyType)))
                {
                    if (countySuccesses.ContainsKey(countyType))
                    {
                        countySuccesses[countyType] = true;
                    }
                    else
                    {
                        countySuccesses.Add(countyType, true);
                    }
                }

                nextStepOnClose = true;
                popUpActive = true;
                RequestPromptWindow("", 0.0f, sprites[1], null, audioClips[2]);

                previous = false;
            }
        }

        public override void AbortMiniGame()
        {
            enablePhotoButton = false;
            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnablePhotoButton, false));
            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableVisualGuidance, true));
            EventBus.SaveDeregisterCallback(this, EventId.CountyPuzzleMiniGameEvents.SuccessChanged, "OnSuccessChanged");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.PhotoButtonPressed, "OnPhotoButtonPressed");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.DiaryContinue, "OnDiaryContinue");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.DiaryWindowClosed, "OnDiaryWindowClosed");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.PromptWindowClosed, "OnPromptWindowClosed");
            EventBus.SaveDeregisterCallback(this, FullScreen.EventId.UIEvents.FullScreenPromptPanelClosed, "OnFullScreenPromptPanelClosed");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.RequestAbortWindow, "OnRequestAbortWindow");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.NextStep, "OnNextStep");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");
            salzburg.visualGuidanceTarget.gameObject.SetActive(true);
        }

        public override void ExitMiniGame()
        {
            enablePhotoButton = false;
            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnablePhotoButton, false));
            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableVisualGuidance, true));
            EventBus.SaveDeregisterCallback(this, EventId.CountyPuzzleMiniGameEvents.SuccessChanged, "OnSuccessChanged");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.PhotoButtonPressed, "OnPhotoButtonPressed");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.DiaryContinue, "OnDiaryContinue");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.DiaryWindowClosed, "OnDiaryWindowClosed");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.PromptWindowClosed, "OnPromptWindowClosed");
            EventBus.SaveDeregisterCallback(this, FullScreen.EventId.UIEvents.FullScreenPromptPanelClosed, "OnFullScreenPromptPanelClosed");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.RequestAbortWindow, "OnRequestAbortWindow");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.NextStep, "OnNextStep");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");
            salzburg.visualGuidanceTarget.gameObject.SetActive(true);
        }

        public override void StartMiniGame()
        {
            salzburg = FindObjectOfType<Salzburg>();

            salzburg.visualGuidanceTarget.gameObject.SetActive(false);

            foreach (CountyType countyType in Enum.GetValues(typeof(CountyType)))
            {
                if (countySuccesses.ContainsKey(countyType))
                {
                    countySuccesses[countyType] = false;
                }
                else
                {
                    countySuccesses.Add(countyType, false);
                }
            }
            EventBus.SaveRegisterCallback(this, EventId.CountyPuzzleMiniGameEvents.SuccessChanged, "OnSuccessChanged");
            EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.PhotoButtonPressed, "OnPhotoButtonPressed");
            EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.DiaryContinue, "OnDiaryContinue");
            EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.DiaryWindowClosed, "OnDiaryWindowClosed");
            EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.PromptWindowClosed, "OnPromptWindowClosed");
            EventBus.SaveRegisterCallback(this, FullScreen.EventId.UIEvents.FullScreenPromptPanelClosed, "OnFullScreenPromptPanelClosed");
            EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.RequestAbortWindow, "OnRequestAbortWindow");
            EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
            EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
            EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.NextStep, "OnNextStep");
            EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");
            EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");

            FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[0], audioClips[0]);
        }

        public void OnSuccessChanged(CountyType countyType, bool countySuccess)
        {
            countySuccesses[countyType] = countySuccess;

            bool tmpSuccess = true;
            foreach (var val in countySuccesses)
            {
                if (!val.Value)
                {
                    tmpSuccess = false;
                }
            }

            if (tmpSuccess)
            {
                nextStepOnClose = true;
                popUpActive = true;
                RequestPromptWindow("", 0.0f, sprites[1], null, audioClips[2]);
            }
        }

        public void OnPhotoButtonPressed()
        {
            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.NextStep));
        }

        public void OnDiaryContinue()
        {
            popUpActive = false;
            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.NextStep));
        }

        public void OnDiaryWindowClosed()
        {
            popUpActive = false;
            if (!abortWindowOpen)
            {
                StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.PreviousStep));
            }
        }

        public void OnPromptWindowClosed()
        {
            popUpActive = false;
            if (nextStepOnClose)
            {
                StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.NextStep));
            }
        }

        public void OnFullScreenPromptPanelClosed()
        {
            if (step == 0 && !visualGuidanceExplained)
            {
                visualGuidanceExplained = true;
                FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, sprites[0], null, audioClips[1]);
            }
        }

        public void OnRequestAbortWindow()
        {
            abortWindowOpen = true;
            audioSource.Stop();
            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, false));
            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnablePhotoButton, false));
        }

        public void OnMiniGameResumed()
        {
            if (startWindowOpen && step == 1)
            {
                previous = true;
            }

            abortWindowOpen = false;
            startWindowOpen = false;

            if (previous)
            {
                StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.PreviousStep));
            }
            else if (nextStepOnClose)
            {
                StartCoroutine(EventBus.PublishCoroutine(EventId.CountyPuzzleMiniGameEvents.SetCounty, true));
                foreach (CountyType countyType in Enum.GetValues(typeof(CountyType)))
                {
                    if (countySuccesses.ContainsKey(countyType))
                    {
                        countySuccesses[countyType] = true;
                    }
                    else
                    {
                        countySuccesses.Add(countyType, true);
                    }
                }
                nextStepOnClose = false;
                audioSource.clip = audioClips[3];
                audioSource.Play();
                enablePhotoButton = true;
                StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableVisualGuidance, false));
                StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, true));
            }
            else
            {
                switch (step)
                {
                    case 0:
                        FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[0], audioClips[0]);
                        break;
                    case 1:
                        audioSource.clip = audioClips[3];
                        audioSource.Play();
                        enablePhotoButton = true;
                        StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableVisualGuidance, false));
                        StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, true));
                        break;
                    case 2:
                        StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.PreviousStep));
                        break;
                }
            }
        }

        public void OnNextStep()
        {
            popUpActive = false;
            audioSource.Stop();
            bool enableSkipAudioButton = false;

            if (miniGameConfig.steps.Length <= step + 1)
            {
                step = miniGameConfig.steps.Length - 1;
                RequestScoreWindow();
            }
            else
            {
                switch (step)
                {
                    case 0:
                        if (!abortWindowOpen && !startWindowOpen)
                        {
                            salzburg.visualGuidanceTarget.gameObject.SetActive(true);
                            StartCoroutine(EventBus.PublishCoroutine(EventId.CountyPuzzleMiniGameEvents.SetCounty, true));
                            foreach (CountyType countyType in Enum.GetValues(typeof(CountyType)))
                            {
                                if (countySuccesses.ContainsKey(countyType))
                                {
                                    countySuccesses[countyType] = true;
                                }
                                else
                                {
                                    countySuccesses.Add(countyType, true);
                                }
                            }
                            nextStepOnClose = false;
                            audioSource.clip = audioClips[3];
                            audioSource.Play();
                            enablePhotoButton = true;
                            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableVisualGuidance, false));
                            enableSkipAudioButton = true;
                        }
                        break;
                    case 1:
                        enablePhotoButton = false;
                        StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnablePhotoButton, false));
                        takeSnapshot = true;
                        break;
                }
                step++;
            }
            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, enableSkipAudioButton));
        }

        public void OnPreviousStep()
        {
            popUpActive = false;
            audioSource.Stop();
            bool enableSkipAudioButton = false;
            step--;
            if (step < 0)
            {
                step = 0;
                startWindowOpen = true;
                RequestStartWindow();
            }
            else
            {
                switch (step)
                {
                    case 0:
                        salzburg.visualGuidanceTarget.gameObject.SetActive(false);
                        enablePhotoButton = false;
                        StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnablePhotoButton, false));
                        StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableVisualGuidance, true));
                        foreach (CountyType countyType in Enum.GetValues(typeof(CountyType)))
                        {
                            if (countySuccesses.ContainsKey(countyType))
                            {
                                countySuccesses[countyType] = false;
                            }
                            else
                            {
                                countySuccesses.Add(countyType, false);
                            }
                        }
                        StartCoroutine(EventBus.PublishCoroutine(EventId.CountyPuzzleMiniGameEvents.SetCounty, false));
                        if (!abortWindowOpen && !previous)
                        {
                            FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[0], audioClips[0]);
                        }
                        break;
                    case 1:
                        if (!abortWindowOpen)
                        {
                            audioSource.clip = audioClips[3];
                            audioSource.Play();
                            enablePhotoButton = true;
                            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableVisualGuidance, false));
                            enableSkipAudioButton = true;
                        }
                        break;
                    default:
                        break;
                }
            }
            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, enableSkipAudioButton));
        }

        public void OnReplayInstruction()
        {
            audioSource.Stop();
            bool enableSkipAudioButton = false;
            if (!popUpActive)
            {
                switch (step)
                {
                    case 0:
                        visualGuidanceExplained = false;
                        FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[0], audioClips[0]);
                        break;
                    case 1:
                        audioSource.clip = audioClips[3];
                        audioSource.Play();
                        enablePhotoButton = true;
                        enableSkipAudioButton = true;
                        StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnablePhotoButton, false));
                        break;
                }
            }
            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, enableSkipAudioButton));
        }

        public void OnSkipAudioButtonPressed()
        {
            audioSource.Stop();
        }

        private void OnDestroy()
        {
            if (snapshotSprite)
            {
                Destroy(snapshotSprite);
            }

            if (snapshot)
            {
                Destroy(snapshot);
            }
        }
    }

    public static partial class EventId
    {
        public static class CountyPuzzleMiniGameEvents
        {
            public const string SuccessChanged = "SuccessChanged";
            public const string SetCounty = "SetCounty";
        }
    }

    public enum CountyType
    {
        Flachgau,
        Pinzgau,
        Pongau,
        Tennengau,
        Lungau
    }
}