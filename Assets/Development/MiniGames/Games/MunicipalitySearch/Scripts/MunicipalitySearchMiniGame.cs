using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

namespace MiniGame.MunicipalitySearch
{
    public class MunicipalitySearchMiniGame : AMiniGame
    {
        public Sprite[] sprites;
        public VideoClip[] videoClips;
        public AudioClip[] audioClips;
        private Texture2D snapshot;
        private Rect snapshotUIRect = new Rect(-Screen.width / 8.0f, 37.5f, Screen.width * 3.0f / 4.0f, Screen.height * 3.0f / 4.0f - 25.0f);
        private Vector2 snapshotPaddingX = new Vector2(0.0f, 0.0f);
        private Vector2 snapshotPaddingY = new Vector2(400.0f, 100.0f);
        private Salzburg salzburg;
        private FederalUnit currentHighlight;
        private string currentName;
        private bool takeSnapshot = false;
        private bool abortWindowOpen = false;
        private Sprite snapshotSprite;
        private int step = 0;
        private int tutorialSubstep = 0;
        private bool popUpActive = false;

        private void Awake()
        {
            Debug.Assert(sprites.Length == 1, "MunicipalitySearchMiniGame needs exactly 1 Sprites");
            Debug.Assert(videoClips.Length == 5, "MunicipalitySearchMiniGame needs exactly 5 VideoClips");
            Debug.Assert(audioClips.Length == 8, "MunicipalitySearchMiniGame needs exactly 8 AudioClips");
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

                FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[4], audioClips[5]);

