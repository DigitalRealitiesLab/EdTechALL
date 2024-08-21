using System.Collections;
using UnityEngine;

public class SettingsPanel : MonoBehaviour
{
    public VisualGuidanceUIPanel visualGuidanceUIPanel;

    private int gameMode = 0;
    private bool reload = false;

    private void Awake()
    {
        Debug.Assert(visualGuidanceUIPanel, "SettingsPanel is missing a reference to a VisualGuidanceUIPanel");
        EventBus.SaveRegisterCallback(this, MiniGame.EventId.GameMode, "OnGameMode");

        if (PlayerPrefs.HasKey(MiniGame.EventId.GameMode))
        {
            gameMode = PlayerPrefs.GetInt(MiniGame.EventId.GameMode);
        }

        StartCoroutine(SetActiveDelayed(0, false));
    }

    public void OpenSettingsPanel()
    {
        UIPanel.ActivateUIPanelSingle(UIPanel.SettingsPanel);
        EventBus.Publish(MiniGame.EventId.RequestCloseAppWindow, false);
        EventBus.Publish(MiniGame.EventId.RequestDeleteSaveWindow, false);
        MiniGameSceneLoadData loadData = null;
        EventBus.Publish(MiniGame.EventId.RequestMiniGameLoadWindow, loadData, false);
    }

    public void OnExitButtonPressed()
    {
        UIPanel.ActivateUIPanelSingle(UIPanel.DefaultPanel);
        Reload();
    }

    public void OnGameMode(int toggle)
    {
        reload = toggle != gameMode;
    }

    public void Reload()
    {
        if (reload)
        {
            PlayerPrefs.SetInt(Tutorial.InitialTutorial.initialTutorialSkipString, 1);
            PlayerPrefs.Save();
            SceneLoader.RequestReloadCurrentScene();
        }
    }

    private IEnumerator SetActiveDelayed(float delay, bool active)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(active);
    }

    private void OnEnable()
    {
        visualGuidanceUIPanel.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        visualGuidanceUIPanel.gameObject.SetActive(true);
    }
}
