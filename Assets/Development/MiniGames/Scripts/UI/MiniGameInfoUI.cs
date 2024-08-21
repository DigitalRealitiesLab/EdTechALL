using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MiniGame.UI
{
    public class MiniGameInfoUI : MonoBehaviour
    {
        public TextMeshProUGUI miniGameInfoText;
        public Button previousButton, nextButton, replayButton, exitButton;
        public Image previousButtonImage, nextButtonImage, replayButtonImage;
        public TextMeshProUGUI previousButtonText, nextButtonText, replayButtonText;
        public Slider progressSlider;
        private int currentStep = 0;
        public int CurrentStep { get { return currentStep; } set { currentStep = value; SetProgressSlider(); } }
        private string[] steps;
        private int maxStep = 0;

        private bool admin = false;

        protected void Awake()
        {
            Debug.Assert(miniGameInfoText, "MiniGameInfoUI is missing a reference to a TMPro");
            Debug.Assert(previousButton, "MiniGameInfoUI is missing a reference to the previous Button");
            Debug.Assert(nextButton, "MiniGameInfoUI is missing a reference to the next Button");
            Debug.Assert(replayButtonImage, "MiniGameInfoUI is missing a reference to the replay Button");
            Debug.Assert(previousButtonImage, "MiniGameInfoUI is missing a reference to the previous Button Image");
            Debug.Assert(nextButtonImage, "MiniGameInfoUI is missing a reference to the next Button Image");
            Debug.Assert(replayButtonImage, "MiniGameInfoUI is missing a reference to the replay Button Image");
            Debug.Assert(previousButtonText, "MiniGameInfoUI is missing a reference to the previous Button Text");
            Debug.Assert(nextButtonText, "MiniGameInfoUI is missing a reference to the next Button Text");
            Debug.Assert(replayButtonText, "MiniGameInfoUI is missing a reference to the replay Button Text");
            Debug.Assert(progressSlider, "MiniGameInfoUI is missing a reference to the progress Slider");
            previousButton.onClick.AddListener(PreviousStepPublish);
            nextButton.onClick.AddListener(NextStepPublish);
            replayButton.onClick.AddListener(ReplayPublish);
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.OverrideText, "OnOverrideText");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.NextStep, "OnNextStep");
            EventBus.SaveRegisterCallback(this, EventId.QuitAdminMode, "OnQuitAdminMode");

            if (PlayerPrefs.HasKey(EventId.AdminMode))
            {
                admin = PlayerPrefs.GetInt(EventId.AdminMode) != 0;
            }
        }

        public void LoadMiniGameConfig(MiniGameConfig config)
        {
            steps = config.steps;
            SetProgressSlider();

            replayButton.interactable = true;
            replayButtonImage.enabled = true;
            replayButtonText.enabled = true;

            previousButton.interactable = true;
            previousButtonImage.enabled = true;
            previousButtonText.enabled = true;

            if (steps.Length > CurrentStep)
            {
                miniGameInfoText.text = steps[CurrentStep];
            }

            if (admin)
            {
                nextButton.interactable = true;
                nextButtonImage.enabled = true;
                nextButtonText.enabled = true;
            }
            else
            {
                nextButton.interactable = false;
                nextButtonImage.enabled = false;
                nextButtonText.enabled = false;
            }
        }

        public void OnOverrideText(string overrideText)
        {
            steps[CurrentStep] = overrideText;
            miniGameInfoText.text = steps[CurrentStep];
        }

        public void NextStepPublish()
        {
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.NextStep));
        }

        public void OnNextStep()
        {
            CurrentStep++;
            if (CurrentStep > maxStep)
            {
                maxStep = CurrentStep;
            }

            if (steps.Length <= CurrentStep)
            {
                CurrentStep = steps.Length - 1;
            }

            if (steps.Length > CurrentStep)
            {
                miniGameInfoText.text = steps[CurrentStep];
            }

            if (admin || CurrentStep < maxStep)
            {
                nextButton.interactable = true;
                nextButtonImage.enabled = true;
                nextButtonText.enabled = true;
            }
            else
            {
                nextButton.interactable = false;
                nextButtonImage.enabled = false;
                nextButtonText.enabled = false;
            }

            previousButton.interactable = true;
            previousButtonImage.enabled = true;
            previousButtonText.enabled = true;
        }

        public void PreviousStepPublish()
        {
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.PreviousStep));
        }

        public void OnPreviousStep()
        {
            if (CurrentStep > maxStep)
            {
                maxStep = CurrentStep;
            }
            CurrentStep--;
            if (CurrentStep < 0)
            {
                CurrentStep = 0;
            }
            if (steps.Length > CurrentStep)
            {
                miniGameInfoText.text = steps[CurrentStep];
            }
            nextButton.interactable = true;
            nextButtonImage.enabled = true;
            nextButtonText.enabled = true;
        }

        public void ReplayPublish()
        {
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.ReplayInstruction));
        }

        private void SetProgressSlider()
        {
            if (currentStep == 0 || steps.Length == 0)
            {
                progressSlider.value = 0;
            }
            else
            {
                progressSlider.value = (float)currentStep / (float)steps.Length;
            }
        }

        public void OnQuitAdminMode()
        {
            admin = false;
            if (CurrentStep >= maxStep)
            {
                nextButton.interactable = false;
                nextButtonImage.enabled = false;
                nextButtonText.enabled = false;
            }
        }

        private void OnDestroy()
        {
            previousButton.onClick.RemoveListener(PreviousStepPublish);
            nextButton.onClick.RemoveListener(NextStepPublish);
            replayButton.onClick.RemoveListener(ReplayPublish);
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.OverrideText, "OnOverrideText");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.NextStep, "OnNextStep");
            EventBus.SaveDeregisterCallback(this, EventId.QuitAdminMode, "OnQuitAdminMode");
        }
    }
}