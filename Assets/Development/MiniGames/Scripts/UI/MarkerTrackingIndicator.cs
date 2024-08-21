using UnityEngine;
using UnityEngine.UI;

public class MarkerTrackingIndicator : MonoBehaviour
{
    public Image guidanceImage;
    public Image frameImage;

    private const float threshold = 0.25f;

    private bool active = true;
    private bool imageEnabled = true;

    private void Awake()
    {
        Debug.Assert(guidanceImage, "MarkerTrackingIndicator is missing a reference to an Image");
        Debug.Assert(frameImage, "MarkerTrackingIndicator is missing a reference to an Image");

        guidanceImage.enabled = false;
        frameImage.enabled = false;

        EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.SendBestMarkerWeight, "OnSendBestMarkerWeight");
        EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.EnableMarkerTrackingIndicator, "OnEnableMarkerTrackingIndicator");
    }

    public void OnSendBestMarkerWeight(float bestWeight)
    {
        if (bestWeight != -1 && bestWeight < threshold)
        {
            imageEnabled = true;
        }
        else
        {
            imageEnabled = false;
        }

        if (active)
        {
            guidanceImage.enabled = imageEnabled;
            frameImage.enabled = false;
        }
        else
        {
            guidanceImage.enabled = false;
            frameImage.enabled = true;

            if (imageEnabled)
            {
                frameImage.color = Color.red;
            }
            else
            {
                frameImage.color = Color.green;
            }
        }
    }

    public void OnEnableMarkerTrackingIndicator(bool enabled)
    {
        active = enabled;

        if (active)
        {
            guidanceImage.enabled = imageEnabled;
            frameImage.enabled = false;
        }
        else
        {
            guidanceImage.enabled = false;
            frameImage.enabled = true;

            if (imageEnabled)
            {
                frameImage.color = Color.red;
            }
            else
            {
                frameImage.color = Color.green;
            }
        }
    }

    private void OnDestroy()
    {
        EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.SendBestMarkerWeight, "OnSendBestMarkerWeight");
        EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.EnableMarkerTrackingIndicator, "OnEnableMarkerTrackingIndicator");
    }
}
