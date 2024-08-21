using UnityEngine;
using UnityEngine.UI;

namespace MiniGame.LandscapeLayers.UI
{
    [RequireComponent(typeof(Toggle))]
    public class LandscapeLayersMiniGameToggle : MonoBehaviour
    {
        public Image background;
        public TMPro.TextMeshProUGUI label;
        private Toggle toggle;

        public LandscapeLayersData landscapeLayersData
        {
            set
            {
                label.text = value.name;
                toggle.onValueChanged.AddListener((toggleValue) =>
                {
                    EventBus.Publish(EventId.LandscapeLayersMiniGameEvents.SetLandscapeLayerActive, value.landscapeType, toggleValue);
                });
                background.color = value.toggleColor;
            }
        }

        private void Awake()
        {
            toggle = GetComponent<Toggle>();
            toggle.isOn = false;
            Debug.Assert(label, "LandscapeLayersMiniGameToggle is missing a reference to a TMPro as its label");
            EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
            EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.NextStep, "OnNextStep");
            EventBus.SaveRegisterCallback(this, EventId.LandscapeLayersMiniGameEvents.SetTogglesInteractable, "OnSetTogglesInteractable");
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.NextStep, "OnNextStep");
            EventBus.SaveDeregisterCallback(this, EventId.LandscapeLayersMiniGameEvents.SetTogglesInteractable, "OnSetTogglesInteractable");
        }

        public void OnPreviousStep()
        {
            toggle.isOn = false;
        }

        public void OnNextStep()
        {
            toggle.isOn = false;
        }

        public void OnSetTogglesInteractable(bool interactable)
        {
            toggle.interactable = interactable;
        }
    }
}