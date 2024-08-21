using UnityEngine;

public class TrackedImagePrefabScaler : MonoBehaviour
{
    public Transform scaleTarget;

    private void Awake()
    {
        if (!scaleTarget && !(scaleTarget = transform.GetChild(0)))
        {
            Debug.LogError("TrackedImagePrefabScaler could not find a scaling target");
        }
    }

    public void ResizeToWidth(float width)
    {
        float scale = width / scaleTarget.lossyScale.x;
        transform.localScale *= scale;
    }
}