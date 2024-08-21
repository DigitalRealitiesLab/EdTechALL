using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TextureToPrefabMapper", menuName = "ScriptableObjects/TextureToPrefabMapper", order = 1)]
public class TextureToPrefabMapper : ScriptableObject
{
    public List<TextureToPrefabPreset> presets = new List<TextureToPrefabPreset>();

    protected Dictionary<string, TextureToPrefabPreset> prefabLookup = new Dictionary<string, TextureToPrefabPreset>();
    protected Dictionary<string, GameObject> instantiatedLookup = new Dictionary<string, GameObject>();

    public TextureToPrefabPreset GetPresetAtImageName(string imageName)
    {
        if (prefabLookup.ContainsKey(imageName))
        {
            return prefabLookup[imageName];
        }
        return default;
    }

    public bool initialized { get { return prefabLookup.Count > 0; } }

    public void Initialize()
    {
        if (!initialized)
        {
            for (int i = 0; i < presets.Count; i++)
            {
                prefabLookup[presets[i].name] = presets[i];
            }
        }
    }

    public GameObject ImageSpawn(ARTrackedImage image, Transform parent)
    {
        if (prefabLookup.ContainsKey(image.referenceImage.name))
        {
            GameObject instance;
            if (!InstanceExists(image))
            {
                instance = Instantiate(prefabLookup[image.referenceImage.name].prefab, parent);
                instantiatedLookup[image.referenceImage.name] = instance;
            }
            else
            {
                instance = instantiatedLookup[image.referenceImage.name];
            }

            instance.GetComponent<TrackedImagePrefabScaler>().ResizeToWidth(image.size.x);
            instance.GetComponent<TrackedImageLerper>().PlaceToImageTarget(image.transform);
            instance.SetActive(image.trackingState != TrackingState.None);
            return instance;
        }
        return null;
    }

    public void ImageUpdate(ARTrackedImage image)
    {
        if (image.trackingState == TrackingState.Tracking && instantiatedLookup.ContainsKey(image.referenceImage.name))
        {
            GameObject instance = instantiatedLookup[image.referenceImage.name];
            instance.GetComponent<TrackedImageLerper>().LerpToTransform();
            instance.GetComponent<TrackedImagePrefabScaler>().ResizeToWidth(image.size.x);
        }
    }

    public void ImageDestroy(ARTrackedImage image)
    {
        if (instantiatedLookup.ContainsKey(image.referenceImage.name))
        {
            Destroy(instantiatedLookup[image.referenceImage.name]);
            instantiatedLookup.Remove(image.referenceImage.name);
        }
    }

    public bool InstanceExists(ARTrackedImage image)
    {
        return instantiatedLookup.ContainsKey(image.referenceImage.name) && instantiatedLookup[image.referenceImage.name] != null;
    }

    public void ImageResize(string name, float width)
    {
        if (instantiatedLookup.ContainsKey(name))
        {
            instantiatedLookup[name].GetComponent<TrackedImagePrefabScaler>().ResizeToWidth(width);
        }
    }
}

[System.Serializable]
public struct TextureToPrefabPreset
{
    public Texture2D texture;
    public Sprite displaySprite;
    public GameObject prefab;
    public string name;
}