                if (currentHighlight)
                {
                    currentHighlight.HighlightTarget(false);
                }
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableVisualGuidance, true));
            }
        }

        private IEnumerator RequestQuestionCoroutine(string text = "", AudioClip audioClip = null)
        {
            yield return new WaitForSeconds(0.1f);

            popUpActive = true;
            RequestQuestionWindow(text, 0.0f, null, null, audioClip);
        }

        public override void AbortMiniGame()
        {
            tutorialSubstep = 0;
            foreach (MeshCollider collider in salzburg.GetComponentsInChildren<MeshCollider>())
            {
                collider.enabled = false;
            }
            if (currentHighlight)
            {
                currentHighlight.HighlightTarget(false);
            }
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnablePhotoButton, false));
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableVisualGuidance, true));
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.MunicipalityTouch, "OnMunicipalityTouch");
            EventBus.SaveDeregisterCallback(this, FullScreen.EventId.UIEvents.FullScreenPromptPanelClosed, "OnFullScreenPromptPanelClosed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.QuestionWindowYes, "OnQuestionWindowYes");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.QuestionWindowNo, "OnQuestionWindowNo");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.DiaryContinue, "OnDiaryContinue");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.DiaryWindowClosed, "OnDiaryWindowClosed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.PhotoButtonPressed, "OnPhotoButtonPressed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.RequestAbortWindow, "OnRequestAbortWindow");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.NextStep, "OnNextStep");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");
            salzburg = null;
        }

        public override void ExitMiniGame()
        {
            tutorialSubstep = 0;
            foreach (MeshCollider collider in salzburg.GetComponentsInChildren<MeshCollider>())
            {
                collider.enabled = false;
            }
            if (currentHighlight)
            {
                currentHighlight.HighlightTarget(false);
            }
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnablePhotoButton, false));
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableVisualGuidance, true));
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.MunicipalityTouch, "OnMunicipalityTouch");
            EventBus.SaveDeregisterCallback(this, FullScreen.EventId.UIEvents.FullScreenPromptPanelClosed, "OnFullScreenPromptPanelClosed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.QuestionWindowYes, "OnQuestionWindowYes");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.QuestionWindowNo, "OnQuestionWindowNo");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.DiaryContinue, "OnDiaryContinue");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.DiaryWindowClosed, "OnDiaryWindowClosed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.PhotoButtonPressed, "OnPhotoButtonPressed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.RequestAbortWindow, "OnRequestAbortWindow");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.NextStep, "OnNextStep");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");
            salzburg = null;
        }

        public override void StartMiniGame()
        {
            salzburg = FindObjectOfType<Salzburg>();

            if (salzburg)
            {
                EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.MunicipalityTouch, "OnMunicipalityTouch");
                EventBus.SaveRegisterCallback(this, FullScreen.EventId.UIEvents.FullScreenPromptPanelClosed, "OnFullScreenPromptPanelClosed");
                EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.QuestionWindowYes, "OnQuestionWindowYes");
                EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.QuestionWindowNo, "OnQuestionWindowNo");
                EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.DiaryContinue, "OnDiaryContinue");
                EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.DiaryWindowClosed, "OnDiaryWindowClosed");
                EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.PhotoButtonPressed, "OnPhotoButtonPressed");
                EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.RequestAbortWindow, "OnRequestAbortWindow");
                EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
                EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
                EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.NextStep, "OnNextStep");
                EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");

                foreach (MeshCollider collider in salzburg.GetComponentsInChildren<MeshCollider>())
                {
                    collider.enabled = true;
                }

                FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[0], audioClips[0]);
            }
            else
            {
                Debug.LogError("Salzburg is null but required for minigame!");
            }
        }

        public void OnMunicipalityTouch(string touchedName)
        {
            currentName = touchedName;
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.NextStep));
        }

        public void OnFullScreenPromptPanelClosed()
        {
            if (!abortWindowOpen)
            {
                switch (step)
                {
                    case 0:
                        switch (tutorialSubstep)
                        {
                            case 0:
                                FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[1], audioClips[1]);
                                break;
                            case 1:
                                FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[2], audioClips[2]);
                                break;
                        }
                        if (tutorialSubstep < 2)
                        {
                            tutorialSubstep++;
                        }
                        break;
                    case 2:
                        StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnablePhotoButton, true));
                        break;
                    case 3:
                        windowText = "";
                        popUpActive = true;
                        RequestDiaryWindow("", "1A", 10.0f, snapshotSprite, null, audioClips[6], "", sprites[0], null, audioClips[7]);
                        break;
                }
            }
        }

        public void OnQuestionWindowYes()
        {
            popUpActive = false;
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.NextStep));
        }

        public void OnQuestionWindowNo()
        {
            popUpActive = false;
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.PreviousStep));
        }

        public void OnDiaryContinue()
        {
            popUpActive = false;
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.NextStep));
        }

        public void OnDiaryWindowClosed()
        {
            popUpActive = false;
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.PreviousStep));
        }

        public void OnPhotoButtonPressed()
        {
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.NextStep));
        }

        public void OnRequestAbortWindow()
        {
            abortWindowOpen = true;
            if (step == 2)
            {
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnablePhotoButton, false));
            }
        }

        public void OnMiniGameResumed()
        {
            abortWindowOpen = false;

            switch (step)
            {
                case 0:
                    FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[0], audioClips[0]);
                    break;
                case 2:
                    FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[3], audioClips[4]);
                    break;
                case 3:
                    StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.PreviousStep));
                    break;

            }
        }

        public void OnNextStep()
        {
            popUpActive = false;
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
                        if (string.IsNullOrEmpty(currentName))
                        {
                            currentName = "50101";
                        }

                        if (currentHighlight)
                        {
                            currentHighlight.HighlightTarget(false);
                        }
                        currentHighlight = salzburg.GetMunicipalityByMunicipalCode(currentName);
                        currentHighlight.HighlightTarget(true);

                        string municipalityName = MunicipalityLookup.Instance.GetAt(currentHighlight.name).MunicipalityName;
                        windowText = municipalityName;

                        foreach (MeshCollider collider in salzburg.GetComponentsInChildren<MeshCollider>())
                        {
                            collider.enabled = false;
                        }

                        StartCoroutine(RequestQuestionCoroutine(municipalityName, audioClips[3]));
                        break;
                    case 1:
                        StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableVisualGuidance, false));
                        if (!abortWindowOpen)
                        {
                            FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[3], audioClips[4]);
                        }
                        break;
                    case 2:
                        StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnablePhotoButton, false));
                        takeSnapshot = true;
                        break;
                }
                step++;
            }
        }

        public void OnPreviousStep()
        {
            popUpActive = false;
            step--;
            if (step < 0)
            {
                step = 0;
                RequestStartWindow();
            }
            else
            {
                switch (step)
                {
                    case 0:
                        foreach (MeshCollider collider in salzburg.GetComponentsInChildren<MeshCollider>())
                        {
                            collider.enabled = true;
                        }

                        if (currentHighlight)
                        {
                            currentHighlight.HighlightTarget(false);
                        }
                        break;
                    case 1:
                        StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnablePhotoButton, false));
                        StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableVisualGuidance, true));
                        OnQuestionWindowNo();
                        if (!abortWindowOpen)
                        {
                            FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[0], audioClips[0]);
                        }
                        break;
                    case 2:
                        StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableVisualGuidance, false));
                        if (!abortWindowOpen)
                        {
                            FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[3], audioClips[4]);
                        }
                        if (currentHighlight)
                        {
                            currentHighlight.HighlightTarget(true);
                        }
                        break;
                }
            }
        }

        public void OnReplayInstruction()
        {
            if (!popUpActive)
            {
                switch (step)
                {
                    case 0:
                        tutorialSubstep = 0;
                        FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[0], audioClips[0]);
                        break;
                    case 2:
                        FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[3], audioClips[4]);
                        break;
                }
            }
        }

        private void OnDestroy()
        {
            if (snapshotSprite)
            {
                Destroy(snapshotSprite);
                snapshotSprite = null;
            }

            if (snapshot)
            {
                Destroy(snapshot);
                snapshot = null;
            }
        }
    }
}