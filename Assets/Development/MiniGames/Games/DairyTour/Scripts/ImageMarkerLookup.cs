using System.Collections.Generic;
using UnityEngine;

public class ImageMarkerLookup<T> : ScriptableObject where T : AImageMarkerLookupData
{
    public List<T> imageMarkerData;
    public Dictionary<string, T> imageMarkerLookupData = new Dictionary<string, T>();

    public T GetLookupDataByMarkerTextureName(string textureName)
    {
        Debug.Assert(imageMarkerLookupData.ContainsKey(textureName), "Image Marker Lookup does not contain Data for an Image Marker with the name " + textureName);
        return imageMarkerLookupData[textureName];
    }

    public virtual void Initialize()
    {
        imageMarkerLookupData.Clear();
        foreach (T markerData in imageMarkerData)
        {
            imageMarkerLookupData[markerData.lookupKey] = markerData;
        }
    }
}

[System.Serializable]
public abstract class AImageMarkerLookupData
{
    public string lookupKey;
}
