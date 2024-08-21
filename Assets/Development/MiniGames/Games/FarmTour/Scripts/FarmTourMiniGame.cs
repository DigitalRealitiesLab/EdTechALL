using UnityEngine;
using System.Collections;
using InfinityCode.uPano.Tours;
using InfinityCode.uPano;
using UnityEngine.Video;

namespace MiniGame.FarmTour
{
    public class FarmTourMiniGame : AMiniGame
    {
        public GameObject switchableTourGyroPrefab;
        public GameObject switchableTourTouchPrefab;
        public VideoClip[] videoClips;
        public AudioClip[] audioClips;
        public AudioSource audioSource;
        public Tour[] tours = null;

        private string currentIdentifier = "00";

        private GameObject tourInstance;

        private bool audioWasPlaying = false;
        private bool virtualTourAudioPlaying = false;
        private float virtualTourAudioPlayingSendTimer = 0.1f;
        private bool abortWindowOpen = false;
        private int interactionMethod = 0;

        private void Awake()
        {
            Debug.Assert(switchableTourGyroPrefab, "FarmTourMiniGame is missing a reference to a GameObject");
            Debug.Assert(switchableTourTouchPrefab, "FarmTourMiniGame is missing a reference to a GameObject");
            Debug.Assert(videoClips.Length == 1, "FarmTourMiniGame needs exactly 1 VideoClip");
            Debug.Assert(audioClips.Length == 3, "FarmTourMiniGame needs exactly 3 AudioClips");
            Debug.Assert(audioSource, "FarmTourMiniGame is missing a reference to an AudioSource");
        }

        private void Update()
        {
            if ((currentIdentifier.Equals("00") || currentIdentifier.Equals("10")) && virtualTourAudioPlaying && !audioSource.isPlaying && !abortWindowOpen)
            {
                virtualTourAudioPlaying = false;
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.AudioSubMiniGameIsPlaying, false));
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, false));
            }
            if ((currentIdentifier.Equals("00") || currentIdentifier.Equals("10")) && virtualTourAudioPlaying && audioSource.isPlaying && !abortWindowOpen)
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

        public void OnVirtualTourZoomChanged(int value)
        {
            foreach (Tour tour in tours)
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
                if (currentIdentifier.Equals("00") || currentIdentifier.Equals("10"))
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
            if (currentIdentifier.Equals("00") || currentIdentifier.Equals("10"))
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
            if (!currentIdentifier.Equals("00") || currentIdentifier.Equals("10"))
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
                    tourInstance = Instantiate(switchableTourGyroPrefab, transform);
                }
                else
                {
                    tourInstance = Instantiate(switchableTourTouchPrefab, transform);
                }

                audioSource.clip = audioClips[interactionMethod];

                StartTour();
            }
        }

        public void OnFullScreenPromptPanelClosed()
        {
            if (interactionMethod == 0)
            {
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.ToggleInteractionSelectionPanel, true, 0));
            }
        }

        private IEnumerator SetUpTours()
        {
            yield return new WaitForSeconds(0.1f);

            tours = tourInstance.GetComponentsInChildren<Tour>();

            foreach (Tour tour in tours)
            {
                foreach (TourItem tourItem in tour.items)
                {
                    tourItem.gameObject.SetActive(false);

                    Pano pano = tourItem.GetComponent<Pano>();
                    pano.fov = 90.0f;
                }

                tour.startItem.gameObject.SetActive(true);
                tour.gameObject.SetActive(false);
            }
            tours[0].gameObject.SetActive(true);

            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableVirtualTourUI));
        }

        private void StartTour()
        {
            audioSource.Play();
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
            currentIdentifier = "00";
            virtualTourAudioPlaying = true;

            tourInstance.transform.localEulerAngles = Vector3.zero;
            tourInstance.transform.position = transform.position;

            StartCoroutine(SetUpTours());
            EventBus.Publish(EventId.MiniGameEvents.OverrideText, "Stall: Vergleiche den kleinen und den großen Stall.");
        }

        public override void AbortMiniGame()
        {
            tours = null;
            currentIdentifier = "00";
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
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.ToggleInteractionSelectionPanel, false, 0));
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
            tours = null;
            currentIdentifier = "00";
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
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.ToggleInteractionSelectionPanel, false, 0));
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
    }
}