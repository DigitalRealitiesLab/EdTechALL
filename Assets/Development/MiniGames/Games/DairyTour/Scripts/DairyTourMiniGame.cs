using InfinityCode.uPano.Tours;
using InfinityCode.uPano;
using UnityEngine;
using System.Collections;
using UnityEngine.Video;

namespace MiniGame.DairyTour
{
    public class DairyTourMiniGame : AMiniGame
    {
        public GameObject tourGyroPrefab;
        public GameObject tourTouchPrefab;
        public VideoClip[] videoClips;
        public AudioClip[] audioClips;
        public AudioSource audioSource;
        public Tour tour = null;

        private string currentIdentifier = "0";

        private GameObject tourInstance;

        private bool audioWasPlaying = false;
        private bool virtualTourAudioPlaying = false;
        private float virtualTourAudioPlayingSendTimer = 0.1f;
        private bool abortWindowOpen = false;
        private int interactionMethod = 0;

        private void Awake()
        {
            Debug.Assert(tourGyroPrefab, "DairyTourMiniGame is missing a reference to a GameObject");
            Debug.Assert(tourTouchPrefab, "DairyTourMiniGame is missing a reference to a GameObject");
            Debug.Assert(videoClips.Length == 1, "DairyTourMiniGame needs exactly 1 VideoClip");
            Debug.Assert(audioClips.Length == 3, "DairyTourMiniGame needs exactly 3 AudioClips");
            Debug.Assert(audioSource, "DairyTourMiniGame is missing a reference to an AudioSource");
        }

