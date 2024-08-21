using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MiniGame.UI
{
    public class VirtualTourInfoUI : MonoBehaviour
    {
        public TextMeshProUGUI virtualTourInfoText;
        public Button replayButton;
        public Image replayButtonImage;
        public TextMeshProUGUI replayButtonText;

        protected void Awake()
        {
            Debug.Assert(virtualTourInfoText, "VirtualTourInfoUI is missing a reference to a TMPro");
            Debug.Assert(replayButtonImage, "VirtualTourInfoUI is missing a reference to the replay Button");
            Debug.Assert(replayButtonImage, "VirtualTourInfoUI is missing a reference to the replay Button Image");
            Debug.Assert(replayButtonText, "VirtualTourInfoUI is missing a reference to the replay Button Text");
            replayButton.onClick.AddListener(ReplayPublish);
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.OverrideText, "OnOverrideText");
            gameObject.SetActive(false);
        }

        public void OnOverrideText(string overrideText)
        {
            virtualTourInfoText.text = overrideText;
        }

        public void ReplayPublish()
        {
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.ReplayInstruction));
        }

        private void OnDestroy()
        {
            replayButton.onClick.RemoveListener(ReplayPublish);
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.OverrideText, "OnOverrideText");
        }
    }
}