using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace MiniGame.LandscapeLayers
{
    public class LandscapeLayersMiniGame : AMiniGame
    {
        public LandscapeLayersLookup landscapeLayersLookup;
        public PositioningQuiz positioningQuiz;
        public VideoClip videoClip;
        public AudioClip[] audioClips;
        public AudioSource audioSource;
        private Dictionary<LandscapeType, LandscapeLayersData> landscapeLayers = new Dictionary<LandscapeType, LandscapeLayersData>();
        private Dictionary<LandscapeType, bool> landscapeLayersClicked = new Dictionary<LandscapeType, bool>();
        private GameObject landscapeLayerReferenceObject;
        private int step = 0;
        private bool abortWindowOpen = false;
        private bool audioWasPlaying = false;
        private bool check = false;
        private bool positioningQuizEnded = false;

        public void Awake()
        {
            Debug.Assert(landscapeLayersLookup, "LandscapeLayersMiniGame is missing a reference to a LandscapeLayersLookup");
            Debug.Assert(positioningQuiz, "LandscapeLayersMiniGame is missing a reference to a PositioningQuiz");
            Debug.Assert(videoClip, "LandscapeLayersMiniGame is missing a reference to a VideoClip");
            Debug.Assert(audioClips.Length == 7, "LandscapeLayersMiniGame needs exactly 7 AudioClips");
            Debug.Assert(audioSource, "LandscapeLayersMiniGame is missing a reference to an AudioSource");
        }

        private void Update()
        {
            if (check && !audioSource.isPlaying && !abortWindowOpen)
            {
                EventBus.Publish(EventId.LandscapeLayersMiniGameEvents.SetTogglesInteractable, true);
                StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, false));
                check = false;
                bool clickedAll = true;
                foreach (bool clicked in landscapeLayersClicked.Values)
                {
                    if (!clicked)
                    {
                        clickedAll = false;
                    }
                }

                if (clickedAll)
                {
                    StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.NextStep));
                }
            }
        }

        public override void AbortMiniGame()
        {
            DeleteLandscapeLayersLookup();
            if (step > 0)
            {
                EventBus.Publish(MiniGame.EventId.MiniGameEvents.AbortPositioningQuiz);
            }
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.NextStep, "OnNextStep");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.RequestAbortWindow, "OnRequestAbortWindow");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
            EventBus.SaveDeregisterCallback(this, EventId.LandscapeLayersMiniGameEvents.SetLandscapeLayerActive, "OnSetLandscapeLayerActive");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.PositioningQuizEnded, "OnPositioningQuizEnded");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");
        }

        public override void ExitMiniGame()
        {
            DeleteLandscapeLayersLookup();
            if (step > 0)
            {
                EventBus.Publish(MiniGame.EventId.MiniGameEvents.AbortPositioningQuiz);
            }
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.NextStep, "OnNextStep");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.RequestAbortWindow, "OnRequestAbortWindow");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
            EventBus.SaveDeregisterCallback(this, EventId.LandscapeLayersMiniGameEvents.SetLandscapeLayerActive, "OnSetLandscapeLayerActive");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.PositioningQuizEnded, "OnPositioningQuizEnded");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");
        }

        public override void StartMiniGame()
        {
            SearchLandscapeLayerReference();
            LoadLandscapeLayersLookup();
            EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
            EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.NextStep, "OnNextStep");
            EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.RequestAbortWindow, "OnRequestAbortWindow");
            EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
            EventBus.SaveRegisterCallback(this, EventId.LandscapeLayersMiniGameEvents.SetLandscapeLayerActive, "OnSetLandscapeLayerActive");
            EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.PositioningQuizEnded, "OnPositioningQuizEnded");
            EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");
            EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");
            StartCoroutine(RequestFullScreenPromptPanel("", videoClip, audioClips[0]));
        }

        private IEnumerator RequestFullScreenPromptPanel(string text = "", VideoClip videoClip = null, AudioClip audioClip = null)
        {
            yield return new WaitForSeconds(0.1f);
            FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel(text, 0.0f, null, videoClip, audioClip);
        }

        private void SearchLandscapeLayerReference()
        {
            landscapeLayerReferenceObject = GameObject.FindGameObjectWithTag("LandscapeLayer");
            Debug.Assert(landscapeLayerReferenceObject, "LandscapeLayersMiniGame could not find a LandscapeLayerReferenceObject with tag LandscapeLayer");
        }

        private void LoadLandscapeLayersLookup()
        {
            foreach (var landscapeLayer in landscapeLayersLookup.landscapeLayersData)
            {
                GameObject clone = Instantiate(landscapeLayerReferenceObject, transform, true);
                clone.transform.localScale = landscapeLayerReferenceObject.transform.localScale;
                clone.transform.rotation = landscapeLayerReferenceObject.transform.rotation;
                clone.transform.position = landscapeLayerReferenceObject.transform.position;
                clone.name = landscapeLayer.name;
                clone.SetActive(false);
                Renderer layerRenderer = clone.GetComponent<Renderer>();
                layerRenderer.enabled = true;
                layerRenderer.material = Instantiate(layerRenderer.material);
                layerRenderer.material.mainTexture = landscapeLayer.texture;
                Color color = layerRenderer.material.color;
                color.a = landscapeLayer.alpha;
                layerRenderer.material.color = color;
                landscapeLayer.instance = clone;
                landscapeLayers.Add(landscapeLayer.landscapeType, landscapeLayer);
                landscapeLayersClicked.Add(landscapeLayer.landscapeType, false);
            }
        }

        private void DeleteLandscapeLayersLookup()
        {
            foreach (var landscapeLayer in landscapeLayersLookup.landscapeLayersData)
            {
                Destroy(landscapeLayer.instance);
                landscapeLayers.Remove(landscapeLayer.landscapeType);
                landscapeLayersClicked.Remove(landscapeLayer.landscapeType);
            }
        }

        public void OnSetLandscapeLayerActive(LandscapeType landscapeType, bool active)
        {
            LandscapeLayersData layer = landscapeLayers.GetValueOrDefault(landscapeType, null);
            if (layer != null)
            {
                layer.instance.SetActive(active);
                if (step == 0 && active && !landscapeLayersClicked.GetValueOrDefault(landscapeType, true))
                {
                    landscapeLayersClicked[landscapeType] = true;
                    check = true;
                    audioSource.clip = audioClips[1 + (int)landscapeType];
                    audioSource.Play();
                    StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, true));
                    EventBus.Publish(EventId.LandscapeLayersMiniGameEvents.SetTogglesInteractable, false);
                }
            }
        }

        public void OnPositioningQuizEnded()
        {
            positioningQuizEnded = true;
            RequestScoreWindow();
        }

        public void OnRequestAbortWindow()
        {
            abortWindowOpen = true;
            if (audioSource.isPlaying)
            {
                audioWasPlaying = true;
                audioSource.Stop();
                StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, false));
            }
        }

        public void OnMiniGameResumed()
        {
            if (positioningQuizEnded)
            {
                positioningQuizEnded = false;
                StartCoroutine(positioningQuiz.StartPositioningQuizCoroutine(positioningQuiz.questions.Length - 1));
            }
            else
            {
                if (audioWasPlaying)
                {
                    audioWasPlaying = false;
                    audioSource.Play();
                    StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, true));
                }
                else
                {
                    switch (step)
                    {
                        case 0:
                            StartCoroutine(RequestFullScreenPromptPanel("", videoClip, audioClips[0]));
                            break;
                    }
                }
            }
            abortWindowOpen = false;
        }

        public void OnNextStep()
        {
            audioSource.Stop();
            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, false));
            if (!abortWindowOpen)
            {
                if (miniGameConfig.steps.Length <= step + 1)
                {
                    step = miniGameConfig.steps.Length - 1;
                }
                else
                {
                    switch (step)
                    {
                        case 0:
                            EventBus.Publish(EventId.LandscapeLayersMiniGameEvents.SetTogglesInteractable, true);
                            check = false;
                            StartCoroutine(positioningQuiz.StartPositioningQuizCoroutine());
                            break;
                    }
                    step++;
                }
            }
        }

        public void OnPreviousStep()
        {
            audioSource.Stop();
            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, false));
            step--;
            if (step < 0)
            {
                step = 0;
                DeleteLandscapeLayersLookup();
                LoadLandscapeLayersLookup();
                RequestStartWindow();
            }
            else
            {
                switch (step)
                {
                    case 0:
                        DeleteLandscapeLayersLookup();
                        LoadLandscapeLayersLookup();
                        StartCoroutine(RequestFullScreenPromptPanel("", videoClip, audioClips[0]));
                        break;
                }
            }
        }

        public void OnReplayInstruction()
        {
            if (step == 0)
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                    audioSource.Play();
                }
                else
                {
                    StartCoroutine(RequestFullScreenPromptPanel("", videoClip, audioClips[0]));
                }
            }
        }

        public void OnSkipAudioButtonPressed()
        {
            audioSource.Stop();
        }
    }

    public partial class EventId
    {
        public class LandscapeLayersMiniGameEvents
        {
            public const string SetLandscapeLayerActive = "SetLandscapeLayerActive";
            public const string SetTogglesInteractable = "SetTogglesInteractable";
        }
    }
}