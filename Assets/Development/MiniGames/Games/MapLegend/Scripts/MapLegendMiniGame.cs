using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using UnityEngine.XR.ARFoundation;

namespace MiniGame.MapLegend
{
    public class MapLegendMiniGame : AMiniGame
    {
        public Sprite[] sprites;
        public VideoClip[] videoClips;
        public AudioClip[] audioClips;
        public AudioSource audioSource;
        public VisualGuidanceTarget visualGuidanceTarget;

        public MapLegendLookup lookup;

        public MapLegendMarkerReactionBehaviour reactionBehaviour;

        public GameObject landmarksObject;
        public string[] landmarks;

        public GameObject salzburgBorderObject;

        private Salzburg salzburg;
        private FederalUnit targetUnit;
        private MapLegendLookupData searchedLookupData;
        private LegendRandomData legendRandomData;
        private const string legendText = "Schaue auf die Legende in der oberen linken Ecke der Bodenkarte.";
        private string[] searchTexts = { "Tippe auf ein Wasserkraftwerk in Salzburg.",
        "Tippe auf eine Autobahn in Salzburg.",
        "Tippe auf eine Burg oder ein Schloss in Salzburg."};
        private const string frameText = "Suche {0} im Rahmen der Karte.";
        private const string searchText = "Suche auf der Karte {0}.";
        private int step = 0;
        private int tutorialSubstep = 0;
        private const int landmarksSteps = 3;
        private const int legendSteps = 1;
        private const int citySteps = 2;
        private bool popUpOpened = false;
        private bool legendScanStarted = true;
        private bool nextStepOnClose = false;
        private bool abortWindowOpen = false;
        private bool scoreWindowOpen = false;
        private bool startWindowOpen = false;
        private bool infoUIExplained = false;
        private bool setLandmarks = false;
        private bool frameExplained = true;
        private bool scanStarted = true;
        private bool searchExplained = false;
        private bool borderExplained = false;
        private bool searchStarted = true;
        private bool popUpActive = false;
        private bool started = false;

        private Vector3[] guidancePositions = { new Vector3(-0.45f, 0.0f, 0.4f), new Vector3(0.48f, 0.0f, 0.38f), new Vector3(-0.48f, 0.0f, -0.19f),
            new Vector3(0.11f, 0.0f, -0.48f), new Vector3(0.225f, 0.0f, -0.48f), new Vector3(-0.31f, 0.0f, -0.48f) };

        public void Awake()
        {
            Debug.Assert(sprites.Length == 14, "MapLegendMiniGame needs exactly 14 Sprites");
            Debug.Assert(videoClips.Length == 6, "MapLegendMiniGame needs exactly 6 VideoClips");
            Debug.Assert(audioClips.Length == 33, "MapLegendMiniGame needs exactly 33 AudioClips");
            Debug.Assert(audioSource, "MapLegendMiniGame is missing a reference to an AudioSource");
            Debug.Assert(visualGuidanceTarget, "MapLegendMiniGame is missing a reference to a VisualGuidanceTarget");
            Debug.Assert(lookup, "MapLegendMiniGame is missing a reference to a MapLegendLookup ScriptableObject");
            Debug.Assert(reactionBehaviour, "MapLegendMiniGame is missing a reference to a MapLegendMarkerReactionBehaviour");
            Debug.Assert(landmarksObject, "MapLegendMiniGame is missing a reference to a GameObject");
            Debug.Assert(landmarks.Length == landmarksSteps, "MapLegendMiniGame needs exactly 3 Landmarks");
        }

