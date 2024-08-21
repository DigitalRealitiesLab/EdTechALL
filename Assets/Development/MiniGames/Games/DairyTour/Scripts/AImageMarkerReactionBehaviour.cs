using UnityEngine;
using UnityEngine.XR.ARFoundation;

public abstract class AImageMarkerReactionBehaviour : MonoBehaviour
{
    public abstract void OnImageMarkerDetected(ARTrackedImage image);
    public abstract void OnImageMarkerLost(ARTrackedImage image);
    public abstract void OnImageMarkerUpdate(ARTrackedImage image);

    protected bool TryGetImageMarkerData<T>(ImageMarkerLookup<T> lookup, string markerTextureId, out T data) where T : AImageMarkerLookupData
    {
        data = default;
        if (string.IsNullOrEmpty(markerTextureId))
        {
            return false;
        }

        bool success = lookup.imageMarkerLookupData.ContainsKey(markerTextureId);
        if (success)
        {
            data = lookup.imageMarkerLookupData[markerTextureId];
        }
        return success;
    }
}
