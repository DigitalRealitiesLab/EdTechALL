using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace MiniGame
{
    public class PositioningQuizConfirmButton : MonoBehaviour
    {
        public Button confirmButton;
        public TextMeshProUGUI confirmButtonText;
        public string confirmButtonTextContent = "Bestätigen";

        private void Awake()
        {
            Debug.Assert(confirmButton, "PositioningQuizConfirmButton is missing a reference to a Button");
            Debug.Assert(confirmButtonText, "PositioningQuizConfirmButton is missing a reference to a TextMeshProUGUI");
            confirmButtonText.text = confirmButtonTextContent;
            confirmButton.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.StartPositioningQuiz, "OnStartPositioningQuiz");
        }

        private void OnDisable()
        {
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.StartPositioningQuiz, "OnStartPositioningQuiz");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.PositioningQuizEnded, "OnPositioningQuizEnded");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.AbortPositioningQuiz, "OnAbortPositioningQuiz");
            confirmButton.gameObject.SetActive(false);
        }

        public void OnStartPositioningQuiz()
        {
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.StartPositioningQuiz, "OnStartPositioningQuiz");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.PositioningQuizEnded, "OnPositioningQuizEnded");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.AbortPositioningQuiz, "OnAbortPositioningQuiz");
            confirmButton.gameObject.SetActive(true);
        }

        public void OnPositioningQuizEnded()
        {
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.StartPositioningQuiz, "OnStartPositioningQuiz");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.PositioningQuizEnded, "OnPositioningQuizEnded");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.AbortPositioningQuiz, "OnAbortPositioningQuiz");
            confirmButton.gameObject.SetActive(false);
        }

        public void OnAbortPositioningQuiz()
        {
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.StartPositioningQuiz, "OnStartPositioningQuiz");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.PositioningQuizEnded, "OnPositioningQuizEnded");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.AbortPositioningQuiz, "OnAbortPositioningQuiz");
            confirmButton.gameObject.SetActive(false);
        }

        public void OnConfirmButtonClick()
        {
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.PositioningQuizConfirm));
        }
    }
}
