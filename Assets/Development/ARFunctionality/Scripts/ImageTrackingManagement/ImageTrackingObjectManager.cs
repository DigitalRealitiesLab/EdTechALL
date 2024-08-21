using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ImageTrackingObjectManager : MonoBehaviour
{
    public ARTrackedImageManager imageManager;

    private void Awake()
    {
        imageManager.trackedImagesChanged += OnTrackedImagesChanged;
        if (!EdTechALLConfig.prefabMapper.initialized)
        {
            EdTechALLConfig.prefabMapper.Initialize();
        }
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs obj)
    {
        for (int i = 0; i < obj.added.Count; i++)
        {
            EdTechALLConfig.prefabMapper.ImageSpawn(obj.added[i], transform);
        }

        for (int i = 0; i < obj.updated.Count; i++)
        {
            if (obj.updated[i].referenceImage.name != null)
            {
                if (!EdTechALLConfig.prefabMapper.InstanceExists(obj.updated[i]))
                {
                    EdTechALLConfig.prefabMapper.ImageSpawn(obj.updated[i], transform);
                }

                EdTechALLConfig.prefabMapper.ImageUpdate(obj.updated[i]);
            }
        }

        for (int i = 0; i < obj.removed.Count; i++)
        {
            EdTechALLConfig.prefabMapper.ImageDestroy(obj.removed[i]);
        }
    }

    private void OnDestroy()
    {
        imageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }
}