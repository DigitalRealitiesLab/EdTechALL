using UnityEngine;
using UnityEngine.UI;

public class DeleteSaveWindow : MonoBehaviour
{
    public UIPanelController panelController;
    public Button confirm;
    public Button abort;

    public void Awake()
    {
        Debug.Assert(confirm, "DeleteSaveWindow is missing a referene to a Button");
        Debug.Assert(abort, "DeleteSaveWindow is missing a referene to a Button");

        EventBus.SaveRegisterCallback(this, MiniGame.EventId.RequestDeleteSaveWindow, "OnRequestDeleteSaveWindow");
        confirm.onClick.AddListener(() =>
        {
            PlayerPrefs.SetInt(MiniGame.EventId.GameModeProgress, 0);
            PlayerPrefs.SetInt(MiniGame.EventId.GameModeMarker, 0);
            PlayerPrefs.SetInt(Tutorial.BaseTutorial.markerTutorialSkipString, 0);
            PlayerPrefs.Save();
            EventBus.Publish(MiniGame.EventId.DeletedSaves);
            gameObject.SetActive(false);
        });
        abort.onClick.AddListener(() => { gameObject.SetActive(false); });
        gameObject.SetActive(false);
    }

    public void OnRequestDeleteSaveWindow(bool active)
    {
        if (!panelController.GetPanelActiveByIndex(UIPanel.SettingsPanel) || !active)
        {
            gameObject.SetActive(active);

            if (active)
            {
                EventBus.Publish(MiniGame.EventId.RequestCloseAppWindow, false);
                MiniGameSceneLoadData loadData = null;
                EventBus.Publish(MiniGame.EventId.RequestMiniGameLoadWindow, loadData, false);
            }
        }
    }

    private void OnDestroy()
    {
        EventBus.SaveDeregisterCallback(this, MiniGame.EventId.RequestDeleteSaveWindow, "OnRequestDeleteSaveWindow");
    }
}