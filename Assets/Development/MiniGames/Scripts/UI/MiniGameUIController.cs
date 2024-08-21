using System.Collections;
using UnityEngine.Video;
using UnityEngine;

namespace MiniGame.UI
{
    public class MiniGameUIController : BaseMiniGameUIController
    {
        public MiniGameInfoUI infoUI;
        public ToggleInfoUI toggleInfoUI;

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
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.EnableVisualGuidance, "OnEnableVisualGuidance");
            abortButton.onClick.AddListener(OnRequestAbortWindow);
        }

        public override void OnRequestStartWindow(bool firstTime = true)
        {
            if (!firstTime)
            {
                startWindowOpen = true;
            }
            startWindowFirstTime = firstTime;
            visualGuidanceState = false;
            visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState);
            toggleInfoUI.toggleInfoImage.gameObject.SetActive(false);
            infoUI.gameObject.SetActive(false);
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
            visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState);
            toggleInfoUI.toggleInfoImage.gameObject.SetActive(false);
            infoUI.gameObject.SetActive(false);

            EventBus.Publish(EventId.MiniGameEvents.RequestAbortWindow);
            activeMiniGameWindow = scoreUI;
        }

        public void OnRequestAbortWindow()
        {
            abortWindowOpen = true;
            visualGuidanceState = false;
            visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState);
            toggleInfoUI.toggleInfoImage.gameObject.SetActive(false);
            infoUI.gameObject.SetActive(false);

            EventBus.Publish(EventId.MiniGameEvents.RequestAbortWindow);
            activeMiniGameWindow = abortUI;
        }

        public void OnRequestPromptWindow(string text, float buttonTime, Sprite sprite, VideoClip videoClip, AudioClip audioClip)
        {
            visualGuidanceState = false;
            visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState);

            BaseOnRequestPromptWindow(text, buttonTime, sprite, videoClip, audioClip);
        }

        public void OnRequestQuestionWindow(string text, float buttonTime, Sprite sprite, VideoClip videoClip, AudioClip audioClip)
        {
            visualGuidanceState = false;
            visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState);

            BaseOnRequestQuestionWindow(text, buttonTime, sprite, videoClip, audioClip);
        }

        public void OnRequestDiaryWindow(string text, string diaryNumberText, float buttonTime, Sprite sprite, VideoClip videoClip, AudioClip audioClip, string promptText, Sprite promptSprite, VideoClip promptVideoClip, AudioClip promptAudioClip)
        {
            visualGuidanceState = false;
            visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState);

            BaseOnRequestDiaryWindow(text, diaryNumberText, buttonTime, sprite, videoClip, audioClip, promptText, promptSprite, promptVideoClip, promptAudioClip);
        }

        public override void OnStartConfirm()
        {
            startWindowOpen = false;
            visualGuidanceState = true;
            visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState);
            toggleInfoUI.toggleInfoImage.gameObject.SetActive(true);
            infoUI.gameObject.SetActive(toggleInfoUI.infoUIActive);

            base.OnStartConfirm();
        }

        public override void OnAbortCancel()
        {
            visualGuidanceState = true;
            visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState);
            toggleInfoUI.toggleInfoImage.gameObject.SetActive(true);
            infoUI.gameObject.SetActive(toggleInfoUI.infoUIActive);

            base.OnAbortCancel();
        }

        public override void CloseActivePanel()
        {
            visualGuidanceState = true;
            visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState);
            toggleInfoUI.toggleInfoImage.gameObject.SetActive(true);
            infoUI.gameObject.SetActive(toggleInfoUI.infoUIActive);

            base.CloseActivePanel();
        }

        public override void OnQuestionWindowYes()
        {
            visualGuidanceState = infoUI.gameObject.activeSelf;
            visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState);
            questionUI.answered = true;

            CloseActivePanel();

            EventBus.Publish(EventId.MiniGameEvents.QuestionWindowYes);
        }

        public override void OnQuestionWindowNo()
        {
            if (!abortWindowOpen && !startWindowOpen)
            {
                visualGuidanceState = infoUI.gameObject.activeSelf;
                visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState);
            }

            questionUI.answered = true;

            CloseActivePanel();

            EventBus.Publish(EventId.MiniGameEvents.QuestionWindowNo);
        }

        public override void OnDiaryPromptYes()
        {
            visualGuidanceState = infoUI.gameObject.activeSelf;
            visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState);
            diaryUI.miniGameDiaryPromptUI.promptVideoPlayer.Stop();
            diaryUI.miniGameDiaryPromptUI.promptAudioSource.Stop();
            diaryUI.answered = true;

            CloseActivePanel();

            EventBus.Publish(EventId.MiniGameEvents.DiaryContinue);
        }

        public override void OnScoreCancel()
        {
            visualGuidanceState = true;
            visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState);
            toggleInfoUI.toggleInfoImage.gameObject.SetActive(true);
            infoUI.gameObject.SetActive(toggleInfoUI.infoUIActive);

            base.OnScoreCancel();
        }

        public void OnDiaryWindowClosed()
        {
            if (!abortWindowOpen && !startWindowOpen)
            {
                visualGuidanceState = infoUI.gameObject.activeSelf;
                visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState);
            }
        }

        public void OnPromptWindowClosed()
        {
            if (!abortWindowOpen && !startWindowOpen)
            {
                visualGuidanceState = infoUI.gameObject.activeSelf;
                visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState);
            }
        }

        public void OnEnableVisualGuidance(bool enabled)
        {
            visualGuidanceEnabled = enabled;
            visualGuidanceUIPanel.gameObject.SetActive(visualGuidanceEnabled && visualGuidanceState);
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
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.EnableVisualGuidance, "OnEnableVisualGuidance");
            abortButton.onClick.RemoveListener(OnRequestAbortWindow);
        }
    }
}