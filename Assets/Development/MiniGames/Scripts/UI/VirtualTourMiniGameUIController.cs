using System.Collections;
using UnityEngine.Video;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGame.UI
{
    public class VirtualTourMiniGameUIController : BaseMiniGameUIController
    {
        public VirtualTourInfoUI virtualTourInfoUI;
        public VirtualTourToggleInfoUI virtualTourToggleInfoUI;
        public VirtualTourZoomSliderUI virtualTourZoomSliderUI;
        public Button abortSubMiniGameButton;
        public GameObject subMiniGameButtons;

        public static string openSubMiniGame = "";
        protected bool interactionSelectionActive = false;

        protected override void OnEnable()
        {
            base.OnEnable();

            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.RequestStartWindow, "OnRequestStartWindow");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.RequestScoreWindow, "OnRequestScoreWindow");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.RequestPromptWindow, "OnRequestPromptWindow");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.RequestQuestionWindow, "OnRequestQuestionWindow");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.RequestDiaryWindow, "OnRequestDiaryWindow");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.DiaryWindowClosed, "OnDiaryWindowClosed");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.PromptWindowClosed, "OnPromptWindowClosed");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SubMiniGameStarted, "OnSubMiniGameStarted");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SubMiniGameEnded, "OnSubMiniGameEnded");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SubMiniGameAborted, "OnSubMiniGameAborted");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.EnableVisualGuidance, "OnEnableVisualGuidance");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.ToggleInteractionSelectionPanel, "OnToggleInteractionSelectionPanel");
            abortButton.onClick.AddListener(OnRequestAbortWindow);
            abortSubMiniGameButton.onClick.AddListener(OnAbortSubMiniGame);
            subMiniGameButtons.SetActive(false);
        }

        public override void OnRequestStartWindow(bool firstTime = true)
        {
            if (!firstTime)
            {
                startWindowOpen = true;
            }
            startWindowFirstTime = firstTime;
            visualGuidanceState = false;
            visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState && !interactionSelectionActive);
            virtualTourToggleInfoUI.toggleInfoImage.gameObject.SetActive(false);
            virtualTourInfoUI.gameObject.SetActive(false);
            activeMiniGameWindow = startUI;
        }

        public void OnRequestScoreWindow()
        {
            StartCoroutine(RequestScoreWindowDelayed());
        }

        public IEnumerator RequestScoreWindowDelayed()
        {
            yield return new WaitForSeconds(0.1f);

            abortWindowOpen = true;
            visualGuidanceState = false;
            visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState && !interactionSelectionActive);
            virtualTourToggleInfoUI.toggleInfoImage.gameObject.SetActive(false);
            virtualTourInfoUI.gameObject.SetActive(false);

            EventBus.Publish(EventId.MiniGameEvents.RequestAbortWindow);
            activeMiniGameWindow = scoreUI;
        }

        public void OnRequestAbortWindow()
        {
            abortWindowOpen = true;
            visualGuidanceState = false;
            visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState && !interactionSelectionActive);
            virtualTourToggleInfoUI.toggleInfoImage.gameObject.SetActive(false);
            virtualTourInfoUI.gameObject.SetActive(false);

            EventBus.Publish(EventId.MiniGameEvents.RequestAbortWindow);
            activeMiniGameWindow = abortUI;
        }

        public void OnRequestPromptWindow(string text, float buttonTime, Sprite sprite, VideoClip videoClip, AudioClip audioClip)
        {
            visualGuidanceState = false;
            visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState && !interactionSelectionActive);

            BaseOnRequestPromptWindow(text, buttonTime, sprite, videoClip, audioClip);
        }

        public void OnRequestQuestionWindow(string text, float buttonTime, Sprite sprite, VideoClip videoClip, AudioClip audioClip)
        {
            visualGuidanceState = false;
            visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState && !interactionSelectionActive);

            BaseOnRequestQuestionWindow(text, buttonTime, sprite, videoClip, audioClip);
        }

        public void OnRequestDiaryWindow(string text, string diaryNumberText, float buttonTime, Sprite sprite, VideoClip videoClip, AudioClip audioClip, string promptText, Sprite promptSprite, VideoClip promptVideoClip, AudioClip promptAudioClip)
        {
            visualGuidanceState = false;
            visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState && !interactionSelectionActive);

            BaseOnRequestDiaryWindow(text, diaryNumberText, buttonTime, sprite, videoClip, audioClip, promptText, promptSprite, promptVideoClip, promptAudioClip);
        }

        public override void OnStartConfirm()
        {
            visualGuidanceState = true;
            visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState && !interactionSelectionActive);
            virtualTourToggleInfoUI.toggleInfoImage.gameObject.SetActive(true);
            virtualTourInfoUI.gameObject.SetActive(virtualTourToggleInfoUI.infoUIActive);

            base.OnStartConfirm();
        }

        public override void OnAbortCancel()
        {
            visualGuidanceState = true;
            visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState && !interactionSelectionActive);
            virtualTourToggleInfoUI.toggleInfoImage.gameObject.SetActive(true);
            virtualTourInfoUI.gameObject.SetActive(virtualTourToggleInfoUI.infoUIActive);

            base.OnAbortCancel();
        }

        public override void CloseActivePanel()
        {
            visualGuidanceState = true;
            visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState && !interactionSelectionActive);
            virtualTourToggleInfoUI.toggleInfoImage.gameObject.SetActive(!virtualTourToggleInfoUI.subMiniGameActive);
            virtualTourInfoUI.gameObject.SetActive(virtualTourToggleInfoUI.infoUIActive && !virtualTourToggleInfoUI.subMiniGameActive);

            base.CloseActivePanel();
        }

        public override void OnQuestionWindowYes()
        {
            visualGuidanceState = virtualTourZoomSliderUI.gameObject.activeSelf;
            visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState && !interactionSelectionActive);
            questionUI.answered = true;

            CloseActivePanel();

            EventBus.Publish(EventId.MiniGameEvents.QuestionWindowYes);
        }

        public override void OnQuestionWindowNo()
        {
            if (!abortWindowOpen && !startWindowOpen)
            {
                visualGuidanceState = virtualTourZoomSliderUI.gameObject.activeSelf;
                visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState && !interactionSelectionActive);
            }

            questionUI.answered = true;

            CloseActivePanel();

            EventBus.Publish(EventId.MiniGameEvents.QuestionWindowNo);
        }

        public override void OnDiaryPromptYes()
        {
            visualGuidanceState = virtualTourZoomSliderUI.gameObject.activeSelf;
            visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState && !interactionSelectionActive);
            diaryUI.miniGameDiaryPromptUI.promptVideoPlayer.Stop();
            diaryUI.miniGameDiaryPromptUI.promptAudioSource.Stop();
            diaryUI.answered = true;

            CloseActivePanel();

            EventBus.Publish(EventId.MiniGameEvents.DiaryContinue);
        }

        public void OnToggleInteractionSelectionPanel(bool toggle, int headingIndex)
        {
            interactionSelectionActive = toggle;
            visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState && !interactionSelectionActive);
        }

        public override void OnScoreCancel()
        {
            visualGuidanceState = true;
            visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState && !interactionSelectionActive);
            virtualTourToggleInfoUI.toggleInfoImage.gameObject.SetActive(true);
            virtualTourInfoUI.gameObject.SetActive(virtualTourToggleInfoUI.infoUIActive);

            base.OnScoreCancel();
        }

        public void OnDiaryWindowClosed()
        {
            if (!abortWindowOpen && !startWindowOpen)
            {
                visualGuidanceState = virtualTourZoomSliderUI.gameObject.activeSelf;
                visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState && !interactionSelectionActive);
            }
        }

        public void OnPromptWindowClosed()
        {
            if (!abortWindowOpen && !startWindowOpen)
            {
                visualGuidanceState = virtualTourZoomSliderUI.gameObject.activeSelf;
                visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState && !interactionSelectionActive);
            }
        }

        public void OnSubMiniGameStarted(InfinityCode.uPano.HotSpots.HotSpot  sender)
        {
            openSubMiniGame = sender.title;
            abortButton.gameObject.SetActive(false);
            if (subMiniGameButtons)
            {
                subMiniGameButtons.SetActive(true);
            }
            visualGuidanceState = false;
            visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState && !interactionSelectionActive);
        }

        public void OnSubMiniGameEnded(string subMiniGameName)
        {
            openSubMiniGame = "";
            if (subMiniGameButtons)
            {
                subMiniGameButtons.SetActive(false);
            }
            abortButton.gameObject.SetActive(true);
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.AudioSubMiniGameIsPlaying, false));
            visualGuidanceState = true;
            visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState && !interactionSelectionActive);
        }

        public void OnSubMiniGameAborted(string subMiniGameName)
        {
            openSubMiniGame = "";
            if (subMiniGameButtons)
            {
                subMiniGameButtons.SetActive(false);
            }
            abortButton.gameObject.SetActive(true);
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.AudioSubMiniGameIsPlaying, false));
            visualGuidanceState = true;
            visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState && !interactionSelectionActive);
        }

        public void OnAbortSubMiniGame()
        {
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, false));
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.SubMiniGameAborted, openSubMiniGame));
        }

        public void OnEnableVisualGuidance(bool enabled)
        {
            visualGuidanceEnabled = enabled;
            visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState && !interactionSelectionActive);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.RequestStartWindow, "OnRequestStartWindow");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.RequestScoreWindow, "OnRequestScoreWindow");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.RequestPromptWindow, "OnRequestPromptWindow");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.RequestQuestionWindow, "OnRequestQuestionWindow");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.RequestDiaryWindow, "OnRequestDiaryWindow");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.DiaryWindowClosed, "OnDiaryWindowClosed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.PromptWindowClosed, "OnPromptWindowClosed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SubMiniGameStarted, "OnSubMiniGameStarted");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SubMiniGameEnded, "OnSubMiniGameEnded");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SubMiniGameAborted, "OnSubMiniGameAborted");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.EnableVisualGuidance, "OnEnableVisualGuidance");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.ToggleInteractionSelectionPanel, "OnToggleInteractionSelectionPanel");
            abortButton.onClick.RemoveListener(OnRequestAbortWindow);
            abortSubMiniGameButton.onClick.RemoveListener(OnAbortSubMiniGame);
        }
    }
}