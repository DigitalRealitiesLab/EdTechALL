using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public ProgressBar progressBar;

    private void Awake()
    {
        EventBus.SaveRegisterCallback(this, EventId.SceneManagementEvents.RequestLoadScene, "OnRequestSceneLoad");
        Debug.Assert(progressBar, "LoadingScreen is missing a reference to ProgressBar");
        gameObject.SetActive(false);
    }

    public void OnRequestSceneLoad(string sceneName, LoadSceneMode loadMode)
    {
        if (loadMode == LoadSceneMode.Single)
        {
            gameObject.SetActive(true);
            EventBus.SaveRegisterCallback(this, EventId.SceneManagementEvents.ReportSceneLoadProgress, "OnSceneProgressUpdate");
            EventBus.SaveRegisterCallback(this, EventId.SceneManagementEvents.SceneFinishedLoading, "OnSceneFinishedLoading");
            progressBar.progress = 0.0f;
        }
    }

    public void OnSceneProgressUpdate(float progress)
    {
        progressBar.progress = progress;
    }

    public void OnSceneFinishedLoading()
    {
        gameObject.SetActive(false);
        EventBus.SaveDeregisterCallback(this, EventId.SceneManagementEvents.ReportSceneLoadProgress, "OnSceneProgressUpdate");
        EventBus.SaveDeregisterCallback(this, EventId.SceneManagementEvents.SceneFinishedLoading, "OnSceneFinishedLoading");
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    private void OnDestroy()
    {
        EventBus.SaveDeregisterCallback(this, EventId.SceneManagementEvents.RequestLoadScene, "OnRequestSceneLoad");
    }
}