        private void Update()
        {
            if (currentIdentifier.Equals("0") && virtualTourAudioPlaying && !audioSource.isPlaying && !abortWindowOpen)
            {
                virtualTourAudioPlaying = false;
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.AudioSubMiniGameIsPlaying, false));
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, false));
            }
            if (currentIdentifier.Equals("0") && virtualTourAudioPlaying && audioSource.isPlaying && !abortWindowOpen)
            {
                if (virtualTourAudioPlayingSendTimer > 0.0f)
                {
                    virtualTourAudioPlayingSendTimer -= Time.deltaTime;
                    if (virtualTourAudioPlayingSendTimer < 0.0f)
                    {
                        virtualTourAudioPlayingSendTimer = 0.0f;
                    }
                }
                else if (virtualTourAudioPlayingSendTimer == 0.0f)
                {
                    virtualTourAudioPlayingSendTimer = -1.0f;
                    StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.AudioSubMiniGameIsPlaying, true));
                    StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
                }
            }
        }

        private IEnumerator SetUpTour()
        {
            yield return new WaitForSeconds(0.1f);

            tour = tourInstance.GetComponentInChildren<Tour>();

            foreach (TourItem tourItem in tour.items)
            {
                tourItem.gameObject.SetActive(false);

                Pano pano = tourItem.GetComponent<Pano>();
                pano.fov = 90.0f;
            }

            tour.startItem.gameObject.SetActive(true);
            tour.gameObject.SetActive(false);
            tour.gameObject.SetActive(true);

            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableVirtualTourUI));
        }

        private void StartTour()
        {
            audioSource.Play();
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
            currentIdentifier = "0";
            virtualTourAudioPlaying = true;

            tourInstance.transform.localEulerAngles = Vector3.zero;
            tourInstance.transform.position = transform.position;

            StartCoroutine(SetUpTour());
            EventBus.Publish(EventId.MiniGameEvents.OverrideText, "Anlieferung: Betritt die Molkerei.");
        }

        public override void AbortMiniGame()
        {
            tour = null;
            currentIdentifier = "0";
            Destroy(tourInstance);
            tourInstance = null;
            audioWasPlaying = false;
            virtualTourAudioPlaying = false;
            virtualTourAudioPlayingSendTimer = 0.1f;
            abortWindowOpen = false;
            interactionMethod = 0;
            if (virtualTourAudioPlaying)
            {
                virtualTourAudioPlaying = false;
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.AudioSubMiniGameIsPlaying, false));
            }
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.ToggleInteractionSelectionPanel, false, 1));
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.NextStep, "OnNextStep");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.RequestAbortWindow, "OnRequestAbortWindow");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.VirtualTourZoomChanged, "OnVirtualTourZoomChanged");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.VirtualTourNavigate, "OnVirtualTourNavigate");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SelectedInteractionMethod, "OnSelectedInteractionMethod");
            EventBus.SaveDeregisterCallback(this, FullScreen.EventId.UIEvents.FullScreenPromptPanelClosed, "OnFullScreenPromptPanelClosed");
        }

        public override void ExitMiniGame()
        {
            tour = null;
            currentIdentifier = "0";
            Destroy(tourInstance);
            tourInstance = null;
            audioWasPlaying = false;
            virtualTourAudioPlaying = false;
            virtualTourAudioPlayingSendTimer = 0.1f;
            abortWindowOpen = false;
            interactionMethod = 0;
            if (virtualTourAudioPlaying)
            {
                virtualTourAudioPlaying = false;
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.AudioSubMiniGameIsPlaying, false));
            }
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.ToggleInteractionSelectionPanel, false, 1));
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.NextStep, "OnNextStep");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.RequestAbortWindow, "OnRequestAbortWindow");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.VirtualTourZoomChanged, "OnVirtualTourZoomChanged");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.VirtualTourNavigate, "OnVirtualTourNavigate");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SelectedInteractionMethod, "OnSelectedInteractionMethod");
            EventBus.SaveDeregisterCallback(this, FullScreen.EventId.UIEvents.FullScreenPromptPanelClosed, "OnFullScreenPromptPanelClosed");
        }

        public override void StartMiniGame()
        {
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.NextStep, "OnNextStep");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.RequestAbortWindow, "OnRequestAbortWindow");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.VirtualTourZoomChanged, "OnVirtualTourZoomChanged");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.VirtualTourNavigate, "OnVirtualTourNavigate");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SelectedInteractionMethod, "OnSelectedInteractionMethod");
            EventBus.SaveRegisterCallback(this, FullScreen.EventId.UIEvents.FullScreenPromptPanelClosed, "OnFullScreenPromptPanelClosed");

            if (PlayerPrefs.HasKey(EventId.InteractionMethod))
            {
                interactionMethod = PlayerPrefs.GetInt(EventId.InteractionMethod);
            }

            switch (interactionMethod)
            {
                case 0:
                    StartCoroutine(FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanelCoroutine("", 0.0f, null, videoClips[0], audioClips[interactionMethod]));
                    break;
                default:
                    StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.ToggleInteractionSelectionPanel, false, interactionMethod - 1));
                    StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.SelectedInteractionMethod, interactionMethod));
                    break;
            }
        }

        public void OnVirtualTourZoomChanged(int value)
        {
            if (tour)
            {
                foreach (TourItem tourItem in tour.items)
                {
                    Pano pano = tourItem.GetComponent<Pano>();
                    pano.fov = 60.0f - (value * 10.0f);
                }
            }
        }

        public void OnRequestAbortWindow()
        {
            abortWindowOpen = true;
            if (audioSource.isPlaying)
            {
                audioWasPlaying = true;
                audioSource.Stop();
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, false));
            }
        }

        public void OnMiniGameResumed()
        {
            abortWindowOpen = false;
            if (audioWasPlaying)
            {
                audioWasPlaying = false;
                if (currentIdentifier.Equals("0"))
                {
                    if (interactionMethod > 0)
                    {
                        audioSource.clip = audioClips[interactionMethod];
                        audioSource.Play();
                        StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
                        virtualTourAudioPlaying = true;
                        virtualTourAudioPlayingSendTimer = 0.1f;
                    }
                }
            }
        }

        public void OnNextStep()
        {
            audioSource.Stop();
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, false));

            RequestScoreWindow();
        }

        public void OnReplayInstruction()
        {
            if (currentIdentifier.Equals("0"))
            {
                if (interactionMethod > 0)
                {
                    audioSource.clip = audioClips[interactionMethod];
                    audioSource.Play();
                    StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
                    virtualTourAudioPlaying = true;
                    virtualTourAudioPlayingSendTimer = 0.1f;
                }
            }
        }

        public void OnVirtualTourNavigate(string identifier, string infoText)
        {
            currentIdentifier = identifier;
            EventBus.Publish(EventId.MiniGameEvents.OverrideText, infoText);
            if (!currentIdentifier.Equals("0"))
            {
                audioSource.Stop();
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, false));
                virtualTourAudioPlaying = false;
            }
        }

        public void OnSkipAudioButtonPressed()
        {
            audioSource.Stop();
        }

        public void OnSelectedInteractionMethod(int toggleIndex)
        {
            interactionMethod = toggleIndex;

            if (interactionMethod > 0)
            {
                if (interactionMethod < 2)
                {
                    tourInstance = Instantiate(tourGyroPrefab, transform);
                }
                else
                {
                    tourInstance = Instantiate(tourTouchPrefab, transform);
                }

                audioSource.clip = audioClips[interactionMethod];

                StartTour();
            }
        }

        public void OnFullScreenPromptPanelClosed()
        {
            if (interactionMethod == 0)
            {
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.ToggleInteractionSelectionPanel, true, 1));
            }
        }
    }
}
