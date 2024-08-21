using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class VisualGuidanceUIPanel : MonoBehaviour
{
    public GameObject pointerPrefab;

    private List<GameObject> pointers;
    private VisualGuidanceTarget[] visualGuidanceTargets;

    private Camera mainCamera;

    private void Awake()
    {
        Debug.Assert(pointerPrefab, "VisualGuidanceTarget is missing a reference to a Prefab");
        pointers = new List<GameObject>();
        visualGuidanceTargets = FindObjectsOfType<VisualGuidanceTarget>();

        foreach (VisualGuidanceTarget visualGuidanceTarget in visualGuidanceTargets)
        {
            if (visualGuidanceTarget.gameObject.activeSelf)
            {
                GameObject instance = Instantiate(pointerPrefab, transform);
                pointers.Add(instance);
            }
            else
            {
                pointers.Add(visualGuidanceTarget.gameObject);
            }
        }

        mainCamera = Camera.main;

        EventBus.SaveRegisterCallback(this, EventId.UIEvents.RefreshVisualGuidance, "OnRefreshVisualGuidance");
    }

    private void Update()
    {
        for (int i = 0; i < pointers.Count; i++)
        {
            if (visualGuidanceTargets[i].gameObject.activeSelf)
            {
                if (pointers[i] != visualGuidanceTargets[i].gameObject)
                {
                    PointTowards(pointers[i], visualGuidanceTargets[i]);
                }
            }
        }
    }

    private void PointTowards(GameObject pointer, VisualGuidanceTarget visualGuidanceTarget)
    {
        Camera camera;

        if (visualGuidanceTarget.virtualTour)
        {
            camera = visualGuidanceTarget.transform.parent.parent.parent.parent.GetComponentInChildren<Camera>();
        }
        else
        {
            camera = mainCamera;
        }

        RectTransform rectTransform = pointer.GetComponent<RectTransform>();
        Vector3 viewportPoint = camera.WorldToViewportPoint(visualGuidanceTarget.transform.position);
        bool visible = viewportPoint.x > 0.0f && viewportPoint.x < 1.0f && viewportPoint.y > 0.0f && viewportPoint.y < 1.0f;
        Vector3 direction = (camera.WorldToScreenPoint(visualGuidanceTarget.transform.position) - new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0.0f)).normalized;
        bool turnAround = ((visualGuidanceTarget.transform.position - camera.transform.position).normalized + camera.transform.forward).magnitude < ((visualGuidanceTarget.transform.position - camera.transform.position).normalized - camera.transform.forward).magnitude;

        if (visible && !turnAround)
        {
            rectTransform.position = camera.WorldToScreenPoint(visualGuidanceTarget.transform.position);
            if (visualGuidanceTarget.onscreenRotating)
            {
                rectTransform.localEulerAngles = new Vector3(0.0f, 0.0f, (180.0f / Mathf.PI) * Mathf.Atan2(direction.y, direction.x) - 90.0f);
            }
            if (visualGuidanceTarget.onscreen)
            {
                Image pointerImage = pointer.GetComponent<Image>();
                pointerImage.sprite = visualGuidanceTarget.onscreenPointerSprite;
                pointerImage.enabled = true;
            }
            else
            {
                Image pointerImage = pointer.GetComponent<Image>();
                pointerImage.enabled = false;
            }
        }
        else
        {
            if (turnAround)
            {
                direction = -direction;
            }

            float y = Screen.width / 2.0f * direction.y / direction.x;

            if (Mathf.Abs(y) < (Screen.height / 2.0f))
            {
                if (Mathf.Sign(direction.x) > 0)
                {
                    rectTransform.position = new Vector3(Screen.width - rectTransform.rect.width / 2.0f, y + Screen.height / 2.0f, 0.0f);
                }
                else
                {
                    rectTransform.position = new Vector3(rectTransform.rect.width / 2.0f, Screen.height - (y + Screen.height / 2.0f), 0.0f);
                }
            }
            else
            {
                float x = Screen.height / 2.0f * direction.x / direction.y;

                if (Mathf.Abs(x) < (Screen.width / 2.0f))
                {
                    if (Mathf.Sign(direction.y) > 0)
                    {
                        rectTransform.position = new Vector3(x + Screen.width / 2.0f, Screen.height - rectTransform.rect.height / 2.0f, 0.0f);
                    }
                    else
                    {
                        rectTransform.position = new Vector3(Screen.width - (x + Screen.width / 2.0f), rectTransform.rect.height / 2.0f, 0.0f);
                    }
                }
                else
                {
                    if (Mathf.Sign(direction.y) > 0)
                    {
                        if (Mathf.Sign(direction.x) > 0)
                        {
                            rectTransform.position = new Vector3(Screen.width - rectTransform.rect.width / 2.0f, Screen.height - rectTransform.rect.height / 2.0f, 0.0f);
                        }
                        else
                        {
                            rectTransform.position = new Vector3(rectTransform.rect.width / 2.0f, Screen.height - rectTransform.rect.height / 2.0f, 0.0f);
                        }
                    }
                    else
                    {
                        if (Mathf.Sign(direction.x) > 0)
                        {
                            rectTransform.position = new Vector3(Screen.width - rectTransform.rect.width / 2.0f, rectTransform.rect.height / 2.0f, 0.0f);
                        }
                        else
                        {
                            rectTransform.position = new Vector3(rectTransform.rect.width / 2.0f, rectTransform.rect.height / 2.0f, 0.0f);
                        }
                    }
                }
            }
            if (visualGuidanceTarget.offscreenRotating)
            {
                rectTransform.localEulerAngles = new Vector3(0.0f, 0.0f, (180.0f / Mathf.PI) * Mathf.Atan2(direction.y, direction.x) - 90.0f);
            }

            if (visualGuidanceTarget.offscreen)
            {
                Image pointerImage = pointer.GetComponent<Image>();
                pointerImage.sprite = visualGuidanceTarget.offscreenPointerSprite;
                pointerImage.enabled = true;
            }
            else
            {
                Image pointerImage = pointer.GetComponent<Image>();
                pointerImage.enabled = false;
            }
        }
    }

    public void OnRefreshVisualGuidance()
    {
        foreach (GameObject pointer in pointers)
        {
            Destroy(pointer);
        }

        pointers.Clear();

        visualGuidanceTargets = FindObjectsOfType<VisualGuidanceTarget>();

        foreach (VisualGuidanceTarget visualGuidanceTarget in visualGuidanceTargets)
        {
            if (visualGuidanceTarget.gameObject.activeSelf)
            {
                GameObject instance = Instantiate(pointerPrefab, transform);
                pointers.Add(instance);
            }
            else
            {
                pointers.Add(visualGuidanceTarget.gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        EventBus.SaveDeregisterCallback(this, EventId.UIEvents.RefreshVisualGuidance, "OnRefreshVisualGuidance");
    }
}
