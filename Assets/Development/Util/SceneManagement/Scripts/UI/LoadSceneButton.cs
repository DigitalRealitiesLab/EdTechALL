using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;

public class LoadSceneButton : MonoBehaviour
{
    public string sceneName = "";

    public Button button;

    public bool autoSetLibrary = false;
    public XRReferenceImageLibrary referenceImageLibrary;
    public TextureToPrefabMapper prefabMapper;

    private void Awake()
    {
        if (!button && !TryGetComponent(out button))
        {
            Debug.LogError("LoadSceneButton is missing a reference to its Button");
        }

        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
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

        if (sceneName == "MiniGameMarkerDetectionScene")
        {
            PlayerPrefs.SetInt(Tutorial.InitialTutorial.initialTutorialSkipString, 1);
            PlayerPrefs.Save();
        }

        SceneLoader.RequestLoadSceneSingle(sceneName);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnButtonClick);
    }
}
