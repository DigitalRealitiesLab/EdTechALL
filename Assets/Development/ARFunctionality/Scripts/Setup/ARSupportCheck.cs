using UnityEngine;
using System.Collections;
using UnityEngine.XR.ARFoundation;

public class ARSupportCheck : MonoBehaviour
{
    public ARSession session;
    public ARTrackedImageManager trackedImageManager;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        ARSession.stateChanged += OnARSessionStateChanged;

        if (!trackedImageManager && !(trackedImageManager = FindObjectOfType<ARTrackedImageManager>(true)))
        {
            Debug.LogError("ARSupportCheck could not validate features because a reference to an ARTrackedImageManagerIsMissing");
        }

        Debug.Log("Image Tracking Features: \n mutable reference libraries support: " + trackedImageManager.descriptor.supportsMutableLibrary
            + "\n requires physical image dimensions: " + trackedImageManager.descriptor.requiresPhysicalImageDimensions
            + "\n supports image validation: " + trackedImageManager.descriptor.supportsImageValidation 
            + "\n supports moving images: " + trackedImageManager.descriptor.supportsMovingImages);

        if (!trackedImageManager.descriptor.supportsMutableLibrary)
        {
            Debug.LogWarning("AR Sub-System Support Init Error: Mutable Libraries not supported. This device and|or operating " +
                "system do not support mutable runtime image libraries, that are required for the application to work correctly.");
        }

        if (!trackedImageManager.descriptor.supportsImageValidation)
        {
            Debug.LogWarning("AR Sub-System Support Init Warning: Image Validation of Mutable Library Images not supported.");
        }
    }

    IEnumerator Start()
    {
        if (ARSession.state == ARSessionState.None || ARSession.state == ARSessionState.CheckingAvailability)
        {
            yield return ARSession.CheckAvailability();
        }

        if (ARSession.state == ARSessionState.Unsupported)
        {

        }
        else
        {
            session.enabled = true;
        }
    }

    protected void OnARSessionStateChanged(ARSessionStateChangedEventArgs stateChangedArgs)
    {
        EventBus.Publish(MiniGame.EventId.ARSessionStateChanged, stateChangedArgs.state);
        Debug.Log("ARSessionStatus changed to " + stateChangedArgs.state);
    }

    private void OnApplicationQuit()
    {
        ARSession.stateChanged -= OnARSessionStateChanged;
    }
}
