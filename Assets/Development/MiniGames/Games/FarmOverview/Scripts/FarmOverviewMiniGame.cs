using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using InteractionSystem.Interactable;
using UnityEngine.Video;

namespace MiniGame.FarmOverview
{
    public class FarmOverviewMiniGame : AMiniGame
    {
        public GameObject[] farmPrefabs;
        public GameObject topography;
        public GameObject municipalities;
        public GameObject countyFramePongau;
        public GameObject countyFrameFlachgau;
        public Sprite[] sprites;
        public AudioClip[] audioClips;
        public AudioSource audioSource;
        private Dictionary<Interactable, bool> interacted = new Dictionary<Interactable, bool>();
        private GameObject firstInstance;
        private GameObject secondInstance;
        private Stack<FarmType> previousFarmTypes = new Stack<FarmType>();
        private Sprite sprite;
        private Texture2D diaryTexture1;
        private Texture2D diaryTexture2;
        private Texture2D snapshot;
        private Rect UIRectIncome = new Rect(-25.0f, Screen.height - 850.0f, 450.0f, 450.0f);
        private Rect UIRectCow = new Rect(-25.0f, Screen.height - 500.0f, 450.0f, 100.0f);
        private Vector2 paddingX = new Vector2(0.0f, 0.0f);
        private Vector2 paddingY = new Vector2(200.0f, 50.0f);
        private Rect snapshotUIRect = new Rect(-Screen.width / 8.0f, 37.5f, Screen.width * 3.0f / 4.0f, Screen.height * 3.0f / 4.0f - 25.0f);
        private Vector2 snapshotPaddingX = new Vector2(0.0f, 0.0f);
        private Vector2 snapshotPaddingY = new Vector2(400.0f, 100.0f);
        private Salzburg salzburg;
        public const string findCows1EndText = "Gefundene Kühe: {0}/40";
        public const string findCows2EndText = "Gefundene Kühe: {0}/10";
        public const string diaryText = "Bearbeite Aufgabe {0} im Lerntagebuch.";
        private int step = 0;
        private bool nextStepOnClose = false;
        private bool nextStepAfterAudio = false;
        private bool audioWasPlaying = false;
        private bool checkCompletionActive = false;
        private bool startAudioPlaying = false;
        private bool activateFinishButtonOnClose = false;
        private bool deactivateFinishButtonOnClose = false;
        private bool abortWindowOpen = false;
        private bool startWindowOpen = false;
        private bool popFarmType = false;
        private bool enablePhotoButton = false;
        private FederalUnit badHofgastein;
        private FederalUnit seekirchen;
        private bool takeSnapshot = false;
        private bool popUpActive = false;

