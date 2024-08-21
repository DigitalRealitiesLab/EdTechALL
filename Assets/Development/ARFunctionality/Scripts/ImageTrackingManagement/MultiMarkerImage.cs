using UnityEngine;

public class MultiMarkerImage : MonoBehaviour
{
    public string imageName = "HandMapMarker";
    public Vector3 offset;
    public float weight = 0.0f;
    public int imageCountSqrt = 2;

    private Renderer childRenderer;

    // parameters for weight calculation
    private const float minDistanceHandMap = 0.4f;
    private const float minDistanceMultiplierHandMap = 4.0f;
    private const float maxDistanceHandMap = 0.8f;
    private const float maxDistanceMultiplierHandMap = 2.0f;
    private const float minDistanceFloorMap = 0.6f;
    private const float minDistanceMultiplierFloorMap = 3.0f;
    private const float maxDistanceFloorMap = 2.0f;
    private const float maxDistanceMultiplierFloorMap = 0.5f;

    private Camera mainCamera;

    private void Awake()
    {
        childRenderer = GetComponentInChildren<Renderer>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        Plane plane = new Plane(transform.parent.parent.up, transform.position);
        Ray ray = mainCamera.ScreenPointToRay(new Vector2(Screen.width / 2.0f, Screen.height / 2.0f));
        float enter;
        if (plane.Raycast(ray, out enter))
        {
            Vector3 closestPosition = childRenderer.bounds.ClosestPoint(ray.GetPoint(enter));
            Vector3 viewportPoint = mainCamera.WorldToViewportPoint(closestPosition);

            if (childRenderer.isVisible)
            {
                weight = Mathf.Max(0.0f, 1.0f - ((Mathf.Abs(Mathf.Min(1.0f, Mathf.Max(0.0f, viewportPoint.x)) - 0.5f) + Mathf.Abs(Mathf.Min(1.0f, Mathf.Max(0.0f, viewportPoint.y)) - 0.5f)) * 2.0f));

                float distance = (closestPosition - mainCamera.transform.position).magnitude;

                if (imageName.Contains("HandMapMarker"))
                {
                    weight -= Mathf.Min(weight, Mathf.Max(0.0f, (distance - maxDistanceHandMap) * maxDistanceMultiplierHandMap));
                    weight -= Mathf.Min(weight, Mathf.Max(0.0f, (minDistanceHandMap - distance) * minDistanceMultiplierHandMap));
                }
                else if (imageName.Contains("FloorMapMarker"))
                {
                    weight -= Mathf.Min(weight, Mathf.Max(0.0f, (distance - maxDistanceFloorMap) * maxDistanceMultiplierFloorMap));
                    weight -= Mathf.Min(weight, Mathf.Max(0.0f, (minDistanceFloorMap - distance) * minDistanceMultiplierFloorMap));
                }
            }
            else
            {
                weight = 0.0f;
            }
        }
        else
        {
            weight = 0.0f;
        }
    }
}