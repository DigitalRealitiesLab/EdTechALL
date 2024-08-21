using UnityEngine;

public class TrackedImageLerper : MonoBehaviour
{
    private Transform target;

    public void PlaceToImageTarget(Transform target)
    {
        this.target = target;
        transform.position = target.position;
        transform.rotation = target.rotation;
    }

    public void LerpToTransform()
    {
        transform.position = Vector3.Lerp(transform.position, target.transform.position, EdTechALLConfig.mapPositionLerpFraction);
        transform.rotation = Quaternion.Lerp(transform.rotation, target.transform.rotation, EdTechALLConfig.mapRotationLerpFraction);
    }
}
