using UnityEngine;
using UnityEngine.UI;

public class CloseAppButton : MonoBehaviour
{
    public Button button;

    public bool startAdminMode = false;
    public bool withPrompt = false;

    private void Awake()
    {
        if (!button && !TryGetComponent(out button))
        {
            Debug.LogError("CloseAppButton is missing a reference to its Button");
        }

        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        if (startAdminMode)
        {
            PlayerPrefs.SetInt(MiniGame.EventId.AdminMode, 1);
            PlayerPrefs.Save();
        }

        if (withPrompt)
        {
            EventBus.Publish(MiniGame.EventId.RequestCloseAppWindow, true);
        }
        else
        {
            Application.Quit();
        }
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnButtonClick);
    }
}
