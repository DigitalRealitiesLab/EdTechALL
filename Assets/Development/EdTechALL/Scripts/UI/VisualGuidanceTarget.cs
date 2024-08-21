using UnityEngine;

public class VisualGuidanceTarget : MonoBehaviour
{
    public Sprite onscreenPointerSprite;
    public Sprite offscreenPointerSprite;
    public bool onscreenRotating = true;
    public bool offscreenRotating = true;
    public bool onscreen = true;
    public bool offscreen = true;
    public bool virtualTour = false;

    private void Awake()
    {
        if (onscreen)
        {
            Debug.Assert(onscreenPointerSprite, "VisualGuidanceTarget is missing a reference to a Sprite");
        }
        if (offscreen)
        {
            Debug.Assert(offscreenPointerSprite, "VisualGuidanceTarget is missing a reference to a Sprite");
        }
        EventBus.Publish(EventId.UIEvents.RefreshVisualGuidance);
    }

    private void OnEnable()
    {
        EventBus.Publish(EventId.UIEvents.RefreshVisualGuidance);
    }

    private void OnDisable()
    {
        EventBus.Publish(EventId.UIEvents.RefreshVisualGuidance);
    }

    private void OnDestroy()
    {
        EventBus.Publish(EventId.UIEvents.RefreshVisualGuidance);
    }
}
