using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;

public class MiniGameLoaderWindow : MonoBehaviour
{
    public Button confirm;
    public Button abort;

    public Image image;

    public bool autoSetLibrary = false;
    public List<XRReferenceImageLibraryLookup> staticReferenceImageLibraries = new List<XRReferenceImageLibraryLookup>();

    private MiniGameSceneLoadData loadData;

    public void Awake()
    {
        Debug.Assert(confirm, "MiniGameLoaderWindow is missing a referene to a Button");
        Debug.Assert(abort, "MiniGameLoaderWindow is missing a referene to a Button");
        Debug.Assert(image, "MiniGameLoaderWindow is missing a referene to an Image");

        EventBus.SaveRegisterCallback(this, MiniGame.EventId.RequestMiniGameLoadWindow, "OnRequestMiniGameLoadWindow");
        confirm.onClick.AddListener(OnConfirm);
        abort.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
        gameObject.SetActive(false);
    }

    public void OnRequestMiniGameLoadWindow(MiniGameSceneLoadData loadData, bool active)
    {
        gameObject.SetActive(active);

        if (active)
        {
            image.sprite = loadData.sceneSprite;
            this.loadData = loadData;
            EventBus.Publish(MiniGame.EventId.RequestCloseAppWindow, false);
            EventBus.Publish(MiniGame.EventId.RequestDeleteSaveWindow, false);
        }
    }

    private void OnConfirm()
    {
        if (autoSetLibrary)
        {
            foreach (XRReferenceImageLibraryLookup lookup in staticReferenceImageLibraries)
            {
                if (loadData.lookupKey == lookup.lookupKey)
                {
                    XRReferenceImage referenceImage = lookup.referenceImageLibrary[0];
                    ReferenceLibrarySignature libraryRequestSignature = new ReferenceLibrarySignature();
                    libraryRequestSignature.texture = referenceImage.texture;
                    libraryRequestSignature.widthInMeters = referenceImage.width;
                    libraryRequestSignature.textureName = referenceImage.name;
                    EdTechALLConfig.referenceLibrary = libraryRequestSignature;
                    EdTechALLConfig.prefabMapper = lookup.prefabMapper;
                }
            }
        }
        SceneLoader.RequestLoadSceneSingle(loadData.sceneName);
    }

    private void OnDestroy()
    {
        EventBus.SaveDeregisterCallback(this, MiniGame.EventId.RequestMiniGameLoadWindow, "OnRequestMiniGameLoadWindow");
    }
}

[System.Serializable]
public class XRReferenceImageLibraryLookup
{
    public string lookupKey;
    public XRReferenceImageLibrary referenceImageLibrary;
    public TextureToPrefabMapper prefabMapper;
}