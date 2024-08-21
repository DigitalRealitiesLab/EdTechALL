using InfinityCode.uPano.HotSpots;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGame.UI
{
    public class VirtualTourToggleInfoUI : MonoBehaviour
    {
        public RectTransform virtualTourInfoUITransform;
        public Image toggleInfoImage;
        public bool infoUIActive = true;
        public bool subMiniGameActive = false;
        private RectTransform rectTransform;

        protected void Awake()
        {
            Debug.Assert(virtualTourInfoUITransform, "VirtualTourToggleInfoUI is missing a reference to a RectTransform");
            Debug.Assert(toggleInfoImage, "VirtualTourToggleInfoUI is missing a reference to a Image");

            if (!rectTransform)
            {
                rectTransform = GetComponent<RectTransform>();
            }
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SubMiniGameStarted, "OnSubMiniGameStarted");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SubMiniGameAborted, "OnSubMiniGameAborted");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SubMiniGameEnded, "OnSubMiniGameEnded");
            toggleInfoImage.gameObject.SetActive(false);
        }

        public void OnSubMiniGameStarted(HotSpot sender)
        {
            subMiniGameActive = true;
            gameObject.SetActive(false);
            virtualTourInfoUITransform.gameObject.SetActive(false);
        }

        public void OnSubMiniGameEnded(string subMiniGameName)
        {
            subMiniGameActive = false;
            gameObject.SetActive(!subMiniGameActive);
            virtualTourInfoUITransform.gameObject.SetActive(!subMiniGameActive && infoUIActive);
        }

        public virtual void OnSubMiniGameAborted(string subMiniGameName)
        {
            subMiniGameActive = false;
            gameObject.SetActive(!subMiniGameActive);
            virtualTourInfoUITransform.gameObject.SetActive(!subMiniGameActive && infoUIActive);
        }

        public void ToggleVirtualTourInfoUIButtonPress()
        {
            infoUIActive = !infoUIActive;
            Vector3 newPosition = virtualTourInfoUITransform.localPosition;
            Vector3 newEulerAngles = rectTransform.eulerAngles;
            if (infoUIActive)
            {
                newPosition.y -= (virtualTourInfoUITransform.rect.height + rectTransform.rect.height) * 0.5f;
                newEulerAngles.z = 180.0f;
            }
            else
            {
                newPosition.y += (virtualTourInfoUITransform.rect.height - rectTransform.rect.height * 2.0f) * 0.5f;
                newEulerAngles.z = 0.0f;
            }
            rectTransform.localPosition = newPosition;
            rectTransform.eulerAngles = newEulerAngles;
            virtualTourInfoUITransform.gameObject.SetActive(!subMiniGameActive && infoUIActive);
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SubMiniGameStarted, "OnSubMiniGameStarted");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SubMiniGameAborted, "OnSubMiniGameAborted");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SubMiniGameEnded, "OnSubMiniGameEnded");
        }
    }
}
