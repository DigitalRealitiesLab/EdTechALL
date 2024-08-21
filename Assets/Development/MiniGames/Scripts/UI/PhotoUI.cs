using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace MiniGame
{
    public class PhotoUI : MonoBehaviour
    {
        public Button photoButton;
        public TextMeshProUGUI photoButtonText;
        public string photoButtonTextContent = "Foto";
        public Image frame;

        private void Awake()
        {
            Debug.Assert(photoButton, "PhotoUI is missing a reference to a Button");
            Debug.Assert(photoButtonText, "PhotoUI is missing a reference to a TextMeshProUGUI");
            Debug.Assert(frame, "PhotoUI is missing a reference to an Image");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.EnablePhotoButton, "OnEnablePhotoButton");
            photoButtonText.text = photoButtonTextContent;
            photoButton.gameObject.SetActive(false);
            frame.gameObject.SetActive(false);

            RectTransform rectTransform = frame.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(Screen.width * 3.0f / 4.0f + 100.0f, Screen.height * 3.0f / 4.0f + 75.0f);
            rectTransform.position = new Vector3(Screen.width / 2.0f, Screen.height * 3.0f / 8.0f + 25f, 0.0f);
        }

        public void OnPhotoButtonClick()
        {
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.PhotoButtonPressed));
        }

        public void OnEnablePhotoButton(bool enabled)
        {
            photoButton.gameObject.SetActive(enabled);
            frame.gameObject.SetActive(enabled);
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.EnablePhotoButton, "OnEnablePhotoButton");
        }
    }
}
