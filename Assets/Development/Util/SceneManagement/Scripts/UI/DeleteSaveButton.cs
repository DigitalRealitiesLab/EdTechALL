using UnityEngine;
using UnityEngine.UI;

public class DeleteSaveButton : MonoBehaviour
{
    public Button button;

    private void Awake()
    {
        if (!button && !TryGetComponent(out button))
        {
            Debug.LogError("DeleteSaveButton is missing a reference to its Button");
        }

        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        PlayerPrefs.SetInt(MiniGame.EventId.GameModeProgress, 0);
        PlayerPrefs.SetInt(MiniGame.EventId.GameModeMarker, 0);
        PlayerPrefs.SetInt(Tutorial.BaseTutorial.markerTutorialSkipString, 0);
        PlayerPrefs.Save();
        EventBus.Publish(MiniGame.EventId.DeletedSaves);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnButtonClick);
    }
}
