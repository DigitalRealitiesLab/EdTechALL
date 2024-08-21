using UnityEngine;
using UnityEngine.UI;

namespace MiniGame.UI
{
    public class ToggleInfoUI : MonoBehaviour
    {
        public RectTransform minigameInfoUITransform;
        public Image toggleInfoImage;
        public bool infoUIActive = true;
        private RectTransform rectTransform;

        protected void Awake()
        {
            Debug.Assert(minigameInfoUITransform, "ToggleInfoUI is missing a reference to a RectTransform");
            Debug.Assert(toggleInfoImage, "ToggleInfoUI is missing a reference to a Image");

            if (!rectTransform)
            {
                rectTransform = GetComponent<RectTransform>();
            }
        }

        public void ToggleInfoUIButtonPress()
        {
            infoUIActive = !infoUIActive;
            Vector3 newPosition = minigameInfoUITransform.localPosition;
            Vector3 newEulerAngles = rectTransform.eulerAngles;
            if (infoUIActive)
            {
                newPosition.y -= (minigameInfoUITransform.rect.height + rectTransform.rect.height) * 0.5f;
                newEulerAngles.z = 180.0f;
            }
            else
            {
                newPosition.y += (minigameInfoUITransform.rect.height - rectTransform.rect.height * 2.0f) * 0.5f;
                newEulerAngles.z = 0.0f;
            }
            rectTransform.localPosition = newPosition;
            rectTransform.eulerAngles = newEulerAngles;
            minigameInfoUITransform.gameObject.SetActive(infoUIActive);
        }
    }
}
