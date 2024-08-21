using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace MiniGame
{
    public class SkipAudioUI : MonoBehaviour
    {
        public Button skipAudioButton;
        public TextMeshProUGUI skipAudioButtonText;
        public string skipAudioButtonTextContent = "Audio überspringen";

        private bool admin = false;

        private void Awake()
        {
            Debug.Assert(skipAudioButton, "SkipAudioUI is missing a reference to a Button");
            Debug.Assert(skipAudioButtonText, "SkipAudioUI is missing a reference to a TextMeshProUGUI");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.EnableSkipAudioButton, "OnEnableSkipAudioButton");
            EventBus.SaveRegisterCallback(this, EventId.QuitAdminMode, "OnQuitAdminMode");
            skipAudioButtonText.text = skipAudioButtonTextContent;
            skipAudioButton.gameObject.SetActive(false);

            if (PlayerPrefs.HasKey(EventId.AdminMode))
            {
                admin = PlayerPrefs.GetInt(EventId.AdminMode) != 0;
            }
        }

        public void OnSkipAudioButtonClick()
        {
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.SkipAudioButtonPressed));
            skipAudioButton.gameObject.SetActive(false);
        }

        public void OnEnableSkipAudioButton(bool enabled)
        {
            if (admin)
            {
                skipAudioButton.gameObject.SetActive(enabled);
            }
        }

        public void OnQuitAdminMode()
        {
            admin = false;
            skipAudioButton.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.EnableSkipAudioButton, "OnEnableSkipAudioButton");
            EventBus.SaveDeregisterCallback(this, EventId.QuitAdminMode, "OnQuitAdminMode");
        }
    }
}