        private void Update()
        {
            if (started)
            {
                if ((step + legendRandomData.overstep) % (landmarksSteps + citySteps + legendSteps) == legendSteps && !popUpOpened && !audioSource.isPlaying && !abortWindowOpen)
                {
                    StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, false));
                    popUpOpened = true;
                    RequestFullScreenPromptPanel("", null, videoClips[1], audioClips[2]);
                }
                else if (setLandmarks && !audioSource.isPlaying)
                {
                    StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, false));
                    setLandmarks = false;
                    landmarksObject.SetActive(true);
                }
            }
        }

        public override void AbortMiniGame()
        {
            salzburg.visualGuidanceTarget.gameObject.SetActive(true);
            reactionBehaviour.StopMarkerSearch();
            foreach (MeshCollider collider in salzburg.GetComponentsInChildren<MeshCollider>())
            {
                collider.enabled = false;
            }
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.NextStep, "OnNextStep");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.PromptWindowClosed, "OnPromptWindowClosed");
            EventBus.SaveDeregisterCallback(this, FullScreen.EventId.UIEvents.FullScreenPromptPanelClosed, "OnFullScreenPromptPanelClosed");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.RequestAbortWindow, "OnRequestAbortWindow");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.RequestScoreWindow, "OnRequestScoreWindow");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
            EventBus.SaveDeregisterCallback(this, EventId.MapLegendMiniGameEvents.MunicipalityMarkerDetected, "OnMunicipalityMarkerDetected");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.MunicipalityTouch, "OnMunicipalityTouch");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.LandmarkTouch, "OnLandmarkTouch");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");
        }

        public override void ExitMiniGame()
        {
            salzburg.visualGuidanceTarget.gameObject.SetActive(true);
            reactionBehaviour.StopMarkerSearch();
            foreach (MeshCollider collider in salzburg.GetComponentsInChildren<MeshCollider>())
            {
                collider.enabled = false;
            }
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.NextStep, "OnNextStep");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.PromptWindowClosed, "OnPromptWindowClosed");
            EventBus.SaveDeregisterCallback(this, FullScreen.EventId.UIEvents.FullScreenPromptPanelClosed, "OnFullScreenPromptPanelClosed");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.RequestAbortWindow, "OnRequestAbortWindow");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.RequestScoreWindow, "OnRequestScoreWindow");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
            EventBus.SaveDeregisterCallback(this, EventId.MapLegendMiniGameEvents.MunicipalityMarkerDetected, "OnMunicipalityMarkerDetected");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.MunicipalityTouch, "OnMunicipalityTouch");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.LandmarkTouch, "OnLandmarkTouch");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");
        }

        public override void StartMiniGame()
        {
            lookup.Initialize();

            salzburg = FindObjectOfType<Salzburg>();

            salzburg.visualGuidanceTarget.gameObject.SetActive(false);
            visualGuidanceTarget.gameObject.SetActive(true);
            landmarksObject.SetActive(false);
            salzburgBorderObject.SetActive(false);

            if (salzburg)
            {
                foreach (MeshCollider collider in salzburg.GetComponentsInChildren<MeshCollider>())
                {
                    collider.enabled = false;
                }

                EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
                EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.NextStep, "OnNextStep");
                EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.PromptWindowClosed, "OnPromptWindowClosed");
                EventBus.SaveRegisterCallback(this, FullScreen.EventId.UIEvents.FullScreenPromptPanelClosed, "OnFullScreenPromptPanelClosed");
                EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.RequestAbortWindow, "OnRequestAbortWindow");
                EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.RequestScoreWindow, "OnRequestScoreWindow");
                EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
                EventBus.SaveRegisterCallback(this, EventId.MapLegendMiniGameEvents.MunicipalityMarkerDetected, "OnMunicipalityMarkerDetected");
                EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.MunicipalityTouch, "OnMunicipalityTouch");
                EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.LandmarkTouch, "OnLandmarkTouch");
                EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");
                EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");

                legendRandomData = new LegendRandomData();

                Random.InitState((int)System.DateTime.Now.TimeOfDay.TotalMilliseconds);

                int random = 0;
                float fRandom = Random.value;
                if (fRandom > 5.0f / 6.0f)
                {
                    random = 5;
                }
                else if (fRandom > 2.0f / 3.0f)
                {
                    random = 4;
                }
                else if (fRandom > 0.5f)
                {
                    random = 3;
                }
                else if (fRandom > 1.0f / 3.0f)
                {
                    random = 2;
                }
                else if (fRandom > 1.0f / 6.0f)
                {
                    random = 1;
                }

                visualGuidanceTarget.transform.localPosition = guidancePositions[random];

                if (random == 0)
                {
                    Random.InitState((int)System.DateTime.Now.TimeOfDay.TotalMilliseconds + 137);

                    fRandom = Random.value;
                    if (fRandom > 4.0f / 5.0f)
                    {
                        random = 5;
                    }
                    else if (fRandom > 3.0f / 5.0f)
                    {
                        random = 4;
                    }
                    else if (fRandom > 2.0f / 5.0f)
                    {
                        random = 3;
                    }
                    else if (fRandom > 1.0f / 5.0f)
                    {
                        random = 2;
                    }
                    else
                    {
                        random = 1;
                    }

                    legendRandomData.overstep = 0;
                    legendRandomData.randomCity = random;
                    legendScanStarted = false;
                    RequestFullScreenPromptPanel("", null, videoClips[0], audioClips[0]);
                    EventBus.Publish(MiniGame.EventId.MiniGameEvents.OverrideText, legendText);
                }
                else
                {
                    frameExplained = false;
                    RequestFullScreenPromptPanel("", sprites[3], null, audioClips[11]);
                    legendRandomData.overstep = landmarksSteps + legendSteps;
                    legendRandomData.randomCity = random;
                }
                started = true;
            }
            else
            {
                Debug.LogError("Salzburg is null but required for minigame!");
            }
        }

        private IEnumerator RequestPromptCoroutine(string text = "", float buttonTime = 0.0f, Sprite sprite = null, VideoClip videoClip = null, AudioClip audioClip = null)
        {
            popUpActive = true;
            yield return new WaitForSeconds(0.1f);

            RequestPromptWindow(text, buttonTime, sprite, videoClip, audioClip);
        }

        private void RequestFullScreenPromptPanel(string text = "", Sprite sprite = null, VideoClip videoClip = null, AudioClip audioClip = null)
        {
            popUpActive = true;
            FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel(text, 0.0f, sprite, videoClip, audioClip);
        }

        private void RequestFullScreenPromptPanelCoroutine(Sprite sprite = null, VideoClip videoClip = null, AudioClip audioClip = null)
        {
            popUpActive = true;
            StartCoroutine(FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanelCoroutine("", 0.0f, sprite, videoClip, audioClip));
        }

        public void OnMunicipalityMarkerDetected(ARTrackedImage image)
        {
            if (!abortWindowOpen && !startWindowOpen && !popUpActive)
            {
                if (searchedLookupData.lookupKey.Equals(image.referenceImage.name))
                {
                    if (!salzburg)
                    {
                        salzburg = FindObjectOfType<Salzburg>();
                    }
                    if (salzburg)
                    {
                        if ((step + legendRandomData.overstep) % (landmarksSteps + citySteps + legendSteps) == 0)
                        {
                            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.NextStep));
                        }
                        else
                        {
                            nextStepOnClose = true;
                            StartCoroutine(RequestPromptCoroutine("", 0.0f, sprites[3 + legendRandomData.randomCity], null, audioClips[16 + legendRandomData.randomCity]));
                        }
                    }
                    else
                    {
                        Debug.LogError("Salzburg is null but required for a lookup");
                    }
                }
            }
        }

        public void OnPromptWindowClosed()
        {
            popUpActive = false;
            if (nextStepOnClose)
            {
                StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.NextStep));
            }
            else if (!searchExplained && !abortWindowOpen && !startWindowOpen && ((step + legendRandomData.overstep) % (landmarksSteps + citySteps + legendSteps)) == landmarksSteps + legendSteps + citySteps - 1)
            {
                searchExplained = true;
                if (borderExplained)
                {
                    searchStarted = false;
                    StartCoroutine(RequestPromptCoroutine("", 0.0f, null, videoClips[5], audioClips[27]));
                }
                else
                {
                    RequestFullScreenPromptPanel("", null, videoClips[3], audioClips[4]);
                }
            }
            else if (!searchStarted && !abortWindowOpen && !startWindowOpen)
            {
                searchStarted = true;
                targetUnit = salzburg.GetMunicipalityByMunicipalCode(searchedLookupData.municipalCode);
                if (targetUnit)
                {
                    foreach (MeshCollider collider in salzburg.GetComponentsInChildren<MeshCollider>())
                    {
                        collider.enabled = true;
                    }
                }
                else
                {
                    Debug.LogError("Municipality " + MunicipalityLookup.Instance.GetAt(searchedLookupData.municipalCode).MunicipalityName + " with municipal code " + searchedLookupData.municipalCode + " not found");
                }
            }
        }

        public void OnFullScreenPromptPanelClosed()
        {
            popUpActive = false;
            if (nextStepOnClose)
            {
                StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.NextStep));
            }
            else if (!borderExplained && ((step + legendRandomData.overstep) % (landmarksSteps + citySteps + legendSteps)) == landmarksSteps + legendSteps + citySteps - 1)
            {
                borderExplained = true;
                searchStarted = false;
                StartCoroutine(RequestPromptCoroutine("", 0.0f, null, videoClips[5], audioClips[27]));
            }
            else if (!legendScanStarted)
            {
                if (!infoUIExplained)
                {
                    infoUIExplained = true;
                    RequestFullScreenPromptPanel("", null, videoClips[2], audioClips[3]);
                }
                else
                {
                    legendScanStarted = true;
                    searchedLookupData = lookup.imageMarkerData[0];
                    reactionBehaviour.ActivateMarkerSearch();
                }
            }
            else if (((step + legendRandomData.overstep) % (landmarksSteps + citySteps + legendSteps)) == legendSteps)
            {
                switch (tutorialSubstep)
                {
                    case 0:
                        if (borderExplained)
                        {
                            tutorialSubstep++;
                            audioSource.clip = audioClips[5];
                            audioSource.Play();
                            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, true));
                            landmarksObject.SetActive(false);
                            setLandmarks = true;
                        }
                        else
                        {
                            borderExplained = true;
                            RequestFullScreenPromptPanel("", null, videoClips[3], audioClips[4]);
                        }
                        break;
                    case 1:
                        audioSource.clip = audioClips[5];
                        audioSource.Play();
                        StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, true));
                        landmarksObject.SetActive(false);
                        setLandmarks = true;
                        break;
                }
                if (tutorialSubstep < 1)
                {
                    tutorialSubstep++;
                }
            }
            else if (!frameExplained)
            {
                frameExplained = true;
                scanStarted = false;
                searchedLookupData = lookup.imageMarkerData[legendRandomData.randomCity];
                EventBus.Publish(MiniGame.EventId.MiniGameEvents.OverrideText, string.Format(frameText, MunicipalityLookup.Instance.GetAt(searchedLookupData.municipalCode).MunicipalityName));
                RequestFullScreenPromptPanel("", null, videoClips[4], audioClips[11 + legendRandomData.randomCity]);
            }
            else if (!scanStarted)
            {
                if (!infoUIExplained)
                {
                    infoUIExplained = true;
                    RequestFullScreenPromptPanel("", null, videoClips[2], audioClips[3]);
                }
                else
                {
                    scanStarted = true;
                    reactionBehaviour.ActivateMarkerSearch();
                }
            }
        }

        public void OnRequestAbortWindow()
        {
            abortWindowOpen = true;
            audioSource.Stop();
            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, false));
        }

        public void OnRequestScoreWindow()
        {
            scoreWindowOpen = true;
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
                switch ((step + legendRandomData.overstep) % (landmarksSteps + citySteps + legendSteps))
                {
                    case legendSteps - 1:
                        RequestFullScreenPromptPanel("", null, videoClips[0], audioClips[0]);
                        break;
                    case landmarksSteps + legendSteps + citySteps - 2:
                        RequestFullScreenPromptPanel("", null, videoClips[4], audioClips[11 + legendRandomData.randomCity]);
                        break;
                    case landmarksSteps + legendSteps + citySteps - 1:
                        if (!searchExplained)
                        {
                            StartCoroutine(RequestPromptCoroutine("", 0.0f, sprites[3 + legendRandomData.randomCity], null, audioClips[21 + legendRandomData.randomCity]));
                        }
                        else if (!borderExplained)
                        {
                            RequestFullScreenPromptPanel("", null, videoClips[3], audioClips[4]);
                        }
                        else
                        {
                            StartCoroutine(RequestPromptCoroutine("", 0.0f, null, videoClips[5], audioClips[27]));
                        }
                        break;
                    case legendSteps:
                        if (popUpOpened)
                        {
                            if (tutorialSubstep < 1)
                            {
                                tutorialSubstep = 0;
                                popUpOpened = false;
                                setLandmarks = false;
                                landmarksObject.SetActive(false);
                                audioSource.clip = audioClips[1];
                                audioSource.Play();
                                StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, true));
                            }
                            else
                            {
                                audioSource.clip = audioClips[(((step + legendRandomData.overstep) % (landmarksSteps + citySteps + legendSteps)) * 2) + 3];
                                audioSource.Play();
                                StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, true));
                                landmarksObject.SetActive(false);
                                setLandmarks = true;
                            }
                        }
                        else
                        {
                            setLandmarks = false;
                            landmarksObject.SetActive(false);
                            audioSource.clip = audioClips[1];
                            audioSource.Play();
                            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, true));
                        }
                        break;
                    default:
                        audioSource.clip = audioClips[(((step + legendRandomData.overstep) % (landmarksSteps + citySteps + legendSteps)) * 2) + 3];
                        audioSource.Play();
                        StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, true));
                        landmarksObject.SetActive(false);
                        setLandmarks = true;
                        break;
                }
            }
        }

        public void OnMunicipalityTouch(string touchedName)
        {
            if (touchedName.Contains(targetUnit.name) && !abortWindowOpen)
            {
                nextStepOnClose = true;
                RequestFullScreenPromptPanel("", sprites[8 + legendRandomData.randomCity], null, audioClips[27 + legendRandomData.randomCity]);
            }
        }

        public void OnLandmarkTouch(string touchedName)
        {
            if (touchedName.Contains(landmarks[((step + legendRandomData.overstep) % (landmarksSteps + citySteps + legendSteps)) - legendSteps]))
            {
                landmarksObject.SetActive(false);
                nextStepOnClose = true;
                StartCoroutine(RequestPromptCoroutine("", 2.0f, sprites[(step + legendRandomData.overstep - 1) % (landmarksSteps + citySteps + legendSteps)], null, audioClips[(((step + legendRandomData.overstep) % (landmarksSteps + citySteps + legendSteps)) * 2) + 4]));
            }
        }

        public void OnNextStep()
        {
            popUpActive = false;
            audioSource.Stop();
            bool enableSkipAudioButton = false;
            if (miniGameConfig.steps.Length <= step + 1)
            {
                nextStepOnClose = false;
                step = miniGameConfig.steps.Length - 1;
                RequestScoreWindow();
            }
            else
            {
                if (scoreWindowOpen)
                {
                    nextStepOnClose = false;
                }
                if (!abortWindowOpen && !startWindowOpen)
                {
                    nextStepOnClose = false;
                    switch ((step + legendRandomData.overstep) % (landmarksSteps + citySteps + legendSteps))
                    {
                        case legendSteps - 1:
                            reactionBehaviour.StopMarkerSearch();
                            visualGuidanceTarget.gameObject.SetActive(false);
                            salzburgBorderObject.SetActive(true);
                            EventBus.Publish(MiniGame.EventId.MiniGameEvents.OverrideText, searchTexts[0]);
                            if (popUpOpened)
                            {
                                if (tutorialSubstep < 1)
                                {
                                    tutorialSubstep = 0;
                                    popUpOpened = false;
                                    setLandmarks = false;
                                    landmarksObject.SetActive(false);
                                    audioSource.clip = audioClips[1];
                                    audioSource.Play();
                                    enableSkipAudioButton = true;
                                }
                                else
                                {
                                    audioSource.clip = audioClips[5];
                                    audioSource.Play();
                                    enableSkipAudioButton = true;
                                    landmarksObject.SetActive(false);
                                    setLandmarks = true;
                                }
                            }
                            else
                            {
                                setLandmarks = false;
                                landmarksObject.SetActive(false);
                                audioSource.clip = audioClips[1];
                                audioSource.Play();
                                enableSkipAudioButton = true;
                            }
                            break;
                        case landmarksSteps + legendSteps - 1:
                            visualGuidanceTarget.gameObject.SetActive(true);
                            visualGuidanceTarget.transform.localPosition = guidancePositions[legendRandomData.randomCity];
                            salzburgBorderObject.SetActive(false);
                            frameExplained = false;
                            StartCoroutine(FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanelCoroutine("", 0.0f, sprites[3], null, audioClips[11]));
                            break;
                        case landmarksSteps + legendSteps + citySteps - 2:
                            reactionBehaviour.StopMarkerSearch();
                            EventBus.Publish(MiniGame.EventId.MiniGameEvents.OverrideText, string.Format(searchText, MunicipalityLookup.Instance.GetAt(searchedLookupData.municipalCode).MunicipalityName));
                            visualGuidanceTarget.gameObject.SetActive(false);
                            salzburgBorderObject.SetActive(true);
                            if (!searchExplained)
                            {
                                StartCoroutine(RequestPromptCoroutine("", 0.0f, sprites[3 + legendRandomData.randomCity], null, audioClips[21 + legendRandomData.randomCity]));
                            }
                            else if (!borderExplained)
                            {
                                RequestFullScreenPromptPanel("", null, videoClips[3], audioClips[4]);
                            }
                            else
                            {
                                targetUnit = salzburg.GetMunicipalityByMunicipalCode(searchedLookupData.municipalCode);
                                if (targetUnit)
                                {
                                    foreach (MeshCollider collider in salzburg.GetComponentsInChildren<MeshCollider>())
                                    {
                                        collider.enabled = true;
                                    }
                                    StartCoroutine(RequestPromptCoroutine("", 0.0f, null, videoClips[5], audioClips[27]));
                                }
                                else
                                {
                                    Debug.LogError("Municipality " + MunicipalityLookup.Instance.GetAt(searchedLookupData.municipalCode).MunicipalityName + " with municipal code " + searchedLookupData.municipalCode + " not found");
                                }
                            }
                            break;
                        case landmarksSteps + legendSteps + citySteps - 1:
                            visualGuidanceTarget.gameObject.SetActive(true);
                            visualGuidanceTarget.transform.localPosition = guidancePositions[0];
                            salzburgBorderObject.SetActive(false);
                            legendScanStarted = false;
                            RequestFullScreenPromptPanelCoroutine(null, videoClips[0], audioClips[0]);
                            EventBus.Publish(MiniGame.EventId.MiniGameEvents.OverrideText, legendText);
                            foreach (MeshCollider collider in salzburg.GetComponentsInChildren<MeshCollider>())
                            {
                                collider.enabled = false;
                            }
                            break;
                        default:
                            EventBus.Publish(MiniGame.EventId.MiniGameEvents.OverrideText, searchTexts[((step + 1 + legendRandomData.overstep) % (landmarksSteps + citySteps + legendSteps)) - legendSteps]);
                            audioSource.clip = audioClips[(((step + 1 + legendRandomData.overstep) % (landmarksSteps + citySteps + legendSteps)) * 2) + 3];
                            audioSource.Play();
                            enableSkipAudioButton = true;
                            landmarksObject.SetActive(false);
                            setLandmarks = true;
                            break;
                    }
                    step++;
                }
            }
            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, enableSkipAudioButton));
        }

        public void OnPreviousStep()
        {
            popUpActive = false;
            audioSource.Stop();
            bool enableSkipAudioButton = false;
            nextStepOnClose = false;
            step--;
            if (step < 0)
            {
                step = 0;
                startWindowOpen = true;
                RequestStartWindow();
            }
            else
            {
                switch ((step + legendRandomData.overstep) % (landmarksSteps + citySteps + legendSteps))
                {
                    case legendSteps - 1:
                        visualGuidanceTarget.gameObject.SetActive(true);
                        visualGuidanceTarget.transform.localPosition = guidancePositions[0];
                        landmarksObject.SetActive(false);
                        salzburgBorderObject.SetActive(false);
                        legendScanStarted = false;
                        RequestFullScreenPromptPanelCoroutine(null, videoClips[0], audioClips[0]);
                        EventBus.Publish(MiniGame.EventId.MiniGameEvents.OverrideText, legendText);
                        break;
                    case landmarksSteps + legendSteps - 1:
                        reactionBehaviour.StopMarkerSearch();
                        visualGuidanceTarget.gameObject.SetActive(false);
                        audioSource.clip = audioClips[(((step + legendRandomData.overstep) % (landmarksSteps + citySteps + legendSteps)) * 2) + 3];
                        audioSource.Play();
                        enableSkipAudioButton = true;
                        salzburgBorderObject.SetActive(true);
                        landmarksObject.SetActive(false);
                        setLandmarks = true;
                        EventBus.Publish(MiniGame.EventId.MiniGameEvents.OverrideText, searchTexts[landmarksSteps - 1]);
                        break;
                    case landmarksSteps + legendSteps + citySteps - 2:
                        visualGuidanceTarget.gameObject.SetActive(true);
                        visualGuidanceTarget.transform.localPosition = guidancePositions[legendRandomData.randomCity];
                        salzburgBorderObject.SetActive(false);
                        searchedLookupData = lookup.imageMarkerData[legendRandomData.randomCity];
                        foreach (MeshCollider collider in salzburg.GetComponentsInChildren<MeshCollider>())
                        {
                            collider.enabled = false;
                        }
                        EventBus.Publish(MiniGame.EventId.MiniGameEvents.OverrideText, string.Format(frameText, MunicipalityLookup.Instance.GetAt(searchedLookupData.municipalCode).MunicipalityName));
                        scanStarted = false;
                        RequestFullScreenPromptPanel("", null, videoClips[4], audioClips[11 + legendRandomData.randomCity]);
                        break;
                    case landmarksSteps + legendSteps + citySteps - 1:
                        reactionBehaviour.StopMarkerSearch();
                        visualGuidanceTarget.gameObject.SetActive(false);
                        salzburgBorderObject.SetActive(true);
                        legendScanStarted = true;
                        searchedLookupData = lookup.imageMarkerData[legendRandomData.randomCity];
                        if (!searchExplained)
                        {
                            StartCoroutine(RequestPromptCoroutine("", 0.0f, sprites[3 + legendRandomData.randomCity], null, audioClips[21 + legendRandomData.randomCity]));
                        }
                        else
                        {
                            targetUnit = salzburg.GetMunicipalityByMunicipalCode(searchedLookupData.municipalCode);
                            if (targetUnit)
                            {
                                foreach (MeshCollider collider in salzburg.GetComponentsInChildren<MeshCollider>())
                                {
                                    collider.enabled = true;
                                }
                                EventBus.Publish(MiniGame.EventId.MiniGameEvents.OverrideText, string.Format(searchText, MunicipalityLookup.Instance.GetAt(searchedLookupData.municipalCode).MunicipalityName));
                                StartCoroutine(RequestPromptCoroutine("", 0.0f, null, videoClips[5], audioClips[27]));
                            }
                            else
                            {
                                Debug.LogError("Municipality " + MunicipalityLookup.Instance.GetAt(searchedLookupData.municipalCode).MunicipalityName + " with municipal code " + searchedLookupData.municipalCode + " not found");
                            }
                        }
                        break;
                    case legendSteps:
                        if (popUpOpened)
                        {
                            if (tutorialSubstep < 1)
                            {
                                tutorialSubstep = 0;
                                popUpOpened = false;
                                setLandmarks = false;
                                landmarksObject.SetActive(false);
                                audioSource.clip = audioClips[1];
                                audioSource.Play();
                                enableSkipAudioButton = true;
                            }
                            else
                            {
                                audioSource.clip = audioClips[(((step + legendRandomData.overstep) % (landmarksSteps + citySteps + legendSteps)) * 2) + 3];
                                audioSource.Play();
                                enableSkipAudioButton = true;
                                landmarksObject.SetActive(false);
                                setLandmarks = true;
                            }
                        }
                        else
                        {
                            setLandmarks = false;
                            landmarksObject.SetActive(false);
                            audioSource.clip = audioClips[1];
                            audioSource.Play();
                            enableSkipAudioButton = true;
                        }
                        break;
                    default:
                        EventBus.Publish(MiniGame.EventId.MiniGameEvents.OverrideText, searchTexts[((step + legendRandomData.overstep) % (landmarksSteps + citySteps + legendSteps)) - legendSteps]);
                        audioSource.clip = audioClips[(((step + legendRandomData.overstep) % (landmarksSteps + citySteps + legendSteps)) * 2) + 3];
                        audioSource.Play();
                        enableSkipAudioButton = true;
                        landmarksObject.SetActive(false);
                        setLandmarks = true;
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
                switch ((step + legendRandomData.overstep) % (landmarksSteps + citySteps + legendSteps))
                {
                    case legendSteps - 1:
                        RequestFullScreenPromptPanel("", null, videoClips[0], audioClips[0]);
                        break;
                    case landmarksSteps + legendSteps + citySteps - 2:
                        frameExplained = false;
                        StartCoroutine(FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanelCoroutine("", 0.0f, sprites[3], null, audioClips[11]));
                        break;
                    case landmarksSteps + legendSteps + citySteps - 1:
                        searchExplained = false;
                        StartCoroutine(RequestPromptCoroutine("", 0.0f, sprites[3 + legendRandomData.randomCity], null, audioClips[21 + legendRandomData.randomCity]));
                        break;
                    case legendSteps:
                        tutorialSubstep = 0;
                        popUpOpened = false;
                        setLandmarks = false;
                        landmarksObject.SetActive(false);
                        audioSource.clip = audioClips[1];
                        audioSource.Play();
                        enableSkipAudioButton = true;
                        break;
                    default:
                        audioSource.clip = audioClips[(((step + legendRandomData.overstep) % (landmarksSteps + citySteps + legendSteps)) * 2) + 3];
                        audioSource.Play();
                        enableSkipAudioButton = true;
                        landmarksObject.SetActive(false);
                        setLandmarks = true;
                        break;
                }
            }
            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, enableSkipAudioButton));
        }

        public void OnSkipAudioButtonPressed()
        {
            audioSource.Stop();
        }
    }


    [System.Serializable]
    public class LegendRandomData
    {
        public int randomCity;
        public int overstep = 0;
    }

    public static partial class EventId
    {
        public static class MapLegendMiniGameEvents
        {
            public const string MunicipalityMarkerDetected = "MunicipalityMarkerDetected";
        }
    }
}