using UnityEngine;

public class QuitAdminModeUI : MonoBehaviour
{
    public GameObject buttonGameObject;

    private bool admin = false;
    private bool subMiniGameAudioPlaying = false;

    private void Awake()
    {
        EventBus.SaveRegisterCallback(this, MiniGame.EventId.AdminMode, "OnAdminMode");
        EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.AudioSubMiniGameIsPlaying, "OnAudioSubMiniGameIsPlaying");

        if (PlayerPrefs.HasKey(MiniGame.EventId.AdminMode))
        {
            admin = PlayerPrefs.GetInt(MiniGame.EventId.AdminMode) != 0;
        }

        buttonGameObject.SetActive(admin && !subMiniGameAudioPlaying);
    }

    public void OnAdminMode(bool toggle)
    {
        admin = toggle;
        buttonGameObject.SetActive(admin && !subMiniGameAudioPlaying);
    }

    public void OnAudioSubMiniGameIsPlaying(bool isPlaying)
    {
        subMiniGameAudioPlaying = isPlaying;
        buttonGameObject.SetActive(admin && !subMiniGameAudioPlaying);
    }

    public void OnQuitAdminModeButtonPressed()
    {
        PlayerPrefs.SetInt(MiniGame.EventId.AdminMode, 0);
        PlayerPrefs.Save();
        EventBus.Publish(MiniGame.EventId.AdminMode, false);
        EventBus.Publish(MiniGame.EventId.QuitAdminMode);
    }

    private void OnDestroy()
    {
        EventBus.SaveDeregisterCallback(this, MiniGame.EventId.AdminMode, "OnAdminMode");
        EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.AudioSubMiniGameIsPlaying, "OnAudioSubMiniGameIsPlaying");
    }
}
