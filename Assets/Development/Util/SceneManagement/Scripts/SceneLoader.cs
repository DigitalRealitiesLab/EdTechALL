using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARSubsystems;

public class SceneLoader : MonoBehaviour
{
    public string initialScene = "MiniGameMarkerDetectionScene";
    public static string previousScene;
    public static string currentScene;

    public bool autoSetLibrary = true;
    public XRReferenceImageLibrary referenceImageLibrary;
    public TextureToPrefabMapper prefabMapper;

    private Coroutine loadingRoutine = null;

    private static SceneLoadingArguments arguments;

    private void Awake()
    {
        EventBus.SaveRegisterCallback(this, EventId.SceneManagementEvents.RequestLoadScene, "StartSceneLoadCoroutine");
        EventBus.SaveRegisterCallback(this, EventId.SceneManagementEvents.RequestUnloadScene, "StartSceneUnloadCoroutine");
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (autoSetLibrary)
        {
            XRReferenceImage referenceImage = referenceImageLibrary[0];
            ReferenceLibrarySignature libraryRequestSignature = new ReferenceLibrarySignature();
            libraryRequestSignature.texture = referenceImage.texture;
            libraryRequestSignature.widthInMeters = referenceImage.width;
            libraryRequestSignature.textureName = referenceImage.name;
            EdTechALLConfig.referenceLibrary = libraryRequestSignature;
            EdTechALLConfig.prefabMapper = prefabMapper;
        }
        RequestLoadSceneSingle(initialScene);
    }

    public void StartSceneLoadCoroutine(string sceneName, LoadSceneMode loadMode)
    {
        if (loadingRoutine == null)
        {
            loadingRoutine = StartCoroutine(OnLoadSceneRequest(sceneName, loadMode));
        }
    }

    private IEnumerator OnLoadSceneRequest(string sceneName, LoadSceneMode loadMode)
    {
        if (sceneName != currentScene)
        {
            previousScene = currentScene;
            currentScene = sceneName;
        }

        AsyncOperation loadOperation = default;

        try
        {
            loadOperation = SceneManager.LoadSceneAsync(sceneName, loadMode);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Scene Loading Exception: " + e.Message);
            yield break;
        }

        loadOperation.completed += (operation) => { EventBus.Publish(EventId.SceneManagementEvents.SceneFinishedLoading); };

        while (!loadOperation.isDone)
        {
            EventBus.Publish(EventId.SceneManagementEvents.ReportSceneLoadProgress, loadOperation.progress);
            yield return null;
        }

        loadingRoutine = null;
    }

    public void StartSceneUnloadCoroutine(string sceneName)
    {
        if (loadingRoutine == null)
        {
            loadingRoutine = StartCoroutine(OnUnloadSceneRequest(sceneName));
        }
    }

    private IEnumerator OnUnloadSceneRequest(string sceneName)
    {
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(sceneName);
        unloadOperation.completed += (operation) => { EventBus.Publish(EventId.SceneManagementEvents.SceneFinishedLoading); };

        while (!unloadOperation.isDone)
        {
            yield return null;
        }

        loadingRoutine = null;
    }

    public static void SetSceneLoadArguments(params object[] arguments)
    {
        SceneLoader.arguments = new SceneLoadingArguments(arguments);
    }

    public static SceneLoadingArguments GetArguments()
    {
        return arguments;
    }

    public static void RequestLoadSceneAdditive(string sceneName)
    {
        EventBus.Publish(EventId.SceneManagementEvents.RequestLoadScene, sceneName, LoadSceneMode.Additive);
    }

    public static void RequestLoadSceneSingle(string sceneName)
    {
        EventBus.Publish(EventId.SceneManagementEvents.RequestLoadScene, sceneName, LoadSceneMode.Single);
    }

    public static void RequestUnloadScene(string sceneName)
    {
        EventBus.Publish(EventId.SceneManagementEvents.RequestUnloadScene, sceneName);
    }

    public static void RequestReloadCurrentScene()
    {
        RequestLoadSceneSingle(currentScene);
    }

    public static void LoadPreviousSceneSingle()
    {
        RequestLoadSceneSingle(previousScene);
    }

    private void OnDestroy()
    {
        EventBus.SaveDeregisterCallback(this, EventId.SceneManagementEvents.RequestLoadScene, "StartSceneLoadCoroutine");
        EventBus.SaveDeregisterCallback(this, EventId.SceneManagementEvents.RequestUnloadScene, "StartSceneUnloadCoroutine");
        if (loadingRoutine != null)
        {
            StopCoroutine(loadingRoutine);
            loadingRoutine = null;
        }
    }
}

public struct SceneLoadingArguments
{
    private object[] arguments;
    private System.Type[] types;

    public SceneLoadingArguments(params object[] args)
    {
        arguments = args;
        types = new System.Type[args.Length];

        for (int i = 0; i < types.Length; i++)
        {
            types[i] = args[i].GetType();
        }
    }

    public T GetSceneLoadArgument<T>(int index)
    {
        return (typeof(T) == types[index]) ? (T)arguments[index] : default(T);
    }

    public bool TryGetSceneLoadArgument<T>(int index, out T argument)
    {
        bool hasArgumentOfType = arguments != null && typeof(T) == types[index];
        argument = hasArgumentOfType ? (T)arguments[index] : default(T);

        return hasArgumentOfType;
    }
}

public partial class EventId
{
    public class SceneManagementEvents
    {
        public const string RequestLoadScene = "RequestLoadScene";
        public const string RequestUnloadScene = "RequestUnloadScene";
        public const string ReportSceneLoadProgress = "ReportSceneLoadProgress";
        public const string SceneFinishedLoading = "SceneFinishedLoading";
    }
}