        private void Awake()
        {
            Debug.Assert(farmPrefabs.Length == Enum.GetNames(typeof(FarmType)).Length, "FarmOverviewMiniGame needs the same amout of FarmTypes and prefabs");
            Debug.Assert(topography, "FarmOverviewMiniGame is missing a reference to a GameObject");
            Debug.Assert(municipalities, "FarmOverviewMiniGame is missing a reference to a GameObject");
            Debug.Assert(countyFramePongau, "FarmOverviewMiniGame is missing a reference to a GameObject");
            Debug.Assert(countyFrameFlachgau, "FarmOverviewMiniGame is missing a reference to a GameObject");
            Debug.Assert(sprites.Length == 7, "FarmOverviewMiniGame needs exactly 7 Sprites");
            Debug.Assert(audioClips.Length == 22, "FarmOverviewMiniGame needs exactly 22 AudioClips");
            Debug.Assert(audioSource, "FarmOverviewMiniGame is missing a reference to an AudioSource");

            if (diaryTexture1)
            {
                Destroy(diaryTexture1);
            }
            diaryTexture1 = new Texture2D((int)UIRectIncome.width + (int)paddingX.x + (int)paddingX.y, (int)UIRectIncome.height + (int)UIRectCow.height + (int)paddingY.x + (int)paddingY.y, TextureFormat.RGB24, false);

            Color[] fillColorArray = new Color[diaryTexture1.width * diaryTexture1.height];
            Array.Fill(fillColorArray, Color.white);
            diaryTexture1.SetPixels(fillColorArray);

            if (diaryTexture2)
            {
                Destroy(diaryTexture2);
            }
            diaryTexture2 = new Texture2D((int)UIRectIncome.width + (int)paddingX.x + (int)paddingX.y, (int)UIRectIncome.height + (int)UIRectCow.height + (int)paddingY.x + (int)paddingY.y, TextureFormat.RGB24, false);

            diaryTexture2.SetPixels(fillColorArray);
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

                if (sprite)
                {
                    Destroy(sprite);
                }
                sprite = Sprite.Create(snapshot, new Rect(0.0f, 0.0f, snapshot.width, snapshot.height), new Vector2(0.5f, 0.5f));

                windowText = "";
                popUpActive = true;
                RequestDiaryWindow("", "2C", 10.0f, sprite, null, audioClips[20], "", sprites[2], null, audioClips[21]);
                StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableVisualGuidance, true));
                seekirchen.HighlightTarget(false);
                badHofgastein.HighlightTarget(false);
            }
        }

        private void Update()
        {
            if (nextStepAfterAudio && !audioSource.isPlaying && !abortWindowOpen)
            {
                nextStepAfterAudio = false;
                StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.NextStep));
                StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, false));
            }
            if (enablePhotoButton && !audioSource.isPlaying && !abortWindowOpen)
            {
                enablePhotoButton = false;
                StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnablePhotoButton, true));
                StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, false));
            }
        }

        private IEnumerator RequestPromptCoroutine(string text = "", Sprite sprite = null, VideoClip videoClip = null, AudioClip audioClip = null)
        {
            yield return new WaitForSeconds(0.1f);
            popUpActive = true;
            RequestPromptWindow(text, 0.0f, sprite, videoClip, audioClip);
        }

        private IEnumerator IncomeSnapShot()
        {
            yield return new WaitForEndOfFrame();

            Sprite sprite;

            if (previousFarmTypes.Peek() == FarmType.PlainsFarm)
            {
                sprite = sprites[0];
                diaryTexture1.ReadPixels(new Rect(Screen.width - (int)UIRectIncome.width + UIRectIncome.position.x, UIRectIncome.position.y, (int)UIRectIncome.width, (int)UIRectIncome.height), (int)paddingX.x, (int)paddingY.x + (int)UIRectCow.height);
                diaryTexture1.Apply();
            }
            else
            {
                sprite = sprites[1];
                diaryTexture2.ReadPixels(new Rect(Screen.width - (int)UIRectIncome.width + UIRectIncome.position.x, UIRectIncome.position.y, (int)UIRectIncome.width, (int)UIRectIncome.height), (int)paddingX.x, (int)paddingY.x + (int)UIRectCow.height);
                diaryTexture2.Apply();
            }

            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableVisualGuidance, true));

            activateFinishButtonOnClose = true;

            windowText = "";
            StartCoroutine(RequestPromptCoroutine("", sprite, null, audioClips[8]));
            EventBus.Publish(EventId.FarmOverviewMiniGameEvents.SetMaxIncomeCount, 0);
        }

        private IEnumerator CowFindSnapShot(string diaryNumberText)
        {
            yield return new WaitForEndOfFrame();

            AudioClip audioClip;
            AudioClip promptAudioClip;

            if (previousFarmTypes.Peek() == FarmType.PlainsFarm)
            {
                diaryTexture1.ReadPixels(new Rect(Screen.width - (int)UIRectCow.width + UIRectCow.position.x, UIRectCow.position.y, (int)UIRectCow.width, (int)UIRectCow.height), (int)paddingX.x, (int)paddingY.x);
                diaryTexture1.Apply();

                if (sprite)
                {
                    Destroy(sprite);
                }
                sprite = Sprite.Create(diaryTexture1, new Rect(0.0f, 0.0f, diaryTexture1.width, diaryTexture1.height), new Vector2(0.5f, 0.5f));

                audioClip = audioClips[11];
                promptAudioClip = audioClips[12];
            }
            else
            {
                diaryTexture2.ReadPixels(new Rect(Screen.width - (int)UIRectCow.width + UIRectCow.position.x, UIRectCow.position.y, (int)UIRectCow.width, (int)UIRectCow.height), (int)paddingX.x, (int)paddingY.x);
                diaryTexture2.Apply();

                if (sprite)
                {
                    Destroy(sprite);
                }
                sprite = Sprite.Create(diaryTexture2, new Rect(0.0f, 0.0f, diaryTexture2.width, diaryTexture2.height), new Vector2(0.5f, 0.5f));

                audioClip = audioClips[13];
                promptAudioClip = audioClips[14];
            }

            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableVisualGuidance, true));

            EventBus.Publish(EventId.FarmOverviewMiniGameEvents.DeactivateUI);
            popUpActive = true;
            RequestDiaryWindow("", diaryNumberText, 10.0f, sprite, null, audioClip, "", sprites[2], null, promptAudioClip);
        }

        public void OnPrefabSpawned(int toggleIndex)
        {
            FarmType farmType = (FarmType)toggleIndex;

            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableVisualGuidance, true));
            salzburg.visualGuidanceTarget.gameObject.SetActive(true);

            firstInstance = Instantiate(farmPrefabs[(int)farmType]);
            firstInstance.transform.parent = transform;
            firstInstance.transform.localEulerAngles = Vector3.zero;
            firstInstance.transform.position = transform.position;
            firstInstance.transform.localScale = Vector3.one;

            previousFarmTypes.Push(farmType);
            Interactable[] interactables = firstInstance.GetComponentsInChildren<Interactable>(false);
            interacted.Clear();
            for (int i = 0; i < interactables.Length; i++)
            {
                Interactable interactable = interactables[i];
                interacted[interactable] = false;
                InteractableEventExtension interactableExtension;
                if (!interactable.TryGetComponent(out interactableExtension))
                {
                    interactableExtension = interactable.gameObject.AddComponent<InteractableEventExtension>();
                }
                interactableExtension.OnInteractEventParam.AddListener((target) =>
                {
                    StartCoroutine(CheckCompletion(target));
                });
            }

            StartCoroutine(PlayStartAudio(farmType));

            CowFindBehaviour[] cowFindBehaviours = firstInstance.GetComponentsInChildren<CowFindBehaviour>(true);

            foreach (CowFindBehaviour cowFindBehaviour in cowFindBehaviours)
            {
                cowFindBehaviour.transform.parent.gameObject.SetActive(false);
            }

            EventBus.Publish(EventId.FarmOverviewMiniGameEvents.SetMaxIncomeCount, interactables.Length);
        }

        public void OnMunicipalityTouch(string touchedName)
        {
            FederalUnit touched = salzburg.GetMunicipalityByMunicipalCode(touchedName);
            if (touched.name == badHofgastein.name && step == 6)
            {
                badHofgastein.HighlightTarget(true);
                nextStepOnClose = true;
                windowText = "";
                StartCoroutine(RequestPromptCoroutine("", sprites[4], null, audioClips[16]));
            }
            else if (touched.name == seekirchen.name && step == 7)
            {
                seekirchen.HighlightTarget(true);
                nextStepOnClose = true;
                windowText = "";
                StartCoroutine(RequestPromptCoroutine("", sprites[6], null, audioClips[18]));
            }
        }

        public void OnPhotoButtonPressed()
        {
            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.NextStep));
        }

        public void OnFinishCountingButtonPressed()
        {
            AudioClip audioClip;
            nextStepOnClose = true;
            int cowCount = 0;
            CowFindBehaviour[] cowFindBehaviours;
            if (step == 1)
            {
                cowFindBehaviours = firstInstance.GetComponentsInChildren<CowFindBehaviour>(true);
            }
            else
            {
                cowFindBehaviours = secondInstance.GetComponentsInChildren<CowFindBehaviour>(true);
            }

            foreach (CowFindBehaviour cowFindBehaviour in cowFindBehaviours)
            {
                if (cowFindBehaviour.interacted)
                {
                    cowCount++;
                }
            }
            if (previousFarmTypes.Peek() == FarmType.PlainsFarm)
            {
                windowText = string.Format(findCows1EndText, cowCount);
                audioClip = audioClips[9];
            }
            else
            {
                windowText = string.Format(findCows2EndText, cowCount);
                audioClip = audioClips[10];
            }
            StartCoroutine(RequestPromptCoroutine("", null, null, audioClip));
        }

        private IEnumerator CheckCompletion(Interactable target)
        {
            checkCompletionActive = true;
            interacted[target] = true;

            IncomePublishBehaviour incomePublishBehaviour = target.gameObject.GetComponent<IncomePublishBehaviour>();

            switch (incomePublishBehaviour.incomeSource)
            {
                case IncomeSource.AgricultureFulltime:
                    audioSource.clip = audioClips[2];
                    break;
                case IncomeSource.AgricultureParttime:
                    audioSource.clip = audioClips[4];
                    break;
                case IncomeSource.MainJob:
                    audioSource.clip = audioClips[5];
                    break;
                case IncomeSource.Renting:
                    audioSource.clip = audioClips[6];
                    break;
            }

            audioSource.Play();
            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, true));

            yield return new WaitForFixedUpdate();

            bool completion = true;
            foreach (KeyValuePair<Interactable, bool> keyValuePair in interacted)
            {
                completion &= keyValuePair.Value;
                if (!keyValuePair.Value)
                {
                    keyValuePair.Key.gameObject.SetActive(false);
                }
            }

            while (checkCompletionActive && (audioSource.isPlaying || abortWindowOpen))
            {
                yield return new WaitForFixedUpdate();
            }

            if (checkCompletionActive)
            {
                foreach (KeyValuePair<Interactable, bool> keyValuePair in interacted)
                {
                    if (!keyValuePair.Value)
                    {
                        keyValuePair.Key.gameObject.SetActive(true);
                    }
                }

                if (completion)
                {
                    nextStepAfterAudio = true;
                    if (previousFarmTypes.Peek() == FarmType.PlainsFarm)
                    {
                        audioSource.clip = audioClips[3];
                    }
                    else
                    {
                        audioSource.clip = audioClips[7];
                    }
                    audioSource.Play();
                    StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, true));
                }
                checkCompletionActive = false;
            }
        }

        private IEnumerator PlayStartAudio(FarmType farmType)
        {
            startAudioPlaying = true;

            if (farmType == FarmType.PlainsFarm)
            {
                audioSource.clip = audioClips[0];
            }
            else
            {
                audioSource.clip = audioClips[1];
            }

            audioSource.Play();
            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, true));

            yield return new WaitForFixedUpdate();

            foreach (KeyValuePair<Interactable, bool> keyValuePair in interacted)
            {
                if (!keyValuePair.Value)
                {
                    keyValuePair.Key.gameObject.SetActive(false);
                }
            }

            while (startAudioPlaying && (audioSource.isPlaying || abortWindowOpen))
            {
                yield return new WaitForFixedUpdate();
            }

            if (startAudioPlaying)
            {
                foreach (KeyValuePair<Interactable, bool> keyValuePair in interacted)
                {
                    if (!keyValuePair.Value)
                    {
                        keyValuePair.Key.gameObject.SetActive(true);
                    }
                }

                startAudioPlaying = false;
                StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, false));
            }
        }

        private void RemoveOldListeners(GameObject instance, bool setInactive = false)
        {
            InteractableEventExtension oldInteractable;
            if (instance)
            {
                if (instance.TryGetComponent(out oldInteractable))
                {
                    oldInteractable.OnInteractEvent.RemoveAllListeners();
                }
                if (setInactive)
                {
                    instance.gameObject.SetActive(false);
                }
            }
        }

        public void OnDiaryContinue()
        {
            popUpActive = false;
            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.NextStep));
        }

        public void OnDiaryWindowClosed()
        {
            popUpActive = false;
            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.PreviousStep));
        }

        public void OnPromptWindowClosed()
        {
            popUpActive = false;
            if (nextStepOnClose)
            {
                StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.NextStep));
            }
            if (activateFinishButtonOnClose && !deactivateFinishButtonOnClose)
            {
                activateFinishButtonOnClose = false;
                StartCoroutine(EventBus.PublishCoroutine(EventId.FarmOverviewMiniGameEvents.EnableFinishCountingButton, true));
            }
            if (deactivateFinishButtonOnClose)
            {
                deactivateFinishButtonOnClose = false;
                activateFinishButtonOnClose = false;
                StartCoroutine(EventBus.PublishCoroutine(EventId.FarmOverviewMiniGameEvents.EnableFinishCountingButton, false));
            }
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
            if (!startWindowOpen && (audioWasPlaying || nextStepAfterAudio))
            {
                audioWasPlaying = false;
                audioSource.Play();
                StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, true));
            }
            else if (!startWindowOpen && nextStepOnClose)
            {
                OnNextStep();
            }
            else
            {
                audioWasPlaying = false;
                nextStepAfterAudio = false;
                nextStepOnClose = false;
                startWindowOpen = false;
                Sprite sprite = null;
                switch (step)
                {
                    case 0:
                        StartCoroutine(PlayStartAudio(previousFarmTypes.Peek()));
                        break;
                    case 1:
                        if (previousFarmTypes.Peek() == FarmType.PlainsFarm)
                        {
                            sprite = sprites[0];
                        }
                        else
                        {
                            sprite = sprites[1];
                        }

                        windowText = "";
                        StartCoroutine(RequestPromptCoroutine("", sprite, null, audioClips[8]));
                        break;
                    case 3:
                        StartCoroutine(PlayStartAudio(previousFarmTypes.Peek()));
                        break;
                    case 4:
                        if (previousFarmTypes.Peek() == FarmType.PlainsFarm)
                        {
                            sprite = sprites[0];
                        }
                        else
                        {
                            sprite = sprites[1];
                        }

                        windowText = "";
                        StartCoroutine(RequestPromptCoroutine("", sprite, null, audioClips[8]));
                        break;
                    case 6:
                        windowText = "";
                        StartCoroutine(RequestPromptCoroutine("", sprites[3], null, audioClips[15]));
                        break;
                    case 7:
                        windowText = "";
                        StartCoroutine(RequestPromptCoroutine("", sprites[5], null, audioClips[17]));
                        break;
                    case 8:
                        audioSource.clip = audioClips[19];
                        audioSource.Play();
                        StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, true));
                        break;
                    case 9:
                        EventBus.Publish(MiniGame.EventId.MiniGameEvents.PreviousStep);
                        break;
                }
            }
        }

        public void OnNextStep()
        {
            popUpActive = false;
            if (!abortWindowOpen)
            {
                audioSource.Stop();
                bool enableSkipAudioButton = false;

                nextStepOnClose = false;
                nextStepAfterAudio = false;
                checkCompletionActive = false;
                startAudioPlaying = false;
                CowFindBehaviour[] cowFindBehaviours;
                IncomePublishBehaviour[] incomePublishBehaviours;
                Interactable[] interactables;

                int cowCount = 0;

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
                            deactivateFinishButtonOnClose = false;

                            RemoveOldListeners(firstInstance);

                            incomePublishBehaviours = firstInstance.GetComponentsInChildren<IncomePublishBehaviour>(true);

                            foreach (IncomePublishBehaviour incomePublishBehaviour in incomePublishBehaviours)
                            {
                                incomePublishBehaviour.OnInteract();
                            }

                            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableVisualGuidance, false));

                            StartCoroutine(IncomeSnapShot());

                            cowFindBehaviours = firstInstance.GetComponentsInChildren<CowFindBehaviour>(true);

                            foreach (CowFindBehaviour cowFindBehaviour in cowFindBehaviours)
                            {
                                if (!cowFindBehaviour.interacted)
                                {
                                    cowFindBehaviour.gameObject.SetActive(true);
                                    cowFindBehaviour.transform.parent.gameObject.SetActive(true);
                                }
                                else
                                {
                                    cowCount++;
                                }
                            }

                            EventBus.Publish(EventId.FarmOverviewMiniGameEvents.SetMaxFindCow, cowFindBehaviours.Length);
                            EventBus.Publish(EventId.FarmOverviewMiniGameEvents.SetFindCow, cowCount);
                            EventBus.Publish(MiniGame.EventId.MiniGameEvents.EnableMarkerTrackingIndicator, false);
                            break;
                        case 1:
                            deactivateFinishButtonOnClose = true;
                            StartCoroutine(EventBus.PublishCoroutine(EventId.FarmOverviewMiniGameEvents.EnableFinishCountingButton, false));
                            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableVisualGuidance, false));
                            windowText = "";

                            if (previousFarmTypes.Peek() == FarmType.PlainsFarm)
                            {
                                EventBus.Publish(MiniGame.EventId.MiniGameEvents.OverrideText, string.Format(diaryText, "2B"));
                                StartCoroutine(CowFindSnapShot("2B"));
                            }
                            else
                            {
                                EventBus.Publish(MiniGame.EventId.MiniGameEvents.OverrideText, string.Format(diaryText, "2A"));
                                StartCoroutine(CowFindSnapShot("2A"));
                            }

                            EventBus.Publish(MiniGame.EventId.MiniGameEvents.EnableMarkerTrackingIndicator, true);
                            break;
                        case 2:
                            int farmType = 0;
                            while (previousFarmTypes.Contains((FarmType)farmType))
                            {
                                farmType++;
                            }

                            previousFarmTypes.Push((FarmType)farmType);

                            StartCoroutine(PlayStartAudio(previousFarmTypes.Peek()));
                            enableSkipAudioButton = true;

                            RemoveOldListeners(firstInstance, true);

                            if (!secondInstance)
                            {
                                secondInstance = Instantiate(farmPrefabs[farmType], transform);
                                secondInstance.transform.localEulerAngles = Vector3.zero;
                                secondInstance.transform.position = transform.position;
                                secondInstance.transform.localScale = Vector3.one;
                            }
                            else
                            {
                                secondInstance.SetActive(true);
                            }

                            interactables = secondInstance.GetComponentsInChildren<Interactable>(false);
                            interacted.Clear();

                            for (int i = 0; i < interactables.Length; i++)
                            {
                                Interactable interactable = interactables[i];
                                interacted[interactable] = false;
                                InteractableEventExtension interactableExtension;
                                if (!interactable.TryGetComponent(out interactableExtension))
                                {
                                    interactableExtension = interactable.gameObject.AddComponent<InteractableEventExtension>();
                                }
                                interactableExtension.OnInteractEventParam.AddListener((target) =>
                                {
                                    StartCoroutine(CheckCompletion(target));
                                });
                            }

                            cowFindBehaviours = secondInstance.GetComponentsInChildren<CowFindBehaviour>(true);

                            foreach (CowFindBehaviour cowFindBehaviour in cowFindBehaviours)
                            {
                                cowFindBehaviour.transform.parent.gameObject.SetActive(false);
                            }

                            EventBus.Publish(EventId.FarmOverviewMiniGameEvents.SetMaxIncomeCount, interactables.Length);
                            break;
                        case 3:
                            deactivateFinishButtonOnClose = false;

                            RemoveOldListeners(secondInstance);

                            incomePublishBehaviours = secondInstance.GetComponentsInChildren<IncomePublishBehaviour>(true);

                            foreach (IncomePublishBehaviour incomePublishBehaviour in incomePublishBehaviours)
                            {
                                incomePublishBehaviour.OnInteract();
                            }

                            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableVisualGuidance, false));

                            StartCoroutine(IncomeSnapShot());

                            cowFindBehaviours = secondInstance.GetComponentsInChildren<CowFindBehaviour>(true);

                            foreach (CowFindBehaviour cowFindBehaviour in cowFindBehaviours)
                            {
                                if (!cowFindBehaviour.interacted)
                                {
                                    cowFindBehaviour.gameObject.SetActive(true);
                                    cowFindBehaviour.transform.parent.gameObject.SetActive(true);
                                }
                                else
                                {
                                    cowCount++;
                                }
                            }

                            EventBus.Publish(EventId.FarmOverviewMiniGameEvents.SetMaxFindCow, cowFindBehaviours.Length);
                            EventBus.Publish(EventId.FarmOverviewMiniGameEvents.SetFindCow, cowCount);
                            EventBus.Publish(MiniGame.EventId.MiniGameEvents.EnableMarkerTrackingIndicator, false);
                            break;
                        case 4:
                            deactivateFinishButtonOnClose = true;
                            StartCoroutine(EventBus.PublishCoroutine(EventId.FarmOverviewMiniGameEvents.EnableFinishCountingButton, false));
                            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableVisualGuidance, false));
                            windowText = "";

                            if (previousFarmTypes.Peek() == FarmType.PlainsFarm)
                            {
                                EventBus.Publish(MiniGame.EventId.MiniGameEvents.OverrideText, string.Format(diaryText, "2B"));
                                StartCoroutine(CowFindSnapShot("2B"));
                            }
                            else
                            {
                                EventBus.Publish(MiniGame.EventId.MiniGameEvents.OverrideText, string.Format(diaryText, "2A"));
                                StartCoroutine(CowFindSnapShot("2A"));
                            }

                            EventBus.Publish(MiniGame.EventId.MiniGameEvents.EnableMarkerTrackingIndicator, true);
                            break;
                        case 5:
                            countyFramePongau.SetActive(true);
                            RemoveOldListeners(secondInstance, true);

                            windowText = "";
                            StartCoroutine(RequestPromptCoroutine("", sprites[3], null, audioClips[15]));
                            municipalities.SetActive(true);
                            foreach (MeshCollider collider in salzburg.GetComponentsInChildren<MeshCollider>())
                            {
                                collider.enabled = true;
                            }
                            break;
                        case 6:
                            countyFramePongau.SetActive(false);
                            countyFrameFlachgau.SetActive(true);
                            badHofgastein.HighlightTarget(true);
                            seekirchen.HighlightTarget(false);

                            windowText = "";
                            StartCoroutine(RequestPromptCoroutine("", sprites[5], null, audioClips[17]));
                            break;
                        case 7:
                            countyFrameFlachgau.SetActive(false);
                            badHofgastein.HighlightTarget(true);
                            seekirchen.HighlightTarget(true);
                            municipalities.SetActive(false);
                            foreach (MeshCollider collider in salzburg.GetComponentsInChildren<MeshCollider>())
                            {
                                collider.enabled = false;
                            }
                            topography.SetActive(true);
                            audioSource.clip = audioClips[19];
                            audioSource.Play();
                            enablePhotoButton = true;
                            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableVisualGuidance, false));
                            enableSkipAudioButton = true;
                            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, true));
                            break;
                        case 8:
                            enablePhotoButton = false;
                            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnablePhotoButton, false));
                            takeSnapshot = true;
                            break;
                    }
                    step++;
                }
                if (!enableSkipAudioButton)
                {
                    StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, false));
                }
            }
        }

        public void OnPreviousStep()
        {
            popUpActive = false;
            audioSource.Stop();
            bool enableSkipAudioButton = false;

            nextStepOnClose = false;
            nextStepAfterAudio = false;
            checkCompletionActive = false;
            startAudioPlaying = false;
            step--;
            CowFindBehaviour[] cowFindBehaviours;
            IncomePublishBehaviour[] incomePublishBehaviours;
            Interactable[] interactables;

            int cowCount = 0;

            if (step < 0)
            {
                RemoveOldListeners(firstInstance);

                incomePublishBehaviours = firstInstance.GetComponentsInChildren<IncomePublishBehaviour>(true);

                foreach (IncomePublishBehaviour incomePublishBehaviour in incomePublishBehaviours)
                {
                    incomePublishBehaviour.interacted = false;
                    incomePublishBehaviour.gameObject.SetActive(true);
                }

                interactables = firstInstance.GetComponentsInChildren<Interactable>(false);
                interacted.Clear();
                for (int i = 0; i < interactables.Length; i++)
                {
                    Interactable interactable = interactables[i];
                    interacted[interactable] = false;
                    InteractableEventExtension interactableExtension;
                    if (!interactable.TryGetComponent(out interactableExtension))
                    {
                        interactableExtension = interactable.gameObject.AddComponent<InteractableEventExtension>();
                    }
                    interactableExtension.OnInteractEventParam.AddListener((target) =>
                    {
                        StartCoroutine(CheckCompletion(target));
                    });
                }
                EventBus.Publish(EventId.FarmOverviewMiniGameEvents.SetMaxIncomeCount, interactables.Length);

                step = 0;
                RequestStartWindow();
            }
            else
            {
                Sprite sprite = null;
                switch (step)
                {
                    case 0:
                        StartCoroutine(EventBus.PublishCoroutine(EventId.FarmOverviewMiniGameEvents.EnableFinishCountingButton, false));

                        if (previousFarmTypes.Peek() == FarmType.PlainsFarm)
                        {
                            audioSource.clip = audioClips[0];
                        }
                        else
                        {
                            audioSource.clip = audioClips[1];
                        }

                        if (!abortWindowOpen)
                        {
                            deactivateFinishButtonOnClose = true;
                            StartCoroutine(PlayStartAudio(previousFarmTypes.Peek()));
                            enableSkipAudioButton = true;
                        }
                        RemoveOldListeners(firstInstance);

                        cowFindBehaviours = firstInstance.GetComponentsInChildren<CowFindBehaviour>(true);

                        foreach (CowFindBehaviour cowFindBehaviour in cowFindBehaviours)
                        {
                            cowFindBehaviour.transform.parent.gameObject.SetActive(false);
                            cowFindBehaviour.gameObject.SetActive(false);
                        }

                        incomePublishBehaviours = firstInstance.GetComponentsInChildren<IncomePublishBehaviour>(true);

                        foreach (IncomePublishBehaviour incomePublishBehaviour in incomePublishBehaviours)
                        {
                            incomePublishBehaviour.interacted = false;
                            incomePublishBehaviour.gameObject.SetActive(true);
                        }

                        interactables = firstInstance.GetComponentsInChildren<Interactable>(false);
                        interacted.Clear();
                        for (int i = 0; i < interactables.Length; i++)
                        {
                            Interactable interactable = interactables[i];
                            interacted[interactable] = false;
                            InteractableEventExtension interactableExtension;
                            if (!interactable.TryGetComponent(out interactableExtension))
                            {
                                interactableExtension = interactable.gameObject.AddComponent<InteractableEventExtension>();
                            }
                            interactableExtension.OnInteractEventParam.AddListener((target) =>
                            {
                                StartCoroutine(CheckCompletion(target));
                            });
                        }
                        EventBus.Publish(EventId.FarmOverviewMiniGameEvents.SetMaxIncomeCount, interactables.Length);
                        EventBus.Publish(MiniGame.EventId.MiniGameEvents.EnableMarkerTrackingIndicator, true);
                        break;
                    case 1:
                        if (previousFarmTypes.Peek() == FarmType.PlainsFarm)
                        {
                            sprite = sprites[0];
                        }
                        else
                        {
                            sprite = sprites[1];
                        }

                        windowText = "";
                        if (!abortWindowOpen)
                        {
                            StartCoroutine(RequestPromptCoroutine("", sprite, null, audioClips[8]));
                        }
                        RemoveOldListeners(secondInstance, true);

                        if (popFarmType)
                        {
                            previousFarmTypes.Pop();
                            popFarmType = false;
                        }

                        firstInstance.SetActive(true);

                        cowFindBehaviours = firstInstance.GetComponentsInChildren<CowFindBehaviour>(true);

                        foreach (CowFindBehaviour cowFindBehaviour in cowFindBehaviours)
                        {
                            if (!cowFindBehaviour.interacted)
                            {
                                cowFindBehaviour.gameObject.SetActive(true);
                                cowFindBehaviour.transform.parent.gameObject.SetActive(true);
                            }
                            else
                            {
                                cowCount++;
                            }
                        }

                        incomePublishBehaviours = firstInstance.GetComponentsInChildren<IncomePublishBehaviour>(true);

                        foreach (IncomePublishBehaviour incomePublishBehaviour in incomePublishBehaviours)
                        {
                            incomePublishBehaviour.interacted = true;
                            incomePublishBehaviour.gameObject.SetActive(false);
                        }

                        deactivateFinishButtonOnClose = false;
                        StartCoroutine(EventBus.PublishCoroutine(EventId.FarmOverviewMiniGameEvents.EnableFinishCountingButton, true));
                        EventBus.Publish(EventId.FarmOverviewMiniGameEvents.SetMaxIncomeCount, 0);
                        EventBus.Publish(EventId.FarmOverviewMiniGameEvents.SetMaxFindCow, cowFindBehaviours.Length);
                        EventBus.Publish(EventId.FarmOverviewMiniGameEvents.SetFindCow, cowCount);
                        EventBus.Publish(MiniGame.EventId.MiniGameEvents.EnableMarkerTrackingIndicator, false);
                        break;
                    case 2:
                        popFarmType = true;

                        incomePublishBehaviours = secondInstance.GetComponentsInChildren<IncomePublishBehaviour>(true);

                        foreach (IncomePublishBehaviour incomePublishBehaviour in incomePublishBehaviours)
                        {
                            incomePublishBehaviour.interacted = false;
                            incomePublishBehaviour.gameObject.SetActive(true);
                        }

                        EventBus.Publish(MiniGame.EventId.MiniGameEvents.PreviousStep);
                        break;
                    case 3:
                        StartCoroutine(EventBus.PublishCoroutine(EventId.FarmOverviewMiniGameEvents.EnableFinishCountingButton, false));

                        if (previousFarmTypes.Peek() == FarmType.PlainsFarm)
                        {
                            audioSource.clip = audioClips[0];
                        }
                        else
                        {
                            audioSource.clip = audioClips[1];
                        }

                        if (!abortWindowOpen)
                        {
                            deactivateFinishButtonOnClose = true;
                            StartCoroutine(PlayStartAudio(previousFarmTypes.Peek()));
                            enableSkipAudioButton = true;
                        }
                        RemoveOldListeners(secondInstance);

                        cowFindBehaviours = secondInstance.GetComponentsInChildren<CowFindBehaviour>(true);

                        foreach (CowFindBehaviour cowFindBehaviour in cowFindBehaviours)
                        {
                            cowFindBehaviour.transform.parent.gameObject.SetActive(false);
                            cowFindBehaviour.gameObject.SetActive(false);
                        }

                        incomePublishBehaviours = secondInstance.GetComponentsInChildren<IncomePublishBehaviour>(true);

                        foreach (IncomePublishBehaviour incomePublishBehaviour in incomePublishBehaviours)
                        {
                            incomePublishBehaviour.interacted = false;
                            incomePublishBehaviour.gameObject.SetActive(true);
                        }

                        interactables = secondInstance.GetComponentsInChildren<Interactable>(false);
                        interacted.Clear();
                        for (int i = 0; i < interactables.Length; i++)
                        {
                            Interactable interactable = interactables[i];
                            interacted[interactable] = false;
                            InteractableEventExtension interactableExtension;
                            if (!interactable.TryGetComponent(out interactableExtension))
                            {
                                interactableExtension = interactable.gameObject.AddComponent<InteractableEventExtension>();
                            }
                            interactableExtension.OnInteractEventParam.AddListener((target) =>
                            {
                                StartCoroutine(CheckCompletion(target));
                            });
                        }
                        EventBus.Publish(EventId.FarmOverviewMiniGameEvents.SetMaxIncomeCount, interactables.Length);
                        EventBus.Publish(MiniGame.EventId.MiniGameEvents.EnableMarkerTrackingIndicator, true);
                        break;
                    case 4:
                        seekirchen.HighlightTarget(false);
                        badHofgastein.HighlightTarget(false);
                        municipalities.SetActive(false);
                        foreach (MeshCollider collider in salzburg.GetComponentsInChildren<MeshCollider>())
                        {
                            collider.enabled = false;
                        }

                        if (previousFarmTypes.Peek() == FarmType.PlainsFarm)
                        {
                            sprite = sprites[0];
                        }
                        else
                        {
                            sprite = sprites[1];
                        }

                        windowText = "";
                        if (!abortWindowOpen)
                        {
                            StartCoroutine(RequestPromptCoroutine("", sprite, null, audioClips[8]));
                        }

                        secondInstance.SetActive(true);

                        cowFindBehaviours = secondInstance.GetComponentsInChildren<CowFindBehaviour>(true);

                        foreach (CowFindBehaviour cowFindBehaviour in cowFindBehaviours)
                        {
                            if (!cowFindBehaviour.interacted)
                            {
                                cowFindBehaviour.gameObject.SetActive(true);
                                cowFindBehaviour.transform.parent.gameObject.SetActive(true);
                            }
                            else
                            {
                                cowCount++;
                            }
                        }

                        incomePublishBehaviours = secondInstance.GetComponentsInChildren<IncomePublishBehaviour>(true);

                        foreach (IncomePublishBehaviour incomePublishBehaviour in incomePublishBehaviours)
                        {
                            incomePublishBehaviour.interacted = true;
                            incomePublishBehaviour.gameObject.SetActive(false);
                        }

                        deactivateFinishButtonOnClose = false;
                        StartCoroutine(EventBus.PublishCoroutine(EventId.FarmOverviewMiniGameEvents.EnableFinishCountingButton, true));
                        EventBus.Publish(EventId.FarmOverviewMiniGameEvents.SetMaxIncomeCount, 0);
                        EventBus.Publish(EventId.FarmOverviewMiniGameEvents.SetMaxFindCow, cowFindBehaviours.Length);
                        EventBus.Publish(EventId.FarmOverviewMiniGameEvents.SetFindCow, cowCount);
                        EventBus.Publish(MiniGame.EventId.MiniGameEvents.EnableMarkerTrackingIndicator, false);
                        break;
                    case 5:
                        countyFramePongau.SetActive(false);
                        EventBus.Publish(MiniGame.EventId.MiniGameEvents.PreviousStep);
                        break;
                    case 6:
                        countyFramePongau.SetActive(true);
                        countyFrameFlachgau.SetActive(false);
                        badHofgastein.HighlightTarget(false);
                        seekirchen.HighlightTarget(false);

                        windowText = "";
                        if (!abortWindowOpen)
                        {
                            StartCoroutine(RequestPromptCoroutine("", sprites[3], null, audioClips[15]));
                        }
                        break;
                    case 7:
                        enablePhotoButton = false;
                        countyFrameFlachgau.SetActive(true);
                        salzburg.visualGuidanceTarget.gameObject.SetActive(true);
                        topography.SetActive(false);
                        StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnablePhotoButton, false));
                        StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableVisualGuidance, true));
                        badHofgastein.HighlightTarget(true);
                        seekirchen.HighlightTarget(false);

                        windowText = "";
                        if (!abortWindowOpen)
                        {
                            StartCoroutine(RequestPromptCoroutine("", sprites[5], null, audioClips[17]));
                        }
                        municipalities.SetActive(true);
                        foreach (MeshCollider collider in salzburg.GetComponentsInChildren<MeshCollider>())
                        {
                            collider.enabled = true;
                        }
                        break;
                    case 8:
                        EventBus.Publish(MiniGame.EventId.MiniGameEvents.PreviousStep);
                        break;
                }
            }

            if (!enableSkipAudioButton)
            {
                StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, false));
            }
        }

        public void OnReplayInstruction()
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
                audioSource.Play();
            }
            else
            {
                bool enableSkipAudioButton = false;
                if (!popUpActive)
                {
                    Sprite sprite = null;
                    switch (step)
                    {
                        case 0:
                            StartCoroutine(PlayStartAudio(previousFarmTypes.Peek()));
                            enableSkipAudioButton = true;
                            break;
                        case 1:
                            if (previousFarmTypes.Peek() == FarmType.PlainsFarm)
                            {
                                sprite = sprites[0];
                            }
                            else
                            {
                                sprite = sprites[1];
                            }

                            windowText = "";
                            StartCoroutine(RequestPromptCoroutine("", sprite, null, audioClips[8]));
                            break;
                        case 3:
                            StartCoroutine(PlayStartAudio(previousFarmTypes.Peek()));
                            enableSkipAudioButton = true;
                            break;
                        case 4:
                            if (previousFarmTypes.Peek() == FarmType.PlainsFarm)
                            {
                                sprite = sprites[0];
                            }
                            else
                            {
                                sprite = sprites[1];
                            }

                            windowText = "";
                            StartCoroutine(RequestPromptCoroutine("", sprite, null, audioClips[8]));
                            break;
                        case 6:
                            windowText = "";
                            StartCoroutine(RequestPromptCoroutine("", sprites[3], null, audioClips[15]));
                            break;
                        case 7:
                            windowText = "";
                            StartCoroutine(RequestPromptCoroutine("", sprites[5], null, audioClips[17]));
                            break;
                        case 8:
                            enableSkipAudioButton = true;
                            audioSource.clip = audioClips[19];
                            audioSource.Play();
                            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, true));
                            break;
                    }
                }

                if (!enableSkipAudioButton)
                {
                    StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableSkipAudioButton, false));
                }
            }
        }

        public void OnSkipAudioButtonPressed()
        {
            audioSource.Stop();
        }

        public override void AbortMiniGame()
        {
            enablePhotoButton = false;
            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnablePhotoButton, false));
            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableVisualGuidance, true));
            salzburg.visualGuidanceTarget.gameObject.SetActive(true);
            municipalities.SetActive(false);
            foreach (MeshCollider collider in salzburg.GetComponentsInChildren<MeshCollider>())
            {
                collider.enabled = false;
            }
            seekirchen.HighlightTarget(false);
            badHofgastein.HighlightTarget(false);
            topography.SetActive(false);
            RemoveOldListeners(firstInstance);
            RemoveOldListeners(secondInstance);
            interacted.Clear();
            previousFarmTypes.Clear();
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.NextStep, "OnNextStep");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.DiaryContinue, "OnDiaryContinue");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.DiaryWindowClosed, "OnDiaryWindowClosed");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.PromptWindowClosed, "OnPromptWindowClosed");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.RequestAbortWindow, "OnRequestAbortWindow");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.RequestStartWindow, "OnRequestStartWindow");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.MunicipalityTouch, "OnMunicipalityTouch");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.PhotoButtonPressed, "OnPhotoButtonPressed");
            EventBus.SaveDeregisterCallback(this, EventId.FarmOverviewMiniGameEvents.PrefabSpawned, "OnPrefabSpawned");
            EventBus.SaveDeregisterCallback(this, EventId.FarmOverviewMiniGameEvents.FinishCountingButtonPressed, "OnFinishCountingButtonPressed");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");
            EventBus.Publish(EventId.FarmOverviewMiniGameEvents.EnableFinishCountingButton, false);
            salzburg = null;
            seekirchen = null;
            badHofgastein = null;
        }

        public override void ExitMiniGame()
        {
            enablePhotoButton = false;
            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnablePhotoButton, false));
            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableVisualGuidance, true));
            salzburg.visualGuidanceTarget.gameObject.SetActive(true);
            municipalities.SetActive(false);
            foreach (MeshCollider collider in salzburg.GetComponentsInChildren<MeshCollider>())
            {
                collider.enabled = false;
            }
            seekirchen.HighlightTarget(false);
            badHofgastein.HighlightTarget(false);
            topography.SetActive(false);
            RemoveOldListeners(firstInstance);
            RemoveOldListeners(secondInstance);
            interacted.Clear();
            previousFarmTypes.Clear();
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.NextStep, "OnNextStep");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.DiaryContinue, "OnDiaryContinue");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.DiaryWindowClosed, "OnDiaryWindowClosed");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.PromptWindowClosed, "OnPromptWindowClosed");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.RequestAbortWindow, "OnRequestAbortWindow");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.RequestStartWindow, "OnRequestStartWindow");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.MunicipalityTouch, "OnMunicipalityTouch");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.PhotoButtonPressed, "OnPhotoButtonPressed");
            EventBus.SaveDeregisterCallback(this, EventId.FarmOverviewMiniGameEvents.PrefabSpawned, "OnPrefabSpawned");
            EventBus.SaveDeregisterCallback(this, EventId.FarmOverviewMiniGameEvents.FinishCountingButtonPressed, "OnFinishCountingButtonPressed");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");
            EventBus.Publish(EventId.FarmOverviewMiniGameEvents.EnableFinishCountingButton, false);
            salzburg = null;
            seekirchen = null;
            badHofgastein = null;
        }

        public override void StartMiniGame()
        {
            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.EnableVisualGuidance, false));

            salzburg = FindObjectOfType<Salzburg>();

            salzburg.visualGuidanceTarget.gameObject.SetActive(false);

            if (salzburg)
            {
                seekirchen = salzburg.GetMunicipalityByMunicipalCode("50339");
                badHofgastein = salzburg.GetMunicipalityByMunicipalCode("50402");

                EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
                EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.NextStep, "OnNextStep");
                EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.DiaryContinue, "OnDiaryContinue");
                EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.DiaryWindowClosed, "OnDiaryWindowClosed");
                EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.PromptWindowClosed, "OnPromptWindowClosed");
                EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.RequestAbortWindow, "OnRequestAbortWindow");
                EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.RequestStartWindow, "OnRequestStartWindow");
                EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
                EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.MunicipalityTouch, "OnMunicipalityTouch");
                EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.PhotoButtonPressed, "OnPhotoButtonPressed");
                EventBus.SaveRegisterCallback(this, EventId.FarmOverviewMiniGameEvents.PrefabSpawned, "OnPrefabSpawned");
                EventBus.SaveRegisterCallback(this, EventId.FarmOverviewMiniGameEvents.FinishCountingButtonPressed, "OnFinishCountingButtonPressed");
                EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");
                EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");
            }
            else
            {
                Debug.LogError("Salzburg is null but required for minigame!");
            }
        }

        private void OnDestroy()
        {
            if (sprite)
            {
                Destroy(sprite);
            }

            if (diaryTexture1)
            {
                Destroy(diaryTexture1);
            }

            if (diaryTexture2)
            {
                Destroy(diaryTexture2);
            }

            if (diaryTexture1)
            {
                Destroy(diaryTexture1);
            }

            if (diaryTexture2)
            {
                Destroy(diaryTexture2);
            }

            if (snapshot)
            {
                Destroy(snapshot);
            }
        }
    }

    public enum FarmType
    {
        MountainFarm,
        PlainsFarm
    }

    public enum IncomeSource
    {
        AgricultureFulltime,
        AgricultureParttime,
        MainJob,
        Renting
    }

    public static partial class EventId
    {
        public static class FarmOverviewMiniGameEvents
        {
            public const string PrefabSpawned = "PrefabSpawned";
            public const string SetMaxIncomeCount = "SetMaxIncomeCount";
            public const string AddIncome = "AddIncome";
            public const string SetMaxFindCow = "SetMaxFindCow";
            public const string SetFindCow = "SetFindCow";
            public const string FindCow = "FindCow";
            public const string DeactivateUI = "DeactivateUI";
            public const string EnableFinishCountingButton = "EnableFinishCountingButton";
            public const string FinishCountingButtonPressed = "FinishCountingButtonPressed";
        }
    }
}