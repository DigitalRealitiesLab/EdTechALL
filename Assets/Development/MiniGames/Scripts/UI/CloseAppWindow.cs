using UnityEngine;
using UnityEngine.UI;

public class CloseAppWindow : MonoBehaviour
{
    public Button confirm;
    public Button abort;

    public void Awake()
    {
        Debug.Assert(confirm, "CloseAppWindow is missing a referene to a Button");
        Debug.Assert(abort, "CloseAppWindow is missing a referene to a Button");

        EventBus.SaveRegisterCallback(this, MiniGame.EventId.RequestCloseAppWindow, "OnRequestCloseAppWindow");
        confirm.onClick.AddListener(Application.Quit);
        abort.onClick.AddListener(() => { gameObject.SetActive(false); });
        gameObject.SetActive(false);
    }

    public void OnRequestCloseAppWindow(bool active)
    {
        gameObject.SetActive(active);

        if (active)
        {
            EventBus.Publish(MiniGame.EventId.RequestDeleteSaveWindow, false);
            MiniGameSceneLoadData loadData = null;
            EventBus.Publish(MiniGame.EventId.RequestMiniGameLoadWindow, loadData, false);
        }
    }

    private void OnDestroy()
    {
        EventBus.SaveDeregisterCallback(this, MiniGame.EventId.RequestCloseAppWindow, "OnRequestCloseAppWindow");
    }
}