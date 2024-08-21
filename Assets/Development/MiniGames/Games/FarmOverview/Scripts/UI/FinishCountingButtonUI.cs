using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace MiniGame
{
    public class FinishCountingButtonUI : MonoBehaviour
    {
        public Button finishButton;
        public TextMeshProUGUI finishButtonText;
        public string finishButtonTextContent = "Fertig";

        private void Awake()
        {
            Debug.Assert(finishButton, "FinishCountingButton is missing a reference to a Button");
            Debug.Assert(finishButtonText, "FinishCountingButton is missing a reference to a TextMeshProUGUI");
            EventBus.SaveRegisterCallback(this, FarmOverview.EventId.FarmOverviewMiniGameEvents.EnableFinishCountingButton, "OnEnableFinishCountingButton");
            finishButtonText.text = finishButtonTextContent;
            finishButton.gameObject.SetActive(false);
        }

        public void OnFinishButtonClick()
        {
            finishButton.gameObject.SetActive(false);
            StartCoroutine(EventBus.PublishCoroutine(FarmOverview.EventId.FarmOverviewMiniGameEvents.FinishCountingButtonPressed));
        }

        public void OnEnableFinishCountingButton(bool enabled)
        {
            finishButton.gameObject.SetActive(enabled);
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, FarmOverview.EventId.FarmOverviewMiniGameEvents.EnableFinishCountingButton, "OnEnableFinishCountingButton");
        }
    }
